using Shared;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure
{
    public class HttpClientProxy : IDisposable
    {
        protected HttpClient _httpClient;
        protected bool _isDisposed = false;

        public HttpClientProxy(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public HttpClientProxy(IHttpClientFactory httpClientFactory, string? httpClientName = null)
        {
            _httpClient = CreateClient(httpClientFactory, httpClientName);
        }

        public HttpRequestHeaders DefaultRequestHeaders => _httpClient.DefaultRequestHeaders;

        protected virtual async Task SetHttpClientAuthorizationAsync(CancellationToken cancellationToken = default) { }
        protected virtual async Task LogRequest(HttpResponseMessage response, long requestDuration) { }

        public virtual async Task<TResponse?> GetAsync<TResponse>(string? relativePath = null)
        {
            HttpResponseMessage response = await GetAsync(relativePath);
            return await Deserialize<TResponse>(response);
        }

        public virtual async Task<HttpResponseMessage> GetAsync(string? relativePath = null)
        {
            string absolutePath = string.IsNullOrWhiteSpace(relativePath)
                ? $"{_httpClient.BaseAddress}"
                : $"{_httpClient.BaseAddress}{relativePath}";

            await SetHttpClientRequestHeadersAsync();

            return await _httpClient.GetAsync(absolutePath);
        }

        public virtual async Task<TResponse?> PostAsync<TResponse>(FormUrlEncodedContent urlEncodedContent, string? relativePath = null, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = await PostAsync(urlEncodedContent, relativePath);
            return await Deserialize<TResponse>(response);
        }

        public virtual async Task<HttpResponseMessage> PostAsync(FormUrlEncodedContent urlEncodedContent, string? relativePath = null, CancellationToken cancellationToken = default)
        {
            string absolutePath = string.IsNullOrWhiteSpace(relativePath)
                ? $"{_httpClient.BaseAddress}"
                : $"{_httpClient.BaseAddress}{relativePath}";

            var stopwatch = Stopwatch.StartNew();
            HttpResponseMessage response = await _httpClient.PostAsync(absolutePath, urlEncodedContent, cancellationToken);
            stopwatch.Stop();

            LogRequest(response, stopwatch.ElapsedMilliseconds);

            return response;
        }

        public virtual async Task<TResponse?> PostAsync<TResponse>(string? relativePath = null, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = await PostAsync(relativePath);
            return await Deserialize<TResponse>(response);
        }

        public virtual async Task<TResponse?> PostAsync<TRequest, TResponse>(TRequest requestModel, string? relativePath = null, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = await PostAsync(requestModel, relativePath, cancellationToken);
            return await Deserialize<TResponse>(response);
        }

        public virtual async Task<HttpResponseMessage> PostAsync<TRequest>(TRequest requestModel, string? relativePath = null, CancellationToken cancellationToken = default)
        {
            HttpContent? requestContent = null;
            if (requestModel is not null)
            {
                requestModel.Validate();
                requestContent = JsonContent.Create(requestModel);
            }

            return await PostAsync(relativePath, requestContent, cancellationToken);
        }

        public virtual async Task<HttpResponseMessage> PostAsync(string? relativePath = null, HttpContent? requestContent = null, CancellationToken cancellationToken = default)
        {
            var stopwatch = new Stopwatch();
            string absolutePath = string.IsNullOrWhiteSpace(relativePath)
                ? $"{_httpClient.BaseAddress}"
                : $"{_httpClient.BaseAddress}{relativePath}";

            await SetHttpClientRequestHeadersAsync();

            stopwatch.Start();
            HttpResponseMessage response = await _httpClient.PostAsync(absolutePath, requestContent, cancellationToken);
            stopwatch.Stop();

            LogRequest(response, stopwatch.ElapsedMilliseconds);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await SetHttpClientAuthorizationAsync();

                stopwatch.Restart();
                response = await _httpClient.PostAsync(absolutePath, requestContent);
                stopwatch.Stop();

                LogRequest(response, stopwatch.ElapsedMilliseconds);
            }

            return response;
        }

        protected async Task SetHttpClientRequestHeadersAsync()
        {
            if (_httpClient.DefaultRequestHeaders.Authorization is null)
                await SetHttpClientAuthorizationAsync();
        }

        private static HttpClient CreateClient(IHttpClientFactory httpClientFactory, string? httpClientName)
        {
            HttpClient client;

            if (string.IsNullOrWhiteSpace(httpClientName))
            {
                client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            }
            else
            {
                client = httpClientFactory.CreateClient(httpClientName);
            }

            return client;
        }

        private async Task<TResponseModel?> Deserialize<TResponseModel>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                return default;

            return await response.Content.ReadFromJsonAsync<TResponseModel>(new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper) }
            }) ?? throw new ArgumentException("Failed to read the response content.");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                _isDisposed = true;
                _httpClient.Dispose();
            }
        }
    }
}