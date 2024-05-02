using System;

namespace Elixir.Models
{
    public class SearchLog : BaseEntity
    {
        public DateTime Created { get; set; }

        public string IPAddress { get; set; }

        public string Search { get; set; }

        public int WordCount { get; set; }
    }
}
