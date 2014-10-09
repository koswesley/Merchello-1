using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Shipping.Other.Models;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;

namespace Merchello.Plugin.Shipping.Other
{
    public class OtherEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
                                              ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<OtherEvents>("Initializing Other Shipping provider registration binding events");


            GatewayProviderService.Saving += delegate(IGatewayProviderService sender, SaveEventArgs<IGatewayProviderSettings> args)
            {
                var key = new Guid("585805D4-1FC1-41D5-A6E8-5CB665A89001");
                var provider = args.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);
                if (provider == null) return;

                provider.ExtendedData.SaveProcessorSettings(new OtherProcessorSettings());

            };
        }
    }
}
