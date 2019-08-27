using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SharpAdbClient.Tests
{
    public class ListingServiceReceiverTests
    {
        /// <summary>
        /// Tests the <see cref="ListingServiceReceiver"/> in a scenario where one of the files could not be
        /// accessed because of an error. This error output is visible in the shell output.
        /// </summary>
        [Fact]
        public void ParseListingWithErrorTest()
        {
            DummyDevice device = new DummyDevice();
            FileEntry root = new FileEntry(device, "/");
            List<FileEntry> entries = new List<FileEntry>();
            List<string> links = new List<string>();

            ListingServiceReceiver receiver = new ListingServiceReceiver(root, entries, links);

            string output = @"drwxr-xr-x root     root              2015-06-01 10:17 acct
drwxrwx--- system   cache             2015-05-13 02:03 cache
-rw-r--r-- root     root          297 1970-01-01 01:00 default.prop
lstat '//factory' failed: Permission denied
lrwxrwxrwx root     root              2015-06-01 10:17 etc -> /system/etc";

            string[] lines = output.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                receiver.AddOutput(line);
            }

            receiver.Flush();
            receiver.FinishLinks();

            Assert.Equal<int>(4, entries.Count);

            // Validate the first entry (/acct/)
            // drwxr-xr-x root     root              2015-06-01 10:17 acct
            Assert.Equal(new DateTime(2015, 6, 1, 10, 17, 00), entries[0].Date);
            Assert.Equal(device, entries[0].Device);
            Assert.True(entries[0].Exists);
            Assert.Equal(0, entries[0].FetchTime);
            Assert.Equal("/acct", entries[0].FullEscapedPath);
            Assert.Equal("/acct/", entries[0].FullPath);
            Assert.Equal("root", entries[0].Group);
            Assert.Null(entries[0].Info);
            Assert.False(entries[0].IsApplicationFileName);
            Assert.False(entries[0].IsApplicationPackage);
            Assert.True(entries[0].IsDirectory);
            Assert.False(entries[0].IsExecutable);
            Assert.False(entries[0].IsLink);
            Assert.False(entries[0].IsRoot);
            Assert.Null(entries[0].LinkName);
            Assert.Equal("acct", entries[0].Name);
            Assert.True(entries[0].NeedFetch);
            Assert.Equal("root", entries[0].Owner);
            Assert.Equal(root, entries[0].Parent);
            Assert.Equal(1, entries[0].PathSegments.Length);
            Assert.Equal("acct", entries[0].PathSegments[0]);
            Assert.Equal("rwxr-tr-t", entries[0].Permissions.ToString());
            Assert.Equal(0, entries[0].Size);
            Assert.Equal(FileListingService.FileTypes.Directory, entries[0].Type);

            // Validate the second entry (/cache)
            // drwxrwx--- system   cache             2015-05-13 02:03 cache
            Assert.Equal(new DateTime(2015, 5, 13, 2, 3, 00), entries[1].Date);
            Assert.Equal(device, entries[1].Device);
            Assert.True(entries[1].Exists);
            Assert.Equal(0, entries[1].FetchTime);
            Assert.Equal("/cache", entries[1].FullEscapedPath);
            Assert.Equal("/cache/", entries[1].FullPath);
            Assert.Equal("cache", entries[1].Group);
            Assert.Null(entries[1].Info);
            Assert.False(entries[1].IsApplicationFileName);
            Assert.False(entries[1].IsApplicationPackage);
            Assert.True(entries[1].IsDirectory);
            Assert.False(entries[1].IsExecutable);
            Assert.False(entries[1].IsLink);
            Assert.False(entries[1].IsRoot);
            Assert.Null(entries[1].LinkName);
            Assert.Equal("cache", entries[1].Name);
            Assert.True(entries[1].NeedFetch);
            Assert.Equal("system", entries[1].Owner);
            Assert.Equal(root, entries[1].Parent);
            Assert.Equal(1, entries[1].PathSegments.Length);
            Assert.Equal("cache", entries[1].PathSegments[0]);
            Assert.Equal("rwxrwx---", entries[1].Permissions.ToString());
            Assert.Equal(0, entries[1].Size);
            Assert.Equal(FileListingService.FileTypes.Directory, entries[1].Type);

            // Validate the third entry (/default.prop)
            // -rw-r--r-- root     root          297 1970-01-01 01:00 default.prop
            Assert.Equal(new DateTime(1970, 1, 1, 1, 0, 0), entries[2].Date);
            Assert.Equal(device, entries[2].Device);
            Assert.True(entries[2].Exists);
            Assert.Equal(0, entries[2].FetchTime);
            Assert.Equal("/default.prop", entries[2].FullEscapedPath);
            Assert.Equal("/default.prop", entries[2].FullPath);
            Assert.Equal("root", entries[2].Group);
            Assert.Null(entries[2].Info);
            Assert.False(entries[2].IsApplicationFileName);
            Assert.False(entries[2].IsApplicationPackage);
            Assert.False(entries[2].IsDirectory);
            Assert.False(entries[2].IsExecutable);
            Assert.False(entries[2].IsLink);
            Assert.False(entries[2].IsRoot);
            Assert.Null(entries[2].LinkName);
            Assert.Equal("default.prop", entries[2].Name);
            Assert.True(entries[2].NeedFetch);
            Assert.Equal("root", entries[2].Owner);
            Assert.Equal(root, entries[2].Parent);
            Assert.Equal(1, entries[2].PathSegments.Length);
            Assert.Equal("default.prop", entries[2].PathSegments[0]);
            Assert.Equal("rw-r--r--", entries[2].Permissions.ToString());
            Assert.Equal(297, entries[2].Size);
            Assert.Equal(FileListingService.FileTypes.File, entries[2].Type);

            // Validate the fourth and final entry (/etc)
            // lrwxrwxrwx root     root              2015-06-01 10:17 etc -> /system/etc
            Assert.Equal(new DateTime(2015, 6, 1, 10, 17, 0), entries[3].Date);
            Assert.Equal(device, entries[3].Device);
            Assert.True(entries[3].Exists);
            Assert.Equal(0, entries[3].FetchTime);
            Assert.Equal("/system/etc", entries[3].FullEscapedPath);
            Assert.Equal("/etc/", entries[3].FullPath);
            Assert.Equal("root", entries[3].Group);
            Assert.Equal("-> /system/etc", entries[3].Info);
            Assert.False(entries[3].IsApplicationFileName);
            Assert.False(entries[3].IsApplicationPackage);
            Assert.True(entries[3].IsDirectory);
            Assert.False(entries[3].IsExecutable);
            Assert.True(entries[3].IsLink);
            Assert.False(entries[3].IsRoot);
            Assert.Equal("/system/etc", entries[3].LinkName);
            Assert.Equal("etc", entries[3].Name);
            Assert.True(entries[3].NeedFetch);
            Assert.Equal("root", entries[3].Owner);
            Assert.Equal(root, entries[3].Parent);
            Assert.Equal(1, entries[3].PathSegments.Length);
            Assert.Equal("etc", entries[3].PathSegments[0]);
            Assert.Equal("rwxrwxrwx", entries[3].Permissions.ToString());
            Assert.Equal(0, entries[3].Size);
            Assert.Equal(FileListingService.FileTypes.DirectoryLink, entries[3].Type);
        }
    }
}
