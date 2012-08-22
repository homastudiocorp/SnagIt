using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Drivers;
using Magelia.WebStore.Web;
using Magelia.WebStore.Services.Clients;
using System.Globalization;
using WebStore.Models;
using WebStore.Services;
using Orchard;
using Magelia.WebStore.Services.Contract.Data;

namespace WebStore.Drivers
{
    public class BasketDetailsDriver : ContentPartDriver<BasketDetailsPart>
    {
        IOrchardServices _orchardServices;
        IBasketServices _basketServices;

        public BasketDetailsDriver(IOrchardServices orchardServices, IBasketServices basketServices)
        {
            this._orchardServices = orchardServices;
            this._basketServices = basketServices;
        }

        protected override DriverResult Display(BasketDetailsPart part, String displayType, dynamic shapeHelper)
        {
            Basket basket = this._basketServices.GetBasket();
            if (basket != null)
            {
                return ContentShape("Parts_BasketDetails", () => shapeHelper.Parts_BasketDetails(
                    SubTotal: basket.SubTotal,
                    Total: basket.Total,
                    Currency: basket.Currency,
                    CurrencyCode: basket.CurrencyCode,
                    Products: basket.OrderFormHeaders[0].LineItems
                    ));
            }
            else
            {
                return ContentShape("Parts_BasketDetails", () => shapeHelper.Parts_BasketDetails(
                    SubTotal: string.Empty,
                    Total: string.Empty,
                    Currency: string.Empty,
                    CurrencyCode: string.Empty,
                    Products: new List<string>()
                    ));
            }
        }
    }
}