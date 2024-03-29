﻿using System.ComponentModel.DataAnnotations;

namespace TamboliyaLibrary.DAL
{
    public class LogLineDTOModel
    {
        [Required]
        public int GameId { get; set; }
        public string Message { get; set; } = null!;
    }
}
