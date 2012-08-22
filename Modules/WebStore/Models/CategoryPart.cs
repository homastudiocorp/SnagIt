using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Magelia.WebStore.Services.Contract.Data;
using WebStore.Services;
using Magelia.WebStore.Services.Contract;
using Orchard;
using System.Threading;

namespace WebStore.Models
{
    public class CategoryPart : ContentPart<CategoryRecord>
    {

        public IOrchardServices OrchardServices { get; set; }

        private IEnumerable<Product> _products;
        private Category _category;

        public Category Category
        {
            get
            {
                if (_products == null)
                {
                    var temp = this.Products;
                }
                return _category;
            }
        }

        public IEnumerable<Product> Products { 
            get {

                if (OrchardServices == null)
                    throw new Exception("OrchardServices must be provided first");


                if (_products == null)
                {
                    var config = new WebStoreConfigurationService(OrchardServices).GetConfiguration();
                    using (var webstoreServices = new WebStoreServices(config))
                    {
                        _category = webstoreServices.CreateWebStoreChannel<ICatalogService>().GetCategory(new GetCategoryParameters { CatalogId = config.CatalogId.Value, CategoryId = this.CategoryGuid, CultureName = Thread.CurrentThread.CurrentUICulture.Name });                         
                        _products = webstoreServices.CreateWebStoreChannel<ICatalogService>().GetProducts(new GetProductsParameters { CountryId = "US   ", CatalogId = config.CatalogId.Value, CategoryId = this.CategoryGuid, CultureName = Thread.CurrentThread.CurrentUICulture.Name });
                    }
                }

                return _products;
            }
        }



        public String Code
        {
            get
            {
                return Record.Code;
            }

            set
            {
                Record.Code = value;
            }
        }

        public Guid CategoryGuid
        {
            get
            {
                return Record.CategoryGuid;
            }

            set
            {
                Record.CategoryGuid = value;
            }
        }

        public DateTime? SynchronizationDate
        {
            get
            {
                return this.Record.SynchronizationDate;
            }
            set
            {
                this.Record.SynchronizationDate = value;
            }
        }

        public String Name { get; set; }
    }
}