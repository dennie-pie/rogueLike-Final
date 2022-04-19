using System;
using System.Collections.Generic;
using System.Linq;

namespace fishlike
{
    abstract class unit
    {
        public int maxHp { get; set; }
        public int armr { get; set; }
        public int hp { get; set; }
        public int dmg { get; set; }
        public int critD { get; set; }
        public int critC { get; set; }
        public static string description { get; protected set; }
        public static string npDescription { get; protected set; }

        public virtual bool checkDead() { return hp <= 0 ? true : false; }
        public virtual int attack()
        {
            int newDmg = dmg;
            Random r = new Random();
            int crit = r.Next(0, 100);
            return crit <= critC ? dmg * critD : dmg;
        }
        public virtual int getAttacked(int dmg)
        {
            return dmg - (armr / 5);
        }
        public virtual void heal(int amnt) { }
    }
    class hero : unit
    {
        public item heldItem { get; set; }
        public Spell spell { get; protected set; }
        public string name { get; protected set; }
        public virtual bool useSpell(player p) { return false; }
        public virtual bool addHeldItem(item i)
        { 
            if(heldItem != null)
            {
                return false;
            }
            heldItem = i;
            return true;
        }
        public virtual void removeHeldItem() 
        {
            heldItem = null;
        }
        public class Spell
        {
            protected int _roomsSinceUse;
            protected int _roomCoolDown;

            public int roomsTillUsable { get { return _roomsSinceUse; } set { _roomsSinceUse = value; } }
            public int roomCoolDown { get { return _roomCoolDown; } set { _roomCoolDown = value; } }
            public virtual int[] doSmth() { return new int[] { }; }
            public virtual void incrementSpell() { roomsTillUsable = roomsTillUsable == 0 ? 0 : roomsTillUsable--; }
        }
    }
    class ghostHero : hero
    {
        public override string ToString()
        {
            return "Ghost";
        }
        public ghostHero()
        {
            Random r = new Random();
            string[] names = new string[] { "?!?", "???", "!!?" };
            name = names[r.Next(0, names.Length)];

            description = "Long forgotten hero";
            npDescription = "Insanely high damage and crit chance, low hp and no armour. Has a high chance to dodge enemy attacks. Does not have a spell.";
            maxHp = 50;
            hp = 50;
            dmg = 60;
            armr = 0; //each 5 armour is 1 dmg
            critC = 40;
            critD = 2;
            spell = new ghostSpell();
        }
        public override bool useSpell(player p)
        {
            Console.WriteLine("The ghost's voice echoes in the dark room.");
            return false;
        }
        public override void heal(int amount)
        {
            hp += amount;
            hp = hp > maxHp ? maxHp : hp;
        }

        public class ghostSpell : Spell
        {
            public ghostSpell()
            {
                roomCoolDown = 0;
                roomsTillUsable = 0;
            }
            public override int[] doSmth()
            {
                roomsTillUsable = roomCoolDown;
                return new int[] { 0, 0 };
            }
            public override void incrementSpell() { base.incrementSpell(); }
        }
        public override int getAttacked(int dmg)
        {
            var r = new Random();
            var dodgeChance = r.Next(0, 100);
            if (dodgeChance > 70)
            {
                dmg = 0;
            }
            return dmg;
        }

    }


class warrior : hero
    {

        public override string ToString()
        {
            return "Warrior";
        }
        public warrior()
        {
            Random r = new Random();
            string[] names = new string[] { "Davion", "Humphrey", "Steffan" };
            name = names[r.Next(0, names.Length)];

            description = "The highest rank given to any soldier from the Kingdom of the Two Peaks.";
            npDescription = "High damage reduction and a spell that gives great sustainability through self healing and stacking armour increase.";
            maxHp = 100;
            hp = 100;
            dmg = 30;
            armr = 60; //each 5 armour is 1 dmg
            critC = 20;
            critD = 2;
            spell = new warriorSpell();
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
                Console.WriteLine("The warrior exclaims, his voice echoing in the chamber.");
                int[] spellUse = spell.doSmth();
                armr = spellUse[0];
                heal(spellUse[1]);
                return true;
            }
        }
        public override void heal(int amount)
        {
            hp += amount;
            hp = hp > maxHp ? maxHp : hp;
        }

