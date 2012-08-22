using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Handlers;
using WebStore.Models;
using Orchard.Data;

namespace WebStore.Handlers
{
    public class CategoryHandler : ContentHandler
    {
        public CategoryHandler(IRepository<CategoryRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }

}