using System.Collections.Generic;
using Xunit;

namespace SharpAdbClient.Tests
{
    public class BaseDeviceTests
    {
        protected Device GetFirstDevice()
        {
            List<DeviceData> devices = AdbClient.Instance.GetDevices();
            Assert.True(devices.Count >= 1);
            return new Device(devices[0]);
        }
    }
}