        public class warriorSpell : Spell
        {
            public warriorSpell()
            {
                roomCoolDown = 2;
                roomsTillUsable = 0;
            }
            public override int[] doSmth()
            {
                roomsTillUsable = roomCoolDown;
                return new int[] { 40, 50 };
            }
            public override void incrementSpell() { base.incrementSpell(); }
        }

    }

    class enemy : unit
    {
        protected int _rwrd;
        protected int _cost;

        public int cost { get { return _cost; } set { _cost = value; } }
        public int rwrd { get { return _rwrd; } set { _rwrd = value; } }
        public virtual void setStats() { }
        public virtual void setStats(int[] args) { }
        public virtual string makeChoice(player p) { return ""; }
        public virtual void ability(player p) { }
    }
    
    class ghost : enemy
    {
        public ghost()
        {
            description = "Maybe this is your fate. Maybe this is you from a past life.";
            npDescription = "Has no armour but a chance to dodge your attacks as your weapons phase through it.";
            cost = 5;
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
                decisions.Add($"{posinparty}", i.hp - dmg <= 0 ? 1000 : 100 * (1 - (i.hp / i.maxHp)));
                posinparty++;
            }

            List<string> myList = sortDict(decisions);
            return myList[0];
        }
        public override void setStats(int[] args)
        {
            int rC = args[0];
            maxHp = 50 + (5 * rC);
            hp = 50 + (5 * rC);
            dmg = 20 + (2 * rC);
            critC = 20;
            critD = 2;
            rwrd = 50 + (10 * rC);
        }
        public override string ToString()
        {
            return "Apparition";
        }
        public override int getAttacked(int dmg)
        {
            var r = new Random();
            var dodgeChance = r.Next(0, 100);
            if (dodgeChance > 50)
            {
                dmg = 0;
            }
            return dmg;
        }
    }
    class rat : enemy
    {
        public rat()
        {
            description = "A small brown rat.";
            cost = 1;
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

            List<string> myList = sortDict(decisions);
            return myList[0];
        }
        public override void setStats(int[] args)
        {
            int rC = args[0];
            maxHp = 3 + (2 * rC);
            hp = 3 + (2 * rC);
            dmg = 5 + (3 * rC);
            armr = 8 + (Convert.ToInt32(2 * rC)); //each 5 armour is 1 dmg
            critC = 20;
            critD = 2;
            rwrd = 15 + (5 * rC);
        }
        public override string ToString()
        {
            return "Gray Rat";
        }
    }

    class goblin : enemy
    {

        private int goblinCount;
        public goblin()
        {
            cost = 2;
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
            //decisions.Add("heal", 100 * (1-(hp/maxHp)));
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

            List<string> myList = sortDict(decisions);
            return myList[0];
        }
        public override void setStats(int[] args)
        {
            int rC = args[0];
            int gC = args[1];
            description = "Small, wrinkly goblin with a makeshift shank.";
            npDescription = "Does more damage the more goblins there are.";
            maxHp = 12 + (3 * rC);
            hp = 12 + (3 * rC);   
            dmg = 9 + (1 * rC);
            armr = 12 + (Convert.ToInt32(3 * rC)); //each 5 armour is 1 dmg
            critC = 50;
            critD = 1;
            rwrd = 30 + (5 * rC);
            goblinCount = gC;
        }
    
        public override string ToString()
        {
            return "Goblin";
        }

        public override int attack()
        {
            int newDmg = dmg + (goblinCount);
            Random r = new Random();
            int crit = r.Next(0, 100);
            return crit <= critC ? dmg * critD : dmg;
        }
    }
    class ratKing : enemy
    {
        private int ratCount;
        public ratKing()
        {
            ratCount = 4;
            cost = 10;
        }
        public override string makeChoice(player p)
        {
            var r = new Random();
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
            var tfPercent = maxHp * 0.25;
            decisions.Add("ability", (r.Next(0, 3) == 2) && ratCount != 1 && (hp - Convert.ToInt32(tfPercent)) > 0 ? 10000 : 0);
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

            List<string> myList = sortDict(decisions);
            return myList[0];
        }
        public override void ability(player p)
        {
            Console.WriteLine("The group parts way with one of their members.");
            var r = new Random();
            var tfpercent = maxHp * 0.25;
            hp -= Convert.ToInt32(tfpercent);
            ratCount -= 1;
            dmg += 30;
        }
        public override void setStats(int[] args)
        {
            int rC = args[0];
            description = "A group of four rats stuck to eachother by their tails.";
            npDescription = "Whenever a hit deals more than 25% of their collective health, one rat will die. They can choose to remove a rat from the colony, increasing the damage of the others.";
            maxHp = 50 + (10 * rC);
            hp = 50 + (10 * rC);
            dmg = 10 + (1 * rC);
            armr = 50 + (Convert.ToInt32(2 * rC)); //each 5 armour is 1 dmg
            critC = 10;
            critD = 2;
            rwrd = 100 + (10 * rC); 
        }
        public override int attack()
        {
            var totalDmg = 0;
            for(int i = 0; i < ratCount; i++)
            {
                totalDmg += dmg;
            }
            return totalDmg;
        }
        public override int getAttacked(int dmg)
        {
            var tfPercent = maxHp * 0.25;
            if (tfPercent < dmg)
            {
                ratCount--;
            }
            if (ratCount == 0)
            {
                hp = 0;
            }
            return dmg - (armr / 5);
        }
        public override string ToString()
        {
            return "Rat King";
        }
    }


    class goblinKing : enemy
    {
        public goblinKing()
        {
            cost = 20;
        }
        public override string makeChoice(player p)
        {
            var r = new Random();
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
            decisions.Add("ability", r.Next(0,3) == 2? 10000 : 0);
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

            List<string> myList = sortDict(decisions);
            return myList[0];
        }
        public override void ability(player p)
        {
            Console.WriteLine("The King becomes enraged, swinging his sword wildly.");
            var r = new Random();
            var swings = r.Next(2, 5);
            for (var i = 0; i < swings; i++)
            {
                var select = r.Next(0, p.getPrty().Count);
                combat.attack("The Goblin King", dmg / 2, p.getPrty()[select]);
            }

        }
        public override void setStats(int[] args)
        {
            int rC = args[0];
            description = "Large mutated goblin with a shiny crown and gigantic sword.";
            npDescription = "Has a chance to become enraged, swinging its sword multiple times dealing less damage each strike.";
            maxHp = 150 + (10 * rC);
            hp = 150 + (10 * rC);
            dmg = 25 + (1 * rC);
            armr = 60 + (Convert.ToInt32(2 * rC)); //each 5 armour is 1 dmg
            critC = 10;
            critD = 2;
            rwrd = 100 + (10 * rC);
        }
        public override string ToString()
        {
            return "The Goblin King";
        }
    }

    static class combat
    {
        public static void heroAttack(hero attacker, unit attacked, player p, List<enemy> enemies)
        {
            var party = p.getPrty();
            onHit(attacker, attacked, party, enemies);
            int dmg = attacker.attack();
            dmg = attacked.getAttacked(dmg);
            if (dmg <= 0)
            {
                Console.WriteLine("Hit blocked!");
                attacked.hp -= 1;
                Console.WriteLine($"{attacked.hp}/{attacked.maxHp}");
            }
            else
            {
                attacked.hp -= dmg;
                attacked.checkDead();
                Console.WriteLine($"{attacker} attacks {attacked}!");
                Console.WriteLine($"Dealing {dmg} damage!");
                if (attacked.checkDead())
                {
                    try
                    {
                        var a = (enemy)attacked;
                        p.coins += a.rwrd;
                    }
                    finally
                    {
                        Console.WriteLine($"{attacked} DIES!!!");
                    }
                }
                else { Console.WriteLine($"{attacked.hp}/{attacked.maxHp}"); }
            }
            Console.ReadLine();
        }
        public static void attack(unit attacker, unit attacked)
        {
            int dmg = attacker.attack();
            dmg = attacked.getAttacked(dmg);
            if (dmg <= 0)
            {
                Console.WriteLine("Hit blocked!");
                attacked.hp -= 1;
                Console.WriteLine($"{attacked.hp}/{attacked.maxHp}");
            }
            else
            {
                attacked.hp -= dmg;
                attacked.checkDead();
                Console.WriteLine($"{attacker} attacks {attacked}!");
                Console.WriteLine($"Dealing {dmg} damage!");
                if (attacked.checkDead()) { Console.WriteLine($"{attacked} DIES!!!"); }
                else { Console.WriteLine($"{attacked.hp}/{attacked.maxHp}"); }
            }
            Console.ReadLine();
        }
        public static void attack(string name, int damage, unit attacked)
        {
            int dmg = attacked.getAttacked(damage);
            if (dmg <= 0)
            {
                Console.WriteLine("Hit blocked!");
                attacked.hp -= 1;
                Console.WriteLine($"{attacked.hp}/{attacked.maxHp}");
            }
            else
            {
                attacked.hp -= dmg;
                attacked.checkDead();
                Console.WriteLine($"{name} attacks {attacked}!");
                Console.WriteLine($"Dealing {dmg} damage!");
                if (attacked.checkDead()) { Console.WriteLine($"{attacked} DIES!!!"); }
                else { Console.WriteLine($"{attacked.hp}/{attacked.maxHp}"); }
            }
            Console.ReadLine();
        }

        public static void onHit(hero h, unit enemy, List<hero> party, List<enemy> enemies)
        {
            if (h.heldItem != null)
            {
                if(h.heldItem.context == backpack.triggerContext.onHit)
                {
                    switch (h.heldItem.afctdUnits)
                    {
                        case backpack.affectedUnits.self:
                            h.heldItem.doSmth(new unit[] { h });
                            break;
                        case backpack.affectedUnits.allies:
                            h.heldItem.doSmth(party.ToArray());
                            break;
                        case backpack.affectedUnits.enemies:
                            h.heldItem.doSmth(enemies.ToArray());
                            break;
                        case backpack.affectedUnits.enemy:
                            h.heldItem.doSmth(new unit[] { h, enemy });
                            break;
                    }
                }
            }
        }
        public static void startOfCombat(List<hero> party, List<enemy> enemies)
        {
            foreach (var h in party)
            {
                if (h.heldItem != null && !h.checkDead())
                {
                    if(h.heldItem.context == backpack.triggerContext.startOfCombat)
                    {
                        switch (h.heldItem.afctdUnits)
                        {
                            case backpack.affectedUnits.self:
                                h.heldItem.doSmth(new unit[] { h });
                                break;
                            case backpack.affectedUnits.allies:
                                h.heldItem.doSmth(party.ToArray());
                                break;
                            case backpack.affectedUnits.enemies:
                                h.heldItem.doSmth(enemies.ToArray());
                                break;
                        }
                    }
                }
            }
        }
        public static void endOfCombat(List<hero> party, List<enemy> enemies)
        {
            foreach (var h in party)
            {
                if (h.heldItem != null && !h.checkDead())
                {
                    if (h.heldItem.context == backpack.triggerContext.endOfCombat)
                    {
                        switch (h.heldItem.afctdUnits)
                        {
                            case backpack.affectedUnits.self:
                                h.heldItem.doSmth(new unit[] { h });
                                break;
                            case backpack.affectedUnits.allies:
                                h.heldItem.doSmth(party.ToArray());
                                break;
                            case backpack.affectedUnits.enemies:
                                h.heldItem.doSmth(enemies.ToArray());
                                break;
                        }
                    }
                }
            }
        }
        public static void startOfRoom(List<hero> party, cell room)
        {
            foreach (var h in party)
            {
                if (h.heldItem != null)
                {
                    if (h.heldItem.context == backpack.triggerContext.startOfRoom)
                    {
                        switch (h.heldItem.afctdUnits)
                        {
                            case backpack.affectedUnits.self:
                                h.heldItem.doSmth(new unit[] { h }, room);
                                break;
                            case backpack.affectedUnits.allies:
                                h.heldItem.doSmth(party.ToArray(), room);
                                break;
                        }
                    }
                }
            }
        }
        public static void endOfRoom(List<hero> party, cell room)
        {
            foreach (var h in party)
            {
                h.spell.incrementSpell();
                if (h.heldItem != null)
                {
                    if (h.heldItem.context == backpack.triggerContext.startOfRoom)
                    {
                        switch (h.heldItem.afctdUnits)
                        {
                            case backpack.affectedUnits.self:
                                h.heldItem.doSmth(new unit[] { h }, room);
                                break;
                            case backpack.affectedUnits.allies:
                                h.heldItem.doSmth(party.ToArray(), room);
                                break;
                        }
                    }
                }
            }
        }
    }
}
