using System;
using Orchard.ContentManagement.Records;

namespace WebStore.Models
{
    public class WebStoreConfigurationPartRecord : ContentPartRecord
    {
        public virtual Guid? MerchantId { get; set; }
        public virtual Guid? CatalogId { get; set; }
        public virtual String ServicesPath { get; set; }
        public virtual String PaypalAccount { get; set; }
    }
}