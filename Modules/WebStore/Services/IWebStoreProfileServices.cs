using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace WebStore.Services
{
    public interface IWebStoreProfileServices : IDependency
    {
        Guid GetUser();
    }
}
