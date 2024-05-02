using System.Web.Mvc;
using Elixir.BusinessLogic.Core.Utils;
using Elixir.BusinessLogic.Exceptions;

namespace Elixir.Models.Filters
{
    public class FilterQueryParameterAttribute : ActionFilterAttribute
    {
        private readonly string _paramToFilter;
        public FilterQueryParameterAttribute(string paramToFilter)
        {
            _paramToFilter = paramToFilter;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            object srcValueRaw;
            if (!filterContext.ActionParameters.TryGetValue(_paramToFilter, out srcValueRaw))
                throw new ModelValidationException(
                    $"There is no '{_paramToFilter}' parameter in current ActionParameters list.");

            if (srcValueRaw == null)
            {
                filterContext.ActionParameters[_paramToFilter] = null;
            }
            else
            {
                var srcValue = srcValueRaw.ToString();
                var filteredValue = srcValue.GetAlphanumericValue();

                filterContext.ActionParameters[_paramToFilter] = filteredValue;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}