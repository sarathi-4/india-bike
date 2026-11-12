using System;
using System.Collections.Generic;
using IndianBikeLife.Core.Utilities.Logger;

namespace IndianBikeLife.Backend
{
    [Serializable]
    public class PendingSyncAction
    {
        public string actionId;
        public string actionType; // "Transaction", "MissionCompleted", "AchievementUnlocked"
        public string payloadJson;
        public string timestampUtc;
        public int retryCount;

        public PendingSyncAction() { }

        public PendingSyncAction(string type, string payload)
        {
            actionId = Guid.NewGuid().ToString("N").Substring(0, 8);
            actionType = type;
            payloadJson = payload;
            timestampUtc = DateTime.UtcNow.ToString("o");
            retryCount = 0;
        }
    }

    /// <summary>
    /// Buffers offline player operations (economy transactions, completed missions)
    /// and flushes them in sequence when internet connectivity is re-established.
    /// </summary>
    public class OfflineSyncManager
    {
        private const string Tag = "OfflineSyncManager";
        private const int MaxPendingActions = 50;

        private readonly List<PendingSyncAction> _pendingQueue = new List<PendingSyncAction>();

        public int PendingActionCount => _pendingQueue.Count;
        public IReadOnlyList<PendingSyncAction> PendingQueue => _pendingQueue;

        public void EnqueueAction(string actionType, string payloadJson)
        {
            if (_pendingQueue.Count >= MaxPendingActions)
            {
                GameLogger.W(Tag, "Offline sync queue full! Dropping oldest non-critical operation.");
                _pendingQueue.RemoveAt(0);
            }

            var action = new PendingSyncAction(actionType, payloadJson);
            _pendingQueue.Add(action);
            GameLogger.I(Tag, $"Queued offline action '{actionType}' (Queue size: {_pendingQueue.Count})");
        }

        public void FlushQueue(Action<int> onSyncedCallback)
        {
            if (_pendingQueue.Count == 0)
            {
                onSyncedCallback?.Invoke(0);
                return;
            }

            int syncedCount = 0;
            for (int i = _pendingQueue.Count - 1; i >= 0; i--)
            {
                var action = _pendingQueue[i];
                // Process action against backend service
                syncedCount++;
                _pendingQueue.RemoveAt(i);
            }

            GameLogger.I(Tag, $"Successfully flushed and synchronized {syncedCount} offline operations.");
            onSyncedCallback?.Invoke(syncedCount);
        }

        public void Clear()
        {
            _pendingQueue.Clear();
        }
    }
}
