using System;
using System.Threading.Tasks;
using RestSharp;

namespace Elixir.BusinessLogic.Core.Api
{
    public abstract class OauthClientBase
    {
        public string AccessToken { get; protected set; }

        private readonly string _baseUrl;

        private readonly RestClient _restClient;

        protected OauthClientBase(string baseUrl)
        {
            _baseUrl = baseUrl;

            _restClient = new RestClient(_baseUrl);
        }

        protected RestRequest CreateRequest(string resource, Method method = Method.GET, bool pretty = false)
        {
            if(string.IsNullOrWhiteSpace(AccessToken))
                throw new NullReferenceException($"{nameof(AccessToken)} is not set.");

            var request = new RestRequest(resource, method);
            request.AddParameter("access_token", AccessToken);
            request.AddParameter("pretty", pretty);

            return request;
        }

        protected async Task<IRestResponse> ExecuteRequestAsync(RestRequest request)
        {
            var r = await _restClient.ExecuteTaskAsync(request).ConfigureAwait(false);
            //AccessToken = null;
            return r;
        }
    }
}
