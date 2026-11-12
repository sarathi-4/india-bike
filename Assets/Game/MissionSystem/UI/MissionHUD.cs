using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using UnityEngine;

namespace IndianBikeLife.MissionSystem.UI
{
    /// <summary>
    /// In-game HUD overlay displaying active mission title, live timer, checkpoints,
    /// objective progression, and vehicle condition.
    /// </summary>
    public class MissionHUD : MonoBehaviour
    {
        private IMissionManager _missionManager;
        private bool _showHUD = true;

        private void Start()
        {
            ServiceLocator.TryGet(out _missionManager);
        }

        public void SetVisibility(bool visible)
        {
            _showHUD = visible;
        }

        private void OnGUI()
        {
            if (!_showHUD || _missionManager == null || _missionManager.CurrentStatus != MissionStatus.Active) return;

            var mission = _missionManager.ActiveMission;
            if (mission == null) return;

            int w = Screen.width;
            int h = Screen.height;

            // Mission Tracker Top-Right Card
            float boxWidth = Mathf.Min(320f, w * 0.35f);
            float boxHeight = 160f;
            Rect hudRect = new Rect(w - boxWidth - 15, 45, boxWidth, boxHeight);

            GUI.Box(hudRect, $"<b>{mission.Title}</b> ({_missionManager.CurrentDifficulty})");

            GUILayout.BeginArea(new Rect(hudRect.x + 10, hudRect.y + 25, hudRect.width - 20, hudRect.height - 30));

            // Timer display
            if (mission.TimeLimitSeconds > 0)
            {
                float rem = _missionManager.MissionRemainingTime;
                int mins = Mathf.FloorToInt(rem / 60f);
                int secs = Mathf.FloorToInt(rem % 60f);
                string colorTag = rem < 15f ? "<color=red>" : "<color=yellow>";
                GUILayout.Label($"Time Remaining: {colorTag}<b>{mins:D2}:{secs:D2}</b></color>");
            }
            else
            {
                float elap = _missionManager.MissionElapsedTime;
                int mins = Mathf.FloorToInt(elap / 60f);
                int secs = Mathf.FloorToInt(elap % 60f);
                GUILayout.Label($"Elapsed Time: <b>{mins:D2}:{secs:D2}</b>");
            }

            // Checkpoints / Destination
            if (mission.Checkpoints != null && mission.Checkpoints.Count > 0)
            {
                GUILayout.Label($"Checkpoints: <b>{mission.CompletedObjectiveCount} / {mission.Objectives.Count}</b>");
            }
            else
            {
                GUILayout.Label($"Destination: <b>{mission.DestinationName}</b>");
            }

            // Objectives summary
            if (mission.Objectives != null)
            {
                for (int i = 0; i < mission.Objectives.Count; i++)
                {
                    var obj = mission.Objectives[i];
                    string status = obj.isCompleted ? "<color=green>✓</color>" : "○";
                    string opt = obj.isOptional ? " [Optional]" : "";
                    GUILayout.Label($"{status} {obj.description}{opt}");
                }
            }

            GUILayout.EndArea();
        }
    }
}
