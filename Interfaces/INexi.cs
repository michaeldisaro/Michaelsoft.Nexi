namespace Michaelsoft.Nexi.Interfaces
{
    public interface INexi
    {

        void GoToPayment(decimal amount,
                         string currency,
                         string code,
                         string method = null,
                         string email = null,
                         bool layout = true);

        string GetPaymentUrl(decimal amount,
                             string currency,
                             string code,
                             string method = null,
                             string email = null,
                             bool layout = true);

        string DataToPayload(decimal amount,
                             string currency,
                             string code,
                             string method,
                             string email);

        void PayloadToData(string payload,
                           out string amount,
                           out string currency,
                           out string code,
                           out string method,
                           out string email);

        string GenerateMac(string input);

    }
}