using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Elixir.Models.Enums;
using Elixir.Utils;
using Microsoft.AspNet.Identity;

namespace Elixir.Models
{
    public class BookUser : IUser<int>
    {
        public BookUser()
        {
            AdminNotes = "";
        }

        public int Id { get; set; }

        public virtual bool IsDeleted { get; set; }
        public virtual bool IsEnabled { get; set; }
        public virtual String IdHashCode { get; set; }
        public virtual int? MemberNumber { get; set; }
        public virtual String UserName { get; set; }
        public virtual String UserNameLast { get; set; }
        public virtual String UserNameDisplay { get; set; }
        public virtual String EmailAddress { get; set; }
        //public virtual BookUserRole BookUserRole { get; set; }
        public virtual Role Role { get; set; }
        public virtual List<Role> AllRole { get; set; }
        public virtual DateTime? LastLogin { get; set; }
        //public virtual DateTime? RenewalDate { get; set; }
        public virtual DateTime? ExpiryDate { get; set; }
        public virtual int? CountryId { get; set; }
        public virtual Country Country { get; set; }
        public virtual String PasswordHash { get; set; }
        public virtual String PasswordSalt { get; set; }
        public virtual DateTime? PasswordUpdatedOn { get; set; }
        public virtual String SecurityCode { get; set; }
        public virtual DateTime? SecurityCodeExpiry { get; set; }
        public virtual bool ProfileIsPublic { get; set; }
        public virtual bool ProfileIsMembersOnly { get; set; }
        public virtual String DescriptionPublic { get; set; }
        public virtual String Biography { get; set; }
        public virtual bool IsFoundingMember { get; set; }
        public virtual String WebsiteUrl { get; set; }
        public virtual String TwitterUrl { get; set; }
        public virtual String FacebookUrl { get; set; }
        public virtual String InstagramUrl { get; set; }
        public virtual String OtherUrl { get; set; }
        public virtual String LinkedInUrl { get; set; }
        public virtual int? RegistrationMemberType { get; set; }
        public virtual bool NewsletterSubscriber { get; set; }
        public virtual String AdminNotes { get; set; }
        public virtual DateTime? UpdatedOn { get; set; }
        public virtual int? UpdatedBy { get; set; }
        public virtual DateTime? CreatedOn { get; set; }
        public virtual int? CreatedBy { get; set; }
        public string DnCountryName { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<BookUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public string CalculateIdHashCode()
        {
            var date = (DateTime)CreatedOn;
            var yearSub = date.Year - 2000;
            var year = yearSub >= 0 ? yearSub : 0;
            var yy = year.ToSimple2CharsBase26();
            var dd = (31 * (date.Month - 1) + date.Day).ToSimple2CharsBase26();
            var h = date.Hour.ToABasedLetter();
            var m = (date.Minute / 3).ToABasedLetter();
            var s = (date.Second / 3).ToABasedLetter();

            var sId = Id.ToString("D3");
            var nnn = sId.Length > 3 ? sId.Substring(sId.Length - 3) : sId;

            var sb = new StringBuilder();
            sb.Append(yy).Append(dd).Append(h).Append(m).Append(s).Append(nnn);

            return sb.ToString();
        }

        //[Obsolete("Consider removing from here - Authentication logic in Models. Most likely should be in BL.")]
        //public bool IsDashboardAuthorized
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(BookUserRole.Name))
        //            return false;

        //        return this.BookUserRole.Name.Equals(UserRoleEnum.Administrator.ToString()) ||
        //               this.BookUserRole.Name.Equals(UserRoleEnum.Author.ToString()) ||
        //               this.BookUserRole.Name.Equals(UserRoleEnum.Reviewer.ToString());
        //    }
        //}
    }
}
