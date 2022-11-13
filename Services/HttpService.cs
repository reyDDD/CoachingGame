﻿using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;
using System.Text;
using StringConverter = Tamboliya.Helpers.StringConverter;
using TamboliyaLibrary.Models;

namespace Tamboliya.Services
{
	public interface IHttpService
	{
		Task<T> Get<T>(string uri);
		Task Post(string uri, object value);
		Task<T> Post<T>(string uri, object value);
		Task Put(string uri, object value);
		Task<T> Put<T>(string uri, object value);
		Task Delete(string uri);
		Task<T> Delete<T>(string uri);
	}

	public class HttpService : IHttpService
	{
		private HttpClient _httpClient;
		private NavigationManager _navigationManager;
		private ILocalStorageService _localStorageService;
		private IConfiguration _configuration;

		public HttpService(
			HttpClient httpClient,
			NavigationManager navigationManager,
			ILocalStorageService localStorageService,
			IConfiguration configuration
		)
		{
			_httpClient = httpClient;
			_navigationManager = navigationManager;
			_localStorageService = localStorageService;
			_configuration = configuration;
		}

		public async Task<T> Get<T>(string uri)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, uri);
			return await SendRequest<T>(request);
		}

		public async Task Post(string uri, object value)
		{
			var request = CreateRequest(HttpMethod.Post, uri, value);
			await sendRequest(request);
		}

		public async Task<T> Post<T>(string uri, object value)
		{
			var request = CreateRequest(HttpMethod.Post, uri, value);
			return await SendRequest<T>(request);
		}

		public async Task Put(string uri, object value)
		{
			var request = CreateRequest(HttpMethod.Put, uri, value);
			await sendRequest(request);
		}

		public async Task<T> Put<T>(string uri, object value)
		{
			var request = CreateRequest(HttpMethod.Put, uri, value);
			return await SendRequest<T>(request);
		}

		public async Task Delete(string uri)
		{
			var request = CreateRequest(HttpMethod.Delete, uri);
			await sendRequest(request);
		}

		public async Task<T> Delete<T>(string uri)
		{
			var request = CreateRequest(HttpMethod.Delete, uri);
			return await SendRequest<T>(request);
		}

		// helper methods

		private HttpRequestMessage CreateRequest(HttpMethod method, string uri, object? value = null)
		{
			var request = new HttpRequestMessage(method, uri);
			if (value != null)
				request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
			return request;
		}

		private async Task sendRequest(HttpRequestMessage request)
		{
			await addJwtHeader(request);

			// send request
			using var response = await _httpClient.SendAsync(request);

			// auto logout on 401 response
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				_navigationManager.NavigateTo("account/logout");
				return;
			}

			await handleErrors(response);
		}

		private async Task<T> SendRequest<T>(HttpRequestMessage request)
		{
			await addJwtHeader(request);

			// send request
			using var response = await _httpClient.SendAsync(request);

			// auto logout on 401 response
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				_navigationManager.NavigateTo("api/account/logout");
				return default;
			}

			await handleErrors(response);

			var options = new JsonSerializerOptions();
			options.PropertyNameCaseInsensitive = true;
			options.Converters.Add(new StringConverter());
			return await response.Content.ReadFromJsonAsync<T>(options);
		}

		private async Task addJwtHeader(HttpRequestMessage request)
		{
			// add jwt auth header if user is logged in and request is to the api url
			var user = await _localStorageService.GetItem<User>("user");
			var isApiUrl = !request.RequestUri.IsAbsoluteUri;
			if (user != null && isApiUrl)
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
		}

		private async Task handleErrors(HttpResponseMessage response)
		{
			// throw exception on error response
			if (!response.IsSuccessStatusCode)
			{
				var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
				throw new Exception(error["message"]);
			}
		}
	}
}
