using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamboliyaLibrary.DAL
{
    public class LogLineDTOModel
    {
        [Required]
        public int GameId { get; set; }
        [Required]
        public int UserId { get; set; }
        public string Message { get; set; } = null!;
    }
}
