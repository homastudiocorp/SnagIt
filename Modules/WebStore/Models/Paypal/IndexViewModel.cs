using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebStore.Models.Paypal
{
    public class IndexViewModel
    {
        public String Target { get; set; }
        public String Business { get; set; }
        public String ReturnUrl { get; set; }
        public String OrderNumber { get; set; }
        public String Price { get; set; }
        public String CurrencyCode { get; set; }
    }
}