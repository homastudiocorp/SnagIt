using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;

namespace WebStore.Models
{
    public class CategoryRecord : ContentPartRecord
    {
        public virtual Guid CategoryGuid { get; set;}
        public virtual DateTime? SynchronizationDate { get; set; }
        public virtual String Code { get; set; }
    }
}