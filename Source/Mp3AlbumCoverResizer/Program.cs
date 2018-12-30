using Mp3AlbumCoverResizer.Helper;
using NDesk.Options;
using System.Collections.Generic;

namespace Mp3AlbumCoverResizer
{
    public class Program
    {
        static private void ShowHelp(OptionSet options)
        {
            System.Console.WriteLine("Usage: Mp3AlbumCoverResizer [OPTIONS]+");
            System.Console.WriteLine("  Resize the images included in the tags of all MP3 files in an given directory.");
            System.Console.WriteLine("\nOptions:");
            options.WriteOptionDescriptions(System.Console.Out);
        }

        static void Main(string[] args)
        {
            SimpleConsoleLogger logger = null;
            int coverWidth = 500;
            int coverHeight = 500;
            int imageQuality = 90;
            bool recursive = false;
            bool showHelp = false;
            string path = null;

            var cmdOptions = new OptionSet()
            {
                {
                    "d|dir=", "Directory containing MP3 files, which should be processed",
                    opt => path = opt
                },
                {
                    "w|width=", "New max width of images",
                    opt => {
                        if(opt != null)
                        {
                            if(!int.TryParse(opt, out coverWidth))
                            {
                                showHelp = true;
                            }
                        }
                    }
                },
                {
                    "h|height=", "New max height of images",
                    opt => {
                        if(opt != null)
                        {
                            if(!int.TryParse(opt, out coverHeight))
                            {
                                showHelp = true;
                            }
                        }
                    }
                },
                {
                    "q|quality=", "Compress image after resizing (value: 0 - 100)",
                    opt => {
                        if(opt != null)
                        {
                            if(!int.TryParse(opt, out imageQuality))
                            {
                                showHelp = true;
                            }
                        }
                    }
                },
                {
                    "r|recursive", "Also process files in subdirectories",
                    opt => recursive = opt != null
                },
                {
                    "v|verbose", "Show progress and other messages",
                    opt => {
                        if(opt != null)
                        {
                            logger = new SimpleConsoleLogger(SimpleConsoleLogger.LogLevels.INFO);
                        }
                        else
                        {
                            logger = new SimpleConsoleLogger(SimpleConsoleLogger.LogLevels.SILENT);
                        }
                    }
                },
                {
                    "help", "Show information about usage",
                    opt => showHelp = opt != null
                }
            };

            List<string> extraArgs = null;
            try
            {
                extraArgs = cmdOptions.Parse(args);
            }
            catch(OptionException)
            {
                System.Console.WriteLine("Oops something went wrong. Try 'Mp3AlbumCoverResizer --help' for more information.");
                return;
            }

            if (showHelp)
            {
                ShowHelp(cmdOptions);
                return;
            }

            if (path == null)
            {
                System.Console.WriteLine(@"No path to an directory with MP3 files provided. Please use 'Mp3AlbumCoverResizer -d ""C:\My Music\Amon Amarth\""' to choose a directory.");
                return;
            }

            var resizer = new AlbumCoverResizer(logger);
            resizer.Resize(path, coverWidth, coverHeight, imageQuality, recursive);
        }
    }
}
