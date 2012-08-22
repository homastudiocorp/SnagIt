using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Magelia.WebStore.Services.Contract.Data;

namespace WebStore.Models.Admin
{
    public class GetMerchantsResponseModel
    {
        public Boolean Success { get; set; }
        public IEnumerable<Merchant> MerchantSettings { get; set; }
    }
}