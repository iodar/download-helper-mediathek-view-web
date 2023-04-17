#!/bin/bash

function print-help {
    echo "Download SRF videos

  USAGE
    $0 [url-from-MediathekViewWeb] [video-name-without-extension]

  PARAMETERS
    url-from-MediathekViewWeb         URL from MediathekViewWeb
                                      (see README on how to obtain that)
    video-name-without-extension      Video name for final video

  AUTHOR
    iodar (2023)"
}

[[ "$1" == "--help" || "$1" == "-h" ]] && \
    print-help && \
    exit 0

# Index ist m3u8 file with names of individual segments
indexFileUrl="$1"

# Name of video file (e.g. name of movie or series)
videoName="$2"
# safe video name (hyphen and space are replaced by underscore)
# used as prefix for all segments to be able to indentify all
# segments / chunks of one video / movie file
videoNameSafe=$(echo $videoName | sed -r 's/[- ]/_/g')

# checks that necessary parameters (url and video name) were
# given to the scripts
[[ -z $indexFileUrl || -z $videoName ]] \
    && echo "Error: Wrong usage" \
    && echo "Usage: $0 [url-from-MediathekViewWeb] [video-name-without-extension]" \
    && exit 1

# replace last resource in index (playlist file) in url with nothing to get
# download base url
baseUrl="$(echo $indexFileUrl | sed -r 's/\/index.*$//g')"

# download index file
curl -sL "$indexFileUrl" -o "${videoNameSafe}.m3u8"

