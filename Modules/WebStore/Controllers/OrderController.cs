using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Services;
using Magelia.WebStore.Services.Contract;
using Magelia.WebStore.Services.Contract.Data;
using System.Threading;
using WebStore.Models;
using Orchard.Localization;
using WebStore.Models.Order;

namespace WebStore.Controllers
{
    public class OrderController : Controller
    {
        private IWebStoreConfigurationService _webStoreConfigurationServices;
        private IBasketServices _basketServices;
        private IWebStoreProfileServices _webstoreProfileServices;
        private Localizer T;

        private const String BillingAddressSessionKey = "BillingAddress";
        private const String ShippingAddressSessionKey = "ShippingAddress";

        private AddressViewModel BillingAddress
        {
            get
            {
                return System.Web.HttpContext.Current.Session[BillingAddressSessionKey] as AddressViewModel;
            }
            set
            {
                System.Web.HttpContext.Current.Session[BillingAddressSessionKey] = value;
            }
        }

        private AddressViewModel ShippingAddress
        {
            get
            {
                return System.Web.HttpContext.Current.Session[ShippingAddressSessionKey] as AddressViewModel;
            }
            set
            {
                System.Web.HttpContext.Current.Session[ShippingAddressSessionKey] = value;
            }
        }

        public OrderController(IWebStoreConfigurationService webStoreConfigurationServices, IBasketServices basketServices, IWebStoreProfileServices webstoreProfileServices)
        {
            this.T = NullLocalizer.Instance;
            this._basketServices = basketServices;
            this._webstoreProfileServices = webstoreProfileServices;
            this._webStoreConfigurationServices = webStoreConfigurationServices;
        }

        private IEnumerable<Country> GetAvailableCountries()
        {
            WebStoreConfigurationPart configuration = this._webStoreConfigurationServices.GetConfiguration();
            WebStoreServices webStoreServices = new WebStoreServices(configuration);
            IEnumerable<Country> availableCountries = webStoreServices.CreateWebStoreChannel<IProfileService>().GetCountries(new GetCountriesParameters { MerchantId = configuration.MerchantId.Value, CultureName = Thread.CurrentThread.CurrentUICulture.Name }).OrderBy(c => c.DisplayName);
            webStoreServices.Dispose();
            return availableCountries;
        }

        [HttpGet]
        public ActionResult OrderPanel()
        {
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Boolean hasBasket = this._basketServices.GetBasket() != default(Basket);
            if (this.User.Identity.IsAuthenticated)
            {
                return this.Address(false);
            }
            else
            {
                return this.View(new OrderPanelViewModel { HasBasket = hasBasket, UserName = this.User.Identity.Name, IsAuthenticated = this.User.Identity.IsAuthenticated, ReturnUrl = VirtualPathUtility.ToAbsolute("~/WebStore/Order") });
            }
        }

        [HttpGet]
        public ActionResult Address(Boolean express)
        {
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            IEnumerable<Country> availableCountries = this.GetAvailableCountries();
            AddressViewModel viewModel = (this.BillingAddress == default(AddressViewModel)) ? new AddressViewModel { AddressType = AddressViewModel.AddressTypes.Billing } : this.BillingAddress;
            viewModel.AvailableCountries = availableCountries;
            return this.View("~/Modules/WebStore/Views/Order/Address.cshtml", viewModel);
        }

        [HttpPost]
        public ActionResult ShippingMethods(ShippingMethodsViewModel viewModel)
        {
            WebStoreServices webStoreServices = new WebStoreServices(this._webStoreConfigurationServices.GetConfiguration());
            if (this.ModelState.IsValid)
            {
                IOrderService orderServices = webStoreServices.CreateWebStoreChannel<IOrderService>();
                orderServices.UpdateBasket(
                    new UpdateBasketParameters
                    {
                        BasketId = this._basketServices.GetBasketId().Value,
                        UserId = this._webstoreProfileServices.GetUser(),
                        ShippingMethodId = Guid.Parse(viewModel.ShippingMethod),
                        CultureName = Thread.CurrentThread.CurrentUICulture.Name,
                        CountryId = this.BillingAddress.ShippingAddressIsDifferent ? this.ShippingAddress.Country : this.BillingAddress.Country
                    }
                );
                webStoreServices.Dispose();
                Basket b = this._basketServices.GetBasket();
                return this.View("~/Modules/WebStore/Views/Order/Pay.cshtml", new PayViewModel { Total = b.Total, ShippingCost = b.ShippingTotal, SubTotal = b.SubTotal, Currency = b.Currency });
            }
            else
            {
                viewModel.AvailableShippingMethods = webStoreServices.CreateWebStoreChannel<IOrderService>().GetShippingMethods(new GetShippingMethodsParameters { BasketId = this._basketServices.GetBasketId().Value, CultureName = Thread.CurrentThread.CurrentUICulture.Name, MerchantId = this._webStoreConfigurationServices.GetConfiguration().MerchantId.Value, UserId = this._webstoreProfileServices.GetUser() });
                webStoreServices.Dispose();
            }
            return this.View(viewModel);
        }

