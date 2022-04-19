using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fishlike
{
    static class backpack
    {
        public enum rarities
        {
            common,
            rare,
            epic
        }
        public enum triggerContext
        {
            startOfCombat,
            endOfCombat,
            startOfRoom,
            endOfRoom,
            onHit,
            onObtain
        }
        public enum affectedUnits
        {
            self,
            allies,
            enemies,
            enemy
        }

        public static List<item> bckpck;
        public static itemShell[] items;
        public static int[] commons, rares, epics;
        public static void setShells()//these are only for easy referral. this may be confusing for readers but makes complete sense to me :)
        {
            commons = new int[] { 3, 5, 6 };
            rares = new int[] { 2, 4 };
            epics = new int[] { 1 };
            var nullitem = new nullitem();
            var a = new AbyssalHeartShell();
            var m = new MjollnirShell();
            var f = new FungalGrowthShell();
            var i = new featherShell();
            var d = new hammerShell();
            var e = new echoShell();
            var p = new plateShell();
            items = new itemShell[] { nullitem, a, m, f, i, d, e, p };
        }
        public static void generateBackpack()
        {
            bckpck = new List<item>();
        }
        public static void addItem(int index)
        {
            items[index].addItem();
        }
        public static void removeItem(int index)
        {
            items[index].removeItem();
        }
    }

    class nullitem : itemShell
    {
        public nullitem()
        {
            name = "sold!";
            index = 0;
        }
        public override void addItem()
        {
            Console.WriteLine("this item has already been sold.");
        }
        public override string getDesc(bool np)
        {
            throw new NotImplementedException();
        }
        public override void removeItem()
        {
            throw new NotImplementedException();
        }
    }
    abstract class itemShell
    {
        public int quantity { get; set; }
        public int index { get; set; }
        public string name { get; set; }
        public backpack.rarities rarity { get; protected set; }
        public abstract void addItem();
        public abstract void removeItem();
        public abstract string getDesc(bool np);
    }
    abstract class item
    {
        public backpack.affectedUnits afctdUnits { get; protected set; }
        public backpack.triggerContext context { get; protected set; }
        public int index { get; protected set; }
        public abstract void doSmth(unit[] us); //some items will affect many enemies or many allies, some items will only affect one unit.
        public abstract void doSmth(unit[] us, cell room); //some items will have effects depending on the roomtype
        public abstract string description(bool np);
    }


    class AbyssalHeartShell : itemShell
    {
        public AbyssalHeartShell()
        {
            quantity = 0;
            index = 1;
            rarity = backpack.rarities.epic;
            name = "Abyssal Heart";
        }
        public override void addItem()
        {
            quantity++;
            backpack.bckpck.Add(new AbyssalHeart());
        }
        public override void removeItem()
        {
            try
            {
                backpack.bckpck.Remove(backpack.bckpck.Single(x => x.ToString() == "Abyssal Heart"));
                quantity--;
            }
            catch { }
        }
        public override string getDesc(bool np)
        {
            var a = new AbyssalHeart();
            return a.description(np);
        }
    }
    class AbyssalHeart : item
    {
        public AbyssalHeart()
        {
            index = 1;
            context = backpack.triggerContext.endOfCombat;
            afctdUnits = backpack.affectedUnits.self;
        }
        public override void doSmth(unit[] us) //end of each combat encounter heals the holder for 10% of their max hp
        {
            var h = (hero)us[0];
            int amnt = h.maxHp / 5;
            h.heal(amnt);
            Console.WriteLine($"healed for {amnt}, {h.hp}/{h.maxHp}");
        }
        public override string ToString()
        {
            return "Abyssal Heart";
        }
        public override void doSmth(unit[] us, cell room) { Console.WriteLine("uhoh"); } //this item does not take the cell as input
        public override string description(bool np)
        {
            return np ? "Heart of an extinct monster. Even though the owner is long dead, it still beats. \nHeals the holder for 20% of their HP every time they exit combat (End of an enemy room encounter)." : "Heart of an extinct monster. Even though the owner is long dead, it still beats.";
        }
    }


    class MjollnirShell : itemShell
    {
        public MjollnirShell()
        {
            index = 2;
            quantity = 0;
            rarity = backpack.rarities.rare;
            name = "Mjollnir";
        }
        public override void addItem()
        {
            quantity++;
            backpack.bckpck.Add(new Mjollnir());
        }
        public override void removeItem()
        {
            try
            {
                backpack.bckpck.Remove(backpack.bckpck.Single(x => x.ToString() == "Mjollnir"));
                quantity--;
            }
            catch { }
        }
        public override string getDesc(bool np)
        {
            var a = new Mjollnir();
            return a.description(np);
        }
    }
    class Mjollnir : item
    {
        public Mjollnir()
        {
            index = 2;
            context = backpack.triggerContext.onHit;
            afctdUnits = backpack.affectedUnits.enemies;
        }
        public override void doSmth(unit[] us)
        {
            Random r = new Random();
            int chance = r.Next(0, 10);
            if (chance > 2) { return; }
            for (int i = 0; i < 3; i++)
            {
                int indexChoice = r.Next(0, us.Length);
                var u = us[indexChoice];
                u.hp -= 30;
                Console.WriteLine($"{u} struck by lightning {u.hp}/{u.maxHp}");
            }
        }
        public override string ToString()
        {
            return "Mjollnir";
        }
        public override string description(bool newplayer)
        {
            return newplayer ? "Hammer of the Thunder God Thor. Imbued with a fragment of his powers. \n20% chance on hitting an enemy to deal 30 damage to 3 random enemies." : "Hammer of the Thunder God Thor. Imbued with a fragment of his powers.";
        }
        public override void doSmth(unit[] us, cell room) { Console.WriteLine("uhoh"); } //this item does not take the cell as input
    }


    class FungalGrowthShell : itemShell
    {
        public FungalGrowthShell()
        {
            index = 3;
            quantity = 0;
            rarity = backpack.rarities.common;
            name = "Fungal Growth";
        }
        public override void addItem()
        {
            quantity++;
            backpack.bckpck.Add(new FungalGrowth());
        }
        public override void removeItem()
        {
            try
            {
                backpack.bckpck.Remove(backpack.bckpck.Single(x => x.ToString() == "Fungal Growth"));
                quantity--;
            }
            catch { }
        }
        public override string getDesc(bool np)
        {
            var a = new FungalGrowth();
            return a.description(np);
        }
    }
    class FungalGrowth : item
    {
        public FungalGrowth()
        {
            index = 3;
            context = backpack.triggerContext.onObtain;
            afctdUnits = backpack.affectedUnits.self;
        }
        public override void doSmth(unit[] us)
        {
            var h = (hero)us[0];
            h.maxHp = Convert.ToInt32(h.maxHp * 1.1);
            h.hp = Convert.ToInt32(h.maxHp * 1.1);
        }
        public override void doSmth(unit[] us, cell room) { Console.WriteLine("uhoh"); } //this item does not take the cell as input
        public override string ToString()
        {
            return "Fungal Growth";
        }
        public override string description(bool np)
        {
            return np ? "The damp environment of a dungeon is perfect for the growth of various fungi, this one seems interested in you. \nIncreases the holders maximum HP by 25%." : "The damp environment of a dungeon is perfect for the growth of various fungi, this one seems interested in you.";
        }
    }


    class featherShell : itemShell
    {
        public featherShell()
        {
            index = 4;
            quantity = 0;
            rarity = backpack.rarities.rare;
            name = "Feather of Icarus";
        }
        public override void addItem()
        {
            quantity++;
            backpack.bckpck.Add(new featherOfIcarus());
        }
        public override void removeItem()
        {
            try
            {
                backpack.bckpck.Remove(backpack.bckpck.Single(x => x.ToString() == "Feather of Icarus"));
                quantity--;
            }
            catch { }
        }
        public override string getDesc(bool np)
        {
            var a = new featherOfIcarus();
            return a.description(np);
        }
    }
    class featherOfIcarus : item
    {
        public featherOfIcarus()
        {
            index = 4;
            context = backpack.triggerContext.onObtain;
            afctdUnits = backpack.affectedUnits.self;
        }
        public override void doSmth(unit[] us)
        {
            var h = (hero)us[0];
            h.dmg = Convert.ToInt32(h.dmg * 2);
            h.hp -= Convert.ToInt32(h.maxHp * 0.5);
        }
        public override void doSmth(unit[] us, cell room) { Console.WriteLine("uhoh"); } //this item does not take the cell as input
        public override string description(bool np)
        {
            return !np ? "Reward for your hubris." : "Reward for your hubris. \nDoubles the players damage while halving their hp.";
        }
        public override string ToString()
        {
            return "Feather of Icarus";
        }
    }


    class hammerShell : itemShell
    {
        public hammerShell()
        {
            index = 5;
            quantity = 0;
            rarity = backpack.rarities.common;
            name = "Hammer of Daedalus";
        }
        public override void addItem()
        {
            quantity++;
            backpack.bckpck.Add(new hammerOfDaedalus());
        }
        public override void removeItem()
        {
            try
            {
                backpack.bckpck.Remove(backpack.bckpck.Single(x => x.ToString() == "Hammer of Daedalus"));
                quantity--;
            }
            catch { }
        }
        public override string getDesc(bool np)
        {
            var a = new hammerOfDaedalus();
            return a.description(np);
        }
    }
    class hammerOfDaedalus : item
    {
        public hammerOfDaedalus()
        {
            index = 5;
            context = backpack.triggerContext.onObtain;
            afctdUnits = backpack.affectedUnits.self;
        }
        public override void doSmth(unit[] us)
        {
            var h = (hero)us[0];
            h.dmg = Convert.ToInt32(h.dmg * 1.25);
        }
        public override void doSmth(unit[] us, cell room) { Console.WriteLine("uhoh"); } //this item does not take the cell as input
        public override string description(bool np)
        {
            return !np ? "One of many that will give any weapon immense power." : "One of many that will give any weapon immense power. \nIncreases damage and armour.";
        }
        public override string ToString()
        {
            return "Hammer of Daedalus";
        }

    }


    class echoShell : itemShell
    {
        public echoShell()
        {
            index = 6;
            quantity = 0;
            rarity = backpack.rarities.common;
            name = "Echo Blade";
        }
        public override void addItem()
        {
            quantity++;
            backpack.bckpck.Add(new echoBlade());
        }
        public override void removeItem()
        {
            try
            {
                backpack.bckpck.Remove(backpack.bckpck.Single(x => x.ToString() == "Echo Blade"));
                quantity--;
            }
            catch { }
        }
        public override string getDesc(bool np)
        {
            var a = new echoBlade();
            return a.description(np);
        }
    }
    class echoBlade : item 
    { 
        public echoBlade()
        {
            index = 6;
            context = backpack.triggerContext.onHit;
            afctdUnits = backpack.affectedUnits.enemy;
        }
        public override void doSmth(unit[] us)
        {
            var h = (hero)us[0];
            var e = (enemy)us[1];
            var dmg = 0;
            var r = new Random();

            if (r.Next(0,100) > 70)
            {
                dmg = h.attack();
                dmg = e.getAttacked(dmg);
                e.hp -= dmg;
                Console.WriteLine("Damage Echoed.");
            }
        }
        public override void doSmth(unit[] us, cell room)
        {
            throw new NotImplementedException();
        }
        public override string description(bool np)
        {
            return !np ? "A swift blade forged in the deepest caverns of the mount olympus.": "A swift blade forged in the deepest caverns of the mount olympus. \nHas a chance to repeat damage dealt.";
        }
        public override string ToString()
        {
            return "Echo Blade";
        }
    }


    class plateShell : itemShell
    {
        public plateShell()
        {
            index = 7;
            quantity = 0;
            rarity = backpack.rarities.common;
            name = "Plate Mail";
        }
        public override void addItem()
        {
            quantity++;
            backpack.bckpck.Add(new plateMail());
        }
        public override void removeItem()
        {
            try
            {
                backpack.bckpck.Remove(backpack.bckpck.Single(x => x.ToString() == "Plate Mail"));
                quantity--;
            }
            catch { }
        }
        public override string getDesc(bool np)
        {
            var a = new plateMail();
            return a.description(np);
        }
    }
    class plateMail : item
    {
        public plateMail()
        {
            index = 7;
            context = backpack.triggerContext.onObtain;
            afctdUnits = backpack.affectedUnits.self;
        }
        public override void doSmth(unit[] us)
        {
            var h = (hero)us[0];
            h.armr += 50;
        }
        public override void doSmth(unit[] us, cell room)
        {
            throw new NotImplementedException();
        }
        public override string description(bool np)
        {
            return !np ? "Second hand but still pretty sturdy." : "Second hand but still pretty sturdy. \nIncreases armour.";
        }
        public override string ToString()
        {
            return "Plate Mail";
        }
    }
}
