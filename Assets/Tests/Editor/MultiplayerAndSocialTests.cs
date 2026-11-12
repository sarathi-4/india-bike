using System.Collections.Generic;
using IndianBikeLife.Communication;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Multiplayer;
using IndianBikeLife.Social;
using NUnit.Framework;
using UnityEngine;

namespace IndianBikeLife.Tests
{
    [TestFixture]
    public class MultiplayerAndSocialTests
    {
        private GameObject _testHolder;

        [SetUp]
        public void SetUp()
        {
            _testHolder = new GameObject("Test_MultiplayerHolder");
        }

        [TearDown]
        public void TearDown()
        {
            if (_testHolder != null)
            {
                Object.DestroyImmediate(_testHolder);
            }
        }

        // -------------------------------------------------------------
        // PART 26: MULTIPLAYER SYSTEM TESTS (1 - 10)
        // -------------------------------------------------------------

        [Test]
        public void Test01_RoomCreation_GeneratesValidRoomDataAndCode()
        {
            var roomManager = new RoomManager();
            var room = roomManager.CreateRoom("Mumbai Riders", "rider_1", "Rajesh", maxPlayers: 4);

            Assert.IsNotNull(room);
            Assert.AreEqual("Mumbai Riders", room.roomName);
            Assert.AreEqual("rider_1", room.hostPlayerId);
            Assert.AreEqual(6, room.roomCode.Length);
            Assert.AreEqual(1, room.players.Count);
            Assert.IsTrue(room.players[0].isHost);
            Assert.AreEqual(0, room.players[0].slotIndex);
            Assert.AreEqual(4, room.maxPlayers);
        }

        [Test]
        public void Test02_RoomJoining_SlotAllocationAndCapacityLimit()
        {
            var roomManager = new RoomManager();
            var room = roomManager.CreateRoom("Goa Coastal Ride", "p1", "Vikram", maxPlayers: 4);
            string code = room.roomCode;

            // Join player 2
            bool j2 = roomManager.JoinRoom(code, "p2", "Sunil", "", out string fail2);
            Assert.IsTrue(j2, fail2);
            Assert.AreEqual(2, room.players.Count);
            Assert.AreEqual(1, room.players[1].slotIndex);

            // Join player 3
            bool j3 = roomManager.JoinRoom(code, "p3", "Pooja", "", out string fail3);
            Assert.IsTrue(j3, fail3);
            Assert.AreEqual(3, room.players.Count);

            // Join player 4
            bool j4 = roomManager.JoinRoom(code, "p4", "Deepak", "", out string fail4);
            Assert.IsTrue(j4, fail4);
            Assert.AreEqual(4, room.players.Count);
            Assert.IsTrue(room.IsFull);

            // 5th player should be rejected (max 4 players limit)
            bool j5 = roomManager.JoinRoom(code, "p5", "ExtraPlayer", "", out string fail5);
            Assert.IsFalse(j5);
            Assert.IsTrue(fail5.Contains("full"));
        }

        [Test]
        public void Test03_RoomLeave_PlayerCleanupAndHostMigration()
        {
            var roomManager = new RoomManager();
            var room = roomManager.CreateRoom("Ladakh Expedition", "host_p1", "Amit", maxPlayers: 4);
            string code = room.roomCode;

            roomManager.JoinRoom(code, "rider_p2", "Suresh", "", out _);
            Assert.AreEqual(2, room.players.Count);
            Assert.AreEqual("host_p1", room.hostPlayerId);

            // Host leaves -> Host migration to rider_p2
            roomManager.LeaveRoom("host_p1");
            Assert.AreEqual(1, room.players.Count);
            Assert.AreEqual("rider_p2", room.hostPlayerId);
            Assert.IsTrue(room.players[0].isHost);
        }

