using System;
using System.Collections.Generic;
using System.Linq;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.ViewModels
{
    public class WebPageViewModel
    {
        public WebPage WebPage { get; set; }
        public IEnumerable<Article> LatestArticles { get; set; }
        public IEnumerable<BlogPost> RelatedBlogPosts { get; set; }
        public List<Resource> AllResources { get; set; }
        public List<Resource> LatestResources { get; set; }

        public bool HasLatestArticles => LatestArticles != null && LatestArticles.ToList().Count > 0 && WebPage.IsSubjectPage;

        public bool HasParent { get; set; }
        public string LinkToParentText { get; set; }
        public string LinkToParentUrlName { get; set; }

        public bool IsHomePage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Page && WebPage.UrlName.Equals("home", StringComparison.OrdinalIgnoreCase);

        public IEnumerable<SubpageViewModel> Subpages { get; set; }
        public bool HasSubpages => Subpages != null && Subpages.Any();

        public List<ShopProduct> ShopProducts { get; set; }
        public bool IsShopHomePage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Shop && WebPage.UrlName.Equals("home", StringComparison.OrdinalIgnoreCase);

        public bool IsCartPage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Shop && WebPage.UrlName.Equals("cart", StringComparison.OrdinalIgnoreCase);
        public List<ShopOrderProduct> ShopOrderProducts { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalQuantity { get; set; }
        public ShopNavigationBar ShopNavigationBar { get; set; }

        public bool IsCheckoutPage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Shop && WebPage.UrlName.Equals("checkout", StringComparison.OrdinalIgnoreCase);
        public decimal? ShippingAmount { get; set; }
        public decimal OrderTotalAmount { get; set; }
        public string Environment { get; set; }
        public string ClientId { get; set; }
        public string WarningMessage { get; set; }
        public bool IsOnlyMembershipProducts { get; set; }

        public bool IsAcknowledgementPage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Shop && WebPage.UrlName.Equals("acknowledgement", StringComparison.OrdinalIgnoreCase);
        public string IdHashCode { get; set; }
        public DateTime? OrderPlacedOn { get; set; }
        public string OrderStatus { get; set; }

        public bool IsShopOrdersPage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Account && WebPage.UrlName.Equals("orders", StringComparison.OrdinalIgnoreCase);
        public List<ShopOrder> TotalShopOrders { get; set; }
        public List<ShopOrder> ShopOrders { get; set; }
        public ShopOrder UserShopCart { get; set; }

        public IEnumerable<BlogPost> RecentBlogPosts { get; set; }

        public bool IsAccountDetailsPage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Account && WebPage.UrlName.Equals("details", StringComparison.OrdinalIgnoreCase);
        public AccountDetails AccountDetails { get; set; }

        public bool IsAccountHomePage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Account && WebPage.UrlName.Equals("home", StringComparison.OrdinalIgnoreCase);

        public bool IsAccountProfilePage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Account && WebPage.UrlName.Equals("profile", StringComparison.OrdinalIgnoreCase);
        public AccountProfileDetails AccountProfileDetails { get; set; }

        public bool IsMembersDirectoryPage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Page && WebPage.UrlName.Equals("members-directory", StringComparison.OrdinalIgnoreCase);
        public List<BookUser> MembersDirectory { get; set; }

        public bool IsFoundingMembersPage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Page && WebPage.UrlName.Equals("founding-members", StringComparison.OrdinalIgnoreCase);
        public List<BookUser> FoundingMembers { get; set; }

        public string MembershipLevel { get; set; }
        public string MembershipNumber { get; set; }
        public string ExpiryDate { get; set; }
        public bool IsFoundingMember { get; set; }
        public bool IsLoginPage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Page && WebPage.UrlName.Equals("login", StringComparison.OrdinalIgnoreCase);


        public bool IsChangePasswordPage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Account && WebPage.UrlName.Equals("password", StringComparison.OrdinalIgnoreCase);
        public bool IsOrderDetailsPage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Account && WebPage.UrlName.Equals("order", StringComparison.OrdinalIgnoreCase);
        public string NotesPublic { get; set; }
        public bool IsShippingDetailsPage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Shop && WebPage.UrlName.Equals("shipping", StringComparison.OrdinalIgnoreCase);
        public ShippingDetails ShippingDetails { get; set; }

        public bool IsRegistrationPage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Page && WebPage.UrlName.Equals("registration", StringComparison.OrdinalIgnoreCase);

        public bool IsEventsHomePage => WebPage != null && WebPage.TypeID == (int)EnumWebPageType.Events && WebPage.UrlName.Equals("events-calendar", StringComparison.OrdinalIgnoreCase);
        public List<Resource> CurrentEventResources { get; set; }
        public List<Resource> PastEventResources { get; set; }
    }
}