using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Elixir.Models
{
    public class ShippingDetails
    {
        [Required(ErrorMessage = "Email Address is required.")]
        [EmailAddress(ErrorMessage = "The Email Address field is not a valid e-mail address.")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Address line 1 is required.")]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }

        [Required(ErrorMessage = "Town is required.")]
        public string Town { get; set; }

        [Required(ErrorMessage = "Postcode is required.")]
        public string Postcode { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }
        public string ContactTelephoneNumber { get; set; }
    }
}