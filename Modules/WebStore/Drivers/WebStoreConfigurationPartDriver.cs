using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JetBrains.Annotations;
using Orchard.ContentManagement.Drivers;
using WebStore.Models;
using Orchard.UI.Notify;
using Orchard;
using Orchard.Localization;
using WebStore.Services;
using Magelia.WebStore.Services.Contract;
using System.Threading;
using System.Web.Mvc;

namespace WebStore.Drivers
{
    [UsedImplicitly]
    public class WebStoreConfigurationPartDriver : ContentPartDriver<WebStoreConfigurationPart>
    {
        private const String TemplateName = "Parts/Webstore.Configuration";
        private readonly INotifier _notifier;
        public Localizer T { get; set; }

        public WebStoreConfigurationPartDriver(INotifier notifier, IOrchardServices services)
        {
            this._notifier = notifier;
            this.T = NullLocalizer.Instance;
        }

        protected override DriverResult Editor(WebStoreConfigurationPart part, dynamic shapeHelper)
        {
            part.Merchants = new List<SelectListItem>();
            part.Catalogs = new List<SelectListItem>();
            if (part != default(WebStoreConfigurationPart) && part.IsComplete)
            {
                try
                {
                    WebStoreServices webstoreServices = new WebStoreServices(part);
                    part.Merchants.AddRange(webstoreServices.CreateWebStoreChannel<IProfileService>().GetMerchants().OrderBy(m => m.Name).Select(m => new SelectListItem { Text = m.Name, Value = m.MerchantId.ToString(), Selected = m.MerchantId == part.MerchantId.Value }));
                    part.Catalogs.AddRange(webstoreServices.CreateWebStoreChannel<ICatalogService>().GetCatalogs(new GetCatalogsParameters { CultureName = Thread.CurrentThread.CurrentUICulture.Name, MerchantId = part.MerchantId.Value }).OrderBy(c => c.Name).Select(c => new SelectListItem { Text = String.IsNullOrEmpty(c.Name) ? c.Code : c.Name, Value = c.CatalogId.ToString(), Selected = c.CatalogId == part.CatalogId.Value }));
                }
                catch (Exception)
                {
                    this._notifier.Error(this.T("Unable to contact configured WebServices"));
                }
            }
            return this.ContentShape("Parts_WebStore_Configuration", () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: part, Prefix: this.Prefix));
        }

        protected override DriverResult Editor(WebStoreConfigurationPart part, Orchard.ContentManagement.IUpdateModel updater, dynamic shapeHelper)
        {
            if (updater.TryUpdateModel(part, this.Prefix, null, null))
            {
                this._notifier.Information(this.T("WebStore configuration updated successfully"));
            }
            else
            {
                this._notifier.Information(this.T("Error during WebStore configuration update"));
            }
            return this.Editor(part, shapeHelper);
        }
    }
}