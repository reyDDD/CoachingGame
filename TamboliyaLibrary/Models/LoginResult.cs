using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamboliyaLibrary.Models
{
    public class LoginResult
    {
        public bool Successful { get; set; }
        public string Error { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
