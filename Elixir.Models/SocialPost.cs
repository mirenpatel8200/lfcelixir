using Elixir.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Models
{
    public class SocialPost
    {
        public virtual EntityType? EntityType { get; set; }

        public virtual string EntityTypeName { get; set; }

        public virtual int? EntityId { get; set; }

        public virtual string FirstLine { get; set; }

        public virtual string OtherContent { get; set; }

        public virtual string LastLine { get; set; }
        public virtual bool IsKeyResourcesShortMatch { get; set; }

        #region KeyOrganisationWithId
        public virtual string DnPublisherName { get; set; }
        public virtual int? KeyOrganisationId { get; set; }
        public virtual bool IsSuffixKeyOrganisation { get; set; }
        public virtual string KeyOrganisationWithId { get; set; }
        #endregion

        #region KeyPersonWithId
        public virtual string DnReporterName { get; set; }
        public virtual int? KeyPersonId { get; set; }
        public virtual bool IsSuffixKeyPerson { get; set; }
        public virtual string KeyPersonWithId { get; set; }
        #endregion

        #region KeyCreationWithId
        public virtual string DnCreationName { get; set; }
        public virtual int? KeyCreationId { get; set; }
        public virtual bool IsSuffixKeyCreation { get; set; }
        public virtual string KeyCreationWithId { get; set; }
        #endregion

        public virtual bool IsMentionedResourcesShortMatch { get; set; }

        public virtual string OrgsMentioned { get; set; }

        public virtual string OrgsMentionedString { get; set; }

        public virtual string PeopleMentioned { get; set; }

        public virtual string PeopleMentionedString { get; set; }

        public virtual string CreationsMentioned { get; set; }

        public virtual string CreationsMentionedString { get; set; }

        public virtual bool IsOrgsMentionedChanged { get; set; }

        public virtual bool IsPeopleMentionedChanged { get; set; }

        public virtual bool IsCreationsMentionedChanged { get; set; }

        public virtual Topic PrimaryTopic { get; set; }
        public virtual Topic SecondaryTopic { get; set; }
        public virtual int? PrimaryTopicID { get; set; }
        public virtual int? SecondaryTopicID { get; set; }

        public virtual string TopicHashtags { get; set; }

        public virtual string OmitHashtags { get; set; }

        public virtual string ExtraHashtags { get; set; }

        public virtual string UrlName { get; set; }

        public virtual DateTime? SocialPostDate { get; set; }

        public virtual string PostFacebook { get; set; }

        public virtual string PostLinkedIn { get; set; }

        public virtual string PostTwitter { get; set; }

        public virtual string ImagePreview { get; set; }
        public virtual bool HasImagePreview => !string.IsNullOrWhiteSpace(ImagePreview);

        public virtual string BackUrl { get; set; }
        public virtual int? TwitterCharacterCount { get; set; }
    }
}
