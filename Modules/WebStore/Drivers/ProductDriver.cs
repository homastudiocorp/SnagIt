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
using Orchard.UI.Notify;

namespace WebStore.Drivers
{
    public class ProductDriver : ContentPartDriver<ProductPart>
    {

        private IOrchardServices _orchardServices;
        private INotifier _notifier;

        public ProductDriver(IOrchardServices orchardServices, INotifier notifier)
        {
            this._notifier = notifier;
            this._orchardServices = orchardServices;
        }

        protected override DriverResult Editor(
            ProductPart part, dynamic shapeHelper)
        {
            part.OrchardServices = _orchardServices;
            if (part.Product == default(Product))
            {
                this._notifier.Add(NotifyType.Warning, new Orchard.Localization.LocalizedString("To add a new Product, please use the Synchronization utility."));
                return ContentShape("Parts_ProductEdit", () => shapeHelper.Parts_ProductEdit(Sku: ""));
            }
            else
            {
                return ContentShape("Parts_ProductEdit", () => shapeHelper.Parts_ProductEdit(Sku: part.Product.Sku));
            }
        }

    }
}