using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace fishlike
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)//this makes it so the player is reset every time the game ends, the items that carry over, do. those that dont, dont.
            {
                var playerConfig = jsonInteraction.loadConfig();
                backpack.setShells();

                player p = new player();
                jsonInteraction.match(p, playerConfig);

                gameMenu.menu(p, playerConfig);
            }
        }
    }
}




