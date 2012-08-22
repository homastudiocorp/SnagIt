using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magelia.WebStore.Services.Contract.Data;
using Orchard;

namespace WebStore.Services
{
    public interface IBasketServices : IDependency
    {
        void SetBasketId(Guid basketId);
        Guid? GetBasketId();
        Basket GetBasket();
        IEnumerable<Country> getCountries();
        String CountryId {get;set;}
    }
}
