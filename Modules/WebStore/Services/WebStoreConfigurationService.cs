using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JetBrains.Annotations;
using Orchard;
using WebStore.Models;
using Orchard.ContentManagement;

namespace WebStore.Services
{
    [UsedImplicitly]
    public class WebStoreConfigurationService : IWebStoreConfigurationService
    {
        private readonly IOrchardServices _orchardServices;

        public WebStoreConfigurationService(IOrchardServices orchardServices)
        {
            this._orchardServices = orchardServices;
        }

        public WebStoreConfigurationPart GetConfiguration()
        {
            return this._orchardServices.ContentManager.Query<WebStoreConfigurationPart, WebStoreConfigurationPartRecord>().List().FirstOrDefault();
        }
    }
}