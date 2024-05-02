using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elixir.BusinessLogic.Core.Api.Buffer.JsonSchema;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Core;
using Elixir.Models.Core.BufferApi;
using RestSharp;

namespace Elixir.BusinessLogic.Core.Api.Buffer
{
    public class BufferClient : OauthClientBase, IBufferClient
    {
        private readonly ISettingsProcessor _settingsProcessor;
        private string _lastDomainName;
        private string _lastAccessToken;

        public BufferClient(ISettingsProcessor settingsProcessor) : base("https://api.bufferapp.com/1/")
        {
            _settingsProcessor = settingsProcessor;
        }

        public IBufferClient WithCredentials(string currentDomainName)
        {
            if (string.IsNullOrWhiteSpace(_lastDomainName) || !currentDomainName.Equals(_lastDomainName))
            {
                // Reload all.
                var liveDomainName = _settingsProcessor.GetLiveServerDomainName();

                if (currentDomainName.IndexOf(liveDomainName, StringComparison.OrdinalIgnoreCase) != -1)
                    AccessToken = _settingsProcessor.GetLiveBufferToken();
                else
                    AccessToken = _settingsProcessor.GetDevBufferToken();

                _lastAccessToken = AccessToken;
                _lastDomainName = currentDomainName;
            }
            else
            {
                AccessToken = _lastAccessToken;
            }

            return this;
        }

        public async Task<Dictionary<string, string>> GetProfiles()
        {
            // Apr20: suddenly getting "Could not create SSL/TLS secure channel" returned - even though hadn't deployed new code or applied updates to server
            // Buffer say they haven't changed anything their end (though could be a Windows update they're not aware of)
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls11;
            var request = CreateRequest("profiles.json");

            var r = await ExecuteRequestAsync(request).ConfigureAwait(false);
            var rObj = JsonDeserializer.TryDeserialize<Profiles>(r.Content);
            if (rObj == null)
            {
                var rError = JsonDeserializer.TryDeserialize<ErrorResponse>(r.Content);
                throw new BufferApiException(rError);
            }

            return rObj.ToDictionary(x => x.id, x => x.service);
        }

        public async Task<string> GetPendingUpdates(string profileId)
        {
            if (string.IsNullOrWhiteSpace(profileId))
                throw new ArgumentNullException(nameof(profileId));

            var request = CreateRequest("profiles/{id}/updates/pending.json");
            request.AddUrlSegment("id", profileId);

            var r = await ExecuteRequestAsync(request).ConfigureAwait(false);
            return r.Content;
        }

        public async Task<CreatePostsResult> CreatePosts(Dictionary<string, string> idsPosts)
        {
            var results = new CreatePostsResult();

            foreach (var idsPost in idsPosts)
                results.AddRange(await CreatePost(idsPost.Value, new[] { idsPost.Key }));

            return results;
        }

        public async Task<CreatePostsResult> CreatePost(string text, string[] profileIds)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text));

            var request = CreateRequest("updates/create.json", Method.POST);

            foreach (var profileId in profileIds)
            {
                request.AddParameter("profile_ids[]", profileId);
            }

            request.AddParameter("text", text);

            var rRaw = await ExecuteRequestAsync(request).ConfigureAwait(false);
            var rObj = JsonDeserializer.TryDeserialize<UpdatesCreateResponse>(rRaw.Content);
            if (rObj == null || rObj.success == false) // should also check !rRaw.IsSuccessful ?
            {
                var error = JsonDeserializer.TryDeserialize<ErrorResponse>(rRaw.Content);
                throw new BufferApiException(error);
            }

            var r = new CreatePostsResult(rObj.updates.Select(x => new CreatePostResult() { IsSuccess = true, ProfileId = x.profile_id, ServiceName = x.profile_service}));

            return r;
        }
    }
}
