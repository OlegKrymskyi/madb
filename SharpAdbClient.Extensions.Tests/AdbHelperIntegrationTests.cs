using SharpAdbClient.Exceptions;
using System;
using System.Drawing.Imaging;
using System.IO;
using Xunit;

namespace SharpAdbClient.Tests
{
    public class AdbHelperIntegrationTests : BaseDeviceTests
    {
        [Fact]
        public void DeviceGetMountPointsTest()
        {
            Device device = GetFirstDevice();
            foreach (var item in device.MountPoints.Keys)
            {
                Console.WriteLine(device.MountPoints[item]);
            }

            Assert.True(device.MountPoints.ContainsKey("/system"));
        }

        [Fact]
        public void DeviceRemountMountPointTest()
        {
            Device device = GetFirstDevice();

            Assert.True(device.MountPoints.ContainsKey("/system"), "Device does not contain mount point /system");
            bool isReadOnly = device.MountPoints["/system"].IsReadOnly;

            device.RemountMountPoint(device.MountPoints["/system"], !isReadOnly);

            Assert.Equal<bool>(!isReadOnly, device.MountPoints["/system"].IsReadOnly);
            Console.WriteLine("Successfully mounted /system as {0}", !isReadOnly ? "ro" : "rw");

            // revert it back...
            device.RemountMountPoint(device.MountPoints["/system"], isReadOnly);
            Assert.Equal<bool>(isReadOnly, device.MountPoints["/system"].IsReadOnly);
            Console.WriteLine("Successfully mounted /system as {0}", isReadOnly ? "ro" : "rw");

        }

        [Fact]
        public void ExecuteRemoteCommandTest()
        {

            Device device = GetFirstDevice();
            ConsoleOutputReceiver creciever = new ConsoleOutputReceiver();


            device.ExecuteShellCommand("pm list packages -f", creciever);

            Console.WriteLine("Executing 'ls':");
            try
            {
                device.ExecuteShellCommand("ls -lF --color=never", creciever);
            }
            catch (UnknownOptionException)
            {
                device.ExecuteShellCommand("ls -l", creciever);
            }


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

            Console.WriteLine("Executing 'unknowncommand':");
            try
            {
                device.ExecuteShellCommand("unknowncommand", creciever);
                throw new Xunit.Sdk.XunitException("");
            }
            catch (FileNotFoundException)
            {
                // Expected exception
            }

            Console.WriteLine("Executing 'ls /system/foo'");
            try
            {
                device.ExecuteShellCommand("ls /system/foo", creciever);
                throw new Xunit.Sdk.XunitException("");
            }
            catch (FileNotFoundException)
            {
                // Expected exception
            }

        }

        [Fact]
        public void ExecuteRemoteRootCommandTest()
        {
            Device device = GetFirstDevice();
            ConsoleOutputReceiver creciever = new ConsoleOutputReceiver();

            Console.WriteLine("Executing 'ls':");
            if (device.CanSU())
            {
                try
                {
                    device.ExecuteRootShellCommand("busybox ls -lFa --color=never", creciever);
                }
                catch (UnknownOptionException)
                {
                    device.ExecuteRootShellCommand("ls -lF", creciever);
                }
            }
            else
            {
                // if the device doesn't have root, then we check that it is throwing the PermissionDeniedException
                try
                {
                    try
                    {
                        device.ExecuteRootShellCommand("busybox ls -lFa --color=never", creciever);
                    }
                    catch (UnknownOptionException)
                    {
                        device.ExecuteRootShellCommand("ls -lF", creciever);
                    }

                    throw new Xunit.Sdk.XunitException("");
                }
                catch (PermissionDeniedException)
                {
                    // Expected exception
                }

            }
        }

        [Fact]
        public void DeviceEnvironmentVariablesTest()
        {
            Device device = GetFirstDevice();
            foreach (var key in device.EnvironmentVariables.Keys)
            {
                Console.WriteLine("{0}={1}", key, device.EnvironmentVariables[key]);
            }

            Assert.True(device.EnvironmentVariables.Count > 0);
            Assert.True(device.EnvironmentVariables.ContainsKey("ANDROID_ROOT"));
        }

        [Fact]
        public void DevicePropertiesTest()
        {
            Device device = GetFirstDevice();
            foreach (var key in device.Properties.Keys)
            {
                Console.WriteLine("[{0}]: {1}", key, device.Properties[key]);
            }

            Assert.True(device.Properties.Count > 0);
            Assert.True(device.Properties.ContainsKey("ro.product.device"));
        }
    }
}
