using System;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Elixir.RouteConstraints
{
    public class ExcludeController : IRouteConstraint
    {
        private readonly string[] _controllers;
        public ExcludeController(params string[] controllers)
        {
            _controllers = controllers;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            var currentName = values["controller"].ToString();
            if (_controllers.Any(x => x.Equals(currentName, StringComparison.OrdinalIgnoreCase)))
                return false;
            return true;
        }
    }
}