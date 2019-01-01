# Mp3AlbumCoverResizer
Commandline tool to resize the images embedded in the tags of MP3 Files. Pass the path to your music library to the tool and Mp3AlbumCoverResizer will search for all MP3 files inside the directory (and subdirectories) to resize embedded images in the audio files. Optimize your music library for usage in your car or on other devices, which only support images of a certain size.

## Usage
Use windows CMD or Powershell to execute program.
```
CMD> Mp3AlbumCoverResizer.exe -d "D:/My Music/Car" -w 500 -h 500 -r -v
```

### Parameter
The following commandline parameter are supported in the current version.

```
-d	--dir=VALUE			    Directory containing MP3 files, which should be processed
-w	--width=VALUE			New max width of the images
-h	--height=VALUE		    New max height of the images
-q	--quality=VALUE		    Compress image after resizing (value: 0 - 100)
-o  --overwrite=VALUE       Overwrites or adds an album cover from file
-r	--recursive		        Also process files in subdirectories
-v	--verbose		        Show progress and other messages
    --help		            Show information about usage
```

### Overwrite
Using the parameter -o=VALUE or --overwrite=VALUE allows to add a cover from a file within the folder to the MP3 files. 

```
CMD> Mp3AlbumCoverResizer.exe -d "D:/My Music/Car" -o cover.jpg -r -v
```

If an folder contains an image file with the given name, all embedded images will be removed from the MP3 files in the folder and the image from the file will be added as cover. If an folder doesn't contain an image file with matching name, the embedded images will be resized, like the option wasn't set.

## Known Problems
* The tool skips files, if there is an error in one of the tag fields
* The tool can't work on files which only have ID3 tags version 1 or no tags at all

## License
This project is licensed under the MIT license. See the [LICENSE](https://github.com/Ste-Tis/Mp3AlbumCoverResizer/blob/master/LICENSE) file for more info.