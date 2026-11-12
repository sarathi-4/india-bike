using System.Collections.Generic;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.SaveSystem.Migrations;
using IndianBikeLife.Core.SaveSystem.Serializers;
using IndianBikeLife.Core.Services;
using IndianBikeLife.MissionSystem;
using IndianBikeLife.Progression.Achievements;
using IndianBikeLife.Progression.Economy;
using IndianBikeLife.Progression.Rewards;
using IndianBikeLife.Progression.XP;
using NUnit.Framework;
using UnityEngine;

namespace IndianBikeLife.Tests
{
    [TestFixture]
    public class EconomyAndProgressionTests
    {
        private GameObject _servicesHolder;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Clear();
            _servicesHolder = new GameObject("Test_EconomyHolder");
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Clear();
            if (_servicesHolder != null) Object.DestroyImmediate(_servicesHolder);
        }

        [Test]
        public void Test09_RewardCalculation()
        {
            var mission = ScriptableObject.CreateInstance<MissionData>();
            var rew = RewardData.CreateRuntimeReward("rew_test", 1000, 10, 500);

            var fieldRew = typeof(MissionData).GetField("_reward", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldRew?.SetValue(mission, rew);

            // Normal difficulty result
            var normalResult = MissionResultController.CalculateSuccessResult(mission, MissionDifficulty.Normal, 45f, 500f, 0f);
            Assert.AreEqual(1500, normalResult.earnedSilver); // 1000 * 1.0 * 1.5 (3 stars flawless)
            Assert.AreEqual(10, normalResult.earnedGold);

            // Expert difficulty result (1.8x multiplier)
            var expertResult = MissionResultController.CalculateSuccessResult(mission, MissionDifficulty.Expert, 45f, 500f, 0f);
            Assert.AreEqual(2700, expertResult.earnedSilver); // 1000 * 1.8 * 1.5
            Assert.AreEqual(18, expertResult.earnedGold); // 10 * 1.8

            Object.DestroyImmediate(mission);
            Object.DestroyImmediate(rew);
        }

        [Test]
        public void Test10_CoinEarning()
        {
            var wallet = new PlayerWallet(1000, 10);
            bool credited = wallet.Credit(CurrencyType.SilverCoins, 500, out int newBal);

            Assert.IsTrue(credited);
            Assert.AreEqual(1500, newBal);
            Assert.AreEqual(1500, wallet.SilverCoins);

            wallet.Credit(CurrencyType.GoldCoins, 5, out int newGold);
            Assert.AreEqual(15, newGold);
        }

        [Test]
        public void Test11_CoinSpending()
        {
            var wallet = new PlayerWallet(1000, 10);
            bool debited = wallet.Debit(CurrencyType.SilverCoins, 400, out int newBal);

            Assert.IsTrue(debited);
            Assert.AreEqual(600, newBal);
            Assert.AreEqual(600, wallet.SilverCoins);
        }

        [Test]
        public void Test12_InsufficientBalanceHandling()
        {
            var wallet = new PlayerWallet(200, 5);

            // Cannot spend more than balance
            bool debited = wallet.Debit(CurrencyType.SilverCoins, 500, out int newBal);
            Assert.IsFalse(debited);
            Assert.AreEqual(200, newBal); // Balance untouched
            Assert.AreEqual(200, wallet.SilverCoins);

            // Cannot pass negative numbers
            bool negDebit = wallet.Debit(CurrencyType.SilverCoins, -100, out int negBal);
            Assert.IsFalse(negDebit);
        }

        [Test]
        public void Test13_XPProgression()
        {
            var config = ScriptableObject.CreateInstance<LevelProgressionConfig>();
            var prog = new LevelProgression(config);

            // Level 1: 0 XP
            int lvl1 = prog.CalculateLevelFromXP(0, out int xpIn1, out int needed1);
            Assert.AreEqual(1, lvl1);
            Assert.AreEqual(0, xpIn1);

            // Level up threshold
            int neededForLvl2 = config.GetXPRequiredForLevelUp(1);
            int lvl2 = prog.CalculateLevelFromXP(neededForLvl2 + 50, out int xpIn2, out int needed2);
            Assert.AreEqual(2, lvl2);
            Assert.AreEqual(50, xpIn2);

            Object.DestroyImmediate(config);
        }

        [Test]
        public void Test14_LevelUp()
        {
            var xpMgr = _servicesHolder.AddComponent<XPManager>();
            xpMgr.Initialize();

            Assert.AreEqual(1, xpMgr.CurrentLevel);

            // Add massive XP to trigger level up
            xpMgr.AddXP(5000, "Test Reward");
            Assert.Greater(xpMgr.CurrentLevel, 1);
        }

        [Test]
        public void Test15_AchievementUnlocking()
        {
            var achMgr = _servicesHolder.AddComponent<AchievementManager>();
            achMgr.Initialize();

            Assert.IsFalse(achMgr.IsAchievementUnlocked("ach_first_ride"));

            achMgr.ReportProgress("ach_first_ride", 1f);
            Assert.IsTrue(achMgr.IsAchievementUnlocked("ach_first_ride"));
            Assert.GreaterOrEqual(achMgr.UnlockedAchievementCount, 1);
        }

        [Test]
        public void Test16_DailyRewardClaiming()
        {
            var rewMgr = _servicesHolder.AddComponent<RewardManager>();
            rewMgr.Initialize();
            var currMgr = _servicesHolder.AddComponent<CurrencyManager>();
            currMgr.Initialize();

            var loginMgr = _servicesHolder.AddComponent<LoginRewardManager>();
            loginMgr.Initialize();

            Assert.IsTrue(loginMgr.CanClaimToday());
            bool claimed = loginMgr.ClaimDailyReward();
            Assert.IsTrue(claimed);
            Assert.AreEqual(1, loginMgr.ConsecutiveStreak);

            // Same day double claim should be rejected
            Assert.IsFalse(loginMgr.CanClaimToday());
            Assert.IsFalse(loginMgr.ClaimDailyReward());
        }

        [Test]
        public void Test17_DuplicateRewardPrevention()
        {
            var currMgr = _servicesHolder.AddComponent<CurrencyManager>();
            currMgr.Initialize();
            var rewMgr = _servicesHolder.AddComponent<RewardManager>();
            rewMgr.Initialize();

            var reward = RewardData.CreateRuntimeReward("unique_quest_reward_101", 500, 5, 200);

            int startSilver = currMgr.SilverCoins;

            // First claim: success
            bool claim1 = rewMgr.ClaimReward(reward, enforceSingleClaim: true);
            Assert.IsTrue(claim1);
            Assert.AreEqual(startSilver + 500, currMgr.SilverCoins);

            // Second claim with identical ID: must fail!
            bool claim2 = rewMgr.ClaimReward(reward, enforceSingleClaim: true);
            Assert.IsFalse(claim2);
            Assert.AreEqual(startSilver + 500, currMgr.SilverCoins); // Unchanged

            Object.DestroyImmediate(reward);
        }

        [Test]
        public void Test18_SaveLoadCompatibility()
        {
            // 1. Create v1 SaveData
            var v1Data = new SaveData
            {
                schemaVersion = 1,
                settings = SettingsSaveData.CreateDefault(),
                playerProfile = new PlayerProfileSaveData
                {
                    playerId = "test_player",
                    playerCoins = 4500,
                    playerLevel = 3,
                    completedMissionIds = new List<string> { "mission_old_1" }
                }
            };

            // 2. Run migrator
            var migrator = new SaveMigratorV1ToV2();
            var v2Data = migrator.Migrate(v1Data);

            Assert.AreEqual(2, v2Data.schemaVersion);
            Assert.IsNotNull(v2Data.progression);
            Assert.AreEqual(4500, v2Data.progression.silverCoins);
            Assert.AreEqual(3, v2Data.progression.currentLevel);
            Assert.Contains("mission_old_1", v2Data.progression.completedMissionIds);

            // 3. Round-trip serialization
            var serializer = new JsonSaveSerializer();
            string json = serializer.Serialize(v2Data);
            var deserialized = serializer.Deserialize(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(2, deserialized.schemaVersion);
            Assert.AreEqual(4500, deserialized.progression.silverCoins);
        }
    }
}
