using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Models
{
    public class UserRole
    {
        public virtual int UserRoleId { get; set; }
        public virtual int? UserId { get; set; }
        public virtual Role Role { get; set; }
        public virtual int? RoleId { get; set; }
    }
}
