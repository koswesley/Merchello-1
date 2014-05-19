using System;
using System.Configuration;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Shipping.UPS;
using Merchello.Plugin.Shipping.UPS.Models;
using Merchello.Plugin.Shipping.UPS.Provider;
using Moq;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Persistence;
using Constants = Merchello.Core.Constants;
using Merchello.Core.Gateways;
using PetaPocoUnitOfWorkProvider = Merchello.Core.Persistence.UnitOfWork.PetaPocoUnitOfWorkProvider;

namespace Merchello.Tests.UPS.Integration.TestHelpers
{
    public abstract class ProviderTestsBase
    {
        protected IGatewayProviderSettings GatewayProviderSettings;
        protected IGatewayProviderService GatewayProviderService;
        protected UPSShippingGatewayProvider Provider;
        private CacheHelper cache;
        private IServiceContext serviceContext;

        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {

            //if (!GatewayProviderResolver.HasCurrent)
            //    GatewayProviderResolver.Current = new GatewayProviderResolver(PluginManager.Current.ResolveGatewayProviders(), serviceContext.GatewayProviderService, cache.RuntimeCache);

            SqlSyntaxProviderTestHelper.EstablishSqlSyntax();

            var cacheProvider = new Mock<IRuntimeCacheProvider>();

            GatewayProviderService = new GatewayProviderService();

            var providers =
                GatewayProviderService.GetAllGatewayProviders()
                    .Where(x => x.GatewayProviderType == GatewayProviderType.Shipping);

            GatewayProviderSettings = providers.FirstOrDefault(x => x.Key == new Guid("AEB14625-B9DE-4DE8-9C92-99204D340342"));

            if (GatewayProviderSettings != null)
            {
                GatewayProviderService.Delete(GatewayProviderSettings);
            }

            var petaPoco = new PetaPocoUnitOfWorkProvider();

            var x_ups_access_key = ConfigurationManager.AppSettings["x_ups_access_key"];
            var x_ups_username = ConfigurationManager.AppSettings["x_ups_username"];
            var x_ups_password = ConfigurationManager.AppSettings["x_ups_password"];

            var sql = new Sql();

            var dto = new GatewayProviderSettingsDto()
            {
                Key = new Guid("AEB14625-B9DE-4DE8-9C92-99204D340342"),
                Name = "UPS",
                Description = "UPS",
                ExtendedData = "<extendedData />",
                EncryptExtendedData = false,
                ProviderTfKey = Constants.TypeFieldKeys.GatewayProvider.ShippingProviderKey,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };


            petaPoco.GetUnitOfWork().Database.Insert(dto);

            GatewayProviderSettings =
                GatewayProviderService.GetGatewayProviderByKey(new Guid("AEB14625-B9DE-4DE8-9C92-99204D340342"));

            var providerSettings = new UPSProcessorSettings()
            {
                UpsAccessKey = x_ups_access_key,
                UpsUserName = x_ups_username,
                UpsPassword = x_ups_password
            };

            GatewayProviderSettings.ExtendedData.SaveProcessorSettings(providerSettings);

            Provider = new UPSShippingGatewayProvider(GatewayProviderService, GatewayProviderSettings,
                cacheProvider.Object);
        }
    }
}