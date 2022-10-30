using System.ComponentModel.DataAnnotations;

namespace TamboliyaLibrary.DAL
{
	public class RegisterModel
	{
		[EmailAddress]
		[Required(ErrorMessage = "Email is required")]
		public string Email { get; set; } = null!;

		[Required(ErrorMessage = "Password is required")]
		public string Password { get; set; } = null!;
	}
}
