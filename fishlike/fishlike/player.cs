using System;
using System.Collections.Generic;
using System.Text;

namespace fishlike
{
    class player
    {
        public enum gameDifficulty
        {
            easy,
            medm,
            hard
        }
        private int sightRange;
        private int[] playerPosOnMap;
        private List<int[]> roomsSeen;
        private List<int[]> roomsAvailable;
        public bool[] achievements { get; set; }
        public string difficultyStr { get; set; }
        public gameDifficulty difficulty { get; set; }
        public bool newPlayer { get; set; }
        public int keys { get; set; }
        public int coins { get; set; }
        public int olympianCoins { get; set; }
        private List<hero> party;

        public int[] pPos { get { return playerPosOnMap; } }
        public player()
        {
            coins = 0;
            keys = 0;
            sightRange = 1;
            roomsSeen = new List<int[]>();
            roomsAvailable = new List<int[]>();
            party = new List<hero>();
        }
        public void addKeys(int num)
        {
            keys += num;
        }
        public bool allDead() 
        {
            bool tempBool = true;
            foreach (var i in party)
            { 
                if (!i.checkDead())
                {
                    tempBool = false;
                }
            }
            return tempBool;
        }

        public void addHero(hero H) { party.Add(H); }
        public List<hero> getPrty() { return party; }
        public List<int[]> getRS() { return roomsSeen; }
        public List<int[]> getRA() { return roomsAvailable; }
        public void movePlayer(int toX, int toY)
        {
            playerPosOnMap = new int[] { toX, toY };
            roomsSeen.Add(playerPosOnMap);
            setRA();
        }
        public void setRA()
        {
            roomsAvailable.Clear();
            int currX = playerPosOnMap[0];
            int currY = playerPosOnMap[1];

            roomsAvailable = new List<int[]>
            {
                new int[] { currX + sightRange, currY },
                new int[] { currX - sightRange, currY },
                new int[] { currX, currY + sightRange },
                new int[] { currX, currY - sightRange }
            };
        }
    }
}
