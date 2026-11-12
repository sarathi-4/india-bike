using System;
using IndianBikeLife.Backend;
using IndianBikeLife.Backend.Security;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using NUnit.Framework;

namespace IndianBikeLife.Tests
{
    [TestFixture]
    public class BackendAndCloudTests
    {
        // -------------------------------------------------------------
        // PART 29: BACKEND & CLOUD SYSTEM TESTS (17 - 22)
        // -------------------------------------------------------------

        [Test]
        public void Test17_GuestAuthentication_DeviceToken()
        {
            var auth = new AuthenticationManager();
            bool callbackInvoked = false;
            bool successResult = false;

            auth.LoginAsGuest((success, msg) =>
            {
                callbackInvoked = true;
                successResult = success;
            });

            Assert.IsTrue(callbackInvoked);
            Assert.IsTrue(successResult);
            Assert.AreEqual(AuthState.AuthenticatedAsGuest, auth.State);
            Assert.IsNotEmpty(auth.CurrentUserId);
            Assert.IsTrue(auth.CurrentUserId.StartsWith("guest_"));
            Assert.IsNotEmpty(auth.SessionToken);

            // Verify session token integrity
            bool validToken = TokenSecurityUtility.ValidateSessionToken(auth.SessionToken, out string validatedUid);
            Assert.IsTrue(validToken);
            Assert.AreEqual(auth.CurrentUserId, validatedUid);
        }

        [Test]
        public void Test18_EmailRegistration_PasswordHashingAndVerification()
        {
            var auth = new AuthenticationManager();

            // Registration
            auth.RegisterAccount("rider@indianbikelife.in", "RoyalEnfield2026", "RoyalRider", (success, msg) =>
            {
                Assert.IsTrue(success, msg);
            });

            Assert.AreEqual(AuthState.AuthenticatedWithAccount, auth.State);
            string userId = auth.CurrentUserId;
            Assert.IsNotEmpty(userId);

            // Logout
            auth.Logout();
            Assert.AreEqual(AuthState.Unauthenticated, auth.State);

            // Login with invalid password
            auth.LoginWithEmail("rider@indianbikelife.in", "WrongPassword", (success, msg) =>
            {
                Assert.IsFalse(success);
                Assert.IsTrue(msg.Contains("Incorrect"));
            });
            Assert.AreEqual(AuthState.Unauthenticated, auth.State);

            // Login with valid credentials
            auth.LoginWithEmail("rider@indianbikelife.in", "RoyalEnfield2026", (success, msg) =>
            {
                Assert.IsTrue(success, msg);
            });
            Assert.AreEqual(AuthState.AuthenticatedWithAccount, auth.State);
            Assert.AreEqual(userId, auth.CurrentUserId);
        }

        [Test]
        public void Test19_FirestoreDatabaseSchema_All16Collections()
        {
            var db = new DatabaseManager();
            db.InitializeSchema();

            // Verify all 16 architecture-specified collections exist and are initialized
            Assert.IsNotNull(db.Users);
            Assert.IsNotNull(db.PlayerProfiles);
            Assert.IsNotNull(db.PlayerBikes);
            Assert.IsNotNull(db.BikeUpgrades);
            Assert.IsNotNull(db.Inventory);
            Assert.IsNotNull(db.Wallet);
            Assert.IsNotNull(db.Transactions);
            Assert.IsNotNull(db.Missions);
            Assert.IsNotNull(db.PlayerMissions);
            Assert.IsNotNull(db.Achievements);
            Assert.IsNotNull(db.Friends);
            Assert.IsNotNull(db.Clubs);
            Assert.IsNotNull(db.Rooms);
            Assert.IsNotNull(db.RideHistory);
            Assert.IsNotNull(db.MapCatalogue);
            Assert.IsNotNull(db.DownloadContent);

            // Validate security rule: Users cannot write to other users' documents
            bool selfWriteAllowed = db.ValidateSecurityRule("PlayerProfiles", "user_101", "user_101", isWrite: true);
            bool unauthorizedWriteRejected = db.ValidateSecurityRule("PlayerProfiles", "user_101", "user_202", isWrite: true);

            Assert.IsTrue(selfWriteAllowed);
            Assert.IsFalse(unauthorizedWriteRejected);
        }

        [Test]
        public void Test20_CloudSave_ConflictResolution_TimestampPrecedence()
        {
            var cloud = new CloudSaveManager();
            cloud.Policy = ConflictResolutionPolicy.ClientWinsIfNewer;

            string uid = "usr_save_test";

            // Local save 1 (Initial save)
            var save1 = new SaveData { schemaVersion = 1, saveTimestampUtc = "2026-08-10T10:00:00Z" };
            cloud.SaveToCloud(uid, save1, (success, msg) => Assert.IsTrue(success, msg));

            // Load and verify
            cloud.LoadFromCloud(uid, (success, loadedData, msg) =>
            {
                Assert.IsTrue(success, msg);
                Assert.IsNotNull(loadedData);
            });

            // Local save with older timestamp attempting to overwrite newer cloud save
            var outdatedLocalSave = new SaveData { schemaVersion = 1, saveTimestampUtc = "2026-08-01T10:00:00Z" };
            bool olderSaveAccepted = false;
            cloud.SaveToCloud(uid, outdatedLocalSave, (success, msg) =>
            {
                olderSaveAccepted = success;
            });

            // Should be rejected by conflict detection (Server is newer)
            Assert.IsFalse(olderSaveAccepted);
        }

        [Test]
        public void Test21_OfflineSyncQueue_FifoAndFlush()
        {
            var sync = new OfflineSyncManager();
            Assert.AreEqual(0, sync.PendingActionCount);

            // Queue 3 offline operations
            sync.EnqueueAction("Transaction", "{\"txId\":\"tx_01\",\"amount\":5000}");
            sync.EnqueueAction("MissionCompleted", "{\"missionId\":\"m_ghat_01\"}");
            sync.EnqueueAction("AchievementUnlocked", "{\"achId\":\"ach_first_ride\"}");

            Assert.AreEqual(3, sync.PendingActionCount);
            Assert.AreEqual("Transaction", sync.PendingQueue[0].actionType);
            Assert.AreEqual("AchievementUnlocked", sync.PendingQueue[2].actionType);

            // Flush queue upon reconnect
            int syncedCount = 0;
            sync.FlushQueue(count => syncedCount = count);

            Assert.AreEqual(3, syncedCount);
            Assert.AreEqual(0, sync.PendingActionCount);
        }

        [Test]
        public void Test22_TransactionSecurity_HmacSignatureValidation()
        {
            var dataManager = new PlayerDataManager();

            string txId = "tx_coin_credit_99";
            int amount = 50000;
            string currency = "INR_COIN";

            // Valid signature computed with shared server secret
            string validPayload = $"{txId}:{amount}:{currency}";
            string validHmac = TokenSecurityUtility.ComputeHmac(validPayload);

            bool validResult = dataManager.ValidateTransaction(txId, amount, currency, validHmac);
            Assert.IsTrue(validResult);

            // Tampered signature or tampered amount (client attempts to inflate coins)
            string forgedHmac = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";
            bool forgedResult = dataManager.ValidateTransaction(txId, amount, currency, forgedHmac);
            Assert.IsFalse(forgedResult);

            // Tampered amount with original HMAC
            bool tamperedAmountResult = dataManager.ValidateTransaction(txId, 999999, currency, validHmac);
            Assert.IsFalse(tamperedAmountResult);
        }
    }
}