        [HttpPost]
        public ActionResult Address(AddressViewModel viewModel)
        {
            IEnumerable<Country> availableCountries = this.GetAvailableCountries();
            if (this.ModelState.IsValid)
            {
                if (viewModel.AddressType == AddressViewModel.AddressTypes.Billing)
                {
                    this.BillingAddress = viewModel;
                }
                else
                {
                    this.ShippingAddress = viewModel;
                }

                if (viewModel.ShippingAddressIsDifferent)
                {
                    this.ModelState.Clear();
                    viewModel = (this.ShippingAddress == default(AddressViewModel)) ? new AddressViewModel { AddressType = AddressViewModel.AddressTypes.Shipping } : this.ShippingAddress;
                }
                else
                {
                    using (WebStoreServices webStoreServices = new WebStoreServices(this._webStoreConfigurationServices.GetConfiguration()))
                    {
                        IProfileService profileService = webStoreServices.CreateWebStoreChannel<IProfileService>();
                        IOrderService orderServices = webStoreServices.CreateWebStoreChannel<IOrderService>();
                        Guid userId = this._webstoreProfileServices.GetUser();
                        UpdateBasketParameters updatBasketParameters = new UpdateBasketParameters { BasketId = this._basketServices.GetBasketId().Value, CountryId = this.BillingAddress.Country, UserId = userId, CultureName = Thread.CurrentThread.CurrentUICulture.Name };
                        this.BillingAddress.AddressId = profileService.SetAddress(this.BillingAddress.GetAddressParameters(userId)).AddressId;
                        updatBasketParameters.BillingAddressId = this.BillingAddress.AddressId;
                        if (this.BillingAddress.ShippingAddressIsDifferent)
                        {
                            this.ShippingAddress.AddressId = profileService.SetAddress(this.ShippingAddress.GetAddressParameters(userId)).AddressId;
                            updatBasketParameters.ShippingAddressId = this.ShippingAddress.AddressId;
                            updatBasketParameters.CountryId = this.ShippingAddress.Country;
                        }
                        orderServices.UpdateBasket(updatBasketParameters);
                    }
                    return this.ShippingMethods();
                }
            }
            viewModel.AvailableCountries = availableCountries;
            return this.View(viewModel);
        }

        [HttpGet]
        public ActionResult ShippingMethods()
        {
            WebStoreConfigurationPart configuration = this._webStoreConfigurationServices.GetConfiguration();
            WebStoreServices webStoreServices = new WebStoreServices(configuration);
            Basket basket = this._basketServices.GetBasket();
            ShippingMethodsViewModel viewModel = new ShippingMethodsViewModel { AvailableShippingMethods = webStoreServices.CreateWebStoreChannel<IOrderService>().GetShippingMethods(new GetShippingMethodsParameters { BasketId = this._basketServices.GetBasketId().Value, CultureName = Thread.CurrentThread.CurrentUICulture.Name, MerchantId = configuration.MerchantId.Value, UserId = this._webstoreProfileServices.GetUser() }), Currency = basket.Currency, ShippingMethod = basket.OrderFormHeaders.First().LineItems.First().ShippingMethodId.ToString() };
            webStoreServices.Dispose();
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return this.View("~/Modules/WebStore/Views/Order/ShippingMethods.cshtml", viewModel);
        }

        [HttpGet]
        public ActionResult LastAddress()
        {
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            AddressViewModel viewModel = (this.BillingAddress.ShippingAddressIsDifferent) ? this.ShippingAddress : this.BillingAddress;
            viewModel.AvailableCountries = this.GetAvailableCountries();
            return this.View("~/Modules/WebStore/Views/Order/Address.cshtml", viewModel);
        }

        [HttpGet]
        public JsonResult GetStates(String countryId)
        {
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Country country = this.GetAvailableCountries().FirstOrDefault(c => c.CountryId == countryId);
            return this.Json(country == default(Country) ? new State[] { } : country.States, JsonRequestBehavior.AllowGet);
        }
    }
}
