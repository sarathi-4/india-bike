using System;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using UnityEngine;

namespace IndianBikeLife.MissionSystem.UI
{
    /// <summary>
    /// Modal dialog overlay displaying mission completion or failure results,
    /// earned currency/XP breakdown, and retry/continue action buttons.
    /// </summary>
    public class MissionResultUI : MonoBehaviour
    {
        private IMissionManager _missionManager;
        private MissionResultSummary _activeSummary;
        private bool _isShowing = false;

        private void Start()
        {
            ServiceLocator.TryGet(out _missionManager);
            if (GetComponent<MissionController>() is MissionController controller)
            {
                controller.OnMissionFinished += ShowResult;
            }
        }

        public void ShowResult(MissionResultSummary summary)
        {
            _activeSummary = summary;
            _isShowing = summary != null;
        }

        public void Hide()
        {
            _isShowing = false;
            _activeSummary = null;
        }

        private void OnGUI()
        {
            if (!_isShowing || _activeSummary == null) return;

            int w = Screen.width;
            int h = Screen.height;

            float panelWidth = Mathf.Min(400f, w * 0.8f);
            float panelHeight = Mathf.Min(320f, h * 0.75f);
            Rect dialogRect = new Rect((w - panelWidth) * 0.5f, (h - panelHeight) * 0.5f, panelWidth, panelHeight);

            string title = _activeSummary.isSuccess ? "🎉 MISSION ACCOMPLISHED!" : "❌ MISSION FAILED";
            GUI.Box(dialogRect, $"<b>{title}</b>");

            GUILayout.BeginArea(new Rect(dialogRect.x + 15, dialogRect.y + 35, dialogRect.width - 30, dialogRect.height - 45));

            GUILayout.Label($"<b>{_activeSummary.missionTitle}</b>");
            GUILayout.Space(6);

            if (_activeSummary.isSuccess)
            {
                string starString = _activeSummary.starRating switch
                {
                    3 => "★★★ (Flawless)",
                    2 => "★★☆ (Great)",
                    _ => "★☆☆ (Completed)"
                };
                GUILayout.Label($"Rating: <color=yellow><b>{starString}</b></color>");
                GUILayout.Label($"Time Taken: <b>{_activeSummary.timeTakenSeconds:0.0}s</b>");
                GUILayout.Label($"Distance: <b>{_activeSummary.distanceCoveredMeters:0}m</b>");
                GUILayout.Space(8);

                GUILayout.Label("<b>Rewards Earned:</b>");
                GUILayout.Label($"• Silver Coins: <color=white>+{_activeSummary.earnedSilver}</color>");
                if (_activeSummary.earnedGold > 0)
                {
                    GUILayout.Label($"• Gold Coins: <color=yellow>+{_activeSummary.earnedGold}</color>");
                }
                GUILayout.Label($"• Player XP: <color=cyan>+{_activeSummary.earnedXP} XP</color>");
            }
            else
            {
                GUILayout.Label($"Reason: <color=red>{_activeSummary.failureReason}</color>");
                GUILayout.Label($"Time Elapsed: {_activeSummary.timeTakenSeconds:0.0}s");
            }

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Retry Mission", GUILayout.Height(35)))
            {
                Hide();
                _missionManager?.RestartActiveMission();
            }

            if (GUILayout.Button("Continue", GUILayout.Height(35)))
            {
                Hide();
                _missionManager?.CancelActiveMission();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
}

