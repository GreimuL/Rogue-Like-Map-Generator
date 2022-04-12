using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLikeMapGenerator.gamemap
{
    public class GameMapGenerator
    {
        private int width;
        private int height;
        private int minWidth = 30;
        private int minHeight = 30;
        private int roomMargin = 2;
        private GameMap gameMap;
        private char[,] charGameMap;
        private Random randomValueGenerator;

        public GameMapGenerator()
        {
            randomValueGenerator = new Random();
        }

        /// <summary>
        /// 게임 맵을 생성하고 string 형태의 맵을 리턴
        /// </summary>
        public string GenerateGameMap(int width, int height, int minWidth, int minHeight, int roomMargin)
        {
            this.roomMargin = roomMargin;
            this.width = width;
            this.height = height;
            this.minWidth = minWidth + roomMargin * 2;
            this.minHeight = minHeight + roomMargin * 2;
            gameMap = new GameMap(width, height);
            RoomNode headNode = gameMap.headNode;

            DivideRoom(headNode);
            try
            {
                charGameMap = GetCharGameMap();
            }
            catch
            {
                throw;
            }
            return CharMapToString(charGameMap);
        }

        /// <summary>
        /// 맵을 반씩 나누면서 재귀적으로 BSP 트리를 만드는 함수
        /// </summary>
        private void DivideRoom(RoomNode parentNode)
        {
            int roomWidth = parentNode.room.width;
            int roomHeight = parentNode.room.height;
            int parentPositionX = parentNode.room.posX;
            int parentPositionY = parentNode.room.posY;

            int[] childRoomWidth = new int[2] { roomWidth, roomWidth };
            int[] childRoomHeight = new int[2] { roomHeight, roomHeight };
            (int, int)[] childRoomPosition = new (int, int)[2];
            childRoomPosition[0] = (parentPositionX, parentPositionY);

            double randomRatio = randomValueGenerator.NextDouble(); //방을 어느 비율로 자를지 결정
            randomRatio = (randomRatio / 2) + 0.25;

            // 자를 방의 가로가 더 길면 세로로 자르고 세로가 더 길면 가로로 자른다.
            if (roomWidth >= roomHeight)
            {
                childRoomWidth[0] = (int)(roomWidth * randomRatio);
                childRoomWidth[1] = roomWidth - childRoomWidth[0];
                childRoomPosition[1] = (parentPositionX, parentPositionY + childRoomWidth[0]);
            }
            else
            {
                childRoomHeight[0] = (int)(roomWidth * randomRatio);
                childRoomHeight[1] = roomHeight - childRoomHeight[0];
                childRoomPosition[1] = (parentPositionX + childRoomHeight[0], parentPositionY);
            }
            //결정한 자식 방의 크기가 최소 크기 조건을 만족한다면 노드를 추가하고 그 자식 노드를 다시 나눈다.
            for (int i = 0; i < 2; i++)
            {
                if (childRoomWidth[i] >= minWidth && childRoomHeight[i] >= minHeight)
                {
                    RoomNode childNode = InsertRoomToGameMap(parentNode, childRoomWidth[i], childRoomHeight[i], childRoomPosition[i]);
                    DivideRoom(childNode);
                }
                else
                {
                    //InsertRoomToGameMap(parentNode, 0, 0,(-1,-1));
                }
            }
            //어느 자식도 생성 되지 않았다면, 그 노드는 실제 생성되는 방(리프노드) 이다.
            if (parentNode.leftNode == null && parentNode.rightNode == null)
            {
                parentNode.isRoom = true;
                parentNode.availableHeight = parentNode.room.height;
                parentNode.availableWidth = parentNode.room.width;
                parentNode.availablePosX = parentNode.room.posX;
                parentNode.availablePosY = parentNode.room.posY;
            }
        }

        /// <summary>
        /// 새로운 방을 생성하여 BSP 트리에 추가
        /// </summary>
        private RoomNode InsertRoomToGameMap(RoomNode parent, int width, int height, (int, int) position)
        {
            GameRoom childRoom = new GameRoom(width, height, position.Item1, position.Item2);
            RoomNode childRoomNode = new RoomNode(childRoom, parent);
            gameMap.InsertRoom(parent, childRoomNode);

            return childRoomNode;
        }

        /// <summary>
        /// 생성된 BSP 트리를 이용하여 방들을 연결하고 실질적인 게임 맵을 그리는 함수.
        /// </summary>
        private char[,] GetCharGameMap()
        {
            char[,] charMap = new char[height, width];
            List<RoomNode> nodeList = gameMap.GetNodeList(false);
            List<RoomNode> roomList = gameMap.GetNodeList(true);

            //모든 맵을 벽으로 채운다.
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    charMap[i, j] = '■';
                }
            }

            try
            {
                //리프 노드들만 받아서 방을 먼저 생성한다.
                roomList.ForEach(node =>
                {
                    int w = node.room.width;
                    int h = node.room.height;
                    int x = node.room.posX;
                    int y = node.room.posY;
                    for (int i = x + roomMargin; i < x + h - roomMargin; i++)
                    {
                        for (int j = y + roomMargin; j < y + w - roomMargin; j++)
                        {
                            charMap[i, j] = '□';
                        }
                    }
                });

                //모든 노드 정보를 받아서 방을 연결한다.
                nodeList.ForEach(node =>
                {
                    if (!node.isRoom)
                    {
                        if (node.leftNode == null && node.rightNode != null)
                        {
                            node.availablePosX = node.rightNode.availablePosX;
                            node.availablePosY = node.rightNode.availablePosY;
                            node.availableWidth = node.rightNode.availableWidth;
                            node.availableHeight = node.rightNode.availableHeight;
                        }
                        else if (node.leftNode != null && node.rightNode == null)
                        {
                            node.availablePosX = node.leftNode.availablePosX;
                            node.availablePosY = node.leftNode.availablePosY;
                            node.availableWidth = node.leftNode.availableWidth;
                            node.availableHeight = node.leftNode.availableHeight;
                        }
                        else
                        {
                            //자식 노드가 모두 있을경우 두 자식을 연결 시켜준다.
                            int selectDoor = randomValueGenerator.Next(2);  //parent 노드를 다른 노드와 합칠 때 연결할 방 위치
                            //parent 노드의 연결 가능 위치 업데이트
                            if (selectDoor == 0)
                            {
                                node.availablePosX = node.leftNode.availablePosX;
                                node.availablePosY = node.leftNode.availablePosY;
                                node.availableWidth = node.leftNode.availableWidth;
                                node.availableHeight = node.leftNode.availableHeight;
                            }
                            else
                            {
                                node.availablePosX = node.rightNode.availablePosX;
                                node.availablePosY = node.rightNode.availablePosY;
                                node.availableWidth = node.rightNode.availableWidth;
                                node.availableHeight = node.rightNode.availableHeight;
                            }

                            (int, int) leftNodeDoorPosition = (0, 0);
                            (int, int) rightNodeDoorPosition = (0, 0);

                            //각 자식 방들의 출구 위치를 정하고 서로 연결한다.
                            if (node.room.width >= node.room.height)
                            {
                                //방이 좌우로 나뉘어진 경우 가로 연결 통로 생성
                                leftNodeDoorPosition.Item2 = node.leftNode.availablePosY + node.leftNode.availableWidth - roomMargin;
                                rightNodeDoorPosition.Item2 = node.rightNode.availablePosY + roomMargin;

                                leftNodeDoorPosition.Item1 = node.leftNode.availablePosX + randomValueGenerator.Next(node.leftNode.availableHeight - roomMargin * 2) + roomMargin;
                                rightNodeDoorPosition.Item1 = node.rightNode.availablePosX + randomValueGenerator.Next(node.rightNode.availableHeight - roomMargin * 2) + roomMargin;

                                int center = (leftNodeDoorPosition.Item2 + rightNodeDoorPosition.Item2) / 2;

                                int top = Math.Min(leftNodeDoorPosition.Item1, rightNodeDoorPosition.Item1);
                                int bottom = Math.Max(leftNodeDoorPosition.Item1, rightNodeDoorPosition.Item1);

                                for (int i = top; i <= bottom; i++)
                                {
                                    charMap[i, center] = '□';
                                }
                                for (int i = leftNodeDoorPosition.Item2; i <= center; i++)
                                {
                                    charMap[leftNodeDoorPosition.Item1, i] = '□';
                                }
                                for (int i = center; i <= rightNodeDoorPosition.Item2; i++)
                                {
                                    charMap[rightNodeDoorPosition.Item1, i] = '□';
                                }
                            }
                            else
                            {
                                //방이 상하로 나뉘어진 경우 세로 연결 통로 생성
                                leftNodeDoorPosition.Item1 = node.leftNode.availablePosX + node.leftNode.availableHeight - roomMargin;
                                rightNodeDoorPosition.Item1 = node.rightNode.availablePosX + roomMargin;

                                leftNodeDoorPosition.Item2 = node.leftNode.availablePosY + randomValueGenerator.Next(node.leftNode.availableWidth - roomMargin * 2) + roomMargin;
                                rightNodeDoorPosition.Item2 = node.rightNode.availablePosY + randomValueGenerator.Next(node.rightNode.availableWidth - roomMargin * 2) + roomMargin;

                                int center = (leftNodeDoorPosition.Item1 + rightNodeDoorPosition.Item1) / 2;

                                int top = Math.Min(leftNodeDoorPosition.Item2, rightNodeDoorPosition.Item2);
                                int bottom = Math.Max(leftNodeDoorPosition.Item2, rightNodeDoorPosition.Item2);

                                for (int i = top; i <= bottom; i++)
                                {
                                    charMap[center, i] = '□';
                                }
                                for (int i = leftNodeDoorPosition.Item1; i <= center; i++)
                                {
                                    charMap[i, leftNodeDoorPosition.Item2] = '□';
                                }
                                for (int i = center; i <= rightNodeDoorPosition.Item1; i++)
                                {
                                    charMap[i, rightNodeDoorPosition.Item2] = '□';
                                }
                            }
                        }


                    }
                });
            }
            catch
            {
                throw;
            }

            return charMap;
        }

        /// <summary>
        /// char array로 이루어진 charMap 을 string으로 변환
        /// </summary>
        private string CharMapToString(char[,] charMap)
        {
            StringBuilder stringBuilder = new StringBuilder("");
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    stringBuilder.Append(charMap[i, j]);
                }
                stringBuilder.Append("\n");
            }
            return stringBuilder.ToString();
        }
    }
}
