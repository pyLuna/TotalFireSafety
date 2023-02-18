using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace TotalFireSafety.Models
{
    public class APIRequestHandler
    {
        private readonly HttpClient _client = new HttpClient();


        /*          localhost for debugging
         *              https://localhost:44309
         *          Change BaseDomain() into tfsti when publishing this project
         *              http://tfsti.somee.com
         *          if returns error change the port to the corresponding port
         */
        //public string BaseDomain() = "https://localhost:44398";
        //public string BaseDomain() = "http://tfsti.somee.com";
        public string BaseDomain()
        {
            string domain;
            domain = HttpContext.Current.Request.Url.Host;

            if (domain == "localhost")
            {
                domain = "https://localhost:44398";
            }
                return domain;
        }

        public string GetAllMethod(string uri, string Token)
        {
            //  Set Base Domain
            if (_client.BaseAddress == null) // to prevent error, check if the base address if empty
            {
                _client.BaseAddress = new Uri(BaseDomain());
            }
            //  Request Headers
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var postTask = _client.GetAsync(uri);   //  Request MEthod

            postTask.Wait();    // wait for result

            var result = postTask.Result; // Get Result from request
            if (result.IsSuccessStatusCode)
            {
                //    Get Result Content
                return result.Content.ReadAsStringAsync().Result;
            }
            return result.StatusCode.ToString();
        }

        public string SetMethod(string uri, string Token, string json)
        {
            if (_client.BaseAddress == null) // to prevent error, check if the base address if empty
            {
                _client.BaseAddress = new Uri(BaseDomain());
            }
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            var _json = JsonConvert.DeserializeObject(json);

            var postTask = _client.PostAsJsonAsync(uri, _json); // Request Method
            postTask.Wait(); // wait for result

            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return result.Content.ReadAsStringAsync().Result;
            }
            return result.StatusCode.ToString();
        }

        public string BarcodeGenerator(string token, string itemCode)
        {
            if(_client.BaseAddress == null)
            {
                _client.BaseAddress = new Uri(BaseDomain());
            }
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //var client = new HttpClient();
            //var content = new StringContent(itemCode);
            var response = _client.PostAsync("/Warehouse/Inventory/Barcode?value="+itemCode,null);
            
            return response.Result.Content.ReadAsStringAsync().Result;
        }
    }
}