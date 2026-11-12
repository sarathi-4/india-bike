using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Services;
using UnityEngine;

namespace IndianBikeLife.MissionSystem.UI
{
    /// <summary>
    /// UI browser for browsing mission catalogue categories, reading mission descriptions,
    /// selecting difficulty, and initiating missions.
    /// </summary>
    public class MissionCatalogueUI : MonoBehaviour
    {
        [SerializeField] private MissionCatalogue _catalogue;

        private IMissionManager _missionManager;
        private MissionCategory _selectedCategory = MissionCategory.Highway;
        private MissionDifficulty _selectedDifficulty = MissionDifficulty.Normal;
        private Vector2 _scrollPos;
        private bool _isOpen = false;

        private void Start()
        {
            ServiceLocator.TryGet(out _missionManager);
        }

        public void ToggleOpen()
        {
            _isOpen = !_isOpen;
        }

        public void SetOpen(bool open)
        {
            _isOpen = open;
        }

        private void OnGUI()
        {
            if (!_isOpen) return;

            int w = Screen.width;
            int h = Screen.height;

            float panelWidth = Mathf.Min(600f, w * 0.9f);
            float panelHeight = Mathf.Min(450f, h * 0.85f);
            Rect windowRect = new Rect((w - panelWidth) * 0.5f, (h - panelHeight) * 0.5f, panelWidth, panelHeight);

            GUI.Box(windowRect, "<b>INDIAN BIKE LIFE — MISSION CATALOGUE</b>");

            GUILayout.BeginArea(new Rect(windowRect.x + 15, windowRect.y + 30, windowRect.width - 30, windowRect.height - 40));

            // Difficulty selection row
            GUILayout.BeginHorizontal();
            GUILayout.Label("Difficulty:", GUILayout.Width(75));
            foreach (MissionDifficulty diff in Enum.GetValues(typeof(MissionDifficulty)))
            {
                if (GUILayout.Toggle(_selectedDifficulty == diff, diff.ToString(), "Button"))
                {
                    _selectedDifficulty = diff;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6);

            // Category Tabs
            GUILayout.BeginHorizontal();
            GUILayout.Label("Category:", GUILayout.Width(75));
            if (GUILayout.Button(_selectedCategory.ToString(), GUILayout.Width(180)))
            {
                // Cycle through categories
                int nextCat = ((int)_selectedCategory + 1) % Enum.GetValues(typeof(MissionCategory)).Length;
                _selectedCategory = (MissionCategory)nextCat;
            }
            GUILayout.Label($"(15 Categories Available)");
            GUILayout.EndHorizontal();

            GUILayout.Space(8);

            // Mission list for selected category
            List<MissionData> missions = _catalogue != null 
                ? _catalogue.GetMissionsByCategory(_selectedCategory) 
                : new List<MissionData>();

            _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.Height(panelHeight - 160));

            if (missions.Count == 0)
            {
                GUILayout.Label($"No missions configured in {_selectedCategory} catalogue.");
            }
            else
            {
                for (int i = 0; i < missions.Count; i++)
                {
                    var m = missions[i];
                    if (m == null) continue;

                    GUILayout.BeginVertical("Box");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"<b>{m.Title}</b>", GUILayout.Width(250));
                    GUILayout.FlexibleSpace();
                    bool completed = _missionManager != null && _missionManager.IsMissionCompleted(m.MissionId);
                    if (completed)
                    {
                        GUILayout.Label("<color=green>[COMPLETED]</color>");
                    }
                    if (GUILayout.Button("Start Mission", GUILayout.Width(110)))
                    {
                        _missionManager?.StartMission(m, _selectedDifficulty);
                        _isOpen = false;
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Label(m.Description);
                    GUILayout.Label($"Route: {m.StartLocationName} → {m.DestinationName} | Time Limit: {(m.TimeLimitSeconds > 0 ? $"{m.TimeLimitSeconds}s" : "Untimed")}");
                    GUILayout.EndVertical();
                    GUILayout.Space(4);
                }
            }

            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Close Catalogue", GUILayout.Height(30)))
            {
                _isOpen = false;
            }

            GUILayout.EndArea();
        }
    }
}
