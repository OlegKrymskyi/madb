using SharpAdbClient.DeviceCommands;
using System;
using Xunit;

namespace SharpAdbClient.Tests
{
    public class LinuxPathTests
    {
        [Fact]
        public void CombineTest()
        {
            String result = LinuxPath.Combine("/system", "busybox");
            Assert.Equal<string>("/system/busybox", result);

            result = LinuxPath.Combine("/system/", "busybox");
            Assert.Equal<string>("/system/busybox", result);

            result = LinuxPath.Combine("/system/xbin", "busybox");
            Assert.Equal<string>("/system/xbin/busybox", result);

            result = LinuxPath.Combine("/system/xbin/", "busybox");
            Assert.Equal<string>("/system/xbin/busybox", result);

            result = LinuxPath.Combine("/system//xbin", "busybox");
            Assert.Equal<string>("/system/xbin/busybox", result);

            result = LinuxPath.Combine("/system", "xbin", "busybox");
            Assert.Equal<string>("/system/xbin/busybox", result);

            result = LinuxPath.Combine("/system", "xbin", "really", "long", "path", "to", "nothing");
            Assert.Equal<string>("/system/xbin/really/long/path/to/nothing", result);
        }

        [Fact]
        public void CombineCurrentDirTest()
        {
            var result = LinuxPath.Combine(".", "test.txt");
            Assert.Equal("./test.txt", result);
        }

        [Fact]
        public void GetDirectoryNameTest()
        {
            String result = LinuxPath.GetDirectoryName("/system/busybox");
            Assert.Equal<string>("/system/", result);

            result = LinuxPath.GetDirectoryName("/");
            Assert.Equal<string>("/", result);

            result = LinuxPath.GetDirectoryName("/system/xbin/");
            Assert.Equal<string>("/system/xbin/", result);
        }

        [Fact]
        public void GetFileNameTest()
        {
            String result = LinuxPath.GetFileName("/system/busybox");
            Assert.Equal<string>("busybox", result);

            result = LinuxPath.GetFileName("/");
            Assert.Equal<string>("", result);

            result = LinuxPath.GetFileName("/system/xbin/");
            Assert.Equal<string>("", result);

            result = LinuxPath.GetFileName("/system/xbin/file.ext");
            Assert.Equal<string>("file.ext", result);
        }
    }
}
