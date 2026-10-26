using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Revmax_Interface_Promun.Services
{
    public class MRALibTest
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        private string _bearerToken;
        private string _terminalId;
        private string _secretKey;
        private MRAConfiguration _configuration;
        private readonly string _configPath;

        public MRALibTest()
        {
            _baseUrl = ConfigurationManager.AppSettings.Get("MRABaseUrl") ?? "http://localhost:5236";
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            _httpClient = new HttpClient();
            LoadStoredCredentials();
        }

        public class ApiResponse<T>
        {
            [JsonProperty("statusCode")]
            public int StatusCode { get; set; }

            [JsonProperty("remark")]
            public string Remark { get; set; }

            [JsonProperty("data")]
            public T Data { get; set; }

            [JsonProperty("errors")]
            public List<string> Errors { get; set; }

            public bool IsSuccess => StatusCode == 1;
        }

        public async Task<ApiResponse<InitializeResponse>> InitializeAsync()
        {
            try
            {
                AddAuthorizationHeader();
                var url = $"{_baseUrl}/api/utility/initialize";
                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ApiResponse<InitializeResponse>>(responseContent);
            }
            catch (Exception ex)
            {
                return new ApiResponse<InitializeResponse>
                {
                    StatusCode = 0,
                    Remark = $"Error initializing terminal: {ex.Message}",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<MRAConfiguration>> GetLatestConfigsAsync()
        {
            if (_configuration != null)
            {
                return new ApiResponse<MRAConfiguration>
                {
                    StatusCode = 1,
                    Remark = "Configuration loaded from cache",
                    Data = _configuration
                };
            }

            var initResponse = await InitializeAsync();
            if (initResponse.IsSuccess)
            {
                _configuration = initResponse.Data?.Configuration;
                return new ApiResponse<MRAConfiguration>
                {
                    StatusCode = initResponse.StatusCode,
                    Remark = initResponse.Remark,
                    Data = _configuration,
                    Errors = initResponse.Errors
                };
            }

            return new ApiResponse<MRAConfiguration>
            {
                StatusCode = 0,
                Remark = "Failed to load configuration",
                Data = null
            };
        }

        private void AddAuthorizationHeader()
        {
            if (!string.IsNullOrEmpty(_bearerToken))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_bearerToken}");
            }
        }

        private void LoadStoredCredentials()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    var content = File.ReadAllText(_configPath);
                    var credentials = JsonConvert.DeserializeAnonymousType(content, new
                    {
                        TerminalId = "",
                        BearerToken = "",
                        SecretKey = "",
                        Configuration = (MRAConfiguration)null,
                        SavedAt = DateTime.MinValue
                    });

                    _terminalId = credentials.TerminalId;
                    _bearerToken = credentials.BearerToken;
                    _secretKey = credentials.SecretKey;
                    _configuration = credentials.Configuration;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading credentials: {ex.Message}");
            }
        }
    }
}
