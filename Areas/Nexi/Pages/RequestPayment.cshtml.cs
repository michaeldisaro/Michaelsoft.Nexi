using System;
using System.Collections.Generic;
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

        private readonly INexiSettings _nexiSettings;

        public RequestPaymentModel(INexiSettings nexiSettings,
                                   INexi nexi)
        {
            _nexiSettings = nexiSettings;
            _nexi = nexi;
        }

        public bool NoLayout { get; set; }

        public void OnGet(string payload,
                          bool layout = true)
        {
            NoLayout = !layout;

            _nexi.PayloadToData(payload, out var amount, out var currency, out var code, out var method, out var email);
            var rand = new Random();
            var codTrans = code + ":" + rand.Next(1111, 9999);
            var mac = _nexi.GenerateMac($"codTrans={codTrans}divisa={currency}importo={amount}");
            var requestParams = new Dictionary<string, string>
            {
                {"alias", _nexiSettings.Alias},
                {"importo", amount},
                {"divisa", currency},
                {"codTrans", codTrans},
                {"url", _nexiSettings.Success},
                {"url_back", _nexiSettings.Cancel},
                {"mac", mac},
            };
            if (email != null)
                requestParams["mail"] = email;
            if (method != null)
                requestParams["selectedcard"] = method;

            ViewData["action"] = QueryHelpers.AddQueryString(_nexiSettings.Url, requestParams);
            ;
        }

    }
}