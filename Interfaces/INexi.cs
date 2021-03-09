using Michaelsoft.Nexi.Models;
using Michaelsoft.Nexi.Settings;

namespace Michaelsoft.Nexi.Interfaces
{
    public interface INexi
    {

        void GoToPayment(PaymentData data,
                         bool layout = true,
                         INexiSettings overrideSettings = null);

        string GetPaymentUrl(PaymentData data,
                             bool layout = true,
                             INexiSettings overrideSettings = null);

        void PayloadToData(string payload,
                           out PaymentData data);

        string GenerateMac(string input,
                           string overrideSecretKey = null);

    }
}