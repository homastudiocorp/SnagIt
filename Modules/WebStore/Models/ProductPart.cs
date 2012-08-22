using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Magelia.WebStore.Services.Contract.Data;
using WebStore.Services;
using Orchard;
using Magelia.WebStore.Services.Contract;
using System.Threading;
using Orchard.Environment;

namespace WebStore.Models
{
    public class ProductPart : ContentPart<ProductRecord>
    {


        public IOrchardServices OrchardServices { private get;  set; }

        private const string WebstoreProducts = "WEBSTORE-PRODUCTS";

        private Product getProduct()
        {
            Dictionary<Guid, Product> products;
            Product product;

            if (this.Record.ProductGuid == Guid.Empty)
            {
                return default(Product);
            }


            if (OrchardServices == null)
                throw new Exception("OrchardServices must be provided first");

            if (HttpContext.Current.Items[WebstoreProducts] == null)
            {
                HttpContext.Current.Items[WebstoreProducts] = new Dictionary<Guid, Product>();
            }

            products = (Dictionary<Guid, Product>)HttpContext.Current.Items[WebstoreProducts];


            if (products.TryGetValue(this.Record.ProductGuid, out product) == false)
            {

                var config = new WebStoreConfigurationService(this.OrchardServices).GetConfiguration();
                using (var webstoreServices = new WebStoreServices(config))
                {
                    product = webstoreServices.CreateWebStoreChannel<ICatalogService>().GetProduct(new GetProductParameters { CatalogId = config.CatalogId.Value, ProductId = this.Record.ProductGuid, CountryId = "US   ", CultureName = Thread.CurrentThread.CurrentUICulture.Name });
                }                

                products.Add(this.Record.ProductGuid, product);
            }

            return product;


        }

        public Product Product
        {
            get
            {
                return getProduct();
            }
        }

        public Guid ProductGuid
        {
            get
            {
                return Record.ProductGuid;
            }

            set
            {
                Record.ProductGuid = value;
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

        public String SKU
        {
            get
            {
                return this.Record.SKU;
            }
            set
            {
                this.Record.SKU = value;
            }
        }
    }
}