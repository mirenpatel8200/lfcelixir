using System;
using System.Collections.Generic;

namespace Elixir.Models
{
    public class Country : ICloneable
    {
        public virtual int CountryAutoID { get; set; }
        public virtual string CountryName { get; set; }
        public virtual string ContinentCode { get; set; }
        public virtual int CountryID { get; set; }
        public object Clone()
        {
            return new Country
            {
                CountryAutoID = CountryAutoID,
               ContinentCode=ContinentCode,
               CountryName=CountryName,
                CountryID= CountryID
            };
        }

    }
}
