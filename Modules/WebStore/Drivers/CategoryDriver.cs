using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStore.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement;
using Orchard;
using WebStore.Services;
using Magelia.WebStore.Services.Contract.Data;
using Magelia.WebStore.Services.Contract;
using System.Threading;

namespace WebStore.Drivers
{
    public class CategoryDriver : ContentPartDriver<CategoryPart>
    {
        IOrchardServices _orchardServices;

        public CategoryDriver(IOrchardServices orchardServices)
        {
            this._orchardServices = orchardServices;
        }

        protected override DriverResult Display(
            CategoryPart part, string displayType, dynamic shapeHelper)
        {
                Category category;
                IEnumerable<Product> products;
            
            
            

                var config = new WebStoreConfigurationService(this._orchardServices).GetConfiguration();
                using (var webstoreServices = new WebStoreServices(config))
                {
                    category = webstoreServices.CreateWebStoreChannel<ICatalogService>().GetCategory(new GetCategoryParameters { CatalogId = config.CatalogId.Value, CategoryId = part.CategoryGuid, CultureName = Thread.CurrentThread.CurrentUICulture.Name});
                    products = webstoreServices.CreateWebStoreChannel<ICatalogService>().GetProducts(new GetProductsParameters { CountryId = "US   ", CatalogId = config.CatalogId.Value, CategoryId = part.CategoryGuid, CultureName = Thread.CurrentThread.CurrentUICulture.Name });
                }

            return ContentShape("Parts_Category", () => shapeHelper.Parts_Category(ContentManager : _orchardServices.ContentManager, PathManager : (new PathManager()) , Category : category, Products : products));
        }

    }
}
