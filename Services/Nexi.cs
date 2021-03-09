using System;
using System.IO;
using System.Security.Cryptography;
using Michaelsoft.Nexi.Extensions;
using Michaelsoft.Nexi.Interfaces;
using Michaelsoft.Nexi.Models;
using Michaelsoft.Nexi.Settings;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Michaelsoft.Nexi.Services
{
    public class Nexi : INexi
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly AesManaged _aes;

        private readonly INexiSettings _nexiSettings;

        public Nexi(INexiSettings nexiSettings,
                    IHttpContextAccessor httpContextAccessor)
        {
            _nexiSettings = nexiSettings;
            _httpContextAccessor = httpContextAccessor;
            _aes = new AesManaged();
            _aes.GenerateIV();
            _aes.GenerateKey();
        }

        public void GoToPayment(PaymentData data,
                                bool layout = true,
                                INexiSettings overrideSettings = null)
        {
            _httpContextAccessor.HttpContext.Response.Redirect(GetPaymentUrl(data, layout, overrideSettings));
        }

        public string GetPaymentUrl(PaymentData data,
                                    bool layout = true,
                                    INexiSettings overrideSettings = null)
        {
            data.NexiSettings = overrideSettings ?? _nexiSettings;
            var payload = DataToPayload(data);
            var url = $"/Nexi/RequestPayment?payload={payload}&layout={layout.ToString()}";
            return url;
        }

        public void PayloadToData(string payload,
                                  out PaymentData data)
        {
            var encrypted = EncodingHelper.fromSafeUrlBase64(payload);
            var json = DecryptStringFromBytes_Aes(encrypted, _aes.Key, _aes.IV);
            data = JsonConvert.DeserializeObject<PaymentData>(json);
        }

        public string GenerateMac(string input,
                                  string overrideSecretKey = null)
        {
            return EncodingHelper.HashMac(input + (overrideSecretKey ?? _nexiSettings.SecretKey));
        }

        private string DataToPayload(PaymentData data)
        {
            var json = JsonConvert.SerializeObject(data);
            var encrypted = EncryptStringToBytes_Aes(json, _aes.Key, _aes.IV);
            return EncodingHelper.toSafeUrlBase64(encrypted);
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