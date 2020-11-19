using System;
using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace Cleaner.Internal
{

    /// <summary>
    /// A class that helps find duplicate files.
    /// </summary>
    internal class DuplicateFilesFinder
    {
        private IFileSystem _fileSystem;

        private readonly HashSet<string> _hashes = new HashSet<string>(StringComparer.InvariantCulture);

        public DuplicateFilesFinder(IFileSystem fileSystem)
        {
            this._fileSystem = fileSystem;
        }

        /// <summary>
        /// Check if a file is duplicate.
        /// Returns true if a file is new, false if was already added before (duplicate).
        /// </summary>
        public bool TryAddFile(string filepath)
        {
            using (var fileStream = this._fileSystem.File.OpenRead(filepath))
            {
                using (var hashAlgorithm = MD5.Create())
                {
                    string hash = Convert.ToHexString(hashAlgorithm.ComputeHash(fileStream));
                    return this._hashes.Add(hash);
                }
            }
        }
    }
}