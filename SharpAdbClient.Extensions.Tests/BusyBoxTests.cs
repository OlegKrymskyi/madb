using System;
using System.IO;
using Xunit;

namespace SharpAdbClient.Tests
{
    public class BusyBoxTests : BaseDeviceTests
    {
        [Fact]
        public void GetCommandsTest()
        {
            Device device = GetFirstDevice();
            BusyBox busyBox = new BusyBox(device);

            bool avail = busyBox.Available;
            Assert.True(avail, "BusyBox is not available");

            foreach (var item in busyBox.Commands)
            {
                Console.Write("{0},", item);
            }

            Assert.True(avail && busyBox.Commands.Count > 0);
        }

        [Fact]
        public void InstallTest()
        {
            Device device = GetFirstDevice();
            BusyBox busyBox = new BusyBox(device);

            bool avail = busyBox.Available;
            if (!avail)
            {
                bool result = busyBox.Install("/sdcard/busybox");
                Assert.True(result, "BusyBox Install returned false");
            }

            device.ExecuteShellCommand("printenv", new ConsoleOutputReceiver());

            Assert.True(busyBox.Available, "BusyBox is not installed");
        }

        [Fact]
        public void ExecuteRemoteCommandTest()
        {
            Device device = GetFirstDevice();
            BusyBox busyBox = new BusyBox(device);

            ConsoleOutputReceiver creciever = new ConsoleOutputReceiver();

            Console.WriteLine("Executing 'busybox':");
            bool hasBB = false;
            try
            {
                device.ExecuteShellCommand("busybox", creciever);
                hasBB = true;
            }
            catch (FileNotFoundException)
            {
                hasBB = false;
            }
            finally
            {
                Console.WriteLine("Busybox enabled: {0}", hasBB);
            }


            Console.WriteLine("Executing 'busybox ls': ");
            try
            {
                busyBox.ExecuteShellCommand("ls", creciever);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

    }
}
