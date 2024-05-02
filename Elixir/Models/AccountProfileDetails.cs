using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Elixir.Models
{
    public class AccountProfileDetails
    {
        public bool ProfileIsPublic { get; set; }

        [MaxLength(255, ErrorMessage = "Description: Length should be less than 255.")]
        public string DescriptionPublic { get; set; }
        
        [MaxLength(255, ErrorMessage = "Visibility (your name and email are NEVER displayed)")]
        public string Visibilty { get; set; }

        [MaxLength(255, ErrorMessage = "Visibility (your name and email are NEVER displayed)")]
        public string IDHashCode { get; set; }

        [MaxLength(255, ErrorMessage = "Biography (tell us a bit more about you)")]
        public string Biography { get; set; }

        [MaxLength(255, ErrorMessage = "Website (personal or company)")]
        public string WebsiteUrl { get; set; }

        [MaxLength(255, ErrorMessage = "Twitter")]
        public string TwitterUrl { get; set; }

        [MaxLength(255, ErrorMessage = "Facebook")]
        public string FacebookUrl { get; set; }

        [MaxLength(255, ErrorMessage = "Instagram")]
        public string InstagramUrl { get; set; }

        [MaxLength(255, ErrorMessage = "Other Url")]
        public string OtherUrl { get; set; }

        [MaxLength(255, ErrorMessage = "LinkedIn")]
        public string LinkedInUrl { get; set; }

    }
}