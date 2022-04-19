using System;
using System.Collections.Generic;
using System.Text;

namespace fishlike
{
    class cell
    {
        public  enum rT
        {
            empty,
            enemy,
            item,
            boss,
            rest,
            centre,
            temp,
            shop
        }
        private int[] pos_;
        private bool complete_, hasPlayer_;
        private rT roomType_;
        private player player_;
        protected string strVal;

        public player p { get { return player_; } set { player_ = value; } }
        public int[] pos { get { return pos_; } set { pos_ = value; } }
        public bool complete { get { return complete_; } set { complete_ = value; } }
        public bool hasPlayer { get { return hasPlayer_; } set { hasPlayer_ = value; } }
        public rT roomType { get { return roomType_; } set { roomType_ = value; } }
        public virtual void roomComplete()
        {
            strVal = "=";
        }
        public virtual string ToString()
        {
            return strVal;
        }


        public cell(int[] po_s, player P)
        {
            p = P;
            pos = pos_;
            complete = false;
            hasPlayer = false;
            roomType = rT.empty;
            strVal = " ";
        }
        public virtual void roomFunction() { }
        public virtual void roomFunction(int[] args) { }
    }

    class tempRoom : cell
    {
        public tempRoom(int[] pos_, player p) : base(pos_, p)
        {
            roomType = rT.temp;
        }
        public override string ToString()
        {
            return base.ToString();
        }
        public override void roomComplete()
        {
            base.roomComplete();
        }
    }

    class enemyRoom : cell
    {
        private List<enemy> enemies = new List<enemy>();
        private int coordCash; //the game uses this value to summon enemies
        private int goblinCount = 0;
        public enemyRoom(int[] pos_, int loopNo, player p) : base(pos_, p)
        {
            coordCash = 0 + (loopNo * 5);
            roomType = rT.enemy;
            strVal = "!";
        }
        public override string ToString()
        {
            return base.ToString();
        }

        public override void roomFunction(int[] rC)
        {
            int roomsComplete = rC[0];
            if (hasPlayer && !complete)
            {
                summonEnemies(roomsComplete);
 
                combat.startOfCombat(p.getPrty(), enemies);

                while (!checkAllDead())
                {
                    Console.Clear();
                    Console.WriteLine("WHAT WILL YOU DO?");

                    if (p.allDead()) { return; }

                    gameMenu.userCombatInput(p, enemies);
 
                    foreach(var e in enemies)
                    {
                        string enemyChoice = e.makeChoice(p);
                        if (!e.checkDead())
                        {
                            try
                            {
                                int attack = Convert.ToInt32(enemyChoice);
                                combat.attack(e, p.getPrty()[attack]);
                            }
                            catch
                            {
                                e.ability(p);
                            }
                        }
                    }
                }
                combat.endOfCombat(p.getPrty(), enemies);
                complete = true;
                roomComplete();
            }
        }

        public override void roomComplete()
        {
            base.roomComplete();
        }

        private bool checkAllDead()
        {
            bool temp = true;
            foreach (var item in enemies)
            {
                if (!item.checkDead()) { temp = false; }
            }
            return temp;
        }

        private void summonEnemies(int roomsComplete)
        {
            int fails = 0;
            while (coordCash != 0)
            {
                var enemyChoice = selectRandomEnemyType();
                if (enemyChoice.cost <= coordCash)
                {
                    coordCash -= enemyChoice.cost;
                    if (enemyChoice.ToString().ToUpper() == "GOBLIN") { goblinCount++; }
                    enemies.Add(enemyChoice);
                }
                else
                {
                    if (fails == 3)//tries 3 different types of enemies before giving up. this is done because the coordinator may get stuck on an amount of coordcash that cannot be spent
                    {
                        coordCash = 0;
                    }
                    else
                    {
                        fails++;
                    }
                }
            }
            foreach (var nmy in enemies)
            {
                if(nmy.ToString().ToUpper() == "GOBLIN") { goblin g = (goblin)nmy; g.setStats(new int[] { roomsComplete, goblinCount }); }
                else { nmy.setStats(new int[] { roomsComplete}); }
            }
        }

        private enemy selectRandomEnemyType()
        {
            var enemyChosen = new enemy();
            Random r = new Random();

            string[] enemyTypes = new string[] { "goblin", "rat", "ghost" };
            var choice = enemyTypes[r.Next(0, enemyTypes.Length)];

            switch (choice)
            {
                case "goblin":
                    enemyChosen = new goblin();
                    break;
                case "rat":
                    enemyChosen = new rat();
                    break;
                case "ghost":
                    enemyChosen = new ghost();
                    break;
            }
            return enemyChosen;
        }

    }

