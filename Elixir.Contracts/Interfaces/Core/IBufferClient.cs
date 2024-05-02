using System.Collections.Generic;
using System.Threading.Tasks;
using Elixir.Models.Core.BufferApi;

namespace Elixir.Contracts.Interfaces.Core
{
    public interface IBufferClient
    {
        /// <summary>
        /// Returns mapping of profile ids to profile services names.
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, string>> GetProfiles();
        Task<string> GetPendingUpdates(string profileId);

        /// <summary>
        /// Creates posts for specified ids (keys of dictionary).
        /// </summary>
        /// <param name="idsPosts">Mapping of ids to posts which will be created.</param>
        /// <returns></returns>
        Task<CreatePostsResult> CreatePosts(Dictionary<string, string> idsPosts);
        Task<CreatePostsResult> CreatePost(string text, string[] profileIds);

        IBufferClient WithCredentials(string currentDomainName);

        string AccessToken { get; }
    }
}
