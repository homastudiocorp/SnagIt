using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Magelia.WebStore.Services.Contract;
using Magelia.WebStore.Services.Contract.Data;
using System.Threading;

namespace WebStore.Services
{
    public class BasketServices : IBasketServices
    {
        public const String BasketIdSessionKey = "webstorebasketId";
        public const String CountryIdSessionKey = "webstorecountryId";

        IWebStoreConfigurationService _webStoreConfigurationServices;
        IWebStoreProfileServices _webStoreProfileServices;

        public IEnumerable<Country> getCountries()
        {

            IEnumerable<Country> countries;
            using (WebStoreServices webStoreServices = new WebStoreServices(this._webStoreConfigurationServices.GetConfiguration()))
            {
                IProfileService service = webStoreServices.CreateWebStoreChannel<IProfileService>();
                countries = service.GetCountries(new GetCountriesParameters() { CultureName = Thread.CurrentThread.CurrentUICulture.Name, MerchantId = this._webStoreConfigurationServices.GetConfiguration().MerchantId.Value });

            }

            return countries;

        }

        public BasketServices(IWebStoreConfigurationService webStoreConfigurationServices, IWebStoreProfileServices webStoreProfileServices)
        {
            this._webStoreConfigurationServices = webStoreConfigurationServices;
            this._webStoreProfileServices = webStoreProfileServices;
        }

        public Guid? BasketId
        {
            get
            {
                Guid? basketId = null;
                Object sessionBasketId = System.Web.HttpContext.Current.Session[BasketIdSessionKey];
                if (sessionBasketId is Guid)
                {
                    basketId = (Guid)sessionBasketId;
                }
                return basketId;
            }
            set
            {
                System.Web.HttpContext.Current.Session[BasketIdSessionKey] = value;
            }
        }

        public Basket GetBasket()
        {
            GetBasketParameters parameters = new GetBasketParameters() { CultureName = Thread.CurrentThread.CurrentUICulture.Name };
            Magelia.WebStore.Services.Contract.Data.Basket basket;
            using (WebStoreServices webStoreServices = new WebStoreServices(this._webStoreConfigurationServices.GetConfiguration()))
            {
                IOrderService service = webStoreServices.CreateWebStoreChannel<IOrderService>();
                if (this.BasketId.HasValue)
                {
                    parameters.BasketId = this.BasketId.Value;
                }
                else
                {
                    parameters.UserId = this._webStoreProfileServices.GetUser();
                }
                basket = service.GetBasket(parameters);
                if (basket != null)
                {
                    this.BasketId = basket.BasketId;
                }
            }
            return basket;
        }

        public void SetBasketId(Guid basketId)
        {
            this.BasketId = basketId;
        }

        public Guid? GetBasketId()
        {
            return this.BasketId;
        }

        public String CountryId
        {
            get
            {
                String countryId = null;
                String sessionCountryId = (String)System.Web.HttpContext.Current.Session[CountryIdSessionKey];
                if (sessionCountryId == String.Empty)
                {
                    System.Web.HttpContext.Current.Session[CountryIdSessionKey] = "US   ";
                }
                else
                {
                    countryId = (String)sessionCountryId;
                }
                return countryId;
            }
            set
            {
                System.Web.HttpContext.Current.Session[CountryIdSessionKey] = value;
            }
        }


    }
}