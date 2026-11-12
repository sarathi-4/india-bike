using IndianBikeLife.WeatherSystem;

namespace IndianBikeLife.Core.Interfaces
{
    public enum WeatherType
    {
        Sunny = 0,
        Cloudy = 1,
        Overcast = 2,
        LightRain = 3,
        HeavyRain = 4,
        Thunderstorm = 5,
        Fog = 6,
        Wind = 7,
        Mist = 8
    }

    /// <summary>
    /// Contract for dynamic weather simulation, particle effects, rain audio, and wet road physics.
    /// </summary>
    public interface IWeatherManager : IGameService
    {
        WeatherType CurrentWeather { get; }
        float WeatherIntensity { get; }
        float RoadGripFactor { get; }

        void SetWeather(WeatherType type, float transitionDuration = 2.0f);
        void SetIntensity(float intensity);
    }
}
