namespace IndianBikeLife.Core.SceneManagement
{
    /// <summary>
    /// Strongly-typed scene indices corresponding to EditorBuildSettings.
    /// Eliminates magic scene strings throughout the codebase.
    /// </summary>
    public enum SceneIndex
    {
        Boot = 0,
        Loading = 1,
        MainMenu = 2,
        Gameplay = 3,    // Prepared for open-world motorcycle simulation
        Showroom = 4     // Prepared for motorcycle garage/showroom
    }
}