        [Test]
        public void Test04_PasswordProtectedRoom_Authentication()
        {
            var roomManager = new RoomManager();
            var room = roomManager.CreateRoom("Secret Ghat Ride", "host_1", "Karan", password: "BulletPassword123");
            string code = room.roomCode;

            // Try joining with wrong password
            bool joinFail = roomManager.JoinRoom(code, "guest_1", "Arjun", "WrongPassword", out string reason);
            Assert.IsFalse(joinFail);
            Assert.IsTrue(reason.Contains("password"));

            // Try joining with correct password
            bool joinSuccess = roomManager.JoinRoom(code, "guest_1", "Arjun", "BulletPassword123", out string successReason);
            Assert.IsTrue(joinSuccess, successReason);
        }

        [Test]
        public void Test05_PlayerReadyState_RideStartAuthorization()
        {
            var roomManager = new RoomManager();
            var room = roomManager.CreateRoom("NH44 Sprint", "p1", "HostRider", maxPlayers: 2);
            string code = room.roomCode;
            roomManager.JoinRoom(code, "p2", "GuestRider", "", out _);

            // Non-host cannot start ride
            bool nonHostStart = roomManager.StartRide("p2", out string err1);
            Assert.IsFalse(nonHostStart);
            Assert.IsTrue(err1.Contains("Only the room host"));

            // Host tries to start when players aren't ready
            bool notReadyStart = roomManager.StartRide("p1", out string err2);
            Assert.IsFalse(notReadyStart);
            Assert.IsTrue(err2.Contains("Ready"));

            // Both players toggle ready
            roomManager.SetReady("p1", true);
            roomManager.SetReady("p2", true);
            Assert.IsTrue(room.AreAllPlayersReady());

            // Host starts ride successfully
            bool readyStart = roomManager.StartRide("p1", out string successErr);
            Assert.IsTrue(readyStart, successErr);
            Assert.IsTrue(room.isRideActive);
        }

        [Test]
        public void Test06_NetworkSnapshot_StatePackaging()
        {
            var snapshot = PlayerNetworkState.CreateDefault("rider_007");
            snapshot.sequenceNumber = 42;
            snapshot.position = new Vector3(100f, 12f, 250f);
            snapshot.rotation = Quaternion.Euler(0f, 90f, 0f);
            snapshot.velocity = new Vector3(20f, 0f, 0f);
            snapshot.forwardSpeedKmph = 72f;
            snapshot.steerAngle = -5.5f;
            snapshot.bikeLeanRoll = -12.0f;
            snapshot.engineRPM = 4500f;
            snapshot.gear = 3;
            snapshot.headlightOn = true;
            snapshot.hornHonking = true;
            snapshot.hasPassenger = true;
            snapshot.passengerLeanRoll = -8.5f;
            snapshot.currentCheckpointIndex = 4;

            Assert.AreEqual("rider_007", snapshot.playerId);
            Assert.AreEqual(42u, snapshot.sequenceNumber);
            Assert.AreEqual(72f, snapshot.forwardSpeedKmph);
            Assert.IsTrue(snapshot.headlightOn);
            Assert.IsTrue(snapshot.hornHonking);
            Assert.IsTrue(snapshot.hasPassenger);
            Assert.AreEqual(-8.5f, snapshot.passengerLeanRoll);
        }

        [Test]
        public void Test07_DeadReckoning_And_Interpolation()
        {
            var go = new GameObject("RemoteRider");
            go.transform.SetParent(_testHolder.transform);
            var netPlayer = go.AddComponent<NetworkPlayerController>();

            Vector3 spawnPos = new Vector3(0, 0, 0);
            netPlayer.Initialize("remote_1", spawnPos, Quaternion.identity);

            var nextState = PlayerNetworkState.CreateDefault("remote_1");
            nextState.position = new Vector3(10f, 0f, 0f);
            nextState.velocity = new Vector3(15f, 0f, 0f);

            bool accepted = netPlayer.ReceiveNetworkState(nextState);
            Assert.IsTrue(accepted);
            Assert.AreEqual(10f, netPlayer.LatestState.position.x);
        }

