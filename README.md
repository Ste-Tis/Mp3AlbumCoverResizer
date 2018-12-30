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
-d	--dir			Directory containing MP3 files, which should be processed
-w	--width			New max width of the images
-h	--height		New max height of the images
-q	--quality		Compress image after resizing (value: 0 - 100)
-r	--recursive		Also process files in subdirectories
-v	--verbose		Show progress and other messages
    --help			Show information about usage
```

## License

This project is licensed under the MIT license. See the [LICENSE](https://github.com/Ste-Tis/Mp3AlbumCoverResizer/blob/master/LICENSE) file for more info.