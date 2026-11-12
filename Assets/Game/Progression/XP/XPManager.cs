using System;
using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.Events.GameEvents;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.SaveSystem.Data;
using IndianBikeLife.Core.Services;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Progression.XP
{
    /// <summary>
    /// Master player progression service implementing IProgressionManager, IGameService, and ISaveable.
    /// Tracks total XP, computes player level, publishes level-up events, and manages level-based unlocks.
    /// </summary>
    public class XPManager : MonoBehaviour, IProgressionManager, ISaveable
    {
        private const string Tag = "XPManager";

        [SerializeField] private LevelProgressionConfig _progressionConfig;

        private LevelProgression _evaluator;
        private int _currentXP = 0;
        private int _currentLevel = 1;
        private int _xpIntoCurrentLevel = 0;
        private int _xpNeededForNext = 1000;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized;
        public int CurrentXP => _currentXP;
        public int CurrentLevel => _currentLevel;
        public int XPForCurrentLevelStart => _currentXP - _xpIntoCurrentLevel;
        public int XPForNextLevel => _xpNeededForNext;
        public float LevelProgressNormalized => _evaluator != null 
            ? _evaluator.CalculateNormalizedProgress(_xpIntoCurrentLevel, _xpNeededForNext) 
            : 0f;

        public string SaveKey => "XPManager";

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            if (_progressionConfig == null)
            {
                _progressionConfig = ScriptableObject.CreateInstance<LevelProgressionConfig>();
            }

            _evaluator = new LevelProgression(_progressionConfig);
            RecalculateLevel(notifyLevelUp: false);

            ServiceLocator.Register<IProgressionManager>(this);

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.RegisterSaveable(this);
            }

            _isInitialized = true;
            GameLogger.I(Tag, $"XPManager initialized. Level: {_currentLevel}, XP: {_currentXP}");
        }

        public void Shutdown()
        {
            if (!_isInitialized) return;

            if (ServiceLocator.TryGet<ISaveService>(out var saveService))
            {
                saveService.UnregisterSaveable(this);
            }

            ServiceLocator.Unregister<IProgressionManager>();
            _isInitialized = false;
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        public void AddXP(int amount, string source)
        {
            if (amount <= 0) return;

            _currentXP += amount;
            GameLogger.I(Tag, $"Added {amount} XP from '{source}'. Total XP: {_currentXP}");

            EventBus.Publish(new XPEarnedEvent(amount, _currentXP, source));
            RecalculateLevel(notifyLevelUp: true);
        }

        public bool IsLevelRequirementMet(int requiredLevel)
        {
            return _currentLevel >= requiredLevel;
        }

        public int GetRequiredXPForLevel(int level)
        {
            return _progressionConfig != null ? _progressionConfig.GetCumulativeXPForLevel(level) : (level - 1) * 1000;
        }

        private void RecalculateLevel(bool notifyLevelUp)
        {
            if (_evaluator == null) return;

            int oldLevel = _currentLevel;
            _currentLevel = _evaluator.CalculateLevelFromXP(_currentXP, out _xpIntoCurrentLevel, out _xpNeededForNext);

            if (notifyLevelUp && _currentLevel > oldLevel)
            {
                GameLogger.I(Tag, $"🎉 PLAYER LEVEL UP! Reached Level {_currentLevel} (from Level {oldLevel})");
                EventBus.Publish(new PlayerLevelUpEvent(_currentLevel, oldLevel));
            }
        }

        public void PopulateSaveData(SaveData data)
        {
            if (data == null) return;

            if (data.playerProfile != null)
            {
                data.playerProfile.playerLevel = _currentLevel;
            }

            if (data.progression == null)
            {
                data.progression = new ProgressionSaveData();
            }

            data.progression.currentXP = _currentXP;
            data.progression.currentLevel = _currentLevel;
        }

        public void RestoreFromSaveData(SaveData data)
        {
            if (data == null) return;

            if (data.progression != null)
            {
                _currentXP = data.progression.currentXP;
            }
            else if (data.playerProfile != null)
            {
                // Fallback for v1
                _currentXP = (data.playerProfile.playerLevel - 1) * 1000;
            }

            RecalculateLevel(notifyLevelUp: false);
        }
    }
}
