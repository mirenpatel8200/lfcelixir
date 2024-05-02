using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Validation;
using Elixir.Utils.Reflection;
using Elixir.Utils.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Elixir.Areas.Admin.Models
{
    public sealed class ResourceModel : Resource
    {
        public ResourceModel()
        {
            IsEnabled = true;
            IsPinnedPrimaryTopic = false;
            IsPinnedSecondaryTopic = false;
            PinnedPrimaryTopicOrder = 255;
            PinnedSecondaryTopicOrder = 255;
            IsClubDiscount = false;
        }

        public ResourceModel(Resource resource)
        {
            IsEnabled = true;
            ReflectionUtils.ClonePublicProperties(resource, this);

            this.ResourceTypeString = GetResourceTypeById(this.ResourceTypeId.ToString());

            CountryId = resource.CountryId;
            DnCountryName = resource.DnCountryName;
            ConstructCountryNameWithId();

            ConstructParentResourceWithId();
        }

        private string GetResourceTypeById(string resourceTypeId)
        {
            var resourceById = GetAllResourceTypes().First(
                rt => rt.Value == resourceTypeId);
            return resourceById.Text;
        }

        [Required(ErrorMessage = "Resource Name is required.")]
        [MaxLength(255, ErrorMessage = "Length should be less than 255.")]
        public override string ResourceName { get; set; }

        [MaxLength(2048, ErrorMessage = "External URL: Length should be less than 255.")]
        public override string ExternalUrl { get; set; }

        [MaxLength(255, ErrorMessage = "Facebook Handle: Length should be less than 255.")]
        public override string FacebookHandle { get; set; }

        [MaxLength(255, ErrorMessage = "Twitter Handle: Length should be less than 255.")]
        public override string TwitterHandle { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public override int ResourceTypeId { get; set; }

        public string ResourceTypeString { get; set; }

        [MaxLength(255, ErrorMessage = "Notes Internal: Length should be less than 255.")]
        public override string NotesInternal { get; set; }

        [Required(ErrorMessage = "Created DateTime is required.")]
        public override DateTime CreatedDT { get; set; }

        [Required(ErrorMessage = "Updated DateTime is required.")]
        public override DateTime UpdatedDT { get; set; }

        [MaxLength(255, ErrorMessage = "Resource Description: Length should be less than 255.")]
        public override string ResourceDescriptionInternal { get; set; }

        [AllowHtml]
        public override string ContentMain { get; set; }

        [MaxLength(255, ErrorMessage = "LinkedIn URL: Length should be less than 255.")]
        public override string LinkedInUrl { get; set; }

        [MaxLength(255, ErrorMessage = "YouTube URL: Length should be less than 255.")]
        public override string YouTubeUrl { get; set; }

        [Required(ErrorMessage = "URL Name is required.")]
        [MaxLength(255, ErrorMessage = "URL Name: Length should be less than 255.")]
        [IsUrlName]
        public override string UrlName { get; set; }

        [MaxLength(20, ErrorMessage = "Amazon Code: Length should be less than 20.")]
        public override string AmazonProductCode { get; set; }

        [MaxLength(255, ErrorMessage = "Resource Description: Maximum 255 characters.")]
        public override string ResourceDescriptionPublic { get; set; }

        public override string OrgsMentioned { get; set; }
        public override string OrgsMentionedString { get; set; }

        public override string PeopleMentioned { get; set; }
        public override string PeopleMentionedString { get; set; }

        public override string CreationsMentioned { get; set; }
        public override string CreationsMentionedString { get; set; }


        public override bool IsOrgsMentionedChanged { get; set; }
        public override bool IsPeopleMentionedChanged { get; set; }
        public override bool IsCreationsMentionedChanged { get; set; }

        public string CountryNameWithId { get; set; }
        public string DnCountryName { get; set; }

        private void ConstructCountryNameWithId()
        {
            CountryNameWithId = DnCountryName;// ViewUtils.FormatAutocompleteResource(DnCountryName, CountryId);
        }

        private IEnumerable<SelectListItem> GetAllResourceTypes()
        {
            var list = new List<SelectListItem>();

            var allResourceTypes = Enum.GetValues(typeof(ResourceTypes));
            foreach (var resourceType in allResourceTypes)
            {
                var sli = new SelectListItem
                {
                    Text = resourceType.ToString(),
                    Value = ((ResourceTypes)resourceType).ToDatabaseValues().First().ToString()
                };

                list.Add(sli);
            }

            return list;
        }

        #region Parent Resource with ID

        [MaxLength(255, ErrorMessage = "Length should be less than 255.")]
        public string ParentResourceWithId { get; set; }

        private void ConstructParentResourceWithId()
        {
            ParentResourceWithId = ViewUtils.FormatAutocompleteParentResource(DnParentResourceName, ParentResourceID);
        }

        public void DeconstructParentResourceWithId()
        {
            if (string.IsNullOrWhiteSpace(ParentResourceWithId))
            {
                ParentResourceID = null;
                DnParentResourceName = null;
            }
            else
            {
                var parsed = ViewUtils.ParseAutocompleteResource(ParentResourceWithId);

                ParentResourceID = parsed.ResourceId;
                DnParentResourceName = parsed.ResourceName;
            }
        }

        #endregion
    }
}