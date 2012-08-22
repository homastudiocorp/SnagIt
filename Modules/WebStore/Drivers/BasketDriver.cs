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
using Magelia.WebStore.Services.Contract.Data;

namespace WebStore.Drivers
{
    public class BasketDriver : ContentPartDriver<BasketPart>
    {
        IOrchardServices _orchardServices;
        IBasketServices _basketServices;

        public BasketDriver(IOrchardServices orchardServices, IBasketServices basketServices)
        {
            this._orchardServices = orchardServices;
            this._basketServices = basketServices;
        }

        protected override DriverResult Display(BasketPart part, string displayType, dynamic shapeHelper)
        {            
            IEnumerable<Country> countries = this._basketServices.getCountries();    
            Basket basket = this._basketServices.GetBasket();
            if (basket != default(Basket))
            {
                return ContentShape("Parts_Basket", () => shapeHelper.Parts_Basket(
                    Quantity: basket.OrderFormHeaders[0].LineItems.Count, Countries: countries, Country: _basketServices.CountryId));
            }
            else
            {
                return ContentShape("Parts_Basket", () => shapeHelper.Parts_Basket(Quantity: 0, Countries : countries, Country : _basketServices.CountryId));
            }
        }

    }
}