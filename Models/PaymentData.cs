using Michaelsoft.Nexi.Settings;

namespace Michaelsoft.Nexi.Models
{
    public class PaymentData
    {

        public NexiSettings NexiSettings { get; set; }

        /// <summary>
        /// Use StringifyAmount to get the correct value to set to this property
        /// </summary>
        public string Amount { get; set; }

        public string Currency { get; set; }

        public string Code { get; set; }

        public string Method { get; set; }

        public string EmailAddress { get; set; }

        public static string StringifyAmount(decimal amount)
        {
            return $"{(int) (amount * 100)}";
        }

    }
}