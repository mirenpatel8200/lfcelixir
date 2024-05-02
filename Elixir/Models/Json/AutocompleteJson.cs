using Newtonsoft.Json;

namespace Elixir.Models.Json
{
    public class AutocompleteJson
    {
        public AutocompleteJson(string label, object value)
        {
            this.label = label;
            this.value = value.ToString();
        }

        public AutocompleteJson(string label, string value)
        {
            this.label = label;
            this.value = value;
        }

        public AutocompleteJson(string label)
        {
            this.label = label;
        }

        public string label;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string value;
    }
}