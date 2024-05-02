using Elixir.BusinessLogic.Processors.SocialPosts;
using Elixir.Models;

namespace Elixir.Contracts.Interfaces
{
    public interface ISocialPostsProcessor
    {
        string ComposeSocialPost(SocialNetwork socialNetwork, SocialPost socialPost);
    }
}
