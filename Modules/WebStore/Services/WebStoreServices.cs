using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStore.Models;
using System.ServiceModel;
using Magelia.WebStore.Services.Contract;
using Orchard;
using JetBrains.Annotations;

namespace WebStore.Services
{
    [UsedImplicitly]
    public class WebStoreServices : IDisposable
    {
        private const String EndPointPatern = "{0}{1}.svc";
        private List<ChannelFactory> channelFactories;
        private String _servicesPath;

        public WebStoreServices(WebStoreConfigurationPart configuration)
        {
            this._servicesPath = configuration.ServicesPath;
            this.InitChannelFactoriesList();
        }

        public WebStoreServices(String servicesPath)
        {
            this._servicesPath = servicesPath;
            this.InitChannelFactoriesList();
        }

        private void InitChannelFactoriesList()
        {
            this.channelFactories = new List<ChannelFactory>();
        }

        public T CreateWebStoreChannel<T>()
        {
            String endpointResource = String.Empty;
            var typeOfT = typeof(T);
            if (typeOfT == typeof(ICatalogService))
            {
                endpointResource = "CatalogService";
            }
            else if (typeOfT == typeof(IContentService))
            {
                endpointResource = "ContentService";
            }
            else if (typeOfT == typeof(IOrderService))
            {
                endpointResource = "OrderService";
            }
            else if (typeOfT == typeof(IProfileService))
            {
                endpointResource = "ProfileService";
            }
            else
            {
                throw new Exception("Unknown service contract");
            }

            var channelFactory = new ChannelFactory<T>(new BasicHttpBinding(), new EndpointAddress(String.Format(EndPointPatern, this._servicesPath, endpointResource)));
            this.channelFactories.Add(channelFactory);
            return channelFactory.CreateChannel();
        }

        public void Dispose()
        {
            this.channelFactories.ForEach(cf => cf.Close());
        }
    }
}