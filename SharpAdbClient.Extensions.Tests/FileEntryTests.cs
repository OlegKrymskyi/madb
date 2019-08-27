using Xunit;

namespace SharpAdbClient.Tests
{
    public class FileEntryTests : BaseDeviceTests
    {
        [Fact]
        public void FindEntryTest()
        {
            Device device = GetFirstDevice();
            FileListingService fileListingService = new FileListingService(device);

            FileEntry fe = FileEntry.Find(device, fileListingService, "/system/");

            fe = FileEntry.Find(device, fileListingService, "/system/bin/");

            fe = FileEntry.Find(device, fileListingService, "/mnt/sdcard/Android/data/com.camalotdesigns.myandroider/Injector.jar");
            // test links
            fe = FileEntry.Find(device, fileListingService, "/sdcard/Android/data/com.camalotdesigns.myandroider/Injector.jar");
        }

        [Fact]
        public void FindOrCreateTest()
        {
            Device device = GetFirstDevice();
            FileListingService fileListingService = new FileListingService(device);
            FileSystem fileSystem = new FileSystem(device);

            var path = "/mnt/sdcard/test/delete/";
            FileEntry fe = FileEntry.FindOrCreate(device, fileListingService, path);
            Assert.True(fe.Exists);
            fileSystem.Delete(fe.FullResolvedPath);
        }
    }
}
