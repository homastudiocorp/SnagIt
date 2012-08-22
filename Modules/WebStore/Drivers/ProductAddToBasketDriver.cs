using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStore.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement;
using WebStore.Services;
using Magelia.WebStore.Services.Contract.Data;
using Orchard;
using Magelia.WebStore.Services.Contract;
using System.Threading;

namespace WebStore.Drivers
{
    public class ProductAddToBasketDriver : ContentPartDriver<ProductPart>
    {

        private IOrchardServices _orchardServices;

        public ProductAddToBasketDriver(IOrchardServices orchardServices)
        {
            this._orchardServices = orchardServices;
        }

        protected override DriverResult Display(
            ProductPart part, string displayType, dynamic shapeHelper)
        {
            part.OrchardServices = _orchardServices;
            return ContentShape("Parts_ProductAddToBasket", () => shapeHelper.Parts_ProductAddToBasket(Product: part.Product));
        }
        /*
        //GET
        protected override DriverResult Editor(
            ProductPart part, dynamic shapeHelper)
        {

            return ContentShape("Parts_Map_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/Map",
                    Model: part,
                    Prefix: Prefix));
        }
        //POST
        protected override DriverResult Editor(
            ProductPart part, IUpdateModel updater, dynamic shapeHelper)
        {

            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
         * */
    }
}