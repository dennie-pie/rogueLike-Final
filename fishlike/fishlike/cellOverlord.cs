using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace fishlike
{
    class cellOverlord
    {
        Random r = new Random();
        private player p;
        private Dictionary<int[], int> distances_ = new Dictionary<int[], int>();
        private cell[,] dungeonMap_;
        private const int mapSize_ = 9, bossRoomCount = 1;
        private int restRoomCount, itemRoomCount;
        private int loopNo;
        private int roomsComplete;

        public Dictionary<int[], int> distances { get { return distances_; } }
        private cell[,] dungeonMap { get { return dungeonMap_; } set { dungeonMap_ = value; } }
        public int mapSize { get { return mapSize_; } } //read only

        public cellOverlord(int rC, int lN, player P)
        {
            p = P;
            loopNo = lN;
            roomsComplete = rC;
            restRoomCount = r.Next(1, 2);
            itemRoomCount = r.Next(1, 3);
            dungeonMap = new cell[mapSize, mapSize];

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (i == 4 & j == 4)
                    {
                        dungeonMap[i, j] = new centreRoom(new int[] { i, j }, p);
                    }
                    else if (tavernkeep.cellMap[(j * 10) + i] == 1)
                    {
                        dungeonMap[i, j] = new enemyRoom(new int[] { i, j }, loopNo, p);
                        distances_.Add(new int[] { i, j }, findDistance(i, j));
                    }
                    else
                    {
                        dungeonMap[i, j] = new cell(new int[] { i, j }, p);
                    }
                }
            }
        }
        public cell.rT getRoomType(int x, int y) { return dungeonMap[x, y].roomType; }
        public string val(int x, int y) { return dungeonMap[x, y].ToString(); }
        public void setHasPlayer(int x, int y, player p)
        {
            bool check()
            {
                bool valid = false;
                bool rtrn = false;
                while (!valid)
                {
                    string s = Console.ReadLine().ToLower();
                    switch (s)
                    {
                        case "y":
                            valid = true;
                            rtrn = true;
                            break;
                        case "n":
                            valid = true;
                            rtrn = false;
                            break;
                    }
                }
                return rtrn;
            }

            dungeonMap[x, y].hasPlayer = true;
            bool escaped = false;

            if (dungeonMap[x, y].roomType == cell.rT.enemy)
            {
                Console.WriteLine("You sense the presence of enemies. Do you wish to escape? y/n");
                
                if (check())
                {
                    if (p.keys != 0)
                    {
                        p.addKeys(-1);
                        escaped = true;
                    }
                    else
                    {
                        Console.WriteLine("You have no keys remaining! You can still try to run for the door. y/n");
                        if (check())
                        {
                            foreach (var i in p.getPrty())
                            {
                                i.hp -= (i.hp / 10);
                            }
                            escaped = true;
                        }
                    }
                }
            }
            if (!escaped)
            {
                combat.startOfRoom(p.getPrty(), dungeonMap[x,y]);
                dungeonMap[x, y].roomFunction(new int[] { roomsComplete });
            }
            combat.endOfRoom(p.getPrty(), dungeonMap[x,y]);
            roomsComplete++;
        }
        public bool complete(int x, int y) { return dungeonMap[x, y].complete; }
        public bool hasRoom(int x, int y) { return dungeonMap[x, y].roomType == cell.rT.empty ? false : true; }
        public int findDistance(int x, int y) { return (Math.Abs(x - 4) + Math.Abs(y - 4)); }
        public bool getHasPlayer(int x, int y) { return dungeonMap[x, y].hasPlayer; }
        public List<int[]> sortDict(Dictionary<int[], int> dict)
        {
            List<int[]> tempList = new List<int[]>();
            foreach (KeyValuePair<int[], int> kvp in dict.OrderByDescending(key => key.Value))
            {
                tempList.Add(kvp.Key);
            }
            return tempList;
        }
        public void roomSetup()
        {
            int posInList = 0;
            List<int[]> sortedList = sortDict(distances);
            for (int i = 0; i < bossRoomCount; i++)
            {
                dungeonMap[sortedList[posInList][0], sortedList[posInList][1]] = new bossRoom(sortedList[posInList], loopNo, p);
                posInList++;
            }
            dungeonMap[sortedList[posInList][0], sortedList[posInList][1]] = new shopRoom(sortedList[posInList], loopNo, p);
            posInList++;
            for (int i = 0; i < itemRoomCount; i++)
            {
                dungeonMap[sortedList[posInList][0], sortedList[posInList][1]] = new itemRoom(sortedList[posInList], loopNo, p);
                posInList++;
            }
            for (int i = 0; i < restRoomCount; i++)
            {
                int room = 0;
                while (room == 0)
                {
                    int roomToCheck = r.Next(posInList, sortedList.Count());
                    if (dungeonMap[sortedList[roomToCheck][0], sortedList[roomToCheck][1]].roomType == cell.rT.enemy)
                    {
                        room += roomToCheck;
                        break;
                    }
                }
                dungeonMap[sortedList[room][0], sortedList[room][1]] = new restRoom(sortedList[room], p);
            }
        }

    }

}
