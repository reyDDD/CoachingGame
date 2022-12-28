﻿using System.ComponentModel.DataAnnotations;

namespace TamboliyaLibrary.Models
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
