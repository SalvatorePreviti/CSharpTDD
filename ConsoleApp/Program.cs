using System;
using System.IO;
using System.IO.Abstractions;
using Cleaner;

namespace ConsoleApp
{
    public static class AppMain
    {
        public static int Main(string[] argv)
        {
            if (argv.Length != 1)
            {
                Console.WriteLine("Error: First argument must be a directory.");
                return -1;
            }

            var filesystem = new FileSystem();
            var directoryCleaner = new DirectoryCleaner(filesystem);

            directoryCleaner.FileDeleted += (e) =>
            {
                System.Console.WriteLine("Deleted " + e.FilePath + " because " + e.Reason);
            };

            directoryCleaner.CleanDirectory(argv[0]);

            return 0;
        }
    }
}
