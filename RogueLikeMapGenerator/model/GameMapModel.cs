using RogueLikeMapGenerator.gamemap;

namespace RogueLikeMapGenerator.model
{
    public class GameMapModel
    {
        public int width { get; set; } = 60;
        public int height { get; set; } = 60;
        public int minWidth { get; set; } = 8;
        public int minHeight { get; set; } = 8;
        public int roomMargin { get; set; } = 2;
        public string gameMapString { get; set; }

        private GameMapGenerator gameMapGenerator;


        public GameMapModel()
        {
            gameMapGenerator = new GameMapGenerator();
        }

        public string GenerateGameMap()
        {
            return gameMapGenerator.GenerateGameMap(width, height, minWidth, minHeight, roomMargin);
        }
    }
}
