namespace IndianBikeLife.Core.Audio
{
    /// <summary>
    /// Distinct audio routing channels for independent volume mixing.
    /// </summary>
    public enum AudioChannel
    {
        Master = 0,
        Music = 1,
        SFX = 2,
        Vehicle = 3,    // Motorcycle engine, exhaust, transmission, skids
        Ambient = 4,    // Indian city noise, horn honking, highway wind, rain
        UI = 5          // Button clicks, purchase chimes, menu navigations
    }
}
