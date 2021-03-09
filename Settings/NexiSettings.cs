namespace Michaelsoft.Nexi.Settings
{
    public class NexiSettings : INexiSettings
    {

        public string Alias { get; set; }

        public string SecretKey { get; set; }

        public string Url { get; set; }

        public string Success { get; set; }

        public string Cancel { get; set; }

    }

    public interface INexiSettings
    {

        public string Alias { get; set; }

        public string SecretKey { get; set; }

        public string Url { get; set; }

        public string Success { get; set; }

        public string Cancel { get; set; }

    }
}