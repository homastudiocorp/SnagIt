using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Magelia.WebStore.Services.Contract.Data;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Order
{
    public class ShippingMethodsViewModel
    {
        private IEnumerable<dynamic> _availableShippingMethods;
        public IEnumerable<dynamic> AvailableShippingMethods
        {
            get
            {
                if (this._availableShippingMethods == default(IEnumerable<dynamic>))
                {
                    this._availableShippingMethods = new dynamic[] { };
                }
                return this._availableShippingMethods;
            }
            set
            {
                this._availableShippingMethods = value;
            }
        }

        [Required]
        public String ShippingMethod { get; set; }

        public String Currency { get; set; }
    }
}