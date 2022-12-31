using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamboliyaLibrary.Models
{
    public class NewParentGame
    {
        private DateTime dateBeginning;

        [Required, MinLength(10, ErrorMessage = "Question's lenght is too short")]
        public string Question { get; set; } = null!;

        [Required]
        public DateTime DateBeginning
        {
            get { return dateBeginning; }
            set
            {
                if (value > DateTime.UtcNow)
                {
                    dateBeginning = value;
                }
                else
                {
                    CultureInfo CI = CultureInfo.CurrentCulture;
                    throw new ArgumentException(value.ToString(CI), "Date isn't right");
                }
            }
        }

        [Required]
        public GameType GameType { get; set; }

        public int? ParentGame { get; set; }

        [Range(1, 6)]
        public int MaxUsersCount { get; set; }
    }
}
