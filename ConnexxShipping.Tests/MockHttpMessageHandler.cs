using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System;

namespace ConnexxShipping.Tests
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handlerFunc;

        public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handlerFunc)
        {
            _handlerFunc = handlerFunc;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = _handlerFunc.Invoke(request);
            return Task.FromResult(response);
        }
    }
}
