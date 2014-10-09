using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Plugin.Shipping.Other.Provider
{
    [GatewayProviderActivation("585805D4-1FC1-41D5-A6E8-5CB665A89001", "Other Shipping Provider", "Other Shipping Provider")]
    [GatewayProviderEditor("Other configuration", "~/App_Plugins/Merchello.Other/editor.html")]
    public class OtherShippingGatewayProvider : ShippingGatewayProviderBase, IOtherShippingGatewayProvider
    {                                                                                                             
        private static readonly GatewayResource GatewayResource = new GatewayResource("1", "Other Shipping");

        public OtherShippingGatewayProvider(IGatewayProviderService gatewayProviderService, 
            IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider) : 
            base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        {
        }

        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            return new List<IGatewayResource> { GatewayResource };
        }

        public override IShippingGatewayMethod CreateShippingGatewayMethod(IGatewayResource gatewayResource, IShipCountry shipCountry,
            string name)
        {
            var attempt = GatewayProviderService.CreateShipMethodWithKey(GatewayProviderSettings.Key, shipCountry, name,
                gatewayResource.ServiceCode);

            if (!attempt.Success) throw attempt.Exception;

            return new OtherShippingGatewayMethod(gatewayResource, attempt.Result, shipCountry, GatewayProviderSettings);
        }

        public override void SaveShippingGatewayMethod(IShippingGatewayMethod shippingGatewayMethod)
        {                                                                
            GatewayProviderService.Save(shippingGatewayMethod.ShipMethod);
        }

        public override IEnumerable<IShippingGatewayMethod> GetAllShippingGatewayMethods(IShipCountry shipCountry)
        {
            var methods = GatewayProviderService.GetShipMethodsByShipCountryKey(GatewayProviderSettings.Key, shipCountry.Key);
            return methods
                .Select(
                    shipMethod =>
                        new OtherShippingGatewayMethod(
                            GatewayResource,
                            shipMethod, shipCountry, GatewayProviderSettings)
                ).OrderBy(x => x.ShipMethod.Name);
        }
    }
}
