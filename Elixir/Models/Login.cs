using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Elixir.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Email Address is required.")]
        [EmailAddress(ErrorMessage = "The Email Address field is not a valid e-mail address.")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}