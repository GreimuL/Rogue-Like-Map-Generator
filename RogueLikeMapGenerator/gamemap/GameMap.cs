using System.Collections.Generic;

namespace RogueLikeMapGenerator.gamemap
{
    //BSP 트리 정보를 가지고 있는 클래스
    class GameMap
    {
        private int width;
        private int height;
        public RoomNode headNode { get; }

        public GameMap(int width, int height)
        {
            this.width = width;
            this.height = height;

            GameRoom headRoom = new GameRoom(width, height, 0, 0);
            headNode = new RoomNode(headRoom);
        }

        /// <summary>
        /// 특정 노드에 자식 추가
        /// </summary>
        /// <param name="parent">부모가 될 노드</param>
        /// <param name="child">자식이 될 노드</param>
        public void InsertRoom(RoomNode parent, RoomNode child)
        {
            if (parent.leftNode == null)
            {
                parent.leftNode = child;
            }
            else
            {
                parent.rightNode = child;
                child.siblingNode = parent.leftNode;
                parent.leftNode.siblingNode = child;
            }
        }
        /// <summary>
        /// BSP 트리를 후위탐색한 결과 return
        /// </summary>
        /// <param name="isOnlyRoom">true일 경우 리프 노드(room)만 결과에 포함</param>
        public List<RoomNode> GetNodeList(bool isOnlyRoom)
        {
            List<RoomNode> nodeList = new List<RoomNode>();
            TraversalGameMap(headNode, nodeList, isOnlyRoom);
            return nodeList;
        }

        /// <summary>
        /// BSP 트리를 후위탐색 하면서 노드리스트에 노드 추가
        /// </summary>
        /// <param name="isOnlyRoom">true일 경우 리프 노드만 노드리스트에 추가</param>
        private void TraversalGameMap(RoomNode currentNode, List<RoomNode> nodeList, bool isOnlyRoom)
        {
            if (currentNode.leftNode != null)
            {
                TraversalGameMap(currentNode.leftNode, nodeList, isOnlyRoom);
            }
            if (currentNode.rightNode != null)
            {
                TraversalGameMap(currentNode.rightNode, nodeList, isOnlyRoom);
            }
            if (currentNode.isRoom && isOnlyRoom)
            {
                nodeList.Add(currentNode);
            }
            else if (!isOnlyRoom)
            {
                nodeList.Add(currentNode);
            }
        }

    }
}
