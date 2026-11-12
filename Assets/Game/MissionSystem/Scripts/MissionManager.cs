using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.MissionSystem
{
    /// <summary>
    /// Master mission service implementing IMissionManager, IGameService, and ISaveable.
    /// Coordinates mission selection, execution lifecycle, completion records, and persistence.
    /// </summary>
    [RequireComponent(typeof(MissionController))]
    public class MissionManager : MonoBehaviour, IMissionManager, ISaveable
    {
        private const string Tag = "MissionManager";

        [SerializeField] private MissionCatalogue _catalogue;

        private MissionController _controller;
        private readonly List<string> _completedMissionIds = new List<string>();
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public string SaveKey => "MissionManager";
        public MissionCatalogue Catalogue => _catalogue;

        public MissionStatus CurrentStatus => _controller?.ActiveMission?.Status ?? MissionStatus.NotStarted;
        public MissionData ActiveMission => _controller?.ActiveMission;
        public MissionDifficulty CurrentDifficulty => _controller?.Difficulty ?? MissionDifficulty.Normal;
        public float MissionElapsedTime => _controller?.Timer?.ElapsedSeconds ?? 0f;
        public float MissionRemainingTime => _controller?.Timer?.RemainingSeconds ?? 0f;
        public IReadOnlyList<string> CompletedMissionIds => _completedMissionIds;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            _controller = GetComponent<MissionController>();
            if (_controller == null)
            {
                _controller = gameObject.AddComponent<MissionController>();
            }

            _controller.OnMissionFinished += HandleMissionFinished;

            ServiceLocator.Register<IMissionManager>(this);

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.RegisterSaveable(this);
            }

            _isInitialized = true;
            GameLogger.I(Tag, "MissionManager initialized.");
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;

            if (_controller != null)
            {
                _controller.OnMissionFinished -= HandleMissionFinished;
            }

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.UnregisterSaveable(this);
            }

            ServiceLocator.Unregister<IMissionManager>();
            _isInitialized = false;
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        public bool StartMission(MissionData mission, MissionDifficulty difficulty = MissionDifficulty.Normal)
        {
            if (_controller == null) return false;
            return _controller.StartMission(mission, difficulty);
        }

        public void PauseMission()
        {
            _controller?.PauseMission();
        }

        public void ResumeMission()
        {
            _controller?.ResumeMission();
        }

        public void CompleteActiveMission()
        {
            _controller?.CompleteMission();
        }

        public void FailActiveMission(string failureReason)
        {
            _controller?.FailMission(failureReason);
        }

        public void RestartActiveMission()
        {
            _controller?.RestartMission();
        }

        public void CancelActiveMission()
        {
            _controller?.CancelMission();
        }

        public bool IsMissionCompleted(string missionId)
        {
            if (string.IsNullOrEmpty(missionId)) return false;
            return _completedMissionIds.Contains(missionId);
        }

        public int GetCompletedMissionCount()
        {
            return _completedMissionIds.Count;
        }

        private void HandleMissionFinished(MissionResultSummary summary)
        {
            if (summary != null && summary.isSuccess)
            {
                if (!string.IsNullOrEmpty(summary.missionId) && !_completedMissionIds.Contains(summary.missionId))
                {
                    _completedMissionIds.Add(summary.missionId);
                    GameLogger.I(Tag, $"Added '{summary.missionId}' to completed missions list. Total completed: {_completedMissionIds.Count}");
                }
            }
        }

        public void PopulateSaveData(SaveData data)
        {
            if (data == null) return;

            // Keep legacy field in sync
            if (data.playerProfile != null)
            {
                data.playerProfile.completedMissionIds = new List<string>(_completedMissionIds);
            }

            if (data.progression == null) data.progression = new ProgressionSaveData();
            data.progression.completedMissionIds = new List<string>(_completedMissionIds);
        }

        public void RestoreFromSaveData(SaveData data)
        {
            if (data == null) return;

            _completedMissionIds.Clear();

            if (data.progression != null && data.progression.completedMissionIds != null)
            {
                _completedMissionIds.AddRange(data.progression.completedMissionIds);
            }
            else if (data.playerProfile != null && data.playerProfile.completedMissionIds != null)
            {
                _completedMissionIds.AddRange(data.playerProfile.completedMissionIds);
            }
        }
    }
}

