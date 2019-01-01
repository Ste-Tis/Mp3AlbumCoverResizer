using System.Collections.Generic;
using System.IO;

namespace Mp3AlbumCoverResizer
{
    /// <summary>
    ///     Functions to make interactions with files and directories easier
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        ///     List all files inside a given directory
        /// </summary>
        /// <param name="dirPath">Path to directory</param>
        /// <param name="filter">Filter files using given pattern</param>
        /// <param name="includeSubdirectories">Also include files inside subdirectories</param>
        /// <returns></returns>
        public static ICollection<FileInfo> GetAllFiles(string dirPath, string filter = "*", bool includeSubdirectories = true)
        {
            SearchOption searchOption = SearchOption.TopDirectoryOnly;
            if(includeSubdirectories)
            {
                searchOption = SearchOption.AllDirectories;
            }

            var dir = new DirectoryInfo(dirPath);
            var files = dir.GetFiles(filter, searchOption);
            return new List<FileInfo>(files);
        }

        /// <summary>
        ///     Returns the absolute path to the directory containing the given file
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <returns>Path to directory</returns>
        public static string GetDirectoryName(string filePath)
        {
            return new FileInfo(filePath).Directory.FullName;
        }

        /// <summary>
        ///     Checks if the given path leads to an existing file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns>Returns TRUE, if path is valid and leads to existing file, otherwise FALSE</returns>
        public static bool IsFile(string path)
        {
            return File.Exists(path);
        }
    }
}
