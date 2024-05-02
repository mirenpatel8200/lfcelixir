namespace Elixir.Utils.View
{
    public class ResourceInfo
    {
        public ResourceInfo(string name, int? id)
        {
            ResourceName = name;
            ResourceId = id;
        }

        public string ResourceName { get; set; }
        public int? ResourceId { get; set; }
    }
}
