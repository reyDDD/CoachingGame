using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Runtime.Serialization;
using Tamboliya.Pages;
using Toolbelt.Blazor;

namespace Tamboliya.Services
{
    public class HttpInterceptorService
    {
        private readonly HttpClientInterceptor _interceptor;
        private readonly NavigationManager _navManager;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly ILogger<HttpInterceptorService> _logger;

        public HttpInterceptorService(HttpClientInterceptor interceptor, NavigationManager navManager,
            AuthenticationStateProvider authenticationStateProvider, ILoggerFactory loggerFactory)
        {
            _interceptor = interceptor;
            _navManager = navManager;
            this.authenticationStateProvider = authenticationStateProvider;
            _logger = loggerFactory.CreateLogger<HttpInterceptorService>();
        }

        public void RegisterEvent() => _interceptor.AfterSend += InterceptResponse!;

        private void InterceptResponse(object sender, HttpClientInterceptorEventArgs e)
        {
            string message = string.Empty;
            if (!e.Response.IsSuccessStatusCode)
            {
                var statusCode = e.Response.StatusCode;

                switch (statusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
                        _navManager.NavigateTo("/Login");
                        message = "User is not authorized";
                        _logger.LogWarning("User is not authorized: " + e.Response.ReasonPhrase + " to URI " + e.Request.RequestUri);
                        break;
                }

                throw new HttpResponseException(message);
            }
        }

        public void DisposeEvent() => _interceptor.AfterSend -= InterceptResponse!;
    }

    [Serializable]
    internal class HttpResponseException : Exception
    {
        public HttpResponseException()
        {
        }
        public HttpResponseException(string message)
            : base(message)
        {
        }
        public HttpResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        protected HttpResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
