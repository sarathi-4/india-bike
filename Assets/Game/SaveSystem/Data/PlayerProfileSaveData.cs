using System;
using System.Collections.Generic;

namespace IndianBikeLife.Core.SaveSystem.Data
{
    /// <summary>
    /// Player game progression, unlocked motorcycles, profile stats, and customization choices.
    /// Structured to be extensible for future showroom, missions, and player profile stages.
    /// Never stores authentication secrets or passwords.
    /// </summary>
    [Serializable]
    public class PlayerProfileSaveData
    {
        public string playerId;
        public string playerName;
        public int playerLevel;
        public int playerCoins;
        public List<string> unlockedMotorcycleIds;
        public string selectedMotorcycleId;
        public string characterGender;
        public float totalDistanceDrivenKm;
        public List<string> completedMissionIds;

        public static PlayerProfileSaveData CreateDefault()
        {
            return new PlayerProfileSaveData
            {
                playerId = Guid.NewGuid().ToString("N"),
                playerName = "Indian Rider",
                playerLevel = 1,
                playerCoins = 1000,
                unlockedMotorcycleIds = new List<string> { "bike_classic_350" },
                selectedMotorcycleId = "bike_classic_350",
                characterGender = "Male",
                totalDistanceDrivenKm = 0f,
                completedMissionIds = new List<string>()
            };
        }
    }
}

