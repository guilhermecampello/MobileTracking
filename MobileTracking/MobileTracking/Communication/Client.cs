using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MobileTracking.Communication
{
    public class Client
    {
        private HttpClient _httpClient;

        private readonly Configuration configuration;

        public Client(Configuration configuration)
        {
            this.configuration = configuration;

            this._httpClient = new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                }
            });

            this._httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        private string apiAddress { get => $"{Hostname}/api"; }

        public string Hostname { get => this.configuration.Hostname; }

        public bool IsHealthy { get; set; }
        
        public async Task<T> Get<T>(string controller, object? query)
        {
            var request = await _httpClient.GetAsync($"{apiAddress}/{controller}/{ConvertQuery(query)}");
            return await GetResponse<T>(request);
        }

        public async Task<T> Get<T>(string controller, string path, object? query)
        {
            var request = await _httpClient.GetAsync($"{apiAddress}/{controller}/{path}{ConvertQuery(query)}");
            return await GetResponse<T>(request);
        }

        public async Task<T> Put<T>(string controller, string path, object? body, object? query = null)
        {
            var request =  await _httpClient.PutAsync($"{apiAddress}/{controller}/{path}{ConvertQuery(query)}",
                    new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8)
               );
            return await GetResponse<T>(request);
        }

        public async Task<T> Post<T>(string controller, object body, string path = "")
        {
            var request = await _httpClient.PostAsync($"{apiAddress}/{controller}/{path}",
                    new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
                );
            return await GetResponse<T>(request);
        }

        public async Task<T> Delete<T>(string controller, string path, object? query = null)
        {
            var request = await _httpClient.DeleteAsync($"{apiAddress}/{controller}/{path}{ConvertQuery(query)}");
            return await GetResponse<T>(request);
        }
        
        public async Task<bool> CheckHealth()
        {
            var request = await _httpClient.GetAsync($"{apiAddress}/health");
            if (request.IsSuccessStatusCode)
            {
                var response = await request.Content.ReadAsStringAsync();
                if (response == "Healthy")
                {
                    this.IsHealthy = true;
                    return true;
                }
            }
            this.IsHealthy = false;
            return false;
        }

        public string ConvertQuery(object? query)
        {
            if (query == null)
            {
                return "";
            }

            var queryString = "?";
            foreach (var property in query.GetType().GetProperties())
            {
                if (property.GetValue(query) != null)
                {
                    if (queryString.Length > 1)
                    {
                        queryString += "&";
                    }

                    var propertyName = property.Name[0].ToString().ToLower() + property.Name.Substring(1);
                    var type = property.GetType();
                    if (property.GetValue(query).GetType().IsArray)
                    {
                        foreach (var value in (Array)property.GetValue(query))
                        {
                            queryString += $"{propertyName}={value}&";
                        }
                        queryString = queryString.Substring(0, queryString.Length - 1);
                    }
                    else
                    {
                        queryString += $"{propertyName}={property.GetValue(query)}";
                    }
                }
            }

            return queryString;
        }

        private async Task<T> GetResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseContent)!;
            }
            else
            {
                var details = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != System.Net.HttpStatusCode.InternalServerError)
                {
                    var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(details);
                    if (problemDetails != null)
                    {
                        throw new Exception(problemDetails.Title, new Exception(problemDetails.Detail));
                    }
                }
                throw new Exception(details);
            }
        }
    }
}
