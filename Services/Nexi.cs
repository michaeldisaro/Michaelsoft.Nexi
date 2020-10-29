using System;
using System.IO;
using System.Security.Cryptography;
using Michaelsoft.Nexi.Extensions;
using Michaelsoft.Nexi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Michaelsoft.Nexi.Services
{
    public class Nexi : INexi
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly AesManaged _aes;

        private string _nexiSecretKey;

        public Nexi(IConfiguration configuration,
                    IHttpContextAccessor httpContextAccessor)
        {
            _nexiSecretKey = configuration["Nexi:SecretKey"];
            _httpContextAccessor = httpContextAccessor;
            _aes = new AesManaged();
            _aes.GenerateIV();
            _aes.GenerateKey();
        }

        public void GoToPayment(decimal amount,
                                string currency,
                                string code,
                                string method = null,
                                string email = null,
                                bool layout = true)
        {
            _httpContextAccessor.HttpContext.Response.Redirect(GetPaymentUrl(amount, currency, code, method, email,
                                                                             layout));
        }

        public string GetPaymentUrl(decimal amount,
                                    string currency,
                                    string code,
                                    string method = null,
                                    string email = null,
                                    bool layout = true)
        {
            var payload = DataToPayload(amount, currency, code, method, email);
            var url = $"/Nexi/RequestPayment?payload={payload}&layout={layout.ToString()}";
            return url;
        }

        public class PaymentData
        {

            public string Amount { get; set; }

            public string Currency { get; set; }

            public string Code { get; set; }

            public string Email { get; set; }

            public string Method { get; set; }

        }

        public string DataToPayload(decimal amount,
                                    string currency,
                                    string code,
                                    string method,
                                    string email)
        {
            var paymentData = new PaymentData
                {Amount = $"{(int) (amount * 100)}", Currency = currency, Code = code, Method = method, Email = email};
            var json = JsonConvert.SerializeObject(paymentData);
            var encrypted = EncryptStringToBytes_Aes(json, _aes.Key, _aes.IV);
            return EncodingHelper.toSafeUrlBase64(encrypted);
        }

        public void PayloadToData(string payload,
                                  out string amount,
                                  out string currency,
                                  out string code,
                                  out string method,
                                  out string email)
        {
            var encrypted = EncodingHelper.fromSafeUrlBase64(payload);
            var json = DecryptStringFromBytes_Aes(encrypted, _aes.Key, _aes.IV);
            var paymentData = JsonConvert.DeserializeObject<PaymentData>(json);
            amount = paymentData.Amount;
            currency = paymentData.Currency;
            code = paymentData.Code;
            method = paymentData.Method;
            email = paymentData.Email;
        }

        public string GenerateMac(string input)
        {
            return EncodingHelper.HashMac(input + _nexiSecretKey);
        }

        private byte[] EncryptStringToBytes_Aes(string plainText,
                                                byte[] key,
                                                byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");
            byte[] encrypted;
            using (var aesAlg = new AesManaged())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        private string DecryptStringFromBytes_Aes(byte[] cipherText,
                                                  byte[] key,
                                                  byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");
            string plaintext = null;
            using (var aesAlg = new AesManaged())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

    }
}