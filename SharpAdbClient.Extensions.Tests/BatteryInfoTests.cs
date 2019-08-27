using System;
using Xunit;

namespace SharpAdbClient.Tests
{
    public class BatteryInfoTests : BaseDeviceTests
    {
        [Fact]
        public void GetBatteryInfoTest()
        {
            Device device = GetFirstDevice();
            Assert.NotNull(device);

            var batteryInfo = device.GetBatteryInfo();
            Assert.True(batteryInfo.Present);
            Console.WriteLine(batteryInfo.ToString());
        }
    }
}
