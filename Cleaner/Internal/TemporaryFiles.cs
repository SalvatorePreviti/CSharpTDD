using System;
using System.Collections.Generic;
using System.IO;

namespace Cleaner.Internal
{
    internal static class TemporaryFiles
    {
        private static HashSet<string> _temporaryExtensions = new HashSet<string>(new string[]{
            ".tmp",
            ".temp",
            ".log",
        }, StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Returns true if a file name is a temporary file that can be deleted.
        /// </summary>
        public static bool IsTemporaryFile(string filename)
        {
            return _temporaryExtensions.Contains(Path.GetExtension(filename));
        }
    }
}