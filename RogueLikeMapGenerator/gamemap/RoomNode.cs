
namespace RogueLikeMapGenerator.gamemap
{
    //각 방의 BSP 트리 노드
    public class RoomNode
    {
        //노드가 가지는 방 정보
        public GameRoom room { get; }
        //트리 노드의 연결 관계
        public RoomNode parentNode { get; set; }
        public RoomNode siblingNode { get; set; }
        public RoomNode leftNode { get; set; }
        public RoomNode rightNode { get; set; }
        //연결 가능한 위치
        public int availablePosX { get; set; }
        public int availablePosY { get; set; }
        public int availableWidth { get; set; }
        public int availableHeight { get; set; }
        //실제 방인지 여부
        public bool isRoom { get; set; }

        public RoomNode(GameRoom room) : this(room, null) { }
        public RoomNode(GameRoom room, RoomNode parentNode)
        {
            this.room = room;
            this.parentNode = parentNode;
        }
    }
}
