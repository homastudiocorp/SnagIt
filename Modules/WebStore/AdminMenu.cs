using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace WebStore
{
    public class AdminMenu : INavigationProvider
    {

        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public AdminMenu()
        {
            this.T = NullLocalizer.Instance;
        }

        public void GetNavigation(NavigationBuilder builder)
        {
           builder.Add(this.T("WebStore e-commerce"), 7.ToString(), i => i.Action("Synchronization", "Admin", new {area = "WebStore"}));
        }
    }

}