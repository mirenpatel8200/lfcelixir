using System.Web;

namespace Elixir.Models.Exceptions
{
    public class ContentNotFoundException : HttpException
    {
        public ContentNotFoundException(string message) : base(404, message) { }

        public ContentNotFoundException() : this("") { }
    }
}