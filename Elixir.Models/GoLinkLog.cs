using System;

namespace Elixir.Models
{
    public class GoLinkLog : BaseEntity
    {
        public DateTime Created { get; set; }
        public int GoLinkId { get; set; }   
        public string IPAddress { get; set; }

        public string GoLinkTitle { get; set; }
        public string GoLinkShortCode { get; set; }
    }
}
