using System;
using System.Collections.Generic;

namespace Elixir.Models
{
    public class Resource : ICloneable
    {
        public virtual int? Id { get; set; }

        public virtual string ResourceName { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual string ExternalUrl { get; set; }

        public virtual string FacebookHandle { get; set; }

        public virtual string TwitterHandle { get; set; }

        public virtual int ResourceTypeId { get; set; }

        public virtual bool IsAcademia { get; set; }

        public virtual bool IsCompany { get; set; }

        public virtual bool IsPublisher { get; set; }

        public virtual bool IsAuthor { get; set; }

        public virtual bool IsJournalist { get; set; }

        public virtual bool IsCompanyRep { get; set; }

        public virtual bool IsAcademic { get; set; }

        public virtual bool IsAdvocate { get; set; }

        public virtual bool IsPolitician { get; set; }

        public virtual bool IsArtist { get; set; }

        public virtual bool IsBook { get; set; }

        public virtual bool IsFilm { get; set; }

        public virtual bool IsVideo { get; set; }

        public virtual bool IsEnabled { get; set; }

        public virtual string NotesInternal { get; set; }

        public virtual DateTime CreatedDT { get; set; }

        public virtual DateTime UpdatedDT { get; set; }

        public virtual string ResourceDescriptionInternal { get; set; }

        public virtual string ContentMain { get; set; }

        public virtual bool TwitterRetweets { get; set; }

        public virtual string LinkedInUrl { get; set; }

        public virtual string YouTubeUrl { get; set; }

        public virtual string UrlName { get; set; }

        public virtual int? PrimaryTopicID { get; set; }
        public virtual int? SecondaryTopicID { get; set; }

        public virtual Topic PrimaryTopic { get; set; }
        public virtual Topic SecondaryTopic { get; set; }

        public virtual bool IsHumour { get; set; }

        public virtual bool IsInstitute { get; set; }
        public virtual bool IsJournal { get; set; }
        public virtual bool IsApplication { get; set; }
        public virtual bool IsCompetition { get; set; }
        public virtual bool IsInformation { get; set; }
        public virtual bool IsProduct { get; set; }
        public virtual bool IsResearch { get; set; }
        public virtual string AmazonProductCode { get; set; }
        public virtual string ResourceDescriptionPublic { get; set; }
        public virtual bool IsAudio { get; set; }
        public virtual bool IsEducation { get; set; }
        public virtual bool IsHealthOrg { get; set; }
        public virtual bool IsHealthPro { get; set; }
        public virtual int? ParentResourceID { get; set; }

        public virtual string DnParentResourceName { get; set; }

        public virtual int? CreatedByUserId { get; set; }
        public virtual int? UpdatedByUserId { get; set; }

        public virtual string LastUpdatedBy { get; set; }

        public virtual DateTime? ProductionDate { get; set; }

        public virtual int MentionedOrder { get; set; }

        public virtual bool IsHiddenPublic { get; set; }

        public virtual bool IsEvent { get; set; }

        public virtual DateTime? EndDate { get; set; }

        public virtual string OrgsMentioned { get; set; }
        public virtual string OrgsMentionedString { get; set; }

        public virtual string PeopleMentioned { get; set; }
        public virtual string PeopleMentionedString { get; set; }

        public virtual string CreationsMentioned { get; set; }
        public virtual string CreationsMentionedString { get; set; }

        public virtual bool IsOrgsMentionedChanged { get; set; }
        public virtual bool IsPeopleMentionedChanged { get; set; }
        public virtual bool IsCreationsMentionedChanged { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public virtual bool IsPinnedPrimaryTopic { get; set; }
        public virtual bool IsPinnedSecondaryTopic { get; set; }
        public virtual byte? PinnedPrimaryTopicOrder { get; set; }
        public virtual byte? PinnedSecondaryTopicOrder { get; set; }
        public virtual bool IsClubDiscount { get; set; }
        public void RemoveSocialMediaLeadingSymbols()
        {
            if (!string.IsNullOrEmpty(this.FacebookHandle) &&
                this.FacebookHandle.StartsWith("@"))
                this.FacebookHandle = this.FacebookHandle.Substring(1);
            if (!string.IsNullOrEmpty(this.TwitterHandle) &&
                this.TwitterHandle.StartsWith("@"))
                this.TwitterHandle = this.TwitterHandle.Substring(1);
        }

        public string IconImage
        {
            get
            {
                // Don't change the sequence of conditions. It is priority order wise.
                if (IsAcademia)
                    return "academia-16.png";
                if (IsCompany)
                    return "company-16.png";
                if (IsHealthOrg)
                    return "health-organisation-16.png";
                if (IsInstitute)
                    return "institute-16.png";
                if (IsJournal)
                    return "journal-16.png";
                if (IsPublisher)
                    return "publisher-16.png";
                if (IsAcademic)
                    return "academic-16.png";
                if (IsAdvocate)
                    return "advocate-16.png";
                if (IsCompanyRep)
                    return "company-rep-16.png";
                if (IsHealthPro)
                    return "healthcare-professional-16.png";
                if (IsPolitician)
                    return "politician-16.png";
                if (IsAuthor)
                    return "author-16.png";
                if (IsArtist)
                    return "artist-16.png";
                if (IsJournalist)
                    return "journalist-16.png";
                if (IsApplication)
                    return "application-16.png";
                if (IsAudio)
                    return "audio-16.png";
                if (IsBook)
                    return "book-16.png";
                if (IsCompetition)
                    return "competition-16.png";
                if (IsEducation)
                    return "education-16.png";
                if (IsEvent)
                    return "event-16.png";
                if (IsFilm)
                    return "film-16.png";
                if (IsInformation)
                    return "information-16.png";
                if (IsResearch)
                    return "research-16.png";
                if (IsVideo)
                    return "video-16.png";
                if (IsProduct)
                    return "product-16.png";
                return "resource-16.png";
            }
        }

        public string IconImageAltText
        {
            get
            {
                // Don't change the sequence of conditions. It is priority order wise.
                if (IsAcademia)
                    return "Academia";
                if (IsCompany)
                    return "Company";
                if (IsHealthOrg)
                    return "Health Organisation";
                if (IsInstitute)
                    return "Institute";
                if (IsJournal)
                    return "Journal";
                if (IsPublisher)
                    return "Publisher";
                if (IsAcademic)
                    return "Academic";
                if (IsAdvocate)
                    return "Advocate";
                if (IsCompanyRep)
                    return "Company Representative";
                if (IsHealthPro)
                    return "Health Professional";
                if (IsPolitician)
                    return "Politician";
                if (IsAuthor)
                    return "Author";
                if (IsArtist)
                    return "Artist";
                if (IsJournalist)
                    return "Journalist";
                if (IsApplication)
                    return "Application";
                if (IsAudio)
                    return "Audio";
                if (IsBook)
                    return "Book";
                if (IsCompetition)
                    return "Competition";
                if (IsEducation)
                    return "Education";
                if (IsEvent)
                    return "Event";
                if (IsFilm)
                    return "Film";
                if (IsInformation)
                    return "Information";
                if (IsResearch)
                    return "Research";
                if (IsVideo)
                    return "Video";
                if (IsProduct)
                    return "Product";
                return "Resource";
            }
        }

        public bool IsVisible { get; set; }
        public string DnCountryName { get; set; }


        public object Clone()
        {
            return new Resource
            {
                Id = Id,
                ResourceName = ResourceName,
                IsDeleted = IsDeleted,
                ExternalUrl = ExternalUrl,
                FacebookHandle = FacebookHandle,
                TwitterHandle = TwitterHandle,
                ResourceTypeId = ResourceTypeId,
                IsAcademia = IsAcademia,
                IsCompany = IsCompany,
                IsPublisher = IsPublisher,
                IsAuthor = IsAuthor,
                IsJournalist = IsJournalist,
                IsCompanyRep = IsCompanyRep,
                IsAcademic = IsAcademia,
                IsAdvocate = IsAdvocate,
                IsPolitician = IsPolitician,
                IsArtist = IsArtist,
                IsBook = IsBook,
                IsFilm = IsFilm,
                IsVideo = IsVideo,
                IsEnabled = IsEnabled,
                NotesInternal = NotesInternal,
                CreatedDT = CreatedDT,
                UpdatedDT = UpdatedDT,
                ResourceDescriptionInternal = ResourceDescriptionInternal,
                ContentMain = ContentMain,
                TwitterRetweets = TwitterRetweets,
                LinkedInUrl = LinkedInUrl,
                YouTubeUrl = YouTubeUrl,
                UrlName = UrlName,
                PrimaryTopicID = PrimaryTopicID,
                SecondaryTopicID = SecondaryTopicID,
                PrimaryTopic = PrimaryTopic,
                SecondaryTopic = SecondaryTopic,
                IsHumour = IsHumour,
                IsInstitute = IsInstitute,
                IsJournal = IsJournal,
                IsApplication = IsApplication,
                IsCompetition = IsCompetition,
                IsInformation = IsInformation,
                IsProduct = IsProduct,
                IsResearch = IsResearch,
                AmazonProductCode = AmazonProductCode,
                ResourceDescriptionPublic = ResourceDescriptionPublic,
                IsAudio = IsAudio,
                IsEducation = IsEducation,
                IsHealthOrg = IsHealthOrg,
                IsHealthPro = IsHealthPro,
                ParentResourceID = ParentResourceID,
                DnParentResourceName = DnParentResourceName,
                CreatedByUserId = CreatedByUserId,
                UpdatedByUserId = UpdatedByUserId,
                LastUpdatedBy = LastUpdatedBy,
                ProductionDate = ProductionDate,
                MentionedOrder = MentionedOrder,
                IsHiddenPublic = IsHiddenPublic,
                IsEvent = IsEvent,
                EndDate = EndDate,
                OrgsMentioned = OrgsMentioned,
                OrgsMentionedString = OrgsMentionedString,
                PeopleMentioned = PeopleMentioned,
                PeopleMentionedString = PeopleMentionedString,
                CreationsMentioned = CreationsMentioned,
                CreationsMentionedString = CreationsMentionedString,
                IsOrgsMentionedChanged = IsOrgsMentionedChanged,
                IsPeopleMentionedChanged = IsPeopleMentionedChanged,
                IsCreationsMentionedChanged = IsCreationsMentionedChanged,
                IsVisible = IsVisible
            };
        }

        public string[] GetHashtags()
        {
            var hashtags = new List<string>();
            var separator = new[] { " " };

            if (PrimaryTopic != null && !string.IsNullOrWhiteSpace(PrimaryTopic.Hashtags))
                hashtags.AddRange(PrimaryTopic.Hashtags.Split(separator, StringSplitOptions.RemoveEmptyEntries));
            if (SecondaryTopic != null && !string.IsNullOrWhiteSpace(SecondaryTopic.Hashtags))
                hashtags.AddRange(SecondaryTopic.Hashtags.Split(separator, StringSplitOptions.RemoveEmptyEntries));

            return hashtags.ToArray();
        }

        /// <summary>
        /// Implements logic of OgImage selection.
        /// </summary>
        /// <returns></returns>
        public string GetOgImageName()
        {
            var ogImageName = "life-extension-news.jpg";
            if (PrimaryTopic != null && !string.IsNullOrWhiteSpace(PrimaryTopic.SocialImageFilenameNews))
                ogImageName = PrimaryTopic.SocialImageFilenameNews;
            else if (PrimaryTopic != null && !string.IsNullOrWhiteSpace(PrimaryTopic.SocialImageFilename))
                ogImageName = PrimaryTopic.SocialImageFilename;

            return ogImageName;
        }

        public bool IsDisableLink { get; set; }

        // This is used in event calendar page (Events Home Page)
        public bool IsEventCalendar { get; set; }
    }
}
