using IndianBikeLife.Core.Events.Core;
using IndianBikeLife.Core.SceneManagement;

namespace IndianBikeLife.Core.Events.GameEvents
{
    /// <summary>
    /// Dispatched when an asynchronous scene load starts.
    /// </summary>
    public readonly struct SceneLoadStartedEvent : IGameEvent
    {
        public SceneIndex TargetScene { get; }
        public string TargetSceneName { get; }

        public SceneLoadStartedEvent(SceneIndex targetScene, string targetSceneName = null)
        {
            TargetScene = targetScene;
            TargetSceneName = targetSceneName ?? targetScene.ToString();
        }
    }

    /// <summary>
    /// Dispatched as scene loading progress updates.
    /// </summary>
    public readonly struct SceneLoadProgressEvent : IGameEvent
    {
        public SceneIndex TargetScene { get; }
        public float Progress { get; }

        public SceneLoadProgressEvent(SceneIndex targetScene, float progress)
        {
            TargetScene = targetScene;
            Progress = progress;
        }
    }

    /// <summary>
    /// Dispatched when scene loading and activation finishes.
    /// </summary>
    public readonly struct SceneLoadCompletedEvent : IGameEvent
    {
        public SceneIndex LoadedScene { get; }
        public string SceneName { get; }

        public SceneLoadCompletedEvent(SceneIndex loadedScene, string sceneName = null)
        {
            LoadedScene = loadedScene;
            SceneName = sceneName ?? loadedScene.ToString();
        }
    }
}

