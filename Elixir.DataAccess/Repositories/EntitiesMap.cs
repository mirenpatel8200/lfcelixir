using System;
using System.Data.OleDb;
using Elixir.Models;

namespace Elixir.DataAccess.Repositories
{
    public abstract class EntitiesMap
    {
        protected OleDbDataReader DataReader;

        public GoLinkLog MapGoLinkLog(int startIndex)
        {
            return new Mapper<GoLinkLog>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.Id)
                .Map(x => x.Created)
                .Map(x => x.GoLinkId)
                .Map(x => x.IPAddress)
                .Create();
        }

        public AuditLog MapAuditLog(int startIndex)
        {
            return new Mapper<AuditLog>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.AuditLogID)
                .Map(x => x.CreatedDT)
                .Map(x => x.IpAddressString)
                .Map(x => x.UserID)
                .Map(x => x.EntityTypeID)
                .Map(x => x.EntityID)
                .Map(x => x.ActionTypeID)
                .Map(x => x.NotesLog)
                .Create();
        }

        public SearchLog MapSearchLog(int startIndex)
        {
            return new Mapper<SearchLog>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.Id)
                .Map(x => x.Created)
                .Map(x => x.IPAddress)
                .Map(x => x.Search)
                .Map(x => x.WordCount)
                .Create();
        }

        public GoLink MapGoLink(int startIndex)
        {
            return new Mapper<GoLink>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.Id)
                .Map(x => x.GoLinkTitle)
                .Map(x => x.IsDeleted)
                .Map(x => x.ShortCode)
                .Map(x => x.DestinationUrl)
                .Map(x => x.IsBookLink)
                .Map(x => x.IsAffiliateLink)
                .Map(x => x.IsEnabled)
                .Map(x => x.NotesInternal)
                .Map(x => x.Created)
                .Map(x => x.Updated)
                .Create();
        }

        public Article MapArticle(int startIndex)
        {
            return new Mapper<Article>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.Id)
                .Map(x => x.Title)
                .Map(x => x.OriginalTitle)
                .Map(x => x.PublisherResourceId)
                .Map(x => x.DnPublisherName)
                .Map(x => x.PublisherUrl)
                .Map(x => x.ArticleDate)
                .Skip(x => x.PrimaryTopicID)
                .Skip(x => x.SecondaryTopicID)
                .Map(x => x.BulletPoints)
                .Map(x => x.IsEnabled)
                .Map(x => x.Notes)
                .Map(x => x.IsDeleted)
                .Map(x => x.Created)
                .Map(x => x.Updated)
                .Map(x => x.Summary)
                .Map(x => x.UrlName)
                .Map(x => x.IdHashCode)
                .Map(x => x.SocialImageFilename)
                .Map(x => x.ReporterResourceId)
                .Map(x => x.DnReporterName)
                .Map(x => x.DisplaySocialImage)
                .Map(x => x.IsHumour)
                .Map(x => x.CreatedByUserId)
                .Map(x => x.UpdatedByUserId)
                .Create();
        }

        public Topic MapTopic(int startIndex)
        {
            return new Mapper<Topic>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.Id)
                .Map(x => x.TopicName)
                .Map(x => x.DescriptionInternal)
                .Map(x => x.PrimaryWebPageId)
                .Map(x => x.NotesInternal)
                .Map(x => x.SocialImageFilename)
                .Map(x => x.SocialImageFilenameNews)
                .Map(x => x.BannerImageFileName)
                .Map(x => x.Hashtags)
                .Map(x => x.ThumbnailImageFilename)
                .Create();
        }

        public WebPage MapWebPage(int startIndex)
        {
            return new Mapper<WebPage>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.Id)
                .Map(x => x.UrlName)
                .Map(x => x.WebPageName)
                .Map(x => x.IsDeleted)
                .Map(x => x.WebPageTitle)
                .Map(x => x.ContentMain)
                .Map(x => x.ParentID)
                .Map(x => x.IsSubjectPage)
                .Map(x => x.DisplayOrder)
                .Map(x => x.IsEnabled)
                .Map(x => x.NotesInternal)
                .Map(x => x.CreatedDateTime)
                .Map(x => x.UpdatedDateTime)
                .Map(x => x.BannerImageFileName)
                .Map(x => x.SocialImageFileName)
                .Map(x => x.MetaDescription)
                .Map(x => x.PrimaryTopicID)
                .Map(x => x.SecondaryTopicID)
                .Map(x => x.PublishedOnDT)
                .Map(x => x.PublishedUpdatedDT)
                .Map(x => x.TypeID)
                .Create();
        }

        public BlogPost MapBlogPost(int startIndex)
        {
            return new Mapper<BlogPost>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.Id)
                .Map(x => x.BlogPostTitle)
                .Map(x => x.UrlName)
                .Map(x => x.IsDeleted)
                .Map(x => x.ContentMain)
                .Map(x => x.PrimaryTopicId)
                .Map(x => x.SecondaryTopicId)
                .Map(x => x.IsEnabled)
                .Map(x => x.CreatedDt)
                .Map(x => x.UpdatedDt)
                .Map(x => x.PreviousBlogPostUrlName)
                .Map(x => x.NextBlogPostUrlName)
                .Map(x => x.NotesInternal)
                .Map(x => x.PublishedOnDT)
                .Map(x => x.PublishedUpdatedDT)
                .Map(x => x.SocialImageFilename)
                .Map(x => x.BlogPostDescriptionPublic)
                .Map(x => x.PreviousBlogPostTitle)
                .Map(x => x.NextBlogPostTitle)
                .Map(x => x.ThumbnailImageFilename)
                .Create();
        }
        public User MapUser(int startIndex)
        {
            return new Mapper<User>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.FirstName)
                .Map(x => x.LastName)
                .Create();
        }
        public Country MapCountry(int startIndex)
        {
            return new Mapper<Country>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.CountryAutoID)
                .Map(x => x.CountryName)
                .Map(x => x.ContinentCode)
                .Map(x => x.CountryID)
                .Create();
        }
        public Resource MapResource(int startIndex)
        {
            return new Mapper<Resource>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.Id)
                .Map(x => x.ResourceName)
                .Map(x => x.IsDeleted)
                .Map(x => x.ExternalUrl)
                .Map(x => x.FacebookHandle)
                .Map(x => x.TwitterHandle)
                .Map(x => x.ResourceTypeId)
                .Map(x => x.IsAcademia)
                .Map(x => x.IsCompany)
                .Map(x => x.IsPublisher)
                .Map(x => x.IsAuthor)
                .Map(x => x.IsJournalist)
                .Map(x => x.IsCompanyRep)
                .Map(x => x.IsAcademic)
                .Map(x => x.IsBook)
                .Map(x => x.IsFilm)
                .Map(x => x.IsVideo)
                .Map(x => x.IsEnabled)
                .Map(x => x.NotesInternal)
                .Map(x => x.CreatedDT)
                .Map(x => x.UpdatedDT)
                .Map(x => x.ResourceDescriptionInternal)
                .Map(x => x.ContentMain)
                .Map(x => x.TwitterRetweets)
                .Map(x => x.LinkedInUrl)
                .Map(x => x.YouTubeUrl)
                .Map(x => x.UrlName)
                .Map(x => x.PrimaryTopicID)
                .Map(x => x.SecondaryTopicID)
                .Map(x => x.IsHumour)
                .Map(x => x.IsInstitute)
                .Map(x => x.IsApplication)
                .Map(x => x.IsCompetition)
                .Map(x => x.IsInformation)
                .Map(x => x.IsProduct)
                .Map(x => x.IsResearch)
                .Map(x => x.AmazonProductCode)
                .Map(x => x.ResourceDescriptionPublic)
                .Map(x => x.IsAudio)
                .Map(x => x.ParentResourceID)
                .Map(x => x.IsJournal)
                .Map(x => x.DnParentResourceName)
                .Map(x => x.CreatedByUserId)
                .Map(x => x.UpdatedByUserId)
                .Map(x => x.ProductionDate)
                .Map(x => x.IsAdvocate)
                .Map(x => x.IsArtist)
                .Map(x => x.IsPolitician)
                .Map(x => x.IsEducation)
                .Map(x => x.IsHealthOrg)
                .Map(x => x.IsHealthPro)
                .Map(x => x.IsHiddenPublic)
                .Map(x => x.IsEvent)
                .Map(x => x.EndDate)
                .Map(x => x.CountryId)
                .Map(x => x.IsPinnedPrimaryTopic)
                .Map(x => x.IsPinnedSecondaryTopic)
                .Map(x => x.PinnedPrimaryTopicOrder)
                .Map(x => x.PinnedSecondaryTopicOrder)
                .Map(x => x.IsClubDiscount)
                .Create();
        }

        public SettingsEntry MapSettingEntry(int startIndex)
        {
            return new Mapper<SettingsEntry>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.SettingsId)
                .Map(x => x.PairName)
                .Map(x => x.PairValue)
                .Map(x => x.SettingsDescription)
                .Map(x => x.PayPalTokenExpirationDT)
                .Create();
        }

        public ShopProduct MapShopProduct(int startIndex)
        {
            return new Mapper<ShopProduct>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.ShopProductId)
                .Map(x => x.IsDeleted)
                .Map(x => x.IsEnabled)
                .Map(x => x.ShopProductName)
                .Map(x => x.UrlName)
                .Map(x => x.SKU)
                .Map(x => x.ShopProductDescription)
                .Map(x => x.ContentMain)
                .Map(x => x.ShopCategoryId)
                .Map(x => x.PriceRRP)
                .Map(x => x.PriceLongevist)
                .Map(x => x.ShippingPrice)
                .Map(x => x.IsLongevistsOnly)
                .Map(x => x.StockLevel)
                .Map(x => x.DisplayOrder)
                .Map(x => x.ImageThumb)
                .Map(x => x.ImageMain)
                .Map(x => x.NotesInternal)
                .Map(x => x.CreatedOn)
                .Map(x => x.UpdatedOn)
                .Map(x => x.CreatedBy)
                .Map(x => x.UpdatedBy)
                .Map(x => x.OptionsUnit)
                .Create();
        }

        public ShopCategory MapShopCategory(int startIndex)
        {
            return new Mapper<ShopCategory>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.ShopCategoryId)
                .Map(x => x.IsDeleted)
                .Map(x => x.IsEnabled)
                .Map(x => x.ShopCategoryName)
                .Map(x => x.PrimaryWebPageId)
                .Map(x => x.DnPrimaryWebPageName)
                .Map(x => x.DnPrimaryWebPageUrlName)
                .Map(x => x.ImageThumb)
                .Map(x => x.ImageMain)
                .Map(x => x.NotesInternal)
                .Map(x => x.CreatedOn)
                .Map(x => x.UpdatedOn)
                .Map(x => x.CreatedByUserId)
                .Map(x => x.UpdatedByUserId)
                .Create();
        }

        public ShopOrder MapShopOrder(int startIndex)
        {
            return new Mapper<ShopOrder>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.ShopOrderId)
                .Map(x => x.IsDeleted)
                .Map(x => x.IDHashCode)
                .Map(x => x.StatusId)
                .Map(x => x.UserId)
                .Map(x => x.OrderPlacedOn)
                .Map(x => x.OrderPlacedIPAddressString)
                .Map(x => x.OrderDespatchedOn)
                .Map(x => x.NotesPublic)
                .Map(x => x.NotesInternal)
                .Map(x => x.CreatedOn)
                .Map(x => x.CreatedByUserId)
                .Map(x => x.UpdatedOn)
                .Map(x => x.UpdatedByUserId)
                .Map(x => x.ItemsTotal)
                .Map(x => x.ShippingPricePaid)
                .Map(x => x.PricePaidTotal)
                .Map(x => x.EmailAddress)
                .Map(x => x.AddressNameFirst)
                .Map(x => x.AddressNameLast)
                .Map(x => x.AddressLine1)
                .Map(x => x.AddressLine2)
                .Map(x => x.AddressLine3)
                .Map(x => x.AddressTown)
                .Map(x => x.AddressPostcode)
                .Map(x => x.AddressCountryID)
                .Map(x => x.TelephoneNumber)
                .Create();
        }

        public ShopOrderProduct MapShopOrderProduct(int startIndex)
        {
            return new Mapper<ShopOrderProduct>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.ShopOrderProductId)
                .Map(x => x.ShopOrderId)
                .Map(x => x.ShopProductId)
                .Map(x => x.DnShopProductName)
                .Map(x => x.ShopProductOptionId)
                .Map(x => x.DnShopProductOptionName)
                .Map(x => x.SKU)
                .Map(x => x.PricePaidPerUnit)
                .Map(x => x.Quantity)
                .Create();
        }

        public ShopProductOption MapShopProductOption(int startIndex)
        {
            return new Mapper<ShopProductOption>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.ShopProductOptionId)
                .Map(x => x.IsDeleted)
                .Map(x => x.IsEnabled)
                .Map(x => x.ShopProductId)
                .Map(x => x.OptionName)
                .Map(x => x.SkuSuffix)
                .Map(x => x.PriceExtra)
                .Map(x => x.StockLevel)
                .Map(x => x.DisplayOrder)
                .Map(x => x.IsDefaultOption)
                .Map(x => x.CreatedOn)
                .Map(x => x.UpdatedOn)
                .Map(x => x.CreatedByUserId)
                .Map(x => x.UpdatedByUserId)
                .Create();
        }

        public Role MapRole(int startIndex)
        {
            return new Mapper<Role>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.RoleId)
                .Map(x => x.RoleName)
                .Map(x => x.LoginRedirectUrl)
                .Map(x => x.DisplayOrder)
                .Create();
        }

        public UserRole MapUserRole(int startIndex)
        {
            return new Mapper<UserRole>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.UserRoleId)
                .Map(x => x.UserId)
                .Map(x => x.RoleId)
                .Create();
        }

        public BookUser MapBookUser(int startIndex)
        {
            return new Mapper<BookUser>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.Id)
                .Map(x => x.IsDeleted)
                .Map(x => x.IsEnabled)
                .Map(x => x.IdHashCode)
                .Map(x => x.MemberNumber)
                .Map(x => x.UserName)
                .Map(x => x.UserNameLast)
                .Map(x => x.UserNameDisplay)
                .Map(x => x.EmailAddress)
                .Map(x => x.LastLogin)
                .Map(x => x.ExpiryDate)
                .Map(x => x.CountryId)
                .Map(x => x.PasswordHash)
                .Map(x => x.PasswordSalt)
                .Map(x => x.PasswordUpdatedOn)
                .Map(x => x.SecurityCode)
                .Map(x => x.SecurityCodeExpiry)
                .Map(x => x.ProfileIsPublic)
                .Map(x => x.ProfileIsMembersOnly)
                .Map(x => x.DescriptionPublic)
                .Map(x => x.Biography)
                .Map(x => x.IsFoundingMember)
                .Map(x => x.WebsiteUrl)
                .Map(x => x.TwitterUrl)
                .Map(x => x.FacebookUrl)
                .Map(x => x.InstagramUrl)
                .Map(x => x.OtherUrl)
                .Map(x => x.RegistrationMemberType)
                .Map(x => x.AdminNotes)
                .Map(x => x.UpdatedOn)
                .Map(x => x.UpdatedBy)
                .Map(x => x.CreatedOn)
                .Map(x => x.CreatedBy)
                .Map(x => x.LinkedInUrl)
                .Map(x => x.NewsletterSubscriber)
                .Create();
        }

        public Payments MapPayment(int startIndex)
        {
            return new Mapper<Payments>(DataReader).SetStartIndex(startIndex)
               .Map(x => x.PaymentId)
               .Map(x => x.PaymentReference)
               .Map(x => x.PaymentStatusId)
               .Map(x => x.Amount)
               .Map(x => x.ProcessorName)
               .Map(x => x.ShopOrderId)
               .Map(x => x.NotesUser)
               .Map(x => x.NotesAdmin)
               .Map(x => x.PaymentDate)
               .Map(x => x.PaymentResponse)
               .Map(x => x.UpdatedOn)
               .Map(x => x.UpdatedBy)
               .Map(x => x.CreatedOn)
               .Map(x => x.CreatedBy)
               .Create();
        }

        public WebPageXTopic MapWebPageXTopic(int startIndex)
        {
            return new Mapper<WebPageXTopic>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.WebPageTopicId)
                .Map(x => x.TopicId)
                .Map(x => x.WebPageTopicId)
               .Create();
        }

        public WebPageType MapWebPageType(int startIndex)
        {
            return new Mapper<WebPageType>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.WebPageTypeID)
                .Map(x => x.WebPageTypeName)
               .Create();
        }

        public BookSection MapBookSection(int startIndex)
        {
            return new Mapper<BookSection>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.Id)
                .Map(x => x.BookSectionName)
                .Map(x => x.DisplayOrder)
                .Map(x => x.IsIncluded)
               .Create();
        }

        public Chapter MapChapter(int startIndex)
        {
            return new Mapper<Chapter>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.Id)
                .Map(x => x.ChapterName)
                .Map(x => x.DisplayOrder)
                .Map(x => x.IsIncluded)
                .Map(x => x.Notes)
                .Map(x => x.Text)
                .Map(x => x.ContentPage2)
                .Map(x => x.ContentPage3)
                .Map(x => x.ContentPage4)
                .Map(x => x.ContentPage5)
                .Map(x => x.ContentPage6)
                .Map(x => x.ContentPage7)
                .Map(x => x.ContentPage8)
                .Map(x => x.ContentPage9)
                .Map(x => x.ContentPage10)
                .Map(x => x.TypeID)
                .Map(x => x.MarginTop)
                .Map(x => x.HasBreakInParagraph1)
                .Map(x => x.HasBreakInParagraph2)
                .Map(x => x.HasBreakInParagraph3)
                .Map(x => x.HasBreakInParagraph4)
                .Map(x => x.HasBreakInParagraph5)
                .Map(x => x.HasBreakInParagraph6)
                .Map(x => x.HasBreakInParagraph7)
                .Map(x => x.HasBreakInParagraph8)
                .Map(x => x.HasBreakInParagraph9)
                .Map(x => x.HasBreakInParagraph10)
                .Map(x => x.PageFirst)
                .Map(x => x.PageLast)
                .Create();
        }

        public BookPage MapBookPage(int startIndex)
        {
            return new Mapper<BookPage>(DataReader).SetStartIndex(startIndex)
                .Map(x => x.Id)
                .Map(x => x.BookPageName)
                .Map(x => x.BookSectionId)
                .Map(x => x.DisplayOrder)
                .Map(x => x.IsIncluded)
                .Map(x => x.BookPageDescription)
                .Map(x => x.LifeExtension40)
                .Map(x => x.Cost)
                .Map(x => x.Difficulty)
                .Map(x => x.ImageFilename)
                .Map(x => x.Status)
                .Map(x => x.Notes)
                .Map(x => x.Author)
                .Map(x => x.Tips)
                .Map(x => x.Resources)
                .Map(x => x.ResearchPapers)
                .Map(x => x.PageFirst)
                .Map(x => x.PageLast)
                .Create();
        }

        public TValue GetTableValue<TValue>(OleDbDataReader dataReader, int rowNumber)
        {
            return Mapper.GetTableValue<TValue>(dataReader, rowNumber);
        }

        public TValue GetTableValue<TValue>(int rowNumber)
        {
            return GetTableValue<TValue>(DataReader, rowNumber);
        }
    }
}
