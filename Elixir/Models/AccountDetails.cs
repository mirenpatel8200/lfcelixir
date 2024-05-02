using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Elixir.Models
{
    public class AccountDetails
    {
        [Required(ErrorMessage = "First Name is required.")]
        [MaxLength(50, ErrorMessage = "First Name: Length should be less than 50.")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "Last Name: Length should be less than 50.")]
        public string LastName { get; set; }

        [MaxLength(255, ErrorMessage = "Description: Length should be less than 255.")]
        public string DescriptionPublic { get; set; }

        [MaxLength(50, ErrorMessage = "Email Address: Length should be less than 50.")]
        public string EmailAddress { get; set; }

        [MaxLength(50, ErrorMessage = "Display Name: Length should be less than 50.")]
        public string DisplayName { get; set; }

        [MaxLength(50, ErrorMessage = "Country: Length should be less than 50.")]
        public string CountryName { get; set; }
    }
}