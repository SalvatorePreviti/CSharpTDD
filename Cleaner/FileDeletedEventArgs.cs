using System;

namespace Cleaner
{
    public class FileDeletedEventArgs : EventArgs
    {
        public string FilePath { get; set; }
        public string Reason { get; set; }

        public FileDeletedEventArgs(string filepath, string reason)
        {
            this.FilePath = filepath;
            this.Reason = reason;
        }
    }
}