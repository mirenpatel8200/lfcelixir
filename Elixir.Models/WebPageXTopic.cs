namespace Elixir.Models
{
    public class WebPageXTopic
    {
        public virtual int? WebPageTopicId { get; set; }

        public virtual int TopicId { get; set; }

        public virtual int WebPageId { get; set; }
    }
}
