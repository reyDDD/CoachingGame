using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamboliyaLibrary.Identity
{
    public class ExternalProvider
    {
        public string DisplayName { get; set; } = null!;
        public string AuthenticationScheme { get; set; } = null!;
    }
}
