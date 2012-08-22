using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using WebStore.Models;

namespace WebStore.Handlers
{
    public class WebStoreConfigurationPartHandler : ContentHandler
    {
        public WebStoreConfigurationPartHandler(IRepository<WebStoreConfigurationPartRecord> repository)
        {
            Filters.Add(new ActivatingFilter<WebStoreConfigurationPart>("Site"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}