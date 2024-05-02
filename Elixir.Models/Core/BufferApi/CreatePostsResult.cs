using System.Collections.Generic;

namespace Elixir.Models.Core.BufferApi
{
    public class CreatePostsResult : List<CreatePostResult>
    {
        public CreatePostsResult() {}

        public CreatePostsResult(IEnumerable<CreatePostResult> collection): base(collection) { }
    }
}
