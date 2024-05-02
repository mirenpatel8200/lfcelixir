using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Elixir.Models
{
    public class Registration
    {
        [Required(ErrorMessage = "First Name is required.")]
        [MaxLength(50, ErrorMessage = "First Name: Length should be less than 50.")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "Last Name: Length should be less than 50.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Address is required.")]
        [EmailAddress(ErrorMessage = "The Email Address field is not a valid e-mail address.")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public string CountryName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MaxLength(50, ErrorMessage = "Password: Length should be less than 50.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Membership Level is required.")]
        public int MembershipLevel { get; set; }

        public bool NewsletterSubscriber { get; set; }
    }
}