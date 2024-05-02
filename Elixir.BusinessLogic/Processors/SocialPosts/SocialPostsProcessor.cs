using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elixir.BusinessLogic.Core.Utils;
using Elixir.BusinessLogic.Exceptions;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;

namespace Elixir.BusinessLogic.Processors.SocialPosts
{
    public class SocialPostsProcessor : ISocialPostsProcessor
    {
        private readonly IResourcesRepository _resourcesRepository;

        public SocialPostsProcessor(IResourcesRepository resourcesRepository)
        {
            _resourcesRepository = resourcesRepository;
        }

        public string ComposeSocialPost(SocialNetwork socialNetwork, SocialPost socialPost)
        {
            var publisherName = "";
            int countFacebook = 0, countLinkedIn = 0, countTwitter = 0;
            if (socialPost.KeyOrganisationId.HasValue)
            {
                if (socialPost.IsSuffixKeyOrganisation)
                    publisherName = $" - {socialPost.DnPublisherName}";
                var resource = _resourcesRepository.GetResourceById(socialPost.KeyOrganisationId.Value);
                if (resource == null)
                    throw new ModelValidationException("Unable to find organization resource with specified id.");
                switch (socialNetwork)
                {
                    case SocialNetwork.Facebook:
                        if (!string.IsNullOrWhiteSpace(resource.FacebookHandle))
                        {
                            publisherName = $" @{resource.FacebookHandle}"; countFacebook++;
                        }
                        break;
                    case SocialNetwork.LinkedIn:
                        var linkedInHandle = ExtractLinkedInHandle(resource.LinkedInUrl);
                        if (!string.IsNullOrEmpty(linkedInHandle))
                        {
                            publisherName = $" @{linkedInHandle}"; countLinkedIn++;
                        }
                        break;
                    case SocialNetwork.Twitter:
                        if (!string.IsNullOrWhiteSpace(resource.TwitterHandle))
                        {
                            publisherName = $" @{resource.TwitterHandle}"; countTwitter++;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(socialNetwork), socialNetwork, null);
                }
            }

            var reporterHandle = "";
            if (socialPost.KeyPersonId.HasValue)
            {
                if (socialPost.IsSuffixKeyPerson)
                    reporterHandle = $" - {socialPost.DnReporterName}";
                var resource = _resourcesRepository.GetResourceById(socialPost.KeyPersonId.Value);
                if (resource == null)
                    throw new ModelValidationException("Unable to find reporter resource with specified id.");

                switch (socialNetwork)
                {
                    case SocialNetwork.Facebook:
                        if (!string.IsNullOrWhiteSpace(resource.FacebookHandle))
                        {
                            reporterHandle = $" @{resource.FacebookHandle}"; countFacebook++;
                        }
                        break;
                    case SocialNetwork.LinkedIn:
                        var linkedInHandle = ExtractLinkedInHandle(resource.LinkedInUrl);
                        if (!string.IsNullOrEmpty(linkedInHandle))
                        {
                            reporterHandle = $" @{linkedInHandle}"; countLinkedIn++;
                        }
                        break;
                    case SocialNetwork.Twitter:
                        if (!string.IsNullOrWhiteSpace(resource.TwitterHandle))
                        {
                            reporterHandle = $" @{resource.TwitterHandle}"; countTwitter++;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(socialNetwork), socialNetwork, null);
                }
            }

            var creationHandle = "";
            if (socialPost.KeyCreationId.HasValue)
            {
                if (socialPost.IsSuffixKeyCreation)
                    creationHandle = $" - {socialPost.DnCreationName}";
                var resource = _resourcesRepository.GetResourceById(socialPost.KeyCreationId.Value);
                if (resource == null)
                    throw new ModelValidationException("Unable to find creation resource with specified id.");
                switch (socialNetwork)
                {
                    case SocialNetwork.Facebook:
                        if (!string.IsNullOrWhiteSpace(resource.FacebookHandle))
                        {
                            creationHandle = $" @{resource.FacebookHandle}"; countFacebook++;
                        }
                        break;
                    case SocialNetwork.LinkedIn:
                        var linkedInHandle = ExtractLinkedInHandle(resource.LinkedInUrl);
                        if (!string.IsNullOrEmpty(linkedInHandle))
                        {
                            creationHandle = $" @{linkedInHandle}"; countLinkedIn++;
                        }
                        break;
                    case SocialNetwork.Twitter:
                        if (!string.IsNullOrWhiteSpace(resource.TwitterHandle))
                        {
                            creationHandle = $" @{resource.TwitterHandle}"; countTwitter++;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(socialNetwork), socialNetwork, null);
                }
            }

            string socialMentions = "";
            var mentionedOrgs = GetResources(socialPost.OrgsMentionedString);
            var mentionedPeople = GetResources(socialPost.PeopleMentionedString);
            var mentionedCreations = GetResources(socialPost.CreationsMentionedString);
            string fbHandle, twHandle, liUrl;
            if (mentionedOrgs.Count() > 0)
            {
                switch (socialNetwork)
                {
                    case SocialNetwork.Facebook:
                        for (int i = 0; i < mentionedOrgs.Count(); i++)
                        {
                            fbHandle = mentionedOrgs[i].FacebookHandle;
                            if (countFacebook < 5 && !string.IsNullOrEmpty(fbHandle) &&
                                !socialMentions.Contains("@" + fbHandle) &&
                                !publisherName.Contains("@" + fbHandle) &&
                                !reporterHandle.Contains("@" + fbHandle))
                            {
                                socialMentions += $" @{mentionedOrgs[i].FacebookHandle}";
                                countFacebook++;
                            }
                            if (countFacebook >= 5) break;
                        }
                        break;
                    case SocialNetwork.Twitter:
                        for (int i = 0; i < mentionedOrgs.Count(); i++)
                        {
                            twHandle = mentionedOrgs[i].TwitterHandle;
                            if (countTwitter < 5 && !string.IsNullOrEmpty(twHandle) &&
                                !socialMentions.Contains("@" + twHandle) &&
                                !publisherName.Contains("@" + twHandle) &&
                                !reporterHandle.Contains("@" + twHandle))
                            {
                                socialMentions += $" @{twHandle}";
                                countTwitter++;
                            }
                            if (countTwitter >= 5) break;
                        }
                        break;
                    case SocialNetwork.LinkedIn:
                        for (int i = 0; i < mentionedOrgs.Count(); i++)
                        {
                            liUrl = mentionedOrgs[i].LinkedInUrl;
                            if (countLinkedIn < 5 && !string.IsNullOrEmpty(liUrl))
                            {
                                string handle = ExtractLinkedInHandle(liUrl);
                                if (handle == null) continue;
                                else
                                {
                                    if (!socialMentions.Contains("@" + handle) &&
                                        !publisherName.Contains("@" + handle) &&
                                        !reporterHandle.Contains("@" + handle))
                                    {
                                        socialMentions += $" @{handle}";
                                        countLinkedIn++;
                                    }
                                }
                            }
                            if (countLinkedIn >= 5) break;
                        }
                        break;
                }
            }
            if (mentionedPeople.Count() > 0)
            {
                switch (socialNetwork)
                {
                    case SocialNetwork.Facebook:
                        for (int i = 0; i < mentionedPeople.Count(); i++)
                        {
                            fbHandle = mentionedPeople[i].FacebookHandle;
                            if (countFacebook < 5 && !string.IsNullOrEmpty(fbHandle) &&
                                !socialMentions.Contains("@" + fbHandle) &&
                                !publisherName.Contains("@" + fbHandle) &&
                                !reporterHandle.Contains("@" + fbHandle))
                            {
                                socialMentions += $" @{fbHandle}";
                                countFacebook++;
                            }
                            if (countFacebook >= 5) break;
                        }
                        break;
                    case SocialNetwork.Twitter:
                        for (int i = 0; i < mentionedPeople.Count(); i++)
                        {
                            twHandle = mentionedPeople[i].TwitterHandle;
                            if (countTwitter < 5 && !string.IsNullOrEmpty(twHandle) &&
                                !socialMentions.Contains("@" + twHandle) &&
                                !publisherName.Contains("@" + twHandle) &&
                                !reporterHandle.Contains("@" + twHandle))
                            {
                                socialMentions += $" @{twHandle}";
                                countTwitter++;
                            }
                            if (countTwitter >= 5) break;
                        }
                        break;
                    case SocialNetwork.LinkedIn:
                        for (int i = 0; i < mentionedPeople.Count(); i++)
                        {
                            liUrl = mentionedPeople[i].LinkedInUrl;
                            if (countLinkedIn < 5 && !string.IsNullOrEmpty(liUrl))
                            {
                                string handle = ExtractLinkedInHandle(liUrl);
                                if (handle == null) continue;
                                else
                                {
                                    if (!socialMentions.Contains("@" + handle) &&
                                        !publisherName.Contains("@" + handle) &&
                                        !reporterHandle.Contains("@" + handle))
                                    {
                                        socialMentions += $" @{handle}";
                                        countLinkedIn++;
                                    }
                                }
                            }
                            if (countLinkedIn >= 5) break;
                        }
                        break;
                }
            }
            if (mentionedCreations.Count() > 0)
            {
                switch (socialNetwork)
                {
                    case SocialNetwork.Facebook:
                        for (int i = 0; i < mentionedCreations.Count(); i++)
                        {
                            fbHandle = mentionedCreations[i].FacebookHandle;
                            if (countFacebook < 5 && !string.IsNullOrEmpty(fbHandle) &&
                                !socialMentions.Contains("@" + fbHandle) &&
                                    !publisherName.Contains("@" + fbHandle) &&
                                    !reporterHandle.Contains("@" + fbHandle))
                            {
                                socialMentions += $" @{fbHandle}";
                                countFacebook++;
                            }
                            if (countFacebook >= 5) break;
                        }
                        break;
                    case SocialNetwork.Twitter:
                        for (int i = 0; i < mentionedCreations.Count(); i++)
                        {
                            twHandle = mentionedCreations[i].TwitterHandle;
                            if (countTwitter < 5 && !string.IsNullOrEmpty(twHandle) &&
                                !socialMentions.Contains("@" + twHandle) &&
                                !publisherName.Contains("@" + twHandle) &&
                                !reporterHandle.Contains("@" + twHandle))
                            {
                                socialMentions += $" @{twHandle}";
                                countTwitter++;
                            }
                            if (countTwitter >= 5) break;
                        }
                        break;
                    case SocialNetwork.LinkedIn:
                        for (int i = 0; i < mentionedCreations.Count(); i++)
                        {
                            liUrl = mentionedCreations[i].LinkedInUrl;
                            if (countLinkedIn < 5 && !string.IsNullOrEmpty(liUrl))
                            {
                                string handle = ExtractLinkedInHandle(liUrl);
                                if (handle == null) continue;
                                else
                                {
                                    if (!socialMentions.Contains("@" + handle) &&
                                        !publisherName.Contains("@" + handle) &&
                                        !reporterHandle.Contains("@" + handle))
                                    {
                                        socialMentions += $" @{handle}";
                                        countLinkedIn++;
                                    }
                                }
                            }
                            if (countLinkedIn >= 5) break;
                        }
                        break;
                }
            }

            var topicHashtags = new List<string>();
            if (!string.IsNullOrEmpty(socialPost.TopicHashtags))
            {
                var tags = socialPost.TopicHashtags.Split(' ').ToList();
                foreach (var tag in tags)
                {
                    if (!string.IsNullOrEmpty(tag))
                        topicHashtags.Add(tag);
                }
            }
            if (!string.IsNullOrEmpty(socialPost.OmitHashtags))
            {
                var omitHashtags = socialPost.OmitHashtags.Split(' ').ToList();
                if (omitHashtags != null && omitHashtags.Count > 0)
                {
                    foreach (var tag in omitHashtags)
                    {
                        if (!string.IsNullOrEmpty(tag))
                            topicHashtags.Remove(tag);
                    }
                }
            }
            if (!string.IsNullOrEmpty(socialPost.ExtraHashtags))
            {
                var extraHashtags = socialPost.ExtraHashtags.Split(' ').ToList();
                if (extraHashtags != null && extraHashtags.Count > 0)
                {
                    foreach (var tag in extraHashtags)
                    {
                        if (!string.IsNullOrEmpty(tag))
                            topicHashtags.Add(tag);
                    }
                }
            }
            var topicHashtagsString = "";
            if (topicHashtags.Count > 0)
                topicHashtagsString = $" {string.Join(" ", topicHashtags)}";
            var sb = new StringBuilder();
            sb.AppendFormat(@"{0}{1}{2}{3}{4}", socialPost.FirstLine, publisherName, reporterHandle, creationHandle, socialMentions).AppendLine();
            if (!string.IsNullOrEmpty(socialPost.OtherContent))
                sb.AppendFormat(@"{0}", socialPost.OtherContent).AppendLine();
            sb.AppendFormat(@"{0}{1}", socialPost.LastLine, topicHashtagsString);
            if (!string.IsNullOrEmpty(socialPost.UrlName))
            {
                sb.AppendLine();
                sb.AppendFormat(@"{0}", socialPost.UrlName);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Extracts 'handle' after /company/ string in linkedin URL.
        /// For example from https://www.linkedin.com/company/frontiers/ the 'frontiers' is to be extracted.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Extracted handle or null if URL is not linked in, doesn't have /company/ part or does not have handle.</returns>
        public string ExtractLinkedInHandle(string url)
        {
            if (string.IsNullOrWhiteSpace(url) || (url.IndexOf("www.linkedin.com", StringComparison.OrdinalIgnoreCase) == -1))
                return null;
            string handle = null;
            string companyString = "/company/", schoolString = "/school/";
            var cleanUrl = url.Trim().TrimEnd('/');
            var companyIndex = cleanUrl.LastIndexOf(companyString, StringComparison.OrdinalIgnoreCase);

            if (companyIndex != -1)
            {
                handle = cleanUrl.Substring(companyIndex + companyString.Length);
            }
            else
            {
                int schoolIndex = cleanUrl.LastIndexOf(schoolString, StringComparison.OrdinalIgnoreCase);
                if (schoolIndex == -1) return null;
                else
                {
                    handle = cleanUrl.Substring(schoolIndex + schoolString.Length);
                }
            }

            return handle;
        }

        private List<Resource> GetResources(string resourcesMentioned)
        {
            var resourceIds = new List<int>();
            var resources = new List<Resource>();
            if (string.IsNullOrEmpty(resourcesMentioned))
                return resources;
            int resourceId;
            var resourceTags = resourcesMentioned.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < resourceTags.Length; i++)
            {
                resourceId = ExtractIdFromResourceTag(resourceTags[i]);
                if (resourceId != -1 && resourceIds.Contains(resourceId) == false)
                    resourceIds.Add(resourceId);
            }
            return GetResourcesByIds(resourceIds);
        }

        private int ExtractIdFromResourceTag(string tagText)
        {
            int resourceId, beginId, endId;
            beginId = tagText.IndexOf("[");
            endId = tagText.IndexOf("]");
            if (beginId == -1 || endId == -1) return -1;
            string idString = tagText.Substring(beginId + 1,
                endId - beginId - 1);

            if (int.TryParse(idString, out resourceId) == true)
                return resourceId;

            return -1;
        }

        private List<Resource> GetResourcesByIds(List<int> resourceIds)
        {
            var resources = new List<Resource>();
            if (resourceIds.Count > 0)
            {
                foreach (var resourceId in resourceIds)
                {
                    var resource = _resourcesRepository.GetResourceById(resourceId);
                    if (resource != null)
                        resources.Add(resource);
                }
            }
            return resources;
        }
    }
}
