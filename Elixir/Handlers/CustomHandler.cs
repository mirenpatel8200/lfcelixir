using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Elixir.Handlers
{
    public class CustomHandler : IHttpHandler
    {
        public RequestContext LocalRequestContext { get; set; }
        public string ImageName { get; set; }
        public string Path { get; set; }

        public CustomHandler(RequestContext requestContext, string path = "")
        {
            LocalRequestContext = requestContext;
            ImageName = requestContext.RouteData.Values["imageName"].ToString();
            Path = path;
        }

        public void ProcessRequest(HttpContext context)
        {
            var response = context.Response;
            var request = context.Request;
            var server = context.Server;
            var requestedImage = ImageName;
            const string requestNotAllowedImage = "broken.jpg";
            
            var actualPath = server.MapPath("~/imagesmembers/");

            response.Clear();
            response.ContentType = GetContentType(request.Url.ToString());

            var referer = request.ServerVariables["HTTP_REFERER"];

            if ((referer == null || referer.Contains("liveforever.club")) && 
                LocalRequestContext.HttpContext.Request.IsAuthenticated)
            {
                response.TransmitFile(actualPath + requestedImage);
            }
            else
            {
                response.TransmitFile(actualPath + requestNotAllowedImage);
            }
            response.End();
            
        }
        
        private static string GetContentType(string url)
        {
            switch (System.IO.Path.GetExtension(url))
            {
                case ".gif":
                    return "Image/gif";
                case ".jpg":
                    return "Image/jpeg";
                case ".png":
                    return "Image/png";
                default:
                    break;
            }
            return null;
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}