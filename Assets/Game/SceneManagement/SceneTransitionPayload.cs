using System;

namespace IndianBikeLife.Core.SceneManagement
{
    /// <summary>
    /// Payload carrying parameters for coordinated scene transitions.
    /// </summary>
    public class SceneTransitionPayload
    {
        public SceneIndex TargetScene { get; set; }
        public string TargetSceneName { get; set; }
        public float MinimumLoadingScreenDuration { get; set; } = 1.0f;
        public bool UseLoadingScreen { get; set; } = true;
        public Action OnComplete { get; set; }

        public SceneTransitionPayload(SceneIndex targetScene, float minDuration = 1.0f)
        {
            TargetScene = targetScene;
            TargetSceneName = targetScene.ToString();
            MinimumLoadingScreenDuration = minDuration;
            UseLoadingScreen = true;
        }

        public SceneTransitionPayload(string targetSceneName, float minDuration = 1.0f)
        {
            TargetScene = SceneIndex.Boot;
            TargetSceneName = targetSceneName;
            MinimumLoadingScreenDuration = minDuration;
            UseLoadingScreen = true;
        }
    }
}
