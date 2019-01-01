using Id3;
using Mp3AlbumCoverResizer.Helper;
using System;
using System.IO;

namespace Mp3AlbumCoverResizer
{
    /// <summary>
    ///     Read current pictures within tags and resize all of them
    /// </summary>
    public class AlbumCoverResizer
    {
        private int coverImageQuality = 90;

        /// <summary>
        ///     Logging
        /// </summary>
        protected SimpleConsoleLogger Logger { get; } = null;

        /// <summary>
        ///     New max width of images
        /// </summary>
        public int CoverImageWidthPx { get; set; }

        /// <summary>
        ///     New max height of images
        /// </summary>
        public int CoverImageHeightPx { get; set; }

        /// <summary>
        ///     Compress images while resizing (value: 0 - 100)
        /// </summary>
        public int CoverImageQuality {
            get => coverImageQuality;
            set => coverImageQuality = Math.Min(100, Math.Max(0, value));
        }

        /// <summary>
        ///     Use given pattern to filter files which should be processed
        /// </summary>
        public string FileFilter { get; set; }

        /// <summary>
        ///     Overwrite all embedded images or add image from file, if none is embedded
        /// </summary>
        /// <remarks>
        ///     If this option is set to true, all embedded images will be removed, if an image with an specific
        ///     name is in the same folder as the MP3 file (see option OverwriteImageName). This image file will
        ///     then be used as new album cover for the audio file. If the MP3 file has no images embedded, the 
        ///     new cover will be added.
        ///     Use this option to quickly add covers to multiple MP3 files.
        /// </remarks>
        public bool OverwriteImageFromFile { get; set; }

        /// <summary>
        ///     Name of the image file, which should be used for overwriting
        /// </summary>
        public string OverwriteImageName { get; set; }

        /// <summary>
        ///     Initialize class instance
        /// </summary>
        /// <param name="logger">Instance of logger to use</param>
        /// <param name="coverWidthPx">New max width of images</param>
        /// <param name="coverHeightPx">New max height of images</param>
        /// <param name="coverImageQuality">Compress images while resizing (value: 0 - 100)</param>
        /// <param name="fileFilter">Use given pattern to filter files which should be processed</param>
        /// <param name="overwriteImagesFromFile">Overwrite all embedded images or add image from file, if specific image file can be found in same folder</param>
        /// <param name="overwriteImageName">Name of the image file, which should be used for overwriting</param>
        public AlbumCoverResizer(SimpleConsoleLogger logger, int coverWidthPx = 500, int coverHeightPx = 500, int coverImageQuality = 90,
            string fileFilter = "*.mp3", bool overwriteImagesFromFile = false, string overwriteImageName = "cover.jpg") {
            Logger = logger;

            CoverImageWidthPx = coverWidthPx;
            CoverImageHeightPx = coverHeightPx;
            CoverImageQuality = coverImageQuality;
            FileFilter = fileFilter;
            OverwriteImageFromFile = overwriteImagesFromFile;
            OverwriteImageName = overwriteImageName;
        }

        /// <summary>
        ///     Resize given image
        /// </summary>
        /// <param name="imgStream">Image data</param>
        /// <returns>Stream with data of resized image</returns>
        private Stream ResizeImage(Stream imgStream)
        {
            imgStream.Seek(0, SeekOrigin.Begin);
            var img = new ImageMagick.MagickImage(imgStream);

            img.Resize(CoverImageWidthPx, CoverImageHeightPx);
            img.Quality = CoverImageQuality;

            var resizedImg = new MemoryStream();
            img.Write(resizedImg);
            return resizedImg;
        }

        /// <summary>
        ///     Removes all embedded images from MP3 file
        /// </summary>
        /// <param name="tag">Tag to remove images from</param>
        private void RemoveEmbeddedImages(Id3Tag tag)
        {
            try
            {
                tag.Pictures.Clear();
            }
            catch (Exception ex)
            {
                Logger?.LogError($"Could not remove images. Error Message: {ex.Message}");
            }
        }

        /// <summary>
        ///     Adds a new image to the tags of the given MP3 file
        /// </summary>
        /// <param name="tag">Tag to add image too</param>
        /// <param name="imgPath">Path to image</param>
        private void AddNewImage(Id3Tag tag, string imgPath)
    {
            FileStream imageData = null;

            try
            {
                var imageFrame = new Id3.Frames.PictureFrame();
                imageData = File.Open(imgPath, FileMode.Open);
                imageFrame.LoadImage(imageData);
                tag.Pictures.Add(imageFrame);
            }
            catch (Exception ex)
            {
                Logger?.LogError($"Could not add new image. Error Message: {ex.Message}");
            }
            finally
            {
                imageData?.Close();
            }
        }

        /// <summary>
        ///     Resize all embedded images of the given file
        /// </summary>
        /// <param name="tag">Tag containing images which should be resized</param>
        private void ResizeEmbeddedImages(Id3Tag tag)
        {
            Stream imageStream = null;
            Stream resizedStream = null;

            try
            {
                foreach (var pic in tag.Pictures)
                {
                    imageStream = new MemoryStream();
                    pic.SaveImage(imageStream);
                    resizedStream = ResizeImage(imageStream);
                    resizedStream.Seek(0, SeekOrigin.Begin);
                    pic.LoadImage(resizedStream);
                    imageStream.Close();
                    resizedStream.Close();
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError($"Could not resize images. Error Message: {ex.Message}");
            }
            finally
            {
                imageStream?.Close();
                resizedStream?.Close();
            }
        }

        /// <summary>
        ///     Process single MP3 file and resize all pictures embedded in the tags or overwrite them
        /// </summary>
        /// <param name="filePath">Path to audio file</param>
        private void ProcessFile(string filePath)
        {
            var file = new Mp3File(filePath, Mp3Permissions.ReadWrite);

            if (file.HasTagOfFamily(Id3TagFamily.Version2x))
            {
                try
                {
                    var tag = file.GetTag(Id3TagFamily.Version2x);

                    if (OverwriteImageFromFile)
                    {
                        var imgPath = Path.Join(FileHelper.GetDirectoryName(filePath), OverwriteImageName);
                        if (FileHelper.IsFile(imgPath))
                        {
                            RemoveEmbeddedImages(tag);
                            AddNewImage(tag, imgPath);
                        }
                    }

                    ResizeEmbeddedImages(tag);

                    file.WriteTag(tag, WriteConflictAction.Replace);
                }
                catch (Exception ex)
                {
                    Logger?.LogError($"Could not modify tag for {filePath}. Error Message: {ex.Message}");
                }
            }
            else
            {
                Logger?.LogError("File does not have tags of ID3 Version 2. Can't remove embedded covers.");
            }

            file.Dispose();
        }

        /// <summary>
        ///     Process all MP3 files inside a given directory and resize all pictures embedded in the tags of every file
        /// </summary>
        /// <param name="dirPath">Path to directory containing files, which should be processed</param>
        /// <param name="includeSubdirectories">Also process files in possible subdirectories</param>
        public void Resize(string dirPath, bool includeSubdirectories = true)
        {
            var processedFiles = 1;

            Logger?.LogInfo($">> Starting resize process for directory {dirPath}");
            Logger?.LogInfo($">> New max size of album covers: {CoverImageWidthPx}x{CoverImageHeightPx} Pixels");

            var files = FileHelper.GetAllFiles(dirPath, FileFilter, includeSubdirectories);

            Logger?.LogInfo($">> Found {files.Count} files, which will be processed");

            foreach(var file in files)
            {
                Logger?.LogInfo($"({processedFiles}/{files.Count}) Processing file {file.FullName}");
                ProcessFile(file.FullName);
                processedFiles++;
            }
        }
    }
}
