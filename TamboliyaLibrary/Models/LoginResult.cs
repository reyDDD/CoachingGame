
namespace TamboliyaLibrary.Models
{
    public class LoginResult
    {
        public bool Successful { get; set; }
        public string? Error { get; set; } = null!;
        public string? Token { get; set; } = null!;
    }
}
