using System;
using System.Collections.Generic;
using Michaelsoft.Nexi.Extensions;
using Michaelsoft.Nexi.Interfaces;
using Michaelsoft.Nexi.Settings;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace Michaelsoft.Nexi.Areas.Nexi.Pages
{
    public class RequestPaymentModel : PageModel
    {

        private readonly INexi _nexi;

        public RequestPaymentModel(INexi nexi)
        {
            _nexi = nexi;
        }

        public bool NoLayout { get; set; }

        public void OnGet(string payload,
                          bool layout = true)
        {
            NoLayout = !layout;

            _nexi.PayloadToData(payload, out var paymentData);
            var rand = new Random();
            var codTrans = paymentData.Code + ":" + rand.Next(1111, 9999);
            var mac = _nexi.GenerateMac($"codTrans={codTrans}divisa={paymentData.Currency}importo={paymentData.Amount}",
                                  paymentData.NexiSettings.SecretKey);
            var requestParams = new Dictionary<string, string>
            {
                {"alias", paymentData.NexiSettings.Alias},
                {"importo", paymentData.Amount},
                {"divisa", paymentData.Currency},
                {"codTrans", codTrans},
                {"url", paymentData.NexiSettings.Success},
                {"url_back", paymentData.NexiSettings.Cancel},
                {"mac", mac},
            };
            if (paymentData.EmailAddress != null)
                requestParams["mail"] = paymentData.EmailAddress;
            if (paymentData.Method != null)
                requestParams["selectedcard"] = paymentData.Method;

            ViewData["action"] = QueryHelpers.AddQueryString(paymentData.NexiSettings.Url, requestParams);
        }

    }
}