using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebStore.Models.Order
{
    public class OrderPanelViewModel
    {
        public Boolean HasBasket { get; set; }
        public Boolean IsAuthenticated { get; set; }
        public String UserName { get; set; }
        public String ReturnUrl { get; set; }
    }
}