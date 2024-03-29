﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamboliyaLibrary.Models
{
    public class NewUserGame
    {
        private DateTime dateBeginning;
        private DateTime dateEnding;
        private int? parentGame;


 
        public int TimeOffset { get; set; } = default;


        [Required, MinLength(10, ErrorMessage = "Question's lenght is too short")]
        public string Question { get; set; } = null!;

        [Required]
        public DateTime DateBeginning
        {
            get { return dateBeginning; }
            set
            {
                dateBeginning = value;
                //TODO: виконати перевірку в іншому місці
                //if (value > DateTime.UtcNow)
                //{
                //    dateBeginning = value;
                //}
                //else
                //{
                //    CultureInfo CI = CultureInfo.CurrentCulture;
                //    throw new ArgumentException(value.ToString(CI), "Date isn't right");
                //}
            }
        }

        [Required]
        public DateTime DateEnding
        {
            get { return dateEnding; }
            set
            {
                if (value > DateTime.UtcNow && value > DateBeginning.AddHours(1))
                {
                    dateEnding = value;
                }
                else
                {
                    CultureInfo CI = CultureInfo.CurrentCulture;
                    throw new ArgumentException(value.ToString(CI), "Date ending isn't right");
                }
            }
        }

        [Required]
        public GameType GameType { get; set; }

        public int? ParentGame
        {
            get
            {
                return parentGame;
            }
            set {
                if (GameType == GameType.Child)
                {
                    parentGame = value;
                }
                else
                {
                    parentGame = null;
                }
            }
        }

        [Range(0, 6)]
        public int MaxUsersCount { get; set; }
    }
}
