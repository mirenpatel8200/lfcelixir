using System.ComponentModel.DataAnnotations;
using Elixir.Models;
using Elixir.Utils.Reflection;

namespace Elixir.Areas.Admin.Models
{
    public sealed class GoLinkModel : GoLink
    {
        public GoLinkModel()
        {
            IsEnabled = true;
        }

        public GoLinkModel(GoLink entity)
        {
            ReflectionUtils.ClonePublicProperties(entity, this);
        }

        [Required]
        [MaxLength(255)]
        public override string GoLinkTitle { get; set; }

        [Required]
        [MaxLength(255)]
        public override string ShortCode { get; set; }

        [Required]
        [MaxLength(1024)]
        public override string DestinationUrl { get; set; }

        [MaxLength(255)]
        public override string NotesInternal { get; set; }
    }
}