using System.IO.Abstractions.TestingHelpers;

namespace Cleaner.Test.Fakes
{

    public class FakeDrive
    {
        public MockFileSystem FileSystem;

        public FakeDrive()
        {
            /// A directory that should not be touched
            FileSystem.AddDirectory("c:\\system");
            FileSystem.AddFile("c:\\system\\file1.log", new MockFileData(""));
            FileSystem.AddFile("c:\\system\\file2.log", new MockFileData(""));
            FileSystem.AddFile("c:\\system\\file3.log", new MockFileData(""));
            FileSystem.AddFile("c:\\system\\subdir1\\file6.txt", new MockFileData(""));

            /// A directory that should be cleaned

            FileSystem.AddDirectory("c:\\dir");
            FileSystem.AddDirectory("c:\\dir\\subdir1");
            FileSystem.AddDirectory("c:\\dir\\subdir2\\subdir3");

            /// Create a bunch of files that should not be deleted

            FileSystem.AddFile("c:\\dir\\file1.txt", new MockFileData(""));
            FileSystem.AddFile("c:\\dir\\file2.txt", new MockFileData("this file stays 1"));
            FileSystem.AddFile("c:\\dir\\file3.txt", new MockFileData("this file stays 2"));
            FileSystem.AddFile("c:\\dir\\subdir1\\file6.txt", new MockFileData("this file stays 5"));

            // Create a bunch of temporary files

            FileSystem.AddFile("c:\\dir\\file2.log", new MockFileData("file 1"));
            FileSystem.AddFile("c:\\dir\\file1.tmp", new MockFileData("file 2"));
            FileSystem.AddFile("c:\\dir\\subdir1\\file3.TMP", new MockFileData("file 3"));
            FileSystem.AddFile("c:\\dir\\subdir2\\file4.LoG", new MockFileData("file 4"));
            FileSystem.AddFile("c:\\dir\\subdir2\\subdir3\\file4.Tmp", new MockFileData("file 5"));

            // Create a bunch of duplicate files

            FileSystem.AddFile("c:\\dir\\dupA1.txt", new MockFileData("duplicated content A"));
            FileSystem.AddFile("c:\\dir\\dupA2.txt", new MockFileData("duplicated content A"));
            FileSystem.AddFile("c:\\dir\\subdir1\\dupA1.tmp", new MockFileData("duplicated content A"));

            FileSystem.AddFile("c:\\dir\\dupB1.txt", new MockFileData("duplicated content B"));
            FileSystem.AddFile("c:\\dir\\dupB2.txt", new MockFileData("duplicated content B"));
            FileSystem.AddFile("c:\\dir\\subdir2\\dupB1.tmp", new MockFileData("duplicated content B"));
            FileSystem.AddFile("c:\\dir\\subdir2\\subdir3\\dupB1.tmp", new MockFileData("duplicated content B"));
        }
    }
}