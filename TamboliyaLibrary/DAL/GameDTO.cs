﻿using TamboliyaLibrary.Models;

namespace TamboliyaLibrary.DAL
{
	public class GameDTO
	{
		public int GameId { get; set; }
		public bool IsFinished { get; set; }
		public DateTime Created { get; set; }

        public DateTime DateBeginning { get; set; }
        public DateTime DateEnding { get; set; }
        public int? ParentGame { get; set; }
        public IEnumerable<GameDTO> ChildsGames = new List<GameDTO>();
        public Guid CreatorGuid { get; set; }
        public int MaxUsersCount { get; set; } = default(int);
		public GameType GameType { get; set; }

        public ActualPositionOnMap ActualPosition { get; set; } = null!;
		public OracleDTO? Oracle { get; set; }
		public List<ActualPositionOnMap> ActualPositionsForSelect { get; set; } = new();
		public string PathToImage { get; init; } = null!;
		public Coordinates Coordinates { get; init; } = null!;
	}
}