    class bossRoom : cell
    {
        private List<enemy> enemies = new List<enemy>();
        private int coordCash;
        public bossRoom(int[] pos_, int loopNo, player p) : base(pos_, p)
        {
            coordCash = 0 + (loopNo * 10);
            roomType = rT.boss;
            strVal = "@";
        }
        public override string ToString()
        {
            return base.ToString();
        }
        public override void roomFunction(int[] rC)
        {
            int roomsComplete = rC[0];
            if (hasPlayer && !complete)
            {
                summonEnemies(roomsComplete);

                combat.startOfCombat(p.getPrty(), enemies);

                while (!checkAllDead())
                {
                    Console.Clear();
                    Console.WriteLine("WHAT WILL YOU DO?");

                    if (p.allDead()) { return; }

                    gameMenu.userCombatInput(p, enemies);

                    foreach (var e in enemies)
                    {
                        string enemyChoice = e.makeChoice(p);
                        if (!e.checkDead())
                        {
                            try
                            {
                                int attack = Convert.ToInt32(enemyChoice);
                                combat.attack(e, p.getPrty()[attack]);
                            }
                            catch
                            {
                                e.ability(p);
                            }
                        }
                    }
                }
                combat.endOfCombat(p.getPrty(), enemies);
                complete = true;
                p.olympianCoins++;
                roomComplete();
            }
        }
        public override void roomComplete()
        {
            base.roomComplete();
        }
        private void summonEnemies(int roomsComplete)
        {
            int fails = 0;
            while (coordCash != 0)
            {
                var enemyChoice = selectRandomEnemyType();
                if (enemyChoice.cost <= coordCash)
                {
                    coordCash -= enemyChoice.cost;
                    enemies.Add(enemyChoice);
                }
                else
                {
                    if (fails == 3)//tries 3 different types of enemies before giving up. this is done because the coordinator may get stuck on an amount of coordcash that cannot be spent
                    {
                        coordCash = 0;
                    }
                    else
                    {
                        fails++;
                    }
                }
            }
            foreach (var nmy in enemies)
            {
                nmy.setStats(new int[] { roomsComplete });
            }
        }
        private bool checkAllDead()
        {
            bool temp = true;
            foreach (var item in enemies)
            {
                if (!item.checkDead()) { temp = false; }
            }
            return temp;
        }
        private enemy selectRandomEnemyType()
        {
            var enemyChosen = new enemy();
            Random r = new Random();

            string[] enemyTypes = new string[] { "goblin", "rat" };
            var choice = enemyTypes[r.Next(0, enemyTypes.Length)];

            switch (choice)
            {
                case "goblin":
                    enemyChosen = new goblinKing();
                    break;
                case "rat":
                    enemyChosen = new ratKing();
                    break;
            }
            return enemyChosen;
        }
    }

    class itemRoom : cell
    {
        private int loopNo;
        public itemRoom(int[] pos_, int lN, player p) : base(pos_, p)
        {
            loopNo = lN;
            roomType = rT.item;
            strVal = "?";
        }
        public override string ToString()
        {
            return base.ToString();
        }
        public override void roomFunction(int[] args)
        {
            if (complete) { return; }
            Console.WriteLine("You find an empty room with a pedestal. The pedestal has an item on it. You grab the item and run.");
            Console.ReadLine();
            Random r = new Random();
            if (hasPlayer)
            {
                int item = 0;
                if (loopNo == 1)
                {
                    int chance = r.Next(0, 100);
                    if (chance < 5)
                    {
                        item = backpack.epics[r.Next(0, backpack.epics.Length)];
                    }
                    else if (chance < 25)
                    {
                        item = backpack.rares[r.Next(0, backpack.rares.Length)];
                    }
                    else
                    {
                        item = backpack.commons[r.Next(0, backpack.commons.Length)];
                    }
                }
                else if (loopNo < 3)
                {
                    int chance = r.Next(0, 100);
                    if (chance < 10)
                    {
                        item = backpack.epics[r.Next(0, backpack.epics.Length)];
                    }
                    else if (chance < 40)
                    {
                        item = backpack.rares[r.Next(0, backpack.rares.Length)];
                    }
                    else
                    {
                        item = backpack.commons[r.Next(0, backpack.commons.Length)];
                    }
                }
                else
                {
                    int chance = r.Next(0, 100);
                    if (chance < 15)
                    {
                        item = backpack.epics[r.Next(0, backpack.epics.Length)];
                    }
                    else if (chance < 55)
                    {
                        item = backpack.rares[r.Next(0, backpack.rares.Length)];
                    }
                    else
                    {
                        item = backpack.commons[r.Next(0, backpack.commons.Length)];
                    }
                }
                backpack.addItem(item);
                complete = true;
                roomComplete();
            }
        }
        public override void roomComplete()
        {
            base.roomComplete();
        }
    }

    class restRoom : cell
    {
        public restRoom(int[] pos_, player p) : base(pos_, p)
        {
            roomType = rT.rest;
            strVal = "#";
        }
        public override string ToString()
        {
            return base.ToString();
        }
        public override void roomFunction(int[] args)
        {
            if(complete) { return; }
            Console.Clear();
            Console.WriteLine("===================================================");
            Console.WriteLine("This room seems safe, thus you take a short break. ");

            foreach (var u in p.getPrty())
            {
                int healamount = u.maxHp / 2;
                u.heal(healamount);
                Console.WriteLine($"{u.name} the {u} healed for {healamount}.");
                Console.WriteLine($"----------------------{u.hp}/{u.maxHp}----------------------");
            }
            Console.WriteLine("===================================================");
            complete = true;
            roomComplete();
            Console.ReadLine();
        }
        public override void roomComplete()
        {
            base.roomComplete();
        }
    }

    class centreRoom : cell
    {
        public centreRoom(int[] pos_, player p) : base(pos_, p)
        {
            roomType = rT.centre;
            strVal = "*";
        }
        public override string ToString()
        {
            return base.ToString();
        }
        public override void roomComplete()
        {
            base.roomComplete();
        }
    }

    class shopRoom : cell
    {
        private int loopNo;
        public shopRoom(int[] pos_,int lN, player p) : base(pos_, p)
        {
            loopNo = lN;
            roomType = rT.shop;
            strVal = "+";
        }
        public override string ToString()
        {
            return base.ToString();
        }
        public override void roomComplete()
        {
            base.roomComplete();
        }
        public override void roomFunction(int[] args)
        {
            if (complete) { return; }
            if(raccoon.playerEncounters == 0)
            {
                raccoon.firstEncounter(p);
            }
            else
            {
                raccoon.shopFunc(p, loopNo);
                raccoon.playerEncounters++;
            }
            complete = true;
            roomComplete();
        }
    }
}