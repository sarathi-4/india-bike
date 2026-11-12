namespace IndianBikeLife.Core.State
{
    /// <summary>
    /// Represents the high-level operational lifecycle states of Indian Bike Life.
    /// </summary>
    public enum GameState
    {
        None = 0,
        Booting = 1,
        Loading = 2,
        MainMenu = 3,
        InShowroom = 4,   // Prepared for motorcycle garage/showroom
        Gameplay = 5,     // Prepared for open-world Indian riding
        Paused = 6,
        GameOver = 7,
        ShuttingDown = 8
    }
}

