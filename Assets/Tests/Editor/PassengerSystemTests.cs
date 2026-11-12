using IndianBikeLife.PassengerSystem;
using NUnit.Framework;
using UnityEngine;

namespace IndianBikeLife.Tests.Editor
{
    public class PassengerSystemTests
    {
        [Test]
        public void PassengerData_InitializesWithValidDefaults()
        {
            PassengerData data = ScriptableObject.CreateInstance<PassengerData>();

            Assert.IsNotNull(data);
            Assert.AreEqual("Priya", data.DisplayName);
            Assert.AreEqual(CharacterGender.Female, data.Gender);
            Assert.IsTrue(data.HasHelmet);
            Assert.IsTrue(data.WeightKg > 30f && data.WeightKg < 120f);

            Object.DestroyImmediate(data);
        }

        [Test]
        public void PassengerSelection_SupportsAllGenderPairings()
        {
            PassengerSelection catalog = ScriptableObject.CreateInstance<PassengerSelection>();

            catalog.ActivePairing = RidingPairingMode.SoloRider;
            Assert.AreEqual(RidingPairingMode.SoloRider, catalog.ActivePairing);

            catalog.ActivePairing = RidingPairingMode.MaleDriver_FemalePassenger;
            Assert.AreEqual(CharacterGender.Male, catalog.DriverGender);

            catalog.ActivePairing = RidingPairingMode.FemaleDriver_MalePassenger;
            Assert.AreEqual(CharacterGender.Female, catalog.DriverGender);

            catalog.ActivePairing = RidingPairingMode.FemaleDriver_FemalePassenger;
            Assert.AreEqual(CharacterGender.Female, catalog.DriverGender);

            catalog.ActivePairing = RidingPairingMode.MaleDriver_MalePassenger;
            Assert.AreEqual(CharacterGender.Male, catalog.DriverGender);

            Object.DestroyImmediate(catalog);
        }

        [Test]
        public void PassengerSeatPosition_ProvidesValidAnchors()
        {
            GameObject bikeGo = new GameObject("TestMotorcycle");
            PassengerSeatPosition seat = bikeGo.AddComponent<PassengerSeatPosition>();

            Assert.IsNotNull(seat.SeatAnchor);
            Assert.IsNotNull(seat.LeftGrabHandle);
            Assert.IsNotNull(seat.RightGrabHandle);
            Assert.IsNotNull(seat.MountStandPoint);

            Object.DestroyImmediate(bikeGo);
        }

        [Test]
        public void PassengerMountController_InitialStateIsNotMounted()
        {
            GameObject passengerGo = new GameObject("TestPassenger");
            passengerGo.AddComponent<PassengerAnimationController>();
            PassengerMountController mount = passengerGo.AddComponent<PassengerMountController>();

            Assert.IsFalse(mount.IsMounted);
            Assert.IsFalse(mount.IsTransitioning);

            Object.DestroyImmediate(passengerGo);
        }
    }
}

