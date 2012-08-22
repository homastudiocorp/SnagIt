using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Magelia.WebStore.Services.Contract.Data;
using Magelia.WebStore.Services.Contract;
using System.Threading;
using Magelia.WebStore.Services.Clients;

namespace WebStore.Models.Order
{
    public class PayViewModel
    {
        public Decimal Total { get; set; }
        public Decimal SubTotal { get; set; }
        public Decimal ShippingCost { get; set; }
        public String Currency { get; set; }
    }
}