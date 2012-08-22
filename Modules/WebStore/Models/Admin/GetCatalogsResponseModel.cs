using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Magelia.WebStore.Services.Contract.Data;

namespace WebStore.Models.Admin
{
    public class GetCatalogsResponseModel
    {
        public Boolean Success { get; set; }
        public IEnumerable<Catalog> CatalogSettings { get; set; }
    }
}