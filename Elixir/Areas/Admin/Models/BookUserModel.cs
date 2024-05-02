using System.ComponentModel.DataAnnotations;
using Elixir.Models;
using Elixir.Utils.View;

namespace Elixir.Areas.Admin.Models
{
    public sealed class BookUserModel : BookUser
    {
        public BookUserModel()
        {
            AdminNotes = "";
        }

        public BookUserModel(BookUser bookUser)
        {
            Id = bookUser.Id;
            IdHashCode = bookUser.IdHashCode;
            UserName = bookUser.UserName;
            UserNameLast = bookUser.UserNameLast;
            EmailAddress = bookUser.EmailAddress;
            IsEnabled = bookUser.IsEnabled;
            Role = bookUser.Role;
            AllRole = bookUser.AllRole;
            AdminNotes = bookUser.AdminNotes;
            ProfileIsPublic = bookUser.ProfileIsPublic;
            WebsiteUrl = bookUser.WebsiteUrl;
            LastLogin = bookUser.LastLogin;
            MemberNumber = bookUser.MemberNumber;
            CreatedOn = bookUser.CreatedOn;
            UpdatedOn = bookUser.UpdatedOn;
            ExpiryDate = bookUser.ExpiryDate;
            CountryId = bookUser.CountryId;
            DnCountryName = bookUser.DnCountryName;
            ConstructCountryNameWithId();
        }

        [MaxLength(64, ErrorMessage = "Length should be less than 64.")]
        [Required(ErrorMessage = "User name field is required.")]
        public override string UserName { get; set; }

        [MaxLength(64, ErrorMessage = "Length should be less than 64.")]
        [Required(ErrorMessage = "User last name field is required.")]
        public override string UserNameLast { get; set; }

        [EmailAddress(ErrorMessage = "The Email Address field is not a valid e-mail address.")]
        [MaxLength(64, ErrorMessage = "Length should be less than 64.")]
        [Required(ErrorMessage = "Email field is required.")]
        public override string EmailAddress { get; set; }

        [MaxLength(255, ErrorMessage = "Length should be less than 255.")]
        public override string AdminNotes { get; set; }

        [MaxLength(255, ErrorMessage = "Length should be less than 255.")]
        public override string WebsiteUrl { get; set; }

        public string CountryNameWithId { get; set; }
        public string DnCountryName { get; set; }

        private void ConstructCountryNameWithId()
        {
            CountryNameWithId = DnCountryName;// ViewUtils.FormatAutocompleteResource(DnCountryName, CountryId);
        }

        public void DeconstructCountryNameWithId()
        {
            if (string.IsNullOrWhiteSpace(CountryNameWithId))
            {
                CountryId = null;
                DnCountryName = null;
            }
            else
            {
                var parsed = ViewUtils.ParseAutocompleteResource(CountryNameWithId);

                CountryId = parsed.ResourceId;
                DnCountryName = parsed.ResourceName;
            }
        }

    }
}