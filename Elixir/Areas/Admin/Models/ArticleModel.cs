using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Elixir.Models;
using Elixir.Models.Validation;
using Elixir.Utils.Reflection;
using Elixir.Utils.View;

namespace Elixir.Areas.Admin.Models
{
    public sealed class ArticleModel : Article
    {
        public ArticleModel()
        {
            InitializeCustomImageFields();
        }

        public ArticleModel(Article model)
        {
            ReflectionUtils.ClonePublicProperties(model, this);

            if (model.PrimaryTopic?.Id != null)
                PrimaryTopicID = model.PrimaryTopic.Id.Value;

            if (model.SecondaryTopic?.Id != null)
                SecondaryTopicID = model.SecondaryTopic.Id.Value;

            BulletPointsWithBrs = PutBrs(model.BulletPoints);

            ConstructPublisherWithId();
            ConstructReporterWithId();
            InitializeCustomImageFields(model.Title);
        }

        [Required(ErrorMessage = "Title name is required.")]
        [MaxLength(256, ErrorMessage = "Length should be less than 256.")]
        public override string Title { get; set; }
        [Required(ErrorMessage = "Original Title is required.")]
        [MaxLength(256, ErrorMessage = "Length should be less than 256.")]
        public override string OriginalTitle { get; set; }

        [Required(ErrorMessage = "Publisher URL is required.")]
        [MaxLength(256, ErrorMessage = "Length should be less than 256.")]
        [IsUrlName(ErrorMessage = "PublisherUrl: Invalid Publisher URL", PublisherUrlCheck = true)]
        public override string PublisherUrl { get; set; }

        [Required(ErrorMessage = "The ArticleDate field is required.")]
        [DisplayFormat(DataFormatString = "{0: MM/dd/yyyy}")]
        public DateTime? ArticleDateNullable { get; set; }

        [Required(ErrorMessage = "Primary topic is required.")]
        public override int PrimaryTopicID { get; set; }

        [MaxLength(1024, ErrorMessage = "Length should be less than 1024.")]
        public override string BulletPoints { get; set; }

        public string BulletPointsWithBrs { get; }

        private string PutBrs(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            return input.Replace("\r\n", "\n").Replace("\n", "<br />");
        }

        [MaxLength(255, ErrorMessage = "Length should be less than 255.")]
        public override string Summary { get; set; }

        public bool HasSummary => !string.IsNullOrWhiteSpace(Summary);

        public override bool IsEnabled { get; set; }

        [MaxLength(512, ErrorMessage = "Length should be less than 512.")]
        public override string Notes { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Length should be less than 255.")]
        [IsUrlName]
        public override string UrlName { get; set; }

        [MaxLength(255, ErrorMessage = "Length should be less than 255.")]
        public override string SocialImageFilename { get; set; }

        public override string OrgsMentioned { get; set; }

        public override string OrgsMentionedString { get; set; }

        public override string PeopleMentioned { get; set; }

        public override string PeopleMentionedString { get; set; }

        public override string CreationsMentioned { get; set; }

        public override string CreationsMentionedString { get; set; }

        public override bool IsOrgsMentionedChanged { get; set; }

        public override bool IsPeopleMentionedChanged { get; set; }

        public override bool IsCreationsMentionedChanged { get; set; }

        #region ReporterNameWithId

        [Required(ErrorMessage = "Reporter name is required. ID can be ommitted, but a name is required.")]
        [MaxLength(256, ErrorMessage = "Length should be less than 256.")]
        public string ReporterNameWithId { get; set; }

        private void ConstructReporterWithId()
        {
            ReporterNameWithId = ViewUtils.FormatAutocompleteResource(DnReporterName, ReporterResourceId);
        }

        public void DeconstructReporterWithId()
        {
            if (string.IsNullOrWhiteSpace(ReporterNameWithId))
            {
                ReporterResourceId = null;
                DnReporterName = null;
            }
            else
            {
                var parsed = ViewUtils.ParseAutocompleteResource(ReporterNameWithId);

                ReporterResourceId = parsed.ResourceId;
                DnReporterName = parsed.ResourceName;
            }
        }

        #endregion

        #region PublisherNameWithID

        [Required(ErrorMessage = "Publisher name with id is required.")]
        [MaxLength(256, ErrorMessage = "Length should be less than 256.")]
        public string PublisherNameWithId { get; set; }

        private void ConstructPublisherWithId()
        {
            PublisherNameWithId = ViewUtils.FormatAutocompleteResource(DnPublisherName, PublisherResourceId);
        }

        /// <summary>
        /// Parses PublisherNameWithId field and extracts publisher name and id.
        /// Then it populates PublisherResourceId and DnPublisherName respectively.
        /// Note that DnPublisherName is populated basing on user input, not on database real publisher name.
        /// </summary>
        public void DeconstructPublisherWithId()
        {
            if (string.IsNullOrWhiteSpace(PublisherNameWithId))
            {
                PublisherResourceId = null;
                DnPublisherName = null;
            }
            else
            {
                var parsed = ViewUtils.ParseAutocompleteResource(PublisherNameWithId);

                PublisherResourceId = parsed.ResourceId;
                DnPublisherName = parsed.ResourceName;
            }
        }

        #endregion

        #region Custom Image form fields
        //Non-entity fields, for custom image generation
        [MaxLength(255, ErrorMessage = "Custom Image Title: length should be less than 255.")]
        public string CustomImageTitle { get; set; }

        public string CustomImageTextColor { get; set; }

        public int CustomImageTextSize { get; set; }

        public string CustomImageBackgroundImagePath { get; set; }

        public Dictionary<string, string> CustomImageTextColors { get; set; }

        public string CustomImageFilename { get; set; }

        private void InitializeCustomImageFields(string imageTitle = null)
        {
            CustomImageTextColors = new Dictionary<string, string>
            {
                { "Dark Grey (#403F3D)", "#403F3D" },
                { "Light Blue (#07C0F8)", "#07C0F8"},
                { "Light Grey (#DFDEDC)", "#DFDEDC" },
                { "Orange (#FE6A00)", "#FE6A00" },
                { "White (#FFFFFD)", "#FFFFFD" }
            };
            CustomImageTextColor = CustomImageTextColors["White (#FFFFFD)"];

            CustomImageTitle = imageTitle;
            CustomImageTextSize = 36;
        }
        #endregion

        public bool ShortMatch { get; set; }

        public bool IsMentionedResourcesShortMatch { get; set; }
    }
}