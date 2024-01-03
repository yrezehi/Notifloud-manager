namespace Core.Models
{
    public class Subscription
    {
        public string Endpoint { get; set; }
        public IDictionary<string, string> Keys { get; set; }
    }
}
