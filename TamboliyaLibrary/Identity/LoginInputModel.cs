using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamboliyaLibrary.Identity
{
    public class LoginInputModel
    {
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; } = null!;
    }
}
