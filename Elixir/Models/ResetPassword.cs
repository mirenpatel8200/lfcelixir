using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Elixir.Models
{
    public class ResetPassword
    {
        [Required(ErrorMessage = "Email Address is required.")]
        [EmailAddress(ErrorMessage = "The Email Address field is not a valid e-mail address.")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "New Password is required.")]
        [MaxLength(50, ErrorMessage = "New Password: Length should be less than 50.")]
        public string NewPassword { get; set; }
        public string UrlEmailAddress { get; set; }
        public string UrlSecurityCode { get; set; }
    }
}