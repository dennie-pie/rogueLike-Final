using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace fishlike
{
    static class raccoon
    {
        public static int playerEncounters; //the first encounter with the raccoon the player is prompted to attack it, chosing so will trigger a boss fight, if won the player gains the raccoon character in their party. if lost the player dies.

        public static void resetRaccoon()
        {
            playerEncounters = 0;
        }

        public static void firstEncounter(player p)
        {
            Console.Clear();
            Console.WriteLine("You feel a presence in the room watching you.");
            Console.WriteLine("Just as you step in, you are greeted by a small raccoon wearing a white tunic and leather body armour.");
            Console.WriteLine("You raise your weapons at the beast expecting another test of strenght.");
            Console.WriteLine("Instead of surrendering the animal grumbles under its breath and points at a large glowing pouch resting on a stool not too far away.");
            Console.WriteLine("WHAT WILL YOU DO? (do you really want to attack it? it seems friendly) y/n");
            var valid = false;
            while (!valid)
            {
                var userchoice = Console.ReadLine().ToLower();
                switch (userchoice)
                {
                    case "y":
                        COMBAT(p);
                        valid = true;
                        break;
                    case "n":
                        shopFunc(p, 1);
                        valid = true;
                        break;
                }
            }
            playerEncounters++;
        }

        public static void shopFunc(player p, int loopNo)
        {
            var r = new Random();
            var itemsForSale = new int[3];
            var costs = new int[3];
            for (int itemAmount = 0; itemAmount < 3; itemAmount++)
            {
                int item = 0;
                int cost = 0;
                if (loopNo == 1)
                {
                    int chance = r.Next(0, 100);
                    if (chance < 5)
                    {
                        item = backpack.epics[r.Next(0, backpack.epics.Length)];
                        cost = 500 + r.Next(0, 100);
                    }
                    else if (chance < 25)
                    {
                        item = backpack.rares[r.Next(0, backpack.rares.Length)];
                        cost = 200 + r.Next(0, 80);
                    }
                    else
                    {
                        item = backpack.commons[r.Next(0, backpack.commons.Length)];
                        cost = 100 + r.Next(0, 50);
                    }
                }
                else if (loopNo < 3)
                {
                    int chance = r.Next(0, 100);
                    if (chance < 10)
                    {
                        item = backpack.epics[r.Next(0, backpack.epics.Length)];
                        cost = 700 + r.Next(0, 200);
                    }
                    else if (chance < 40)
                    {
                        item = backpack.rares[r.Next(0, backpack.rares.Length)];
                        cost = 500 + r.Next(0, 100);
                    }
                    else
                    {
                        item = backpack.commons[r.Next(0, backpack.commons.Length)];
                        cost = 200 + r.Next(0, 60);
                    }
                }
                else
                {
                    int chance = r.Next(0, 100);
                    if (chance < 15)
                    {
                        item = backpack.epics[r.Next(0, backpack.epics.Length)];
                        cost = 1000 + r.Next(100 * loopNo, 200 * loopNo);
                    }
                    else if (chance < 55)
                    {
                        item = backpack.rares[r.Next(0, backpack.rares.Length)];
                        cost = 800 + r.Next(50 * loopNo, 100 * loopNo);
                    }
                    else
                    {
                        item = backpack.commons[r.Next(0, backpack.commons.Length)];
                        cost = 500 + r.Next(50 * loopNo, 100 * loopNo);
                    }
                }
                itemsForSale[itemAmount] = item;
                costs[itemAmount] = cost;
            }
            Console.WriteLine($"You have {p.coins} gold coins.");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"{backpack.items[itemsForSale[i]].name} for {costs[i]} gold coins");
            }
            Console.WriteLine("Do you wish to buy any items from the furry businessman? y/n");
            var yesno = Console.ReadLine().ToLower();
            if (yesno == "y")
            {
                var itemChoice = "";
                while (itemChoice != "q")
                {
                    Console.WriteLine("Which item would you like to buy? 1, 2, 3 ([q]uit)?");
                    itemChoice = "";
                    var valid = false;
                    while (!valid)
                    {
                        itemChoice = Console.ReadLine().ToLower();
                        valid = gameMenu.check(new string[] { "1", "2", "3", "q" }, itemChoice);
                    }
                    if (itemChoice != "q")
                    {
                        var itemChoiceInt = Convert.ToInt32(itemChoice);
                        if (p.coins - costs[itemChoiceInt - 1] >= 0)
                        {
                            backpack.addItem(backpack.items[itemsForSale[itemChoiceInt-1]].index);
                            itemsForSale[itemChoiceInt - 1] = 0;
                        }
                        else { Console.WriteLine("You do not have enough money."); }
                    }

                }
                
            }
        }

        private static void COMBAT(player p)
        {
            raccoonBoss r = new raccoonBoss();
            List<enemy> enemies = new List<enemy>();
            enemies.Add(r);

            combat.startOfCombat(p.getPrty(), enemies);
            while (!p.allDead())
            {
                while (!r.checkDead())
                {
                    Console.Clear();
                    Console.WriteLine("WHAT WILL YOU DO?");
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
                            Console.ReadLine();
                        }
                    }
                }
                Console.WriteLine("The raccoon falls to the floor, defeated.");
                Console.WriteLine("Right as you're about to move on you are showered by holy light. You turn to see the source to be the raccoon who managed to climb into the sack.");
                Console.WriteLine("Soon the raccoon emerges and charges at you. You dont have time to raise your sword. The beast ferociously lunges onto you.");
                Console.WriteLine("It hugs you. Pleads to join the party, tells you it has never seen any warrior as strong as you before. You oblige.");
                p.achievements[0] = true;
                p.addHero(new raccoonHero());
                Console.ReadLine();
                return;
            }
            
            combat.endOfCombat(p.getPrty(), enemies);
        }
    }
    class raccoonHero : hero
    {
        public raccoonHero()
        {
            name = "Tia";
            hp = 80;
            maxHp = 80;
            dmg = 50;
            armr = 30;
            critC = 50;
            critD = 4;
            description = "Once a humble shopkeeper, now a valiant warrior.";
            npDescription = "Low stats but high crit chance and damage. Has a spell with powerful healing for all party members.";
            spell = new raccoonSpell();
        }
        public override string ToString()
        {
            return "Raccoon";
        }
        public override bool useSpell(player p)
        {
            if (spell.roomsTillUsable != 0)
            {
                Console.WriteLine("spell on cooldown.");
                return false;
            }
            else
            {
                Console.WriteLine("The raccoon reaches into the sack it carries around with it pulling out a powerful trinket.");
                int[] spellUse = spell.doSmth();
                p.coins += spellUse[1];
                if (spellUse[0] == 0)
                {
                    foreach(var u in p.getPrty())
                    {
                        int amnt = u.hp / 5;
                        u.heal(amnt);
                        Console.WriteLine($"{u.name} healed for {amnt}. {u.hp}/{u.maxHp}");
                    }
                }
                return true;
            }
        }
        public override int attack()
        {
            Random r = new Random();
            int crit = r.Next(0, 100);
            return crit <= critC ? dmg * critD : dmg;
        }
        public override int getAttacked(int dmg)
        {
            return dmg - (armr / 5);
        }
        public class raccoonSpell : Spell
        {
            public raccoonSpell()
            {
                roomCoolDown = 5;
                roomsTillUsable = 0;
            }
            public override int[] doSmth()
            {
                roomsTillUsable = roomCoolDown;
                Random r = new Random();
                int chance = r.Next(0, 2);
                return new int[] { chance, 200 };
            }
            public override void incrementSpell() { base.incrementSpell(); }
        }
    }
    class raccoonBoss : enemy
    {
        public raccoonBoss()
        {
            description = "This is your fault. You angered the shopkeeper";
            npDescription = "Powerful but unpredictable raccoon that can throw trinkets at you that either deal great area damage or heal itself.";
            maxHp = 100;
            hp = 100;
            dmg = 40;
            armr = 30;
            critC = 50;
            critD = 1;
        }
        public override string makeChoice(player p)
        {
            List<string> sortDict(Dictionary<string, int> dict)
            {
                List<string> tempList = new List<string>();
                foreach (KeyValuePair<string, int> kvp in dict.OrderByDescending(key => key.Value))
                {
                    tempList.Add(kvp.Key);
                }
                return tempList;
            }
            Dictionary<string, int> decisions = new Dictionary<string, int>();
            int posinparty = 0;
            foreach (var i in p.getPrty())
            {
                if (!i.checkDead())
                {
                    decisions.Add($"{posinparty}", i.hp - dmg <= 0 ? 1000 : 100 * (1 - (i.hp / i.maxHp)));
                    posinparty++;
                }
                else
                {
                    decisions.Add($"{posinparty}", 0);
                    posinparty++;
                }
            }
            if (hp == (maxHp / 10))
            {
                decisions.Add("ability", 99999);
            }

            List<string> myList = sortDict(decisions);
            return myList[0];
        }
        public override void heal(int amount)
        {
            hp += amount;
            hp = hp > maxHp ? maxHp : hp;
        }

        public override void ability(player p)
        {
            Random r = new Random();
            var chance = r.Next(0, 5);
            if (chance < 3)
            {
                Console.WriteLine("The raccoon pulls out a strange looking fruit out of its bag and eats it.");
                Console.WriteLine("The raccoon is healed back to full hp.");
                heal(maxHp);
            }
            else
            {
                Console.WriteLine("The raccoon pulls out a battle axe from its bag. It seems physically impossible for the animal to be supporting the weight of the weapon but it throws the axe right at you.");
                foreach (var unit in p.getPrty())
                {
                    combat.attack("The Raccoon", unit.maxHp / 2, unit);
                }
            }
        }
        public override int attack()
        {
            return dmg;
        }
        public override int getAttacked(int dmg)
        {
            return dmg - (armr / 5);
        }
        public override string ToString()
        {
            return "Raccoon";
        }
    }

}
