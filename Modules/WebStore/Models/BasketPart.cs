using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;

namespace WebStore.Models
{
    public class BasketPart : ContentPart
    {
        public Int32 Quantity
        {
            get;
            set;
        }

    }
}