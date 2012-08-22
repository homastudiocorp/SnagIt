using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Handlers;
using WebStore.Models;
using Orchard.Data;

namespace WebStore.Handlers
{
    public class ProductHandler : ContentHandler
    {
        public ProductHandler(IRepository<ProductRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
    
}