using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace fishlike
{
    class savedPlayer
    {
        [JsonProperty("difficulty")]
        public string difficulty { get; set; }
        [JsonProperty("newPlayer")]
        public bool newPlayer { get; set; }
        [JsonProperty("achievements")]
        public bool[] achievements { get; set; }
        [JsonProperty("olympianCoins")]
        public int olympianCoins { get; set; }
    }
    static class jsonInteraction
    {
        public static savedPlayer resetConfig()
        {
            var pC = new savedPlayer();
            pC.achievements = new bool[] { false, false };
            pC.difficulty = "medm";
            pC.newPlayer = true;
            pC.olympianCoins = 0;

            saveConfig(pC);
            return pC;
        }
        public static savedPlayer loadConfig()
        {
            try
            {
                string jsonString = File.ReadAllText("config.json");
                var json = JsonConvert.DeserializeObject<savedPlayer>(jsonString);
                return json;
            }
            catch
            {
                return resetConfig();
            }
        }
        public static void saveConfig(savedPlayer pConfig)
        {
            var jsonString = JsonConvert.SerializeObject(pConfig, Formatting.Indented);
            File.WriteAllText("config.json", jsonString);
        }
        public static void match(player p, savedPlayer playerConfig)
        {
            p.achievements = playerConfig.achievements;
            if (playerConfig.difficulty == "easy") { p.difficulty = player.gameDifficulty.easy; }
            if (playerConfig.difficulty == "medm") { p.difficulty = player.gameDifficulty.medm; }
            if (playerConfig.difficulty == "hard") { p.difficulty = player.gameDifficulty.hard; }
            p.difficultyStr = playerConfig.difficulty;
            p.olympianCoins = playerConfig.olympianCoins;
            p.newPlayer = playerConfig.newPlayer;
        }
    }

}
