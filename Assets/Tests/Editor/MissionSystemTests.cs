using System.Collections.Generic;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.MissionSystem;
using NUnit.Framework;
using UnityEngine;

namespace IndianBikeLife.Tests
{
    [TestFixture]
    public class MissionSystemTests
    {
        private GameObject _holder;
        private MissionController _controller;
        private MissionData _sampleMission;

        [SetUp]
        public void SetUp()
        {
            _holder = new GameObject("Test_MissionHolder");
            _controller = _holder.AddComponent<MissionController>();

            _sampleMission = ScriptableObject.CreateInstance<MissionData>();
            _sampleMission.name = "Test_HighwayMission";

            // Add sample objective
            var obj = new MissionObjective("test_obj_1", MissionObjectiveType.HitCheckpoints, "Clear 2 Checkpoints", 2f);
            var objDest = new MissionObjective("test_obj_2", MissionObjectiveType.ReachDestination, "Reach End", 1f);

            var fieldObjectives = typeof(MissionData).GetField("_objectives", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldObjectives != null)
            {
                fieldObjectives.SetValue(_sampleMission, new List<MissionObjective> { obj, objDest });
            }

            var fieldTime = typeof(MissionData).GetField("_timeLimitSeconds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldTime != null)
            {
                fieldTime.SetValue(_sampleMission, 60.0f);
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (_holder != null) Object.DestroyImmediate(_holder);
            if (_sampleMission != null) Object.DestroyImmediate(_sampleMission);
        }

        [Test]
        public void Test01_MissionCreationAndLoading()
        {
            Assert.IsNotNull(_sampleMission);
            Assert.AreEqual("Test_HighwayMission", _sampleMission.MissionId);

            var runtimeClone = _sampleMission.CloneRuntimeInstance();
            Assert.IsNotNull(runtimeClone);
            Assert.AreEqual(MissionStatus.NotStarted, runtimeClone.Status);
            Assert.AreEqual(2, runtimeClone.Objectives.Count);
        }

        [Test]
        public void Test02_MissionSelection()
        {
            var catalogue = ScriptableObject.CreateInstance<MissionCatalogue>();
            var fieldMissions = typeof(MissionCatalogue).GetField("_missions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldMissions?.SetValue(catalogue, new List<MissionData> { _sampleMission });

            var found = catalogue.FindById("Test_HighwayMission");
            Assert.IsNotNull(found);
            Assert.AreEqual(_sampleMission.MissionId, found.MissionId);
        }

        [Test]
        public void Test03_MissionStart()
        {
            bool started = _controller.StartMission(_sampleMission, MissionDifficulty.Normal);
            Assert.IsTrue(started);
            Assert.IsNotNull(_controller.ActiveMission);
            Assert.AreEqual(MissionStatus.Active, _controller.ActiveMission.Status);
            Assert.AreEqual(MissionDifficulty.Normal, _controller.Difficulty);
            Assert.IsTrue(_controller.Timer.IsRunning);
        }

        [Test]
        public void Test04_ObjectiveProgress()
        {
            _controller.StartMission(_sampleMission, MissionDifficulty.Normal);
            var tracker = _controller.Tracker;
            Assert.IsNotNull(tracker);

            tracker.UpdateObjectiveProgress(MissionObjectiveType.HitCheckpoints, 1f);
            var obj = _controller.ActiveMission.Objectives[0];
            Assert.AreEqual(1f, obj.currentProgress);
            Assert.IsFalse(obj.isCompleted);

            tracker.UpdateObjectiveProgress(MissionObjectiveType.HitCheckpoints, 1f);
            Assert.AreEqual(2f, obj.currentProgress);
            Assert.IsTrue(obj.isCompleted);
        }

        [Test]
        public void Test05_CheckpointDetection()
        {
            var cpGo = new GameObject("Test_Checkpoint");
            var cp = cpGo.AddComponent<MissionCheckpoint>();
            bool passed = false;

            cp.Initialize(0, 10f, idx => passed = true);

            // Outside radius
            bool triggeredFar = cp.CheckProximity(new Vector3(50, 0, 0));
            Assert.IsFalse(triggeredFar);
            Assert.IsFalse(passed);

            // Inside radius
            bool triggeredNear = cp.CheckProximity(new Vector3(5, 0, 0));
            Assert.IsTrue(triggeredNear);
            Assert.IsTrue(passed);

            Object.DestroyImmediate(cpGo);
        }

        [Test]
        public void Test06_MissionCompletion()
        {
            _controller.StartMission(_sampleMission, MissionDifficulty.Normal);
            _controller.Tracker.ForceCompleteAllObjectives();

            _controller.CompleteMission();

            Assert.AreEqual(MissionStatus.Completed, _controller.ActiveMission.Status);
            Assert.IsNotNull(_controller.LastResult);
            Assert.IsTrue(_controller.LastResult.isSuccess);
            Assert.GreaterOrEqual(_controller.LastResult.starRating, 1);
        }

        [Test]
        public void Test07_MissionFailure()
        {
            _controller.StartMission(_sampleMission, MissionDifficulty.Normal);
            _controller.FailMission("Time expired.");

            Assert.AreEqual(MissionStatus.Failed, _controller.ActiveMission.Status);
            Assert.IsNotNull(_controller.LastResult);
            Assert.IsFalse(_controller.LastResult.isSuccess);
            Assert.AreEqual("Time expired.", _controller.LastResult.failureReason);
        }

        [Test]
        public void Test08_MissionRestart()
        {
            _controller.StartMission(_sampleMission, MissionDifficulty.Normal);
            _controller.Tracker.UpdateObjectiveProgress(MissionObjectiveType.HitCheckpoints, 1f);

            _controller.RestartMission();

            Assert.AreEqual(MissionStatus.Active, _controller.ActiveMission.Status);
            Assert.AreEqual(0f, _controller.ActiveMission.Objectives[0].currentProgress);
            Assert.AreEqual(0f, _controller.Timer.ElapsedSeconds);
        }
    }
}
