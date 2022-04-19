using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace fishlike
{
    class game
    {

        protected cellOverlord c;
        protected player p;
        protected int loopNo = 0;
        protected int roomsComplete = 0;
        protected savedPlayer playerConfig;

        public game(player P)
        {

            p = P;
        }

        public void gameStart(savedPlayer pC)
        {
            playerConfig = pC;
            Console.WriteLine("Select 2 Heroes to start your adventure. ");
            p.addHero(gameMenu.characterSelection(p));
            p.addHero(gameMenu.characterSelection(p));

            raccoon.resetRaccoon();

            while (!p.allDead())
            {
                loopNo++;
                if (loopNo % 4 == 0)
                {
                    Console.WriteLine("The Gods upon mount onlympus notice your feats. They invite you up to meet them. (y/n)");
                    var choice = Console.ReadLine().ToLower();
                    if (choice == "y")
                    {
                        olympianShop(p, playerConfig);
                    }
                }
                gameLoop();
            }
            Console.WriteLine("Your party perishes. What remains of them is used by the local wildlife as shelter.");
            Console.WriteLine($"Loops Survived: {loopNo}");
            Console.WriteLine($"Rooms Completed: {roomsComplete}");
            Console.WriteLine($"Difficulty: {p.difficulty}");
            p.coins = 0;
            playerConfig.achievements = p.achievements;
            playerConfig.olympianCoins = p.olympianCoins;
            jsonInteraction.saveConfig(playerConfig);
        }
        public void olympianShop(player p, savedPlayer pC)
        {
            Console.WriteLine("Hermes' Shop of Once Famous, Now Imprisoned Heroes!");
            Console.WriteLine($"You Have: {p.olympianCoins} Olympian Coins.");
            Console.WriteLine("Spend 5 for a new hero?");
            var heroes = new string[] { "ghost" };
            var posInAchievements = 1;
            foreach (var h in heroes)
            {
                //pos 0 in achievements is for the secret racoon hero which cannot be purchased
                if (p.achievements[posInAchievements] == false)
                {
                    Console.WriteLine(h);
                    Console.WriteLine("Would you like to purchase? y/n");
                    var choice = Console.ReadLine().ToLower();
                    if (choice == "y")
                    {
                        if (p.olympianCoins - 5 >= 0)
                        {
                            p.achievements[posInAchievements] = true;
                            p.olympianCoins -= 5;
                        }
                        else
                        {
                            Console.WriteLine("not enough coins :(");
                        }
                    }
                }
            }
        }
        public void gameLoop()
        {
            Console.WriteLine($"Floor {loopNo}");
            tavernkeep.initTavernKeep();

            c = new cellOverlord(roomsComplete, loopNo, p);
            c.roomSetup();

            movePlayer(4, 4);
            var loop = true;
            while (loop)
            {
                if (!p.allDead())
                {
                    loop = !roomTraversal();
                }
                else
                {
                    loop = false;
                }
            }
        }
        public bool roomTraversal()
        {
            var retrn = false;
            gameMenu.displayMap(p, c);
            gameMenu.displayPlayerUnit(p);
            gameMenu.itemMenu(p);
            int[] pos = gameMenu.directionalInput(c, p.pPos);

            int x = pos[0];
            int y = pos[1];

            movePlayer(x, y);
            if (c.getRoomType(x, y) == cell.rT.boss && c.complete(x, y))
            {
                Console.WriteLine("The walls begin moving, revealing a dimly lit staircase down.");
                bool valid = false;
                while (!valid)
                {
                    Console.WriteLine("Go down? y/n");
                    string choice = Console.ReadLine().ToLower();
                    switch (choice)
                    {
                        case "y":
                            retrn = true;
                            valid = true;
                            break;
                    }
                }
            }
            roomsComplete++;
            foreach(var i in p.getPrty())
            {
                i.spell.incrementSpell();
            }
            return retrn;
            
        }

        public void movePlayer(int x, int y)
        {
            p.movePlayer(x, y);
            c.setHasPlayer(x, y, p);
        }
    }

    static class gameMenu
    {
        public static bool check(string[] valid, string check)
        { 
            var val = false;
            foreach (var s in valid)
            {
                if (s == check) { val = true; } 
            }
            return val;
        }
        public static void menu(player p, savedPlayer pC)
        {
            Console.Clear();
            Console.WriteLine("===== CONSOLE ROGUELIKE =====");
            Console.WriteLine("|                           |");
            Console.WriteLine("|        [P]LAY GAME        |");
            Console.WriteLine("|          [W]IKI           |");
            Console.WriteLine("|        [S]ETTINGS         |");
            Console.WriteLine("|          [Q]UIT           |");
            Console.WriteLine("|                           |");
            Console.WriteLine("=============================");
            var choice = "";
            var valid = false;
            while (!valid)
            {
                choice = Console.ReadLine().ToLower();
                valid = check(new string[] { "p", "s", "w", "q" }, choice);
            }
            switch (choice)
            {
                case "w":
                    guide(p, pC);
                    break;
                case "p":
                    gameStart(p, pC);
                    break;
                case "s":
                    settings(p, pC);
                    break;
                case "q":
                    pC.achievements = p.achievements;
                    pC.difficulty = p.difficultyStr;
                    pC.newPlayer = p.newPlayer;
                    pC.olympianCoins = p.olympianCoins;

                    jsonInteraction.saveConfig(pC);
                    Environment.Exit(0);
                    break;
            }
        }
        public static void gameStart(player p, savedPlayer pC)
        {
            var g = new game(p);
            backpack.generateBackpack();
            raccoon.resetRaccoon();
            g.gameStart(pC);
        }
        public static void settings(player p, savedPlayer pC)
        {
            Console.Clear();
            Console.WriteLine("=================== SETTINGS =================");
            Console.WriteLine("|                                            |");
            Console.WriteLine("|           [N]EW PLAYER MODE TOGGLE         |");
            var str = p.newPlayer ? " ON" : "OFF";
            Console.WriteLine($"|                (currently {str})             |");
            Console.WriteLine("|                 DIFFICULTIES:              |");
            Console.WriteLine("|                    [E]ASY                  |");
            Console.WriteLine("|                   [M]EDIUM                 |");
            Console.WriteLine("|                    [H]ARD                  |");
            str = p.difficulty.ToString();
            Console.WriteLine($"|               (currently {str})             |");
            Console.WriteLine("|                                            |");
            Console.WriteLine("|               [R]ESET USER DATA            |");
            Console.WriteLine("|          (DATA CANNOT BE RECOVERED.)       |");
            Console.WriteLine("|                                            |");
            Console.WriteLine("|                    [Q]UIT                  |");
            Console.WriteLine("|                                            |");
            Console.WriteLine("==============================================");

            var choice = "";
            var valid = false;
            while (!valid)
            {
                choice = Console.ReadLine().ToLower();
                valid = check(new string[] { "n", "e", "m", "h", "q", "r" }, choice);
            }

            switch (choice)
            {
                case "n":
                    p.newPlayer = p.newPlayer ? false : true;
                    settings(p, pC);
                    break;
                case "e":
                    p.difficulty = player.gameDifficulty.easy;
                    p.difficultyStr = "easy";
                    settings(p, pC);
                    break;
                case "m":
                    p.difficulty = player.gameDifficulty.medm;
                    p.difficultyStr = "medium";
                    settings(p, pC);
                    break;
                case "h":
                    p.difficulty = player.gameDifficulty.hard;
                    p.difficultyStr = "hard";
                    settings(p, pC);
                    break;
                case "q":
                    menu(p, pC);
                    break;
                case "r":
                    pC = jsonInteraction.resetConfig();
                    jsonInteraction.match(p, pC);
                    settings(p, pC);
                    break;
            }

        }
        public static int[] directionalInput(cellOverlord c, int[] pos)
        {
            var finalPos = new int[2];
            Console.WriteLine("  W  ");
            Console.WriteLine("A/S/D");
            var validInp = false;
            while (!validInp)
            {
                string userInput = Console.ReadLine().ToLower();
                switch (userInput)
                {
                    case "w":
                        finalPos = new int[] {pos[0] - 1, pos[1]};
                        if (c.getRoomType(finalPos[0], finalPos[1]) != cell.rT.empty)
                        {
                            validInp = true;
                        }
                        break;
                    case "a":
                        finalPos = new int[] { pos[0], pos[1] - 1 };
                        if (c.getRoomType(finalPos[0], finalPos[1]) != cell.rT.empty)
                        {
                            validInp = true;
                        }
                        break;
                    case "s":
                        finalPos = new int[] { pos[0] + 1, pos[1] };
                        if (c.getRoomType(finalPos[0], finalPos[1]) != cell.rT.empty)
                        {
                            validInp = true;
                        }
                        break;
                    case "d":
                        finalPos = new int[] { pos[0], pos[1] + 1};
                        if (c.getRoomType(finalPos[0], finalPos[1]) != cell.rT.empty)
                        {
                            validInp = true;
                        }
                        break;
                }
            }
            return finalPos;
        }
        public static void itemMenu(player p)
        {
            displayItemList(p);
        }
        public static hero characterSelection(player p)
        {
            var str = "";

            Console.WriteLine($"[W]ARRIOR {"The highest rank given to any soldier from the Kingdom of the Two Peaks."} {(p.newPlayer? "High damage reduction and a spell that gives great sustainability through self healing and stacking armour increase." : str)}");
            if (p.achievements[0] == true)
            {
                Console.WriteLine($"[R]ACCOON {"Once a humble shopkeeper, now a valiant warrior."} {(p.newPlayer? "Low stats but high crit chance and damage. Has a spell with powerful healing for all party members." : str)}");
            }
            if (p.achievements[1] == true)
            {
                Console.WriteLine($"[G]HOST {"Long forgotten hero"} {(p.newPlayer ? "Insanely high damage and crit chance, low hp and no armour. Has a high chance to dodge enemy attacks. Does not have a spell." : str)}");
            }
            bool validInp = false;
            var unitChosen = new hero();
            while (!validInp)
            {
                string playerInput = Console.ReadLine().ToLower();
                switch (playerInput)
                {
                    case "w":
                        unitChosen = new warrior();
                        validInp = true;
                        break;
                    case "r":
                        unitChosen = new raccoonHero();
                        validInp = true;
                        break;
                }
            }
            return unitChosen;
        }
        public static void displayPlayerUnit(player p)
        {
            Console.WriteLine($"COINS: {p.coins}");
            Console.WriteLine($"KEYS: {p.keys}");
            displayParty(p);
        }
        public static void displayParty(player p)
        {
            foreach (var u in p.getPrty())
            {
                if (!u.checkDead())
                {
                    Console.WriteLine($"{u.name} the {u}");
                    Console.WriteLine($"HP: {u.hp}/{u.maxHp}  ARMOUR: {u.armr} \nDMG: {u.dmg}");
                    Console.WriteLine($"SPELL: {u.spell.roomCoolDown - u.spell.roomsTillUsable}/{u.spell.roomCoolDown}");
                    Console.WriteLine($"ITEM: {u.heldItem}");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine($"{u.name} the {u}");
                    Console.WriteLine($"dead...");
                    Console.WriteLine($"ITEM: {u.heldItem}");
                    Console.WriteLine();
                }
                
            }
        }
        public static void displayItemList(player p)
        {
            if(backpack.bckpck.Count != 0)
            {
                Console.WriteLine("Type E to equip any item.");
                try
                {
                    foreach (item i in backpack.bckpck)
                    {
                        Console.WriteLine($"{i}, {i.description(p.newPlayer)}");
                        var choice = Console.ReadLine().ToLower();
                        if (choice == "e")
                        {
                            equipItem(p, i);
                        }

                    }
                }
                catch { }
            }
        }
        public static void equipItem(player p, item i)
        {
            Console.WriteLine("Type E to equip the item on the hero.");
            var party = p.getPrty();
            foreach(var h in party)
            {
                Console.WriteLine($"{h.name} the {h}");
                var choice = Console.ReadLine().ToLower();
                if(choice == "e")
                {
                    if (i.context == backpack.triggerContext.onObtain)
                    {
                        i.doSmth(new unit[] { h });
                    }
                    else
                    {
                        if (!h.addHeldItem(i))
                        {
                            Console.WriteLine("this unit already has a held item. \nremove? y/n");
                            var c = Console.ReadLine().ToLower();
                            if (c == "y")
                            {
                                h.removeHeldItem();
                                
                            }      
                        }
                        h.addHeldItem(i);
                        backpack.removeItem(i.index);
                    }
                }
            }

        }
        public static void displayMap(player p, cellOverlord c)
        {
            Console.Clear();
            List<int[]> rs = p.getRS();
            List<int[]> ra = p.getRA();
            Console.WriteLine("MAP OF THE DUNGEON:");
            for (int i = 0; i < c.mapSize; i++)
            {
                for (int j = 0; j < c.mapSize; j++)
                {
                    var val = c.val(i, j);
                    int[] pos = new int[] { i, j };

                    bool cont = false;
                    foreach (var item in rs)
                    {
                        cont = item[0] == i && item[1] == j;
                    }

                    if (cont)
                    {
                        Console.Write(c.getHasPlayer(i, j) ? $"{{{val}}}" : $"[{val}]");
                    }
                    else
                    {
                        bool _cont = false;
                        foreach (var item in ra)
                        {
                            _cont = item[0] == i && item[1] == j;
                            if (_cont) { break; }
                        }
                        if (!_cont) { Console.Write(c.getRoomType(i, j) == cell.rT.empty ? "   " : "[ ]"); }
                        else { Console.Write(c.getRoomType(i,j) == cell.rT.empty ? "   " : $"[{val}]"); }
                    }             
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public static void userCombatInput(player p, List<enemy> enemies)
        {
            bool checkAllDead(List<enemy> enemies)
            {
                var alldead = true;
                foreach (var i in enemies)
                {
                    if (!i.checkDead())
                    {
                        alldead = false;
                    }
                }
                return alldead;
            }
            displayEnemies(enemies, p);

            foreach (var pU in p.getPrty())
            {
                
                if (!pU.checkDead() && !checkAllDead(enemies))
                {
                    Console.WriteLine("=============================================================");
                    Console.WriteLine($"{pU.name} the {pU}");
                    Console.WriteLine("[A]TTACK!");
                    Console.WriteLine($"[S]PELL ({pU.spell.roomCoolDown - pU.spell.roomsTillUsable}/{pU.spell.roomCoolDown})");
                    Console.WriteLine("[P]ARTY");
                    Console.WriteLine("=============================================================");
                    bool validInp = false;
                    while (!validInp)
                    {
                        string userInput = Console.ReadLine().ToLower();
                        switch (userInput)
                        {
                            case "a":
                                int index = combatEnemySelection(enemies);
                                combat.heroAttack(pU, enemies[index], p, enemies);
                                validInp = true; //the characters turn should only end once they either use their spell or attack.
                                break;
                            case "s":
                                validInp = pU.useSpell(p);
                                break;
                            case "p":
                                displayParty(p);
                                break;
                        }
                    }
                }
            }
        }

        private static void displayEnemies(List<enemy> enemies, player p)
        {
            foreach (var enemy in enemies)
            {
                if (!enemy.checkDead())
                {
                    Console.WriteLine($"{enemy}  ");
                    Console.WriteLine($"HP: {enemy.hp}/{enemy.maxHp}     ");
                    Console.WriteLine($"DMG: {enemy.dmg}       ");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine($"{enemy}---  ");
                    Console.WriteLine("dead...       ");
                    Console.WriteLine("-----------   ");
                    Console.WriteLine();
                }

            }
            Console.WriteLine();

            displayPlayerUnit(p);
        }

        private static int combatEnemySelection(List<enemy> enemies)
        {
            Console.WriteLine($"Select the enemy you wish to attack! (1-{enemies.Count})");
            bool validInp = false;
            int userInput = 1;
            while (!validInp)
            {
                bool validConversion = false;
                while (!validConversion)
                {
                    try
                    {
                        userInput = Convert.ToInt32(Console.ReadLine());
                        validConversion = true;
                    }
                    catch{}
                }
                
                userInput -= 1;
                if (userInput <= enemies.Count && userInput >= 0)
                {
                    validInp = true;
                    if (enemies[userInput].checkDead())
                    {
                        Console.WriteLine("thats just beating a dead horse, come on now.");
                        validInp = false;
                    }
                }
                else
                {
                    validInp = false;
                }
                
            }
            return userInput;
        }
        public static void guide(player p, savedPlayer pC)
        {
            Console.Clear();
            Console.WriteLine("-------WIKI-------");
            Console.WriteLine("|                |");
            Console.WriteLine("|    [E]nemies   |");
            Console.WriteLine("|    [B]osses    |");
            Console.WriteLine("|    [H]eroes    |");
            Console.WriteLine("|    [I]tems     |");
            Console.WriteLine("|                |");
            Console.WriteLine("|    [Q]uit      |");
            Console.WriteLine("|                |");
            Console.WriteLine("------------------");

            var choice = "";
            while (true)
            {
                choice = Console.ReadLine().ToLower();
                Console.Clear();
                switch (choice)
                {
                    case "e":
                        enemies();
                        Console.ReadLine();
                        guide(p, pC);
                        break;
                    case "i":
                        items();
                        Console.ReadLine();
                        guide(p, pC);
                        break;
                    case "b":
                        bosses();
                        Console.ReadLine();
                        guide(p, pC);
                        break;
                    case "h":
                        heroes();
                        Console.ReadLine();
                        guide(p, pC);
                        break;
                    case "q":
                        menu(p, pC);
                        break;
                }
            }
            void items()
            {
                Console.WriteLine("-Items-");
                Console.WriteLine("Epics");
                foreach (var e in backpack.epics)
                {
                    var i = backpack.items[e];
                    Console.WriteLine($"{i.name}\n{i.getDesc(p.newPlayer)}");
                    Console.WriteLine();
                }
                Console.WriteLine("Rares");
                foreach (var r in backpack.rares)
                {
                    var i = backpack.items[r];
                    Console.WriteLine($"{i.name}\n{i.getDesc(p.newPlayer)}");
                    Console.WriteLine();
                }
                Console.WriteLine("Commons");
                foreach (var c in backpack.commons)
                {
                    var i = backpack.items[c];
                    Console.WriteLine($"{i.name}\n{i.getDesc(p.newPlayer)}");
                }
            }

            void enemies()
            {
                Console.WriteLine("-Enemies-");
                var str = p.newPlayer ? "Small, wrinkly goblin with a makeshift shank." + "\nDoes more damage the more goblins there are." : "Small, wrinkly goblin with a makeshift shank.";
                Console.WriteLine($"Goblin \n{str}");
                str = p.newPlayer ? "Maybe this is your fate. Maybe this is you from a past life." + "\nHas no armour but a chance to dodge your attacks as your weapons phase through it." : "Maybe this is your fate. Maybe this is you from a past life.";
                Console.WriteLine($"Ghost \n{str}");
                str = p.newPlayer ? "A small brown rat." : "A small brown rat.";
                Console.WriteLine($"Rat \n{str}");
            }

            void bosses()
            {
                Console.WriteLine("-Bosses-");
                var str = p.newPlayer ? "Large mutated goblin with a shiny crown and gigantic sword." + "\nHas a chance to become enraged, swinging its sword multiple times dealing less damage each strike." : "Large mutated goblin with a shiny crown and gigantic sword.";
                Console.WriteLine($"Goblin King\n{str}");
                str = p.newPlayer ? "A group of four rats stuck to eachother by their tails." + "\nWhenever a hit deals more than 25% of their collective health, one rat will die. They can choose to remove a rat from the colony, increasing the damage of the others." : "A group of four rats stuck to eachother by their tails.";
                Console.WriteLine($"Rat King \n{str}");
                if (p.achievements[0] == true)
                {
                    str = p.newPlayer ? "This is your fault. You angered the shopkeeper" + "\nPowerful but unpredictable raccoon that can throw trinkets at you that either deal great area damage or heal itself." : "This is your fault. You angered the shopkeeper";
                    Console.WriteLine($"Raccoon \n{str}");
                }
            }

            void heroes()
            {
                Console.WriteLine("-Heroes-");
                var str = p.newPlayer ? "The highest rank given to any soldier from the Kingdom of the Two Peaks." + "\nHigh damage reduction and a spell that gives great sustainability through self healing and stacking armour increase." : "The highest rank given to any soldier from the Kingdom of the Two Peaks.";
                Console.WriteLine($"Warrior \n{str}");
                str = p.newPlayer ? "Long forgotten hero" + "\nInsanely high damage and crit chance, low hp and no armour. Has a high chance to dodge enemy attacks. Does not have a spell." : "Long forgotten hero";
                Console.WriteLine($"Ghost \n{str}");
                if (p.achievements[0] == true)
                {
                    str = p.newPlayer ? "Once a humble shopkeeper, now a valiant warrior." + "\nLow stats but high crit chance and damage. Has a spell with powerful healing for all party members." : "Once a humble shopkeeper, now a valiant warrior.";
                    Console.WriteLine($"Raccoon \n{str}");
                }
            }
        }
    }
}
