using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStore.Models;
using Orchard;

namespace WebStore.Services
{
    public interface IWebStoreConfigurationService : IDependency
    {
        WebStoreConfigurationPart GetConfiguration();
    }
}