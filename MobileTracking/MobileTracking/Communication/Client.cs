using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MobileTracking.Communication
{
    class Client
    {
        private HttpClient _httpClient;

        public string BaseAddress { get; set; } = "https://localhost:5001/api/";

        public Client()
        {
            this._httpClient = new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                }
            });
        }
        
        public async Task<T> Get<T>(string controller, object? query)
        {
            var request = await _httpClient.GetAsync($"{BaseAddress}/{controller}/{ConvertQuery(query)}");
            return await GetResponse<T>(request);
        }

        public async Task<T> Put<T>(string controller, string path, object body)
        {
            var request =  await _httpClient.PutAsync($"{BaseAddress}/{controller}/{path}",
                    new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8)
               );
            return await GetResponse<T>(request);
        }

        public async Task<T> Post<T>(string controller, object body, string path = "")
        {
            var request = await _httpClient.PostAsync($"{BaseAddress}/{controller}/{path}",
                    new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8)
                );
            return await GetResponse<T>(request);
        }

        public async Task<T> Delete<T>(string controller, string path)
        {
            var request = await _httpClient.DeleteAsync($"{BaseAddress}/{controller}/{path}");
            return await GetResponse<T>(request);
        }

        private async Task<T> GetResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync())!;
            }
            else
            {
                var details = await response.Content.ReadAsStringAsync();
                throw new Exception(details);
            }
        }

        private string ConvertQuery(object? query)
        {
            if( query == null)
            {
                return "";
            }

            var queryString = "?";
            foreach (var property in query.GetType().GetProperties())
            {
                queryString += $"{property.Name}={property.GetValue(query)}";
            }

            return queryString;
        }
    }
}
