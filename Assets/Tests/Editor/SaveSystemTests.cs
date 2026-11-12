using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.SaveSystem.Serializers;
using NUnit.Framework;

namespace IndianBikeLife.Tests.Editor
{
    public class SaveSystemTests
    {
        [Test]
        public void CreateDefault_PopulatesValidInitialState()
        {
            SaveData defaultData = SaveData.CreateDefault();

            Assert.IsNotNull(defaultData);
            Assert.AreEqual(SaveData.CurrentSchemaVersion, defaultData.schemaVersion);
            Assert.IsNotNull(defaultData.settings);
            Assert.IsNotNull(defaultData.playerProfile);

            Assert.AreEqual("Indian Rider", defaultData.playerProfile.playerName);
            Assert.AreEqual(1000, defaultData.playerProfile.playerCoins);
            Assert.AreEqual("bike_classic_350", defaultData.playerProfile.selectedMotorcycleId);
            Assert.IsTrue(defaultData.playerProfile.unlockedMotorcycleIds.Contains("bike_classic_350"));
        }

        [Test]
        public void JsonSerializer_RoundtripPreservesData()
        {
            JsonSaveSerializer serializer = new JsonSaveSerializer(prettyPrint: false);
            SaveData original = SaveData.CreateDefault();
            original.playerProfile.playerCoins = 50000;
            original.settings.masterVolume = 0.42f;

            string json = serializer.Serialize(original);
            Assert.IsFalse(string.IsNullOrEmpty(json));

            SaveData deserialized = serializer.Deserialize(json);
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(50000, deserialized.playerProfile.playerCoins);
            Assert.AreEqual(0.42f, deserialized.settings.masterVolume, 0.001f);
        }

        [Test]
        public void Checksum_MatchesExpectedCalculation()
        {
            SaveData data = SaveData.CreateDefault();
            string checksum1 = data.ComputeChecksum();
            string checksum2 = data.ComputeChecksum();

            Assert.AreEqual(checksum1, checksum2);

            // Modify a key property and verify checksum changes
            data.playerProfile.playerCoins += 100;
            string checksumModified = data.ComputeChecksum();
            Assert.AreNotEqual(checksum1, checksumModified);
        }
    }
}

