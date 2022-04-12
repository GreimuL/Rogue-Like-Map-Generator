
namespace RogueLikeMapGenerator.gamemap
{
    //방 정보
    public class GameRoom
    {
        //방 크기
        public int width { get; set; }
        public int height { get; set; }
        //방 위치
        public int posX { get; set; }
        public int posY { get; set; }
        public GameRoom(int width, int height, int x, int y)
        {
            this.width = width;
            this.height = height;
            posX = x;
            posY = y;
        }
    }
}
