using Michaelsoft.Nexi.Settings;

namespace Michaelsoft.Nexi.Models
{
    public class PaymentData
    {
        public INexiSettings NexiSettings { get; set; }

        public string Amount { get; set; }

        public string Currency { get; set; }

        public string Code { get; set; }

        public string Method { get; set; }

        public string EmailAddress { get; set; }

    }
}