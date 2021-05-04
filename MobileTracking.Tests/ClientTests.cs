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
            var client = new Client();
            var query = new CalibrationsQuery()
            {
                LocaleId = 1,
                PositionId = 1
            };

            Assert.AreEqual("?localeId=1&positionId=1", client.ConvertQuery(query));
        }
    }
}
