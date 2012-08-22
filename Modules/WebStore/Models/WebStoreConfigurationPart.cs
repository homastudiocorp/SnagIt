using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using System.ComponentModel.DataAnnotations;
using Magelia.WebStore.Services.Contract.Data;
using System.Web.Mvc;

namespace WebStore.Models
{
    public class WebStoreConfigurationPart : ContentPart<WebStoreConfigurationPartRecord>
    {
        public Guid? MerchantId
        {
            get
            {
                return this.Record.MerchantId;
            }
            set
            {
                this.Record.MerchantId = value;
            }
        }

        public Guid? CatalogId
        {
            get
            {
                return this.Record.CatalogId;
            }
            set
            {
                this.Record.CatalogId = value;
            }
        }

        [RegularExpression(@"((https?):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)/")]
        public String ServicesPath
        {
            get
            {
                return this.Record.ServicesPath;
            }
            set
            {
                this.Record.ServicesPath = value;
            }
        }

        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?")]
        public String PaypalAccount
        {
            get
            {
                return this.Record.PaypalAccount;
            }
            set
            {
                this.Record.PaypalAccount = value;
            }
        }


        public List<SelectListItem> Merchants;
        public List<SelectListItem> Catalogs;

        public Boolean IsComplete
        { 
            get
            {
                return this.CatalogId.HasValue && this.MerchantId.HasValue && !String.IsNullOrEmpty(this.ServicesPath);
            }
        }
    }
}