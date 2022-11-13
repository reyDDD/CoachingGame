using Microsoft.AspNetCore.Components;
using Tamboliya.Pages.Account;
using TamboliyaLibrary.DAL;
using TamboliyaLibrary.Models;

namespace Tamboliya.Services
{
	public interface IAccountService
	{
		User? User { get; }
		Task Initialize();
		Task Login(RegisterModel model);
		Task Logout();
		Task Register(RegisterModel model);
		//Task<IList<User>> GetAll();
		//Task<User> GetById(string id);
		//Task Update(string id, EditUser model);
		//Task Delete(string id);
	}

	public class AccountService : IAccountService
	{
		private IHttpService _httpService;
		private NavigationManager _navigationManager;
		private ILocalStorageService _localStorageService;
		private string _userKey = "user";

		public User? User { get; private set; }

		public AccountService(
			IHttpService httpService,
			NavigationManager navigationManager,
			ILocalStorageService localStorageService
		)
		{
			_httpService = httpService;
			_navigationManager = navigationManager;
			_localStorageService = localStorageService;
		}

		public async Task Initialize()
		{
			User = await _localStorageService.GetItem<User>(_userKey);
		}

		public async Task Login(RegisterModel model)
		{
			User = await _httpService.Post<User>("/api/account/login", model);
			await _localStorageService.SetItem(_userKey, User);
		}

		public async Task Register(RegisterModel model)
		{
			await _httpService.Post("/api/account/register", model);
		}


		public async Task Logout()
		{
			User = null;
			await _localStorageService.RemoveItem(_userKey);
			_navigationManager.NavigateTo("");
		}


		//public async Task<IList<User>> GetAll()
		//{
		//	return await _httpService.Get<IList<User>>("/users");
		//}

		//public async Task<User> GetById(string id)
		//{
		//	return await _httpService.Get<User>($"/users/{id}");
		//}

		//public async Task Update(string id, EditUser model)
		//{
		//	await _httpService.Put($"/users/{id}", model);

		//	// update stored user if the logged in user updated their own record
		//	if (id == User.Id)
		//	{
		//		// update local storage
		//		User.FirstName = model.FirstName;
		//		User.LastName = model.LastName;
		//		User.Username = model.Username;
		//		await _localStorageService.SetItem(_userKey, User);
		//	}
		//}

		//public async Task Delete(string id)
		//{
		//	await _httpService.Delete($"/users/{id}");

		//	// auto logout if the logged in user deleted their own record
		//	if (id == User.Id)
		//		await Logout();
		//}
	}
}
