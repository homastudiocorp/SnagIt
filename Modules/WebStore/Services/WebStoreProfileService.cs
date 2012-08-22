using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Magelia.WebStore.Services.Contract;
using System.Web.Security;

namespace WebStore.Services
{
    public class WebStoreProfileService : IWebStoreProfileServices
    {
        private static String K_AnonymousEmailTemplate = "{0}@anonymous.user";
        private static String K_AnonymousDefaultPassword = "P@ssw0rd";

        IWebStoreConfigurationService _service;

        public WebStoreProfileService(IWebStoreConfigurationService service)
        {
            this._service = service;
        }

        public Guid GetUser()
        {
            Guid userId;
            using (WebStoreServices webStoreServices = new WebStoreServices(this._service.GetConfiguration()))
            {
                IProfileService service = webStoreServices.CreateWebStoreChannel<IProfileService>();
                if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    Magelia.WebStore.Services.Contract.Data.User user = service.GetUser(
                        new GetUserParameters
                        {
                            LoginType = LoginType.UserName,
                            UserKey = System.Web.HttpContext.Current.User.Identity.Name
                        }
                    );
                    if (user != null)
                    {
                        userId = user.UserId;
                    }
                    else
                    {
                        user = service.CreateUser(
                            new CreateUserParameters
                            {
                                Email = Membership.GetUser().Email,
                                FirstName = string.Empty,
                                LastName = string.Empty,
                                MerchantId = this._service.GetConfiguration().MerchantId.Value,
                                Newsletter = false,
                                Password = K_AnonymousDefaultPassword,
                                UserName = System.Web.HttpContext.Current.User.Identity.Name,
                                UserType = Magelia.WebStore.Services.Contract.Data.UserType.Normal
                            }
                        );
                        userId = user.UserId;
                    }
                }
                else
                {
                    string username = "Anonymous" + Guid.NewGuid();
                    Magelia.WebStore.Services.Contract.Data.User user = service.CreateUser(
                        new CreateUserParameters
                        {
                            Email = String.Format(K_AnonymousEmailTemplate, username),
                            FirstName = string.Empty,
                            LastName = string.Empty,
                            MerchantId = this._service.GetConfiguration().MerchantId.Value,
                            Newsletter = false,
                            Password = K_AnonymousDefaultPassword,
                            UserName = username,
                            UserType = Magelia.WebStore.Services.Contract.Data.UserType.Anonymous,
                            TitleId = Magelia.WebStore.Services.Contract.Data.UserTitleEnum.Unknown
                        }
                    );
                    userId = user.UserId;
                }
            }
            return userId;
        }
    }
}