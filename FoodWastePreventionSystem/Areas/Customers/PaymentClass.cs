using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

namespace FoodWastePreventionSystem.Areas.Customers
{
    public class TransactionResult
    {
        public bool status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        [JsonProperty("authorization_url")]
        public string authorization_url { get; set; }

        [JsonProperty("access_code")]
        public string access_code { get; set; }

        [JsonProperty("reference")]
        public string reference { get; set; }
    }

    public class Payment
    {
        private readonly string _payStackSecretKey;

        public Payment(string secretKey)
        {
            _payStackSecretKey = secretKey;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public async Task<TransactionResult> InitializeTransaction(int amountInKobo, string email,
            string metadata = "", string resource = "https://api.paystack.co/transaction/initialize")
        {
            var client = new HttpClient { BaseAddress = new Uri(resource) };
            client.DefaultRequestHeaders.Add("Authorization", _payStackSecretKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var body = new Dictionary<string, string>()
            {
                {"amount", amountInKobo.ToString() },
                {"email",email },
                {"metadata", metadata }
            };
            var content = new FormUrlEncodedContent(body);
            var response = await client.PostAsync(resource, content);
            if (response.IsSuccessStatusCode)
            {
                TransactionResult responseData = await response.Content.ReadAsAsync<TransactionResult>();
                return responseData;
            }
            return null;
        }

        public async Task<string> VerifyTransaction(string reference, string resource = "https://api.paystack.co/transaction/verify")
        {
            var client = new HttpClient { BaseAddress = new Uri(resource) };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", _payStackSecretKey);
            var response = await client.GetAsync(reference + "/");
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                return responseData;
            }
            return null;
        }
    }
}