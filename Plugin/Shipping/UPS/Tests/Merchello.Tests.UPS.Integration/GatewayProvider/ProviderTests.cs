using System.Configuration;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Plugin.Shipping.UPS;
using Merchello.Plugin.Shipping.UPS.Provider;
using Merchello.Tests.UPS.Integration.TestHelpers;
using Merchello.Web.Models.ContentEditing;
using NUnit.Framework;

namespace Merchello.Tests.UPS.Integration.GatewayProvider
{
    [TestFixture]
    public class ProviderTests : ProviderTestsBase
    {
        [SetUp]
        public void Init()
        {
            foreach (var method in Provider.ShipMethods)
            {
                GatewayProviderService.Delete(method);
            }

        }

        /// <summary>
        /// Test confirms that extended data processor args (which will be saved via the Angular dialog flyout) can be deserialized.
        /// </summary>
        [Test]
        public void Can_Retrieve_ProcessorSettings_From_ExtendedData()
        {
            //// Arrange
            // mainly handled in base class
            var x_ups_access_key = ConfigurationManager.AppSettings["x_ups_access_key"];
            var x_ups_username = ConfigurationManager.AppSettings["x_ups_username"];
            var x_ups_password = ConfigurationManager.AppSettings["x_ups_password"];

            //// Act
            var settings = Provider.GatewayProviderSettings.ExtendedData.GetProcessorSettings();

            //// Assert
            Assert.NotNull(settings);
            Assert.AreEqual(x_ups_access_key, settings.UpsAccessKey);
            Assert.AreEqual(x_ups_username, settings.UpsUserName);
            Assert.AreEqual(x_ups_password, settings.UpsPassword);
        }

        /// <summary>
        /// Test confirms that a payment method can be created
        /// </summary>
        [Test]
        public void Can_Create_ShipMethod()
        {
            //// Arrange
            var type = UPSShippingGatewayMethod.UPSType.UPS2ndDayAir;
            var shipCountry = GatewayProviderService.GetAllShipCountries().FirstOrDefault();
            

            //// Act
            var method = Provider.CreateShipMethod(type, shipCountry, "TestShipment");

            //// Assert
            Assert.NotNull(method);
        }
    }
}