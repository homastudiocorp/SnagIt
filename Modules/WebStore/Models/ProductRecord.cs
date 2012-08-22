using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;

namespace WebStore.Models
{
    public class ProductRecord : ContentPartRecord
    {
        public virtual Guid ProductGuid { get; set; }
        public virtual DateTime? SynchronizationDate { get; set; }
        public virtual String SKU { get; set; }

    }
}