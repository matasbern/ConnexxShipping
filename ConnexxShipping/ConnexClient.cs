using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System;

namespace ConnexxShipping
{
    public class ConnexxClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Creates a new ConnexxClient instance, using the specified bearer token and optional settings
        /// </summary>
        /// <param name="bearer">The bearer token provided by ITD (Connexx).</param>
        /// <param name="options">Optional settings, including the base URL.</param>
        /// <param name="httpMessageHandler">Allows injecting a custom HttpMessageHandler for testing. If null, a default HttpClientHandler is used.
        /// </param>
        public ConnexxClient(string bearer, ConnexxOptions options = null, HttpMessageHandler httpMessageHandler = null)
        {
            if (string.IsNullOrEmpty(bearer))
                throw new ArgumentNullException(nameof(bearer));

            options = options ?? new ConnexxOptions();

            _httpClient = httpMessageHandler == null
                ? new HttpClient()
                : new HttpClient(httpMessageHandler, false);

            _httpClient.BaseAddress = new Uri(options.BaseUrl);
            _httpClient.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Bearer", bearer);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        /// <summary>
        /// Gets a list of all available carrier services.
        /// </summary>
        /// <returns>A list of <see cref="Service"/> objects.</returns>
        public async Task<List<Service>> GetAvailableServicesAsync()
        {
            const string endpoint = "carriers-services";
            using (var response = await _httpClient.GetAsync(endpoint).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var wrapper = JsonSerializer.Deserialize<BaseResponse<List<Service>>>(json, _jsonOptions);
                return wrapper?.Data ?? new List<Service>();
            }
        }

        /// <summary>
        /// Creates shipping labels.
        /// </summary>
        /// <param name="request">Information needed to create labels.</param>
        /// <returns>A <see cref="CreateLabelResponse"/>.</returns>
        public async Task<CreateLabelResponse> CreateLabelsAsync(CreateLabelRequest request)
        {
            const string endpoint = "shipments";

            var jsonBody = JsonSerializer.Serialize(request, _jsonOptions);
            using (var content = new StringContent(jsonBody, Encoding.UTF8, "application/json"))
            using (var response = await _httpClient.PostAsync(endpoint, content).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var result = JsonSerializer.Deserialize<BaseResponse<CreateLabelResponse>>(responseContent, _jsonOptions);

                return result?.Data ?? new CreateLabelResponse();
            }
        }

        /// <summary>
        /// Gets tracking updates for specified barcodes.
        /// </summary>
        /// <param name="barcodes">A list of one or more tracking barcodes.</param>
        /// <returns>A <see cref="TrackingResponse"/> containing tracking data.</returns>
        public async Task<TrackingResponse> GetTrackingAsync(List<string> barcodes)
        {
            const string endpoint = "tracking";
            var bodyObj = new TrackingRequest
            {
                TrackingBarCodes = barcodes
            };

            var jsonBody = JsonSerializer.Serialize(bodyObj, _jsonOptions);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using (var response = await _httpClient.PostAsync(endpoint, content).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var result = JsonSerializer.Deserialize<BaseResponse<TrackingData>>(responseContent, _jsonOptions);

                return new TrackingResponse
                {
                    ValidBarcodes = result?.Data?.ValidBarcodes ?? new List<TrackingBarcodeData>(),
                    InvalidBarcodes = result?.Data?.InvalidBarcodes ?? new List<string>()
                };
            }
        }

        /// <summary>
        /// Cancels label(s) given their barcodes.
        /// </summary>
        /// <param name="barcodes">A list of barcodes (tracking codes) to cancel.</param>
        /// <returns>A <see cref="CancelLabelResponse"/> containing cancellation status.</returns>
        public async Task<List<CancelLabelResponse>> CancelLabelAsync(List<string> barcodes)
        {
            const string endpoint = "shipments/cancel";

            var jsonBody = JsonSerializer.Serialize(barcodes, _jsonOptions);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using (var response = await _httpClient.PostAsync(endpoint, content).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var result = JsonSerializer.Deserialize<BaseResponse<List<CancelLabelResponse>>>(responseContent, _jsonOptions);

                return result?.Data ?? new List<CancelLabelResponse>();
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    public class BaseResponse<T>
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public T Data { get; set; }
    }
}
