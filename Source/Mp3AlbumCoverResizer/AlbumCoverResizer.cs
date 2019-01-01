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
            get { return CoverImageQuality; }
            set { CoverImageQuality = Math.Min(100, Math.Max(0, value)); }
        }

        /// <summary>
        ///     Use given pattern to filter files which should be processed
        /// </summary>
        public string FileFilter { get; set; }

        /// <summary>
        ///     Initialize class instance
        /// </summary>
        /// <param name="logger">Instance of logger to use</param>
        /// <param name="coverWidthPx">New max width of images</param>
        /// <param name="coverHeightPx">New max height of images</param>
        /// <param name="coverImageQuality">Compress images while resizing (value: 0 - 100)</param>
        /// <param name="fileFilter">Use given pattern to filter files which should be processed</param>
        public AlbumCoverResizer(SimpleConsoleLogger logger, int coverWidthPx = 500, int coverHeightPx = 500, int coverImageQuality = 90,
            string fileFilter = "*.mp3") {
            Logger = logger;

            CoverImageWidthPx = coverWidthPx;
            CoverImageHeightPx = coverHeightPx;
            CoverImageQuality = coverImageQuality;
            FileFilter = fileFilter;
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
        ///     Process single MP3 file and resize all pictures embedded in the tags
        /// </summary>
        /// <param name="filePath">Path to audio file</param>
        private void ProcessFile(string filePath)
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
                        resizedStream = ResizeImage(imageStream);
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
