using System;
using Newtonsoft.Json;

namespace Elixir.BusinessLogic.Core.Api.Buffer
{
    class JsonDeserializer
    {
        public static T TryDeserialize<T>(string content)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception e)
            {
                return default(T);
            }
        }
    }
}
