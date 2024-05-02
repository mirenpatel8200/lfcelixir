using System.Collections.Generic;

namespace Elixir.BusinessLogic.Core.Api.Buffer.JsonSchema
{
    internal class Profiles : List<Profiles.Profile>
    {
        internal class Profile
        {
            public string avatar { get; set; }
            public int created_at { get; set; }
            public bool @default { get; set; }
            public string formatted_username { get; set; }
            public string id { get; set; }
            public List<Schedule> schedules { get; set; }
            public string service { get; set; }
            public string service_id { get; set; }
            public string service_username { get; set; }
            //public Statistics statistics { get; set; }
            //public List<string> team_members { get; set; }
            public string timezone { get; set; }
            public string user_id { get; set; }
        }

        internal class Schedule
        {
            public List<string> days { get; set; }
            public List<string> times { get; set; }
        }

        internal class Statistics
        {
            public int followers { get; set; }
        }
    }
}
