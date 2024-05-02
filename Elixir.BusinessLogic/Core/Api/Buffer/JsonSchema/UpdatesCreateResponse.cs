using System.Collections.Generic;

namespace Elixir.BusinessLogic.Core.Api.Buffer.JsonSchema
{
    internal class UpdatesCreateResponse
    {
        public bool success { get; set; }
        public int buffer_count { get; set; }
        public float buffer_percentage { get; set; } // 26-May-2020: ALC Buffer API now returning as range 0.00 to 1.00 - e.g. 25% = 0.25 - deserialization failing
        public List<Update> updates { get; set; }

        internal class Media
        {
            public string link { get; set; }
            public string title { get; set; }
            public string description { get; set; }
        }

        internal class Update
        {
            public string id { get; set; }
            public int created_at { get; set; }
            public string day { get; set; }
            public int due_at { get; set; }
            public string due_time { get; set; }
            public Media media { get; set; }
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
