
namespace TamboliyaLibrary.Models
{
	public class Alert
	{
		public string Id { get; set; } = null!;
		public AlertType Type { get; set; }
		public string Message { get; set; } = null!;
		public bool AutoClose { get; set; }
		public bool KeepAfterRouteChange { get; set; }
		public bool Fade { get; set; }
	}

	public enum AlertType
	{
		Success,
		Error,
		Info,
		Warning
	}
}
