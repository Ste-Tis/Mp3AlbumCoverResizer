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
    }
}
