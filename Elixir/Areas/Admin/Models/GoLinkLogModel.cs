using Elixir.Models;
using Elixir.Utils.Reflection;

namespace Elixir.Areas.Admin.Models
{
    public class GoLinkLogModel : GoLinkLog
    {
        public GoLinkLogModel()
        {
            
        }

        public GoLinkLogModel(GoLinkLog log)
        {
            ReflectionUtils.ClonePublicProperties(log, this);
        }
    }
}