﻿namespace TamboliyaLibrary.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
    }
}
