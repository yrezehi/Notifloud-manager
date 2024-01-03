namespace Core.Models
{
    public class Subscription
    {
        public string Endpoint { get; set; }
        public IDictionary<string, string> Keys { get; set; }

        public Subscription(string endpoint, IDictionary<string, string> keys) =>
            (Endpoint, Keys) = (endpoint, keys);

        public static Subscription Create(string endpoint, IDictionary<string, string> keys) =>
            new Subscription(endpoint, keys);
    }
}
