using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Drivers;
using WebStore.Models;
using Magelia.WebStore.Web;
using Magelia.WebStore.Services.Clients;
using System.Globalization;
using Magelia.WebStore.Services.Contract;
using WebStore.Services;
using Orchard;

namespace WebStore.Drivers
{
    public class OrderDriver : ContentPartDriver<OrderPart>
    {
        IOrchardServices _orchardServices;
        IBasketServices _basketServices;

        public OrderDriver(IOrchardServices orchardServices, IBasketServices basketServices)
        {
            this._orchardServices = orchardServices;
            this._basketServices = basketServices;
        }

        protected override DriverResult Display(OrderPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_Order", () => shapeHelper.Parts_Order());
        }

    }
}