        [Test]
        public void Test08_SpeedhackAndAntiTeleport_Detection()
        {
            var go = new GameObject("RemoteRiderCheatCheck");
            go.transform.SetParent(_testHolder.transform);
            var netPlayer = go.AddComponent<NetworkPlayerController>();

            Vector3 spawnPos = new Vector3(0, 0, 0);
            netPlayer.Initialize("suspect_rider", spawnPos, Quaternion.identity);

            // Simulate teleport jump of 500 meters in 1 frame (> 48 m/s / 172 km/h)
            var cheatState = PlayerNetworkState.CreateDefault("suspect_rider");
            cheatState.position = new Vector3(500f, 0f, 0f);

            bool accepted = netPlayer.ReceiveNetworkState(cheatState);
            // Must be rejected by server-side / client-side anti-teleport check
            Assert.IsFalse(accepted);
        }

        [Test]
        public void Test09_RemoteBikeVisualSyncing()
        {
            var bikeGO = new GameObject("RemoteBike");
            bikeGO.transform.SetParent(_testHolder.transform);
            var netBike = bikeGO.AddComponent<NetworkBikeController>();

            netBike.ApplyRemoteState(steerAngle: 15.0f, leanRoll: -18.0f, headlight: true, horn: true);

            Assert.AreEqual(15.0f, netBike.SteerAngle);
            Assert.AreEqual(-18.0f, netBike.LeanRoll);
            Assert.IsTrue(netBike.HeadlightOn);
            Assert.IsTrue(netBike.HornHonking);
        }

        [Test]
        public void Test10_RemotePassengerSyncing()
        {
            var passengerGO = new GameObject("RemotePassenger");
            passengerGO.transform.SetParent(_testHolder.transform);
            var netPassenger = passengerGO.AddComponent<NetworkPassengerController>();

            netPassenger.ApplyPassengerState(hasPassenger: true, passengerLeanRoll: -10.0f);
            Assert.IsTrue(netPassenger.HasPassenger);
            Assert.AreEqual(-10.0f, netPassenger.PassengerLeanRoll);

            netPassenger.ApplyPassengerState(hasPassenger: false, passengerLeanRoll: 0f);
            Assert.IsFalse(netPassenger.HasPassenger);
        }

        // -------------------------------------------------------------
        // PART 27: SOCIAL SYSTEM TESTS (11 - 14)
        // -------------------------------------------------------------

        [Test]
        public void Test11_FriendRequestWorkflow()
        {
            var controller = new FriendRequestController();
            var existingFriends = new List<FriendEntry>();

            // Can send request
            Assert.IsTrue(controller.CanSendRequest("target_rider", existingFriends));

            // Block player
            controller.BlockPlayer("target_rider");
            Assert.IsTrue(controller.IsBlocked("target_rider"));
            Assert.IsFalse(controller.CanSendRequest("target_rider", existingFriends));

            // Unblock player
            controller.UnblockPlayer("target_rider");
            Assert.IsFalse(controller.IsBlocked("target_rider"));
            Assert.IsTrue(controller.CanSendRequest("target_rider", existingFriends));

            // Add incoming request
            var req = new FriendRequest("req_101", "target_rider", "Rahul", "Hey let's ride!");
            controller.AddIncomingRequest(req);
            Assert.AreEqual(1, controller.IncomingRequests.Count);

            // Accept request
            var resolved = controller.ResolveRequest("req_101", accept: true);
            Assert.IsNotNull(resolved);
            Assert.AreEqual(FriendRequestStatus.Accepted, resolved.status);
            Assert.AreEqual(0, controller.IncomingRequests.Count);
        }

        [Test]
        public void Test12_SocialProfilePrivacy_Sanitization()
        {
            var manager = new SocialProfileManager();
            var profile = new SocialProfileData("rider_alpha", "Rohan", "Royal Rider", 15, 250000);
            profile.privacyLevel = ProfilePrivacyLevel.FriendsOnly;

            manager.UpdateLocalProfile(profile);

            // Public viewer viewing FriendsOnly profile -> sanitized
            var publicView = manager.GetSanitizedProfile("rider_alpha", isFriend: false);
            Assert.IsNotNull(publicView);
            Assert.AreEqual("Rohan", publicView.username);
            Assert.AreEqual(0, publicView.level); // Hidden for non-friends
            Assert.AreEqual(0, publicView.distanceRiddenMeters);

            // Friend viewer viewing FriendsOnly profile -> full access
            var friendView = manager.GetSanitizedProfile("rider_alpha", isFriend: true);
            Assert.AreEqual(15, friendView.level);
            Assert.AreEqual(250000, friendView.distanceRiddenMeters);
        }

