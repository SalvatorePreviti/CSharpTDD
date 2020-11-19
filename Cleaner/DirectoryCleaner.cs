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
            this._fileSystem = fileSystem;
        }

        /// <summary>
        /// Removes all unnecessary files from a directory
        /// </summary>
        public void CleanDirectory(string path)
        {
            var files = this._fileSystem.Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            Array.Sort(files);

            var duplicateFilesFinder = new DuplicateFilesFinder(this._fileSystem);

            foreach (string filepath in files)
            {
                if (TemporaryFiles.IsTemporaryFile(filepath))
                {
                    this.DeleteFile(filepath, "temporary");
                    continue;
                }

                if (!duplicateFilesFinder.TryAddFile(filepath))
                {
                    this.DeleteFile(filepath, "duplicate");
                    continue;
                }
            }
        }

        private void DeleteFile(string filepath, string reason)
        {
            this._fileSystem.File.Delete(filepath);
            this.FileDeleted?.Invoke(new FileDeletedEventArgs(filepath, reason));
        }
    }
}
