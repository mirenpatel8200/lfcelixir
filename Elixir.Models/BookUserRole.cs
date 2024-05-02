using System;
using Microsoft.AspNet.Identity;

namespace Elixir.Models
{
    public class BookUserRole : IRole<int>
    {
        public BookUserRole()
        {
            
        }

        public BookUserRole(String roleName)
        {
            Name = roleName;
        }

        public virtual String Name { get; set; }
        public int Id { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