        [Test]
        public void Test13_RidingClub_CreationAndRoleManagement()
        {
            var clubManager = new ClubManager();
            var club = clubManager.CreateClub("Deccan Bulls", "Cruising Maharashtra highways", "DECCAN", "leader_1", "Aditya");

            Assert.IsNotNull(club);
            Assert.AreEqual("Deccan Bulls", club.clubName);
            Assert.AreEqual("DECCAN", club.clubTag);
            Assert.AreEqual(1, club.members.Count);
            Assert.AreEqual(ClubMemberRole.Leader, club.members[0].role);

            // Member joins
            bool joined = clubManager.JoinClub(club.clubId, "member_2", "Praveen");
            Assert.IsTrue(joined);
            Assert.AreEqual(2, club.members.Count);

            // Update role to Officer
            bool promoted = club.UpdateMemberRole("member_2", ClubMemberRole.Officer);
            Assert.IsTrue(promoted);
            Assert.AreEqual(ClubMemberRole.Officer, club.members.Find(m => m.playerId == "member_2").role);
        }

        [Test]
        public void Test14_GroupRideScheduling_AndRSVP()
        {
            var rideManager = new GroupRideManager();
            var ride = rideManager.ScheduleRide(
                title: "Western Ghats Monsoon Cruise",
                destination: "Lonavala",
                meetingPoint: "Navi Mumbai Expressway Toll",
                scheduledTimeUtc: "2026-08-15T06:00:00Z",
                creatorId: "lead_1",
                creatorName: "Vijay"
            );

            Assert.IsNotNull(ride);
            Assert.AreEqual("Lonavala", ride.destinationLocation);
            Assert.AreEqual(1, ride.rsvpPlayerIds.Count);

            // Additional riders RSVP
            bool rsvp1 = rideManager.RSVPToRide(ride.rideId, "rider_2");
            bool rsvp2 = rideManager.RSVPToRide(ride.rideId, "rider_3");
            Assert.IsTrue(rsvp1);
            Assert.IsTrue(rsvp2);
            Assert.AreEqual(3, ride.rsvpPlayerIds.Count);
        }

        // -------------------------------------------------------------
        // PART 28: COMMUNICATION SYSTEM TESTS (15 - 16)
        // -------------------------------------------------------------

        [Test]
        public void Test15_QuickChat_IndianPhrases()
        {
            Assert.GreaterOrEqual(QuickChatController.PhraseCount, 8);
            string phrase0 = QuickChatController.GetPhrase(0);
            string phrase3 = QuickChatController.GetPhrase(3);

            Assert.AreEqual("Horn OK Please!", phrase0);
            Assert.AreEqual("Slow down — Speed breaker ahead!", phrase3);
        }

        [Test]
        public void Test16_ChatModeration_ProfanityFilterAndRateLimit()
        {
            var chatGO = new GameObject("ChatManagerTest");
            chatGO.transform.SetParent(_testHolder.transform);
            var chat = chatGO.AddComponent<ChatManager>();
            chat.Initialize();

            // Test profanity filtering
            string dirtyMessage = "You are an idiot and a cheat!";
            string cleanMessage = chat.FilterProfanity(dirtyMessage);

            Assert.IsFalse(cleanMessage.ToLower().Contains("idiot"));
            Assert.IsFalse(cleanMessage.ToLower().Contains("cheat"));
            Assert.IsTrue(cleanMessage.Contains("***"));

            // Test mute functionality
            chat.MutePlayer("toxic_rider");
            Assert.IsTrue(chat.IsPlayerMuted("toxic_rider"));

            chat.UnmutePlayer("toxic_rider");
            Assert.IsFalse(chat.IsPlayerMuted("toxic_rider"));
        }
    }
}

