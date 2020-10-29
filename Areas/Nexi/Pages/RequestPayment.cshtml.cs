using System;
using System.Collections.Generic;
using Michaelsoft.Nexi.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace Michaelsoft.Nexi.Areas.Nexi.Pages
{
    public class RequestPaymentModel : PageModel
    {

        private readonly INexi _nexi;

        private readonly string _nexiAlias;

        private readonly string _nexiSecretKey;

        private readonly string _nexiUrl;

        private readonly string _successPostbackUrl;

        private readonly string _cancelPostbackUrl;

        public RequestPaymentModel(IConfiguration configuration,
                                   INexi nexi)
        {
            _nexiAlias = configuration["Nexi:Alias"];
            _nexiSecretKey = configuration["Nexi:SecretKey"];
            _nexiUrl = configuration["Nexi:Url"];
            _successPostbackUrl = configuration["Nexi:Success"];
            _cancelPostbackUrl = configuration["Nexi:Cancel"];
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
                {"alias", _nexiAlias},
                {"importo", amount},
                {"divisa", currency},
                {"codTrans", codTrans},
                {"url", _successPostbackUrl},
                {"url_back", _cancelPostbackUrl},
                {"mac", mac},
            };
            if (email != null)
                requestParams["mail"] = email;
            if (method != null)
                requestParams["selectedcard"] = method;
            
            ViewData["action"] = QueryHelpers.AddQueryString(_nexiUrl, requestParams);;
        }

    }
}