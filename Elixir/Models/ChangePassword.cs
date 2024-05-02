using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Elixir.Models
{
    public class ChangePassword
    {
        [Required(ErrorMessage = "Current Password is required.")]
        [MaxLength(50, ErrorMessage = "Current Password: Length should be less than 50.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New Password is required.")]
        [MaxLength(50, ErrorMessage = "New Password: Length should be less than 50.")]
        public string NewPassword { get; set; }
    }
}