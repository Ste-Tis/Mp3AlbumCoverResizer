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
        /// <summary>
        ///     Logging
        /// </summary>
        protected SimpleConsoleLogger Logger { get; } = null;

        /// <summary>
        ///     Initialize class instance
        /// </summary>
        /// <param name="logger">Instance of logger to use</param>
        public AlbumCoverResizer(SimpleConsoleLogger logger) {
            Logger = logger;
        }

        /// <summary>
        ///     Resize given image
        /// </summary>
        /// <param name="imgStream">Image data</param>
        /// <param name="coverWidthPx">New width of image</param>
        /// <param name="coverHeightPx">New Height of image</param>
        /// <param name="coverImageQuality">Change quality of image to this value (0-100)</param>
        /// <returns>Stream with data of resized image</returns>
        private Stream ResizeImage(Stream imgStream, int coverWidthPx = 500, int coverHeightPx = 500, int coverImageQuality = 90)
        {
            if(coverImageQuality < 0)
            {
                coverImageQuality = 0;
            }
            if(coverImageQuality > 100)
            {
                coverImageQuality = 100;
            }

            imgStream.Seek(0, SeekOrigin.Begin);
            var img = new ImageMagick.MagickImage(imgStream);

            img.Resize(coverWidthPx, coverHeightPx);
            img.Quality = coverImageQuality;

            var resizedImg = new MemoryStream();
            img.Write(resizedImg);
            return resizedImg;
        }

        /// <summary>
        ///     Process single MP3 file and resize all pictures embedded in the tags
        /// </summary>
        /// <param name="filePath">Path to audio file</param>
        /// <param name="coverWidthPx">New width of image</param>
        /// <param name="coverHeightPx">New Height of image</param>
        /// <param name="coverImageQuality">Change quality of image to this value (0-100)</param>
        private void ProcessFile(string filePath, int coverWidthPx = 500, int coverHeightPx = 500, int coverImageQuality = 90)
        {
            var file = new Mp3File(filePath, Mp3Permissions.ReadWrite);

            if(file.HasTagOfFamily(Id3TagFamily.Version2x))
            {
                Stream imageStream = null;
                Stream resizedStream = null;

                try
                {
                    var tag = file.GetTag(Id3TagFamily.Version2x);

                    foreach (var pic in tag.Pictures)
                    {
                        imageStream = new MemoryStream();
                        pic.SaveImage(imageStream);
                        resizedStream = ResizeImage(imageStream, coverWidthPx, coverHeightPx, coverImageQuality);
                        resizedStream.Seek(0, SeekOrigin.Begin);
                        pic.LoadImage(resizedStream);
                        imageStream.Close();
                        resizedStream.Close();
                    }

                    file.WriteTag(tag, WriteConflictAction.Replace);
                }
                catch(Exception ex)
                {
                    Logger?.LogError($"Could not process file {filePath}. Error Message: {ex.Message}");
                }
                finally
                {
                    imageStream?.Close();
                    resizedStream?.Close();
                }
            }
            else
            {
                Logger?.LogError("File does not have tags of ID3 Version 2. No cover available.");
            }
        }

        /// <summary>
        ///     Process all MP3 files inside a given directory and resize all pictures embedded in the tags of every file
        /// </summary>
        /// <param name="dirPath">Path to directory containing files, which should be processed</param>
        /// <param name="coverWidthPx">New width of image</param>
        /// <param name="coverHeightPx">New Height of image</param>
        /// <param name="coverImageQuality">Change quality of image to this value (0-100)</param>
        /// <param name="includeSubdirectories">Also process files in possible subdirectories</param>
        /// <param name="fileFilter">Filter to only process certain files</param>
        public void Resize(string dirPath, int coverWidthPx = 500, int coverHeightPx = 500, int coverImageQuality = 90, bool includeSubdirectories = true, string fileFilter = "*.mp3")
        {
            var processedFiles = 1;

            Logger?.LogInfo($">> Starting resize process for directory {dirPath}");
            Logger?.LogInfo($">> New max size of album covers: {coverWidthPx}x{coverHeightPx} Pixels");

            var files = FileHelper.GetAllFiles(dirPath, fileFilter, includeSubdirectories);

            Logger?.LogInfo($">> Found {files.Count} files, which will be processed");

            foreach(var file in files)
            {
                Logger?.LogInfo($"({processedFiles}/{files.Count}) Processing file {file.FullName}");
                ProcessFile(file.FullName, coverWidthPx, coverHeightPx, coverImageQuality);
                processedFiles++;
            }
        }
    }
}
