using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamboliyaLibrary.Identity
{
    public class LoggedOutViewModel
    {
        public string PostLogoutRedirectUri { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string SignOutIframeUrl { get; set; } = null!;

        public bool AutomaticRedirectAfterSignOut { get; set; }

        public string LogoutId { get; set; } = null!;
        public bool TriggerExternalSignout => ExternalAuthenticationScheme != null;
        public string ExternalAuthenticationScheme { get; set; } = null!;
    }
}
