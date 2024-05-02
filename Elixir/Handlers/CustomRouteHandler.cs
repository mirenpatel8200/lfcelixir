using System.Web;
using System.Web.Routing;

namespace Elixir.Handlers
{
    public class CustomRouteHandler : IRouteHandler
    {
        public string Path { get; set; }

        public CustomRouteHandler(string path)
        {
            Path = path;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new CustomHandler(requestContext, Path);
        }
    }
}