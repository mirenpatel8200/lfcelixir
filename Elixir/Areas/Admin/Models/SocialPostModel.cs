using Elixir.Attributes;
using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Validation;
using Elixir.Utils.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Elixir.Areas.Admin.Models
{
    public class SocialPostModel : SocialPost
    {
        public override EntityType? EntityType { get; set; }

        public override string EntityTypeName { get; set; }

        public override int? EntityId { get; set; }

        [MaxLength(255, ErrorMessage = "First Line: Length should be less than 255.")]
        public override string FirstLine { get; set; }

        [MaxLength(255, ErrorMessage = "Other Content: Length should be less than 255.")]
        public override string OtherContent { get; set; }

        [MaxLength(255, ErrorMessage = "Last Line: Length should be less than 255.")]
        public override string LastLine { get; set; }
        public override bool IsKeyResourcesShortMatch { get; set; }

        #region KeyOrganisationWithId
        public override string DnPublisherName { get; set; }
        public override int? KeyOrganisationId { get; set; }
        public override bool IsSuffixKeyOrganisation { get; set; }
        public override string KeyOrganisationWithId { get; set; }
        #endregion

        #region KeyPersonWithId
        public override string DnReporterName { get; set; }
        public override int? KeyPersonId { get; set; }
        public override bool IsSuffixKeyPerson { get; set; }
        public override string KeyPersonWithId { get; set; }
        #endregion

        #region KeyCreationWithId
        public override string DnCreationName { get; set; }
        public override int? KeyCreationId { get; set; }
        public override bool IsSuffixKeyCreation { get; set; }
        public override string KeyCreationWithId { get; set; }
        #endregion

        public override bool IsMentionedResourcesShortMatch { get; set; }

        public override string OrgsMentioned { get; set; }

        public override string OrgsMentionedString { get; set; }

        public override string PeopleMentioned { get; set; }

        public override string PeopleMentionedString { get; set; }

        public override string CreationsMentioned { get; set; }

        public override string CreationsMentionedString { get; set; }

        public override bool IsOrgsMentionedChanged { get; set; }

        public override bool IsPeopleMentionedChanged { get; set; }

        public override bool IsCreationsMentionedChanged { get; set; }

        public override Topic PrimaryTopic { get; set; }
        public override Topic SecondaryTopic { get; set; }
        public override int? PrimaryTopicID { get; set; }
        public override int? SecondaryTopicID { get; set; }

        [HashtagsTextValidator("Topic Hashtags: Invalid format. Please include the preceeding '#', and if multiple hashtags, separate them by space.")]
        public override string TopicHashtags { get; set; }

        [HashtagsTextValidator("Omit Hashtags: Invalid format. Please include the preceeding '#', and if multiple hashtags, separate them by space.")]
        public override string OmitHashtags { get; set; }

        [HashtagsTextValidator("Extra Hashtags: Invalid format. Please include the preceeding '#', and if multiple hashtags, separate them by space.")]
        public override string ExtraHashtags { get; set; }

        public override string UrlName { get; set; }

        public override DateTime? SocialPostDate { get; set; }

        public override string PostFacebook { get; set; }

        public override string PostLinkedIn { get; set; }

        public override string PostTwitter { get; set; }

        public override string ImagePreview { get; set; }
        public override bool HasImagePreview => !string.IsNullOrWhiteSpace(ImagePreview);

        public override string BackUrl { get; set; }
        public override int? TwitterCharacterCount { get; set; }
    }
}