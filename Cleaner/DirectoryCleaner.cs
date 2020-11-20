using System;
using System.IO;
using System.IO.Abstractions;
using Cleaner.Internal;

namespace Cleaner
{
    public class DirectoryCleaner
    {
        private IFileSystem _fileSystem;


        /// <summary>
        /// An event that is raised every time a file is deleted.
        /// </summary>
        public event Action<FileDeletedEventArgs> FileDeleted;

        public DirectoryCleaner(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        /// <summary>
        /// Removes all unnecessary files from a directory
        /// </summary>
        public void CleanDirectory(string path)
        {
            var files = _fileSystem.Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            Array.Sort(files);

            var duplicateFilesFinder = new DuplicateFilesFinder(_fileSystem);

            foreach (string filepath in files)
            {
                if (TemporaryFiles.IsTemporaryFile(filepath))
                {
                    DeleteFile(filepath, "temporary");
                    continue;
                }

                if (!duplicateFilesFinder.TryAddFile(filepath))
                {
                    DeleteFile(filepath, "duplicate");
                    continue;
                }

                if (_fileSystem.FileInfo.FromFileName(filepath).Length == 0)
                {
                    DeleteFile(filepath, "empty");
                    continue;
                }
            }
        }

        private void DeleteFile(string filepath, string reason)
        {
            _fileSystem.File.Delete(filepath);
            FileDeleted?.Invoke(new FileDeletedEventArgs(filepath, reason));
        }
    }
}
