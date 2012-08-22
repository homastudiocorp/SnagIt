using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Magelia.WebStore.Services.Contract.Data;
using Magelia.WebStore.Services.Contract;
using System.Threading;
using Magelia.WebStore.Services.Clients;

namespace WebStore.Models.Order
{
    public class AddressViewModel
    {
        public enum AddressTypes { Billing, Shipping }
        public AddressTypes AddressType { get; set; }

        public enum Titles { Mrs = 2, Miss = 1, Mr = 3 }

        [Required]
        public Titles? Title { get; set; }

        [StringLength(256)]
        public String Company { get; set; }

        [StringLength(256)]
        [Required]
        public String LastName { get; set; }

        [StringLength(256)]
        [Required]
        public String FirstName { get; set; }

        [StringLength(256)]
        [Required]
        public String AddressLine1 { get; set; }

        [StringLength(256)]
        public String AddressLine2 { get; set; }

        [StringLength(256)]
        public String AddressLine3 { get; set; }

        [StringLength(256)]
        public String DigiCode { get; set; }

        [StringLength(256)]
        public String Floor { get; set; }

        [StringLength(256)]
        [Required]
        public String ZipCode { get; set; }

        [StringLength(256)]
        [Required]
        public String City { get; set; }

        [Required]
        public String Country { get; set; }

        public String State { get; set; }

        [StringLength(256)]
        public String PhoneNumber { get; set; }

        [StringLength(256)]
        public String CellPhoneNumber { get; set; }

        public Boolean ShippingAddressIsDifferent { get; set; }

        public Guid? AddressId { get; set; }

        private IEnumerable<dynamic> _availableCountries;
        public IEnumerable<dynamic> AvailableCountries
        {
            get
            {
                if (this._availableCountries == default(IEnumerable<dynamic>))
                {
                    this._availableCountries = new Country[] { };
                }
                return this._availableCountries;
            }
            set
            {
                this._availableCountries = value;
            }
        }

        public SetAddressParameters GetAddressParameters(Guid userId)
        {
            SetAddressParameters parameters = new SetAddressParameters
            {
                AddressLine1 = this.AddressLine1,
                AddressLine2 = this.AddressLine2,
                AddressLine3 = this.AddressLine3,
                AdressName = Guid.NewGuid().ToString(),
                CityName = this.City,
                Comment = String.Empty,
                Company = this.Company,
                CountryId = this.Country,
                CultureName = Thread.CurrentThread.CurrentUICulture.Name,
                Digicode = this.DigiCode,
                FaxNumber = String.Empty,
                FirstName = this.FirstName,
                FloorNumber = this.Floor,
                LastName = this.LastName,
                MobileNumber = this.CellPhoneNumber,
                PhoneNumber = this.PhoneNumber,
                StateId = this.State,
                Title = (SetAddressParameters.Civility)this.Title.Value,
                ZipCode = this.ZipCode,
                UserId = userId
            };
            if (!this.AddressId.HasValue)
            {
                parameters.IsNew = true;
            }
            else
            {
                parameters.AddressId = this.AddressId.Value;
            }
            return parameters;
        }
    }
}