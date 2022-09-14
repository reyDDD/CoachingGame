using TamboliyaLibrary.Models;

namespace TamboliyaLibrary.DAL
{
	public class GameDTO
	{
		public int GameId { get; set; }
		public bool IsFinished { get; set; }
		public ActualPositionOnMap ActualPosition { get; set; } = null!;
		public OracleDTO? Oracle { get; set; }
		public List<ActualPositionOnMap> ActualPositionsForSelect { get; set; } = new();
		public string PathToImage { get; init; } = null!;
		public Coordinates Coordinates { get; init; } = null!;
	}
}
