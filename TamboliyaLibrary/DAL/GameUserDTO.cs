using System.ComponentModel.DataAnnotations;

namespace TamboliyaLibrary.DAL
{
    public class GameUserDTO
    {
        [Required]
        public int GameId { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }
}
