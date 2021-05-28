using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobileTracking.Communication;
using MobileTracking.Core.Application;

namespace MobileTracking.Tests
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        public void ConvertQuery()
        {
            var client = new Client(new Configuration());
            var query = new CalibrationsQuery()
            {
                LocaleId = 1,
                PositionId = 1
            };

            Assert.AreEqual("?localeId=1&positionId=1", client.ConvertQuery(query));
        }

        [TestMethod]
        public void ConvertArrayInQuery()
        {
            var client = new Client(new Configuration());
            var query = new CalibrationsQuery()
            {
                CalibrationIds = new int[]{ 1, 2, 3 }
            };

            Assert.AreEqual("?calibrationIds[]=1&calibrationIds[]=2&calibrationIds[]=3", client.ConvertQuery(query));
        }
    }
}
