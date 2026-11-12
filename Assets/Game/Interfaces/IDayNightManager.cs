using IndianBikeLife.TimeAndSeasons;

namespace IndianBikeLife.Core.Interfaces
{
    public enum TimeOfDayPeriod
    {
        Sunrise = 0,    // 05:00 - 07:00
        Morning = 1,    // 07:00 - 11:00
        Noon = 2,       // 11:00 - 14:00
        Afternoon = 3,  // 14:00 - 17:00
        Sunset = 4,     // 17:00 - 19:00
        Evening = 5,    // 19:00 - 21:00
        Night = 6,      // 21:00 - 00:00
        Midnight = 7    // 00:00 - 05:00
    }

    public enum IndianSeason
    {
        Summer = 0,   // Hot dry sun, heat haze
        Monsoon = 1,  // High rainfall frequency, lush greenery
        Winter = 2    // Crisp cool air, morning fog/mist in hills
    }

    /// <summary>
    /// Contract for dynamic time-of-day progression, celestial lighting, and seasonal cycles.
    /// </summary>
    public interface IDayNightManager : IGameService
    {
        float CurrentHour { get; }
        TimeOfDayPeriod CurrentPeriod { get; }
        IndianSeason CurrentSeason { get; }
        bool IsNight { get; }

        void SetHour(float hour);
        void SetTimeSpeedMultiplier(float speed);
        void SetSeason(IndianSeason season);
    }
}

