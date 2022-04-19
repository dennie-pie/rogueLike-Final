using System;
using System.Collections.Generic;
using System.Text;


namespace fishlike  
{
    static class tavernkeep
    {
        public static int[] cellMap = new int[100];

        public static void initTavernKeep()
        {
            Random r = new Random();
            int drunkardNo;
            for (int i = 0; i < 100; i++)
            {
                cellMap[i] = 0;
            }
            addRoom(44);
            drunkardNo = r.Next(2, 4);
            for ( int i = 0; i < drunkardNo; i++)
            {
                drunkard d = new drunkard(44);
            }
        }
        public static void addRoom(int pos)
        {
            cellMap[pos] = 1;
        }

        public static bool hasCell(int pos)
        {
            return cellMap[pos] == 1 ? true : false;
        }
    }

    class drunkard
    {
        private Random r = new Random();
        private int pos;
        private const int maxRoomAmount = 5;
        private int roomAmount = 0;
        public drunkard(int p)
        {
            pos = p;
            getOutThere();
        }

        private void getOutThere()
        {
            if(roomAmount < maxRoomAmount)
            {
                List<int> possibleStartPos = checkAround(pos);

                try
                {
                    pos = possibleStartPos[r.Next(possibleStartPos.Count)];
                    tavernkeep.addRoom(pos);
                    roomAmount++;
                    getOutThere();
                }
                catch { }
                
            }
        }
        private List<int> checkAround(int pos)
        {
            List<int> availPos = new List<int>();
            if (validatePos(pos + 1)) { if (!check(pos + 1)) { availPos.Add(pos + 1); } }
            if (validatePos(pos + 10)) { if (!check(pos + 10)) { availPos.Add(pos + 10); } }
            if (validatePos(pos - 1)) { if (!check(pos - 1)) { availPos.Add(pos - 1); } }
            if (validatePos(pos - 10)) { if (!check(pos - 10)) { availPos.Add(pos - 10); } }

            return availPos;
        }

        private bool validatePos(int pos)
        {
            if (pos < 100 && pos >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool check(int pos)
        {
            return tavernkeep.hasCell(pos);
        }
    }
}
