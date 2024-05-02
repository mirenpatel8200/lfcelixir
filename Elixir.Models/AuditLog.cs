using System;

namespace Elixir.Models
{
    public class AuditLog : BaseEntity
    {
        public int AuditLogID { get; set; }
        public DateTime CreatedDT { get; set; }
        public string IpAddressString { get; set; }   
        public int UserID { get; set; }
        public byte EntityTypeID { get; set; }
        public int EntityID { get; set; }
        public byte ActionTypeID { get; set; }
        public string NotesLog { get; set; }
    }
}
