using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Shipping.Other.Models;
using Umbraco.Core;

namespace Merchello.Plugin.Shipping.Other.Provider
{

    [GatewayMethodEditor("Other Shipping Method Editor", "~/App_Plugins/Merchello.Other/shippingmethod.html")]
    public class OtherShippingGatewayMethod : ShippingGatewayMethodBase, IOtherShippingGatewayMethod
    {
        private IShipMethod _shipMethod;
        private readonly OtherProcessorSettings _processorSettings;
        public OtherShippingGatewayMethod(IGatewayResource gatewayResource, IShipMethod shipMethod, IShipCountry shipCountry, IGatewayProviderSettings gatewayProviderSettings) : 
            base(gatewayResource, shipMethod, shipCountry)
        {
            _shipMethod = shipMethod;
        }

        public override Attempt<IShipmentRateQuote> QuoteShipment(IShipment shipment)
        {
            try
            {
                var visitor = new OtherShipmentLineItemVisitor() { UseOnSalePriceIfOnSale = false };
                
                shipment.Items.Accept(visitor);

                var rateQuote = new ShipmentRateQuote(shipment, ShipMethod) {Rate = 0};

                rateQuote.ExtendedData.SetValue(Constants.ExtendedDataKeys.OtherShippingField, visitor.OtherShipmentField);

                shipment.Items.RemoveItem(shipment.ShippingLineItems().First(x => x.Name == "OtherShippingField").Key.ToString());
                
                if (!string.IsNullOrEmpty(visitor.OtherShipmentField))
                {
                    return Attempt<IShipmentRateQuote>.Succeed(rateQuote);
                }

                return Attempt<IShipmentRateQuote>.Fail(
                    new Exception("The Other Shipment field is empty. Please fill out Other Shipment Field and try again."));
                
            }
            catch (Exception ex)
            {
                return Attempt<IShipmentRateQuote>.Fail(
                           new Exception("An error occured during your request : " +
                                                        ex.Message +
                                                        " Please contact your administrator or try again."));
            }
        }
    }
}
