using System.ComponentModel.DataAnnotations;

namespace TamboliyaLibrary.DAL
{
    public class GameChatLogDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public int GameId { get; set; }
        [Required]
        public string Message { get; set; } = null!;

        
    }
}
