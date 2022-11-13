using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using System.Net;
using Tamboliya.Services;

namespace Tamboliya.Helpers
{
	public class AppRouteView : RouteView
	{
		[Inject]
		public NavigationManager NavigationManager { get; set; } = null!;

		[Inject]
		public IAccountService AccountService { get; set; } = null!;

		protected override void Render(RenderTreeBuilder builder)
		{
			var authorize = Attribute.GetCustomAttribute(RouteData.PageType, typeof(AuthorizeAttribute)) != null;
			if (authorize && AccountService.User == null)
			{
				var returnUrl = WebUtility.UrlEncode(new Uri(NavigationManager.Uri).PathAndQuery);
				NavigationManager.NavigateTo($"account/login?returnUrl={returnUrl}");
			}
			else
			{
				base.Render(builder);
			}
		}
	}
}
