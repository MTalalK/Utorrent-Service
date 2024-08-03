using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace WorkerServiceNew
{
    internal class ApiClient : HttpClient
    {
        private readonly HttpClient _webClient;
        private readonly string? _userName;
        private readonly string? _password;
        public string _token = string.Empty;
        private readonly ILogger<Worker> _logger;
        public ApiClient(ILogger<Worker> logger, IConfiguration config) {
            _webClient = new HttpClient();
            _userName = config.GetSection("Credentials").GetValue<string>("Name");
            _password = config.GetSection("Credentials").GetValue<string>("Password");
            _logger = logger;
            AddHeaders();
            
        }

        private async Task RefreshToken()
        {
            HttpResponseMessage tokenHtmlResponse = await _webClient.GetAsync($"token.html?username={_userName}&password={_password}");
            string htmlContent = string.Empty;
            htmlContent = await tokenHtmlResponse.Content.ReadAsStringAsync();

            // Load HTML content into HtmlDocument
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);

            // Find the div element with id 'token'
            HtmlNode tokenNode = htmlDocument.GetElementbyId("token");

            // Check if the tokenNode is found
            if (tokenNode != null)
            {
                // Extract the token from the inner text of the div element
                _token = tokenNode.InnerText.Trim();
                _logger.LogInformation("New token fetched at: {time}", DateTimeOffset.Now);
            }
            else
            {
                Console.WriteLine("Token not found in the HTML content.");
            }
        }

        public async Task<HttpResponseMessage> FetchData(string QueryParams)
        {
            if (string.IsNullOrWhiteSpace(_token))
            {
                await RefreshToken();
            }
            HttpResponseMessage response = new HttpResponseMessage();
            response = await SendRequest(QueryParams);

            // Check if response is 401 Unauthorized
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Refresh the token
                await RefreshToken();

                response = await SendRequest(QueryParams);
            }

            return response;
        }

        public async Task<HttpResponseMessage> DeleteTorrent(string QueryParams)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response = await SendRequest(QueryParams);
            return response;
        }

        private void AddHeaders()
        {
            _webClient.BaseAddress = new Uri("http://localhost:3050/gui/");
            _webClient.DefaultRequestHeaders.Accept.Clear();
            _webClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_userName}:{_password}"));
            // Add basic authentication header to the request
            _webClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
        }

        private async Task<HttpResponseMessage> SendRequest(string queryString)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            // Send the request
            string uri = "?token=" + _token + queryString;
            response = await _webClient.GetAsync(uri);

            return response;
        } 
    }
}