function download-chunks {
# download all segments of the video file
# uses videoNameSafe as prefix for every segment
# to make it distinguishable from segments of
# other video files
  echo -ne "Downloading files ..."
  local segmentsArray=($@)
  totalNumberOfSegments=${#segmentsArray[@]}
  index=1
  # read chunks from index file and download individual chunks
  for chunk in ${segmentsArray[@]}; do
      downloadedChunksSizeMsg="Download starting"
      # check size of downloaded chunks
      if [ $(ls -1 ${videoNameSafe}_segment*.ts > /dev/null 2>&1; echo $?) -eq 0 ]; then
        if [ $(ls -1 ${videoNameSafe}_segment*.ts | wc -l | awk '{print $1}') -ge 1 ]; then
          sizeOfDownloadedChunksInByte="$(ls -la ${videoNameSafe}_segment*.ts | awk '{sum += $5} END{printf "%0.2f", sum}')"
        else
          sizeOfDownloadedChunksInByte=0
        fi

        # calculate size in MB if there is at least one
        # chunk present
        if [ $sizeOfDownloadedChunksInByte -ne 0 ]; then
          downloadedChunkSizeAsDecimal="$sizeOfDownloadedChunksInByte.0"
          downloadedChunkSizeInMb=$(echo "$downloadedChunkSizeAsDecimal" | awk '{printf "%0.2f\n", ($1 / 1048576.0); }')
          downloadedChunksSizeMsg="$downloadedChunkSizeInMb MB"
        fi
      fi
      echo -ne "\r                                                               "
      echo -ne "\rDownloading files (${index}/$totalNumberOfSegments) ($downloadedChunksSizeMsg) ... "
      
      if [ ! -f "${videoNameSafe}_${chunk}" ]; then
        curl -sL "${baseUrl}/${chunk}" -o "${videoNameSafe}_${chunk}"
      fi
      index=$(expr $index + 1)
  done
  # clear stdout
  echo -e "Done"
}

# download all segments of the video file
# uses videoNameSafe as prefix for every segment
# to make it distinguishable from segments of
# other video files
segmentsArray=($(cat $videoNameSafe.m3u8 | grep -v "^#"))
download-chunks ${segmentsArray[@]}

# Check all video files for validity using
# ffprobe. If not valid, that download chunk
# with different quality

echo -ne "Checking segments for faults ... "
segmentsToReDownload=()
allSegmentFiles=($(ls -1 ${videoNameSafe}_segment*.ts))
totalNumberOfSegments=${#allSegmentFiles[@]}
checkSegmentIndex=1
for segment in ${allSegmentFiles[@]}; do
  echo -ne "\rChecking segments for faults ($checkSegmentIndex/$totalNumberOfSegments) ... "
  ffprobe -i $segment > /dev/null 2>&1
  if [ $? -ne 0 ]; then
    rm $segment
    orignalSegmentName=$(echo $segment | sed -r "s/^${videoNameSafe}_//g")
    segmentMediumQualityName=$(echo $orignalSegmentName | sed -r 's/f[[:digit:]]/f1/g')
    curlResult="$(curl -sLI "$baseUrl/$segmentMediumQualityName" | head -n 1 | grep -oE "HTTP\/1\.1 200 OK")"
    if [[ "$curlResult" == "HTTP/1.1 200 OK" ]]; then
      segmentsToReDownload+=($segmentMediumQualityName)
    fi
  fi
  checkSegmentIndex=$(expr $checkSegmentIndex + 1)
done
echo "Done"

if [ ${#segmentsToReDownload[@]} -gt 0 ]; then
  echo "Found ${#segmentsToReDownload[@]} faulty segments. Attempting to download segments ..."
  # re downloading faulty segments
  download-chunks ${segmentsToReDownload[@]}
else
  echo "Found no faulty segments. Skipping re download"
fi

# Renaming segments to allow for sorting
#
# Each segment has this structure:
# segment-[seg-id]-f6-v1-a1.ts where [seg-id] is
# the number of the segment
# 
# Sorting this would result in scrambled chunks
# since sort in bash results in this
#   segment-1-f6-v1-a1.ts
#   segment-10-f6-v1-a1.ts
#   segment-100-f6-v1-a1.ts
#
# Therefore every chunk number is left padded
# to reach five digits like so:
#   segment-00001-f6-v1-a1.ts
#   segment-00010-f6-v1-a1.ts
#   segment-00100-f6-v1-a1.ts
# This allows us to sort the chunks in their
# real order which results in a correct video file
echo -ne "Renaming segments ..."
allSegmentsToRename=($(ls ${videoNameSafe}_segment*.ts))
totalSegmentsToRename=${#allSegmentsToRename[@]}
renamingIndex=1
for segment in ${allSegmentsToRename[@]}; do
    segmentNumber=$(echo $segment | cut -d- -f2)
    paddedSegmentNumber=$(printf %05d $segmentNumber)
    newSegmentName=$(echo $segment | sed -r "s/segment-${segmentNumber}/segment-${paddedSegmentNumber}/g")
    
    echo -ne "\rRenaming segments (${renamingIndex}/${totalSegmentsToRename}) ..."
    mv $segment $newSegmentName
    renamingIndex=$(expr $renamingIndex + 1)
done
# clear stdout
echo -ne "\r                                              "
echo -e "\rRenaming segments ... Done"

# writing all names of segments into a file
# in preparation to concatenate them using ffmpeg
allSegmentFiles=$(ls -1 ${videoNameSafe}_segment*.ts)
for segment in $allSegmentFiles; do
    echo "file '$segment'" >> "${videoNameSafe}.allFiles.list"
done

# Now concatenating chunks into one video file
# and using the provided video name (param $2)
# as it's file name
echo "Concatenating files using ffmpeg ..."
ffmpeg -loglevel repeat+warning -f concat -safe 0 -i "${videoNameSafe}.allFiles.list" -c copy "${videoName}.ts"
ffmpegExitCode=$?
echo "Concatenating files using ffmpeg ... Done"


# removing all chunks / segments, ffmpeg all files list and
# .m3u8 playlist file

if [ $ffmpegExitCode -ne 0 ]; then
  echo "ffmpeg command return exit code not equal to 0: $ffmpegExitCode"
  echo "Skipping clean up ..."
else 
  echo "Performing clean up ..."
  echo "rm \"${videoNameSafe}.allFiles.list\""
  rm "${videoNameSafe}.allFiles.list"
  echo "rm ${videoNameSafe}_segment*.ts"
  rm ${videoNameSafe}_segment*.ts
  echo "rm \"${videoNameSafe}.m3u8\""
  rm "${videoNameSafe}.m3u8"
  echo "Performing clean up ... Done"
fi
