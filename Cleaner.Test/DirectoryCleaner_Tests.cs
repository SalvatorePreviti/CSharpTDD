using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Cleaner.Test
{
    public class DirectoryCleaner_DeletesFiles_Tests
    {
        [Fact]
        public void test_does_not_delete_files_in_a_different_folder()
        {
            var fs = new MockFileSystem();
            fs.AddFile("/system/file1.log", new MockFileData(""));
            fs.AddFile("/system/file2.tmp", new MockFileData(""));
            fs.AddDirectory("/dir");

            var cleaner = new DirectoryCleaner(fs);
            int fileDeletedHandlerCalled = 0;
            cleaner.FileDeleted += (e) => fileDeletedHandlerCalled++;

            cleaner.CleanDirectory("/dir");

            Assert.True(fs.FileExists("/system/file1.log"), "File in another folder should not have been deleted");
            Assert.True(fs.FileExists("/system/file2.tmp"), "File in another folder should not have been deleted");

            Assert.Equal(0, fileDeletedHandlerCalled);
        }

        [Fact]
        public void test_does_not_delete_files_to_keep()
        {
            var fs = new MockFileSystem();
            fs.AddFile("/dir/f1.js", new MockFileData(""));
            fs.AddFile("/dir/f2.c", new MockFileData("a2"));
            fs.AddFile("/dir/subdir/f3.txt", new MockFileData("a3"));

            var cleaner = new DirectoryCleaner(fs);
            int fileDeletedHandlerCalled = 0;
            cleaner.FileDeleted += (e) => fileDeletedHandlerCalled++;

            cleaner.CleanDirectory("/dir");

            Assert.True(fs.FileExists("/dir/f1.js"), "File is supposed to remain");
            Assert.True(fs.FileExists("/dir/f2.c"), "File is supposed to remain");
            Assert.True(fs.FileExists("/dir/subdir/f3.txt"), "File is supposed to remain");

            Assert.Equal(0, fileDeletedHandlerCalled);
        }


        [Fact]
        public void test_deletes_temporary_files()
        {
            var fs = new MockFileSystem();
            fs.AddFile("/dir/f1.tmp", new MockFileData("X1"));
            fs.AddFile("/dir/subdir/f2.log", new MockFileData("X2"));
            fs.AddFile("/dir/subdir/subdir1/f3.Tmp", new MockFileData("X3"));

            var cleaner = new DirectoryCleaner(fs);

            var fileDeletedList = new List<FileDeletedEventArgs>();
            cleaner.FileDeleted += (e) => fileDeletedList.Add(e);

            cleaner.CleanDirectory("/dir");

            Assert.False(fs.FileExists("/dir/f1.tmp"), "File is supposed to be deleted");
            Assert.False(fs.FileExists("/dir/subdir/f2.log"), "File is supposed to be deleted");
            Assert.False(fs.FileExists("/dir/subdir/subdir1/f3.tmp"), "File is supposed to be deleted");

            Assert.Equal(3, fileDeletedList.Count);
            Assert.Equal(Path.GetFullPath("/dir/f1.tmp"), fileDeletedList[0].FilePath);
            Assert.Equal("temporary", fileDeletedList[0].Reason);

            Assert.Equal(Path.GetFullPath("/dir/subdir/f2.log"), fileDeletedList[1].FilePath);
            Assert.Equal("temporary", fileDeletedList[1].Reason);

            Assert.Equal(Path.GetFullPath("/dir/subdir/subdir1/f3.Tmp"), fileDeletedList[2].FilePath);
            Assert.Equal("temporary", fileDeletedList[2].Reason);
        }

        [Fact]
        public void test_deletes_duplicates_sorted()
        {
            var fs = new MockFileSystem();
            fs.AddFile("/dir/f1.txt", new MockFileData("DATA0"));
            fs.AddFile("/dir/subdir/f2.txt", new MockFileData("DATA0"));
            fs.AddFile("/dir/subdir/subdir1/f3.txt", new MockFileData("DATA0"));

            fs.AddFile("/dir/x1.txt", new MockFileData("DATA1"));
            fs.AddFile("/dir/x2.txt", new MockFileData("DATA1"));

            var cleaner = new DirectoryCleaner(fs);

            var fileDeletedList = new List<FileDeletedEventArgs>();
            cleaner.FileDeleted += (e) => fileDeletedList.Add(e);

            cleaner.CleanDirectory("/dir");

            Assert.True(fs.FileExists("/dir/f1.txt"), "File is supposed to stay");
            Assert.True(fs.FileExists("/dir/x1.txt"), "File is supposed to stay");

            Assert.False(fs.FileExists("/dir/subdir/f2.txt"), "File is supposed to be deleted");
            Assert.False(fs.FileExists("/dir/subdir/subdir1/f3.txt"), "File is supposed to be deleted");
            Assert.False(fs.FileExists("/dir/x2.txt"), "File is supposed to be deleted");

            Assert.Equal(3, fileDeletedList.Count);

            Assert.Equal(Path.GetFullPath("/dir/subdir/f2.txt"), fileDeletedList[0].FilePath);
            Assert.Equal("duplicate", fileDeletedList[0].Reason);

            Assert.Equal(Path.GetFullPath("/dir/subdir/subdir1/f3.txt"), fileDeletedList[1].FilePath);
            Assert.Equal("duplicate", fileDeletedList[1].Reason);

            Assert.Equal(Path.GetFullPath("/dir/x2.txt"), fileDeletedList[2].FilePath);
            Assert.Equal("duplicate", fileDeletedList[2].Reason);
        }
    }
}
