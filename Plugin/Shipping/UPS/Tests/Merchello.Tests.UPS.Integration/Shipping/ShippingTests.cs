using System;
using System.Configuration;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Services;
using Merchello.Plugin.Shipping.UPS;
using Merchello.Plugin.Shipping.UPS.Models;
using Merchello.Plugin.Shipping.UPS.Provider;
using Merchello.Tests.UPS.Integration.TestHelpers;
using Merchello.Web.Workflow;
using NUnit.Framework;
using Constants = Merchello.Core.Constants;

namespace Merchello.Tests.UPS.Integration.Shipping
{
        
    [TestFixture]
    public class ShippingTests : ProviderTestsBase
    {
        private IShipCountry _shipCountry;
        private ICustomerBase _customer;
        private IBasket _basket;
        private const int ProductCount = 3;
        private IAddress _destination;
        private const decimal WeightPerProduct = 2M;
        private const decimal PricePerProduct = 10M;
        private IInvoice _invoice;
        protected IGatewayProviderService GatewayProviderService;
        protected IWarehouseCatalog Catalog;
        protected IStoreSettingService StoreSettingService;
        internal IShipCountryService ShipCountryService;

        [SetUp]
        public void Init()
        {
            var billTo = new Address()
            {
                Organization = "Mindfly Web Design Studios",
                Address1 = "114 W. Magnolia St. Suite 504",
                Locality = "Bellingham",
                Region = "WA",
                PostalCode = "98225",
                CountryCode = "US",
                Email = "someone@mindfly.com",
                Phone = "555-555-5555"
            };

            // create an invoice
            var invoiceService = new InvoiceService();

            _invoice = invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);

            _invoice.SetBillingAddress(billTo);

            _invoice.Total = 120M;
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            // make up some line items
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 10, 1, extendedData);
            var l2 = new InvoiceLineItem(LineItemType.Product, "Item 2", "I2", 2, 40, extendedData);
            var l3 = new InvoiceLineItem(LineItemType.Shipping, "Shipping", "shipping", 1, 10M, extendedData);
            var l4 = new InvoiceLineItem(LineItemType.Tax, "Tax", "tax", 1, 10M, extendedData);

            _invoice.Items.Add(l1);
            _invoice.Items.Add(l2);
            _invoice.Items.Add(l3);
            _invoice.Items.Add(l4);

            var x_ups_access_key = ConfigurationManager.AppSettings["x_ups_access_key"];
            var x_ups_username = ConfigurationManager.AppSettings["x_ups_username"];
            var x_ups_password = ConfigurationManager.AppSettings["x_ups_password"];

            var processorSettings = new UPSProcessorSettings()
            {
                UpsAccessKey = x_ups_access_key,
                UpsUserName = x_ups_username,
                UpsPassword = x_ups_password
            };

            Provider.GatewayProviderSettings.ExtendedData.SaveProcessorSettings(processorSettings);

            if (Provider.ShipMethods.Any()) return;

            var type = UPSShippingGatewayMethod.UPSType.UPS2ndDayAir;
            var shipCountry = GatewayProviderService.GetAllShipCountries().FirstOrDefault();


            //// Act
            Provider.CreateShipMethod(type, shipCountry, "upsShipMethod");
        }

        
    }
}
