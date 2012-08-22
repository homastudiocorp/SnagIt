using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using Magelia.WebStore.Web;
using WebStore.Services;
using Magelia.WebStore.Services.Contract;
using Magelia.WebStore.Services.Contract.Data;
using WebStore.Models;
using System.Threading;

namespace WebStore.Controllers
{
    public class BasketController : Controller
    {
        private const string K_PAYPAL_PAYMENT_URL = "https://www.sandbox.paypal.com/cgi-bin/webscr";

        IWebStoreConfigurationService _webStoreConfigurationServices;
        IBasketServices _basketServices;
        IWebStoreProfileServices _webStoreProfileServices;

        public BasketController(IWebStoreConfigurationService webStoreConfigurationServices, IBasketServices basketServices, IWebStoreProfileServices webStoreProfileServices)
        {
            this._webStoreConfigurationServices = webStoreConfigurationServices;
            this._basketServices = basketServices;
            this._webStoreProfileServices = webStoreProfileServices;
        }

        public ActionResult Add(Guid productId, Guid catalogId)
        {
            AddProductToBasketParameters parameters = new AddProductToBasketParameters
            {
                ProductId = productId,
                CatalogId = catalogId,
                Quantity = 1,
                Culture = Thread.CurrentThread.CurrentUICulture.Name,
                CountryId = "US   ",
                UserId = this._webStoreProfileServices.GetUser()
            };
            if (this._basketServices.GetBasketId().HasValue)
            {
                parameters.BasketId = this._basketServices.GetBasketId().Value;
            }
            using (WebStoreServices webStoreServices = new WebStoreServices(this._webStoreConfigurationServices.GetConfiguration()))
            {
                this._basketServices.SetBasketId(webStoreServices.CreateWebStoreChannel<IOrderService>().AddProductToBasket(parameters).BasketId);
            }
            return this.Redirect(this.Request.UrlReferrer.PathAndQuery);
        }

        [HttpPost]
        public EmptyResult SetContextCountry(String countryId)
        {
            this._basketServices.CountryId = countryId;
            return null;
        }

        public ActionResult Remove(Guid productId, Guid catalogId)
        {
            if (this._basketServices.GetBasketId().HasValue)
            {
                using (WebStoreServices webstoreServices = new WebStoreServices(this._webStoreConfigurationServices.GetConfiguration()))
                {
                    webstoreServices.CreateWebStoreChannel<IOrderService>().UpdateBasket(
                        new UpdateBasketParameters
                        {
                            BasketId = this._basketServices.GetBasketId().Value,
                            UserId = this._webStoreProfileServices.GetUser(),
                            ProductId = productId,
                            Quantity = 0,
                            CountryId = "US   ",
                            CultureName = Thread.CurrentThread.CurrentUICulture.Name,
                            CatalogId = catalogId
                        }
                    );
                }
            }
            return new RedirectResult(this.Request.UrlReferrer.PathAndQuery);
        }

        public ActionResult Update(Guid productId, Guid catalogId, String rawQuantity)
        {
            int quantity;
            if (int.TryParse(rawQuantity, out quantity) && quantity > 0 && this._basketServices.GetBasketId().HasValue)
            {
                IOrderService service = new WebStoreServices(this._webStoreConfigurationServices.GetConfiguration()).CreateWebStoreChannel<IOrderService>();
                service.UpdateBasket(
                    new UpdateBasketParameters
                    {
                        BasketId = this._basketServices.GetBasketId().Value,
                        UserId = new WebStoreProfileService(this._webStoreConfigurationServices).GetUser(),
                        ProductId = productId,
                        Quantity = quantity,
                        CountryId = "US   ",
                        CultureName = Thread.CurrentThread.CurrentUICulture.Name,
                        CatalogId = catalogId
                    }
                );
            }
            return new RedirectResult(this.Request.UrlReferrer.AbsoluteUri);
        }

        public ViewResult ProceedToCheckout()
        {
            Basket basket = this._basketServices.GetBasket();
            WebStoreConfigurationPart configuration = this._webStoreConfigurationServices.GetConfiguration();
            using (WebStoreServices webStoreServices = new WebStoreServices(configuration))
            {
                IOrderService service = webStoreServices.CreateWebStoreChannel<IOrderService>();
                string orderNumber = service.SaveAsOrder(
                    new SaveAsOrderParameters
                    {
                        BasketId = basket.BasketId,
                        UserId = this._webStoreProfileServices.GetUser()
                    }
                );

                String url = String.Concat(this.Request.Url.GetLeftPart(UriPartial.Authority), VirtualPathUtility.ToAbsolute("~/"));

                Models.Paypal.IndexViewModel viewModel = new Models.Paypal.IndexViewModel { Business = configuration.PaypalAccount, CurrencyCode = basket.CurrencyCode, OrderNumber = orderNumber, Price = basket.Total.ToString(".00"), ReturnUrl = url, Target = K_PAYPAL_PAYMENT_URL };

                return View(viewModel);
            }
        }
    }
}
