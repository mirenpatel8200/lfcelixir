using System.Collections.Generic;

namespace Elixir.BusinessLogic.Core.Api.Buffer.JsonSchema
{
    internal class Updates
    {
        public int total { get; set; }
        public List<Update> updates { get; set; }

        internal class Update
        {
            public string id { get; set; }
            public int created_at { get; set; }
            public string day { get; set; }
            public int due_at { get; set; }
            public string due_time { get; set; }
            public string profile_id { get; set; }
            public string profile_service { get; set; }
            public string status { get; set; }
            public string text { get; set; }
            public string text_formatted { get; set; }
            public string user_id { get; set; }
            public string via { get; set; }
        }
    }
}
