using System;
using System.Collections;
using IndianBikeLife.Core.SceneManagement;

namespace IndianBikeLife.Core.Interfaces
{
    /// <summary>
    /// Contract for asynchronous scene loading, transition coordination, and additive streaming.
    /// </summary>
    public interface ISceneLoader : IGameService
    {
        /// <summary>
        /// Gets the currently active scene index.
        /// </summary>
        SceneIndex CurrentScene { get; }

        /// <summary>
        /// Gets whether a scene transition is currently in progress.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Gets current normalized loading progress (0.0 to 1.0).
        /// </summary>
        float LoadingProgress { get; }

        /// <summary>
        /// Asynchronously loads a target scene using the loading screen pipeline.
        /// </summary>
        /// <param name="targetScene">The scene to load.</param>
        /// <param name="onComplete">Optional completion callback.</param>
        void LoadScene(SceneIndex targetScene, Action onComplete = null);

        /// <summary>
        /// Asynchronously loads a target scene by scene build index.
        /// </summary>
        /// <param name="sceneBuildIndex">Scene index in build settings.</param>
        /// <param name="onComplete">Optional completion callback.</param>
        void LoadScene(int sceneBuildIndex, Action onComplete = null);

        /// <summary>
        /// Asynchronously loads a target scene by scene name.
        /// </summary>
        /// <param name="sceneName">Name of scene asset.</param>
        /// <param name="onComplete">Optional completion callback.</param>
        void LoadScene(string sceneName, Action onComplete = null);

        /// <summary>
        /// Asynchronously loads a scene additively (prepared for open world streaming).
        /// </summary>
        /// <param name="sceneName">Additive scene name.</param>
        /// <param name="onComplete">Optional completion callback.</param>
        void LoadSceneAdditive(string sceneName, Action onComplete = null);

        /// <summary>
        /// Asynchronously unloads an additively loaded scene.
        /// </summary>
        /// <param name="sceneName">Additive scene name.</param>
        /// <param name="onComplete">Optional completion callback.</param>
        void UnloadSceneAdditive(string sceneName, Action onComplete = null);
    }
}
