using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SharpAdbClient.Tests
{
    public class FileSystemTests : BaseDeviceTests
    {
        [Fact]
        public void GetDeviceBlocksTest()
        {
            Device d = GetFirstDevice();
            FileSystem fileSystem = new FileSystem(d);

            IEnumerable<FileEntry> blocks = fileSystem.DeviceBlocks;
            foreach (var b in blocks)
            {
                Console.WriteLine(b.ToString());
            }

            Assert.True(blocks.Count() > 0);
        }

        [Fact]
        public void MakeDirectory()
        {
            Device d = GetFirstDevice();
            FileSystem fileSystem = new FileSystem(d);

            var testPath = "/mnt/sdcard/test/delete/";
            Console.WriteLine("Making directory: {0}", testPath);
            fileSystem.MakeDirectory(testPath);
            Assert.True(fileSystem.Exists(testPath));
            Console.WriteLine("Deleting {0}", testPath);
            fileSystem.Delete(testPath);
            Assert.True(!fileSystem.Exists(testPath));

            Console.WriteLine("Making directory (forced): {0}", testPath);
            fileSystem.MakeDirectory(testPath, true);
            Assert.True(fileSystem.Exists(testPath));
            Console.WriteLine("Deleting {0}", testPath);
            fileSystem.Delete(testPath);
            Assert.True(!fileSystem.Exists(testPath));
        }

        [Fact]
        public void ResolveLink()
        {
            Device d = GetFirstDevice();
            FileSystem fileSystem = new FileSystem(d);

            var vendor = fileSystem.ResolveLink("/vendor");
            Assert.Equal("/system/vendor", vendor);
            Console.WriteLine($"/vendor -> {vendor}");

            var nonsymlink = fileSystem.ResolveLink("/system");
            Assert.Equal("/system", nonsymlink);
            Console.WriteLine($"/system -> {nonsymlink}");

            var legacy = "/storage/emulated/legacy";
            var sdcard0 = "/storage/sdcard0";

            var sdcard = fileSystem.ResolveLink("/sdcard");
            // depending on the version of android
            Assert.True(sdcard.Equals(legacy) || sdcard.Equals(sdcard0));
            Console.WriteLine($"/sdcard -> {sdcard}");
        }
    }
}
