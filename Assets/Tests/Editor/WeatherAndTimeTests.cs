using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.TimeAndSeasons;
using IndianBikeLife.WeatherSystem;
using NUnit.Framework;
using UnityEngine;

namespace IndianBikeLife.Tests.Editor
{
    public class WeatherAndTimeTests
    {
        [Test]
        public void WeatherConfig_ReducesGripDuringHeavyRain()
        {
            WeatherConfiguration config = ScriptableObject.CreateInstance<WeatherConfiguration>();

            WeatherConditionSettings sunny = config.GetSettings(WeatherType.Sunny);
            WeatherConditionSettings rain = config.GetSettings(WeatherType.HeavyRain);

            Assert.AreEqual(1.0f, sunny.roadGripModifier);
            Assert.AreEqual(0.65f, rain.roadGripModifier);
            Assert.IsTrue(rain.roadGripModifier < sunny.roadGripModifier);

            Object.DestroyImmediate(config);
        }

        [Test]
        public void TimeOfDayController_HourWrapsBetween0And24()
        {
            GameObject timeGo = new GameObject("TestTime");
            TimeOfDayController timeCtrl = timeGo.AddComponent<TimeOfDayController>();

            timeCtrl.SetHour(14f);
            Assert.AreEqual(14f, timeCtrl.CurrentHour, 0.01f);
            Assert.IsFalse(timeCtrl.IsNight);
            Assert.AreEqual(TimeOfDayPeriod.Noon, timeCtrl.CurrentPeriod);

            timeCtrl.SetHour(23f);
            Assert.IsTrue(timeCtrl.IsNight);
            Assert.AreEqual(TimeOfDayPeriod.Night, timeCtrl.CurrentPeriod);

            timeCtrl.SetHour(26f); // 26 % 24 = 2 (Midnight)
            Assert.AreEqual(2f, timeCtrl.CurrentHour, 0.01f);
            Assert.IsTrue(timeCtrl.IsNight);
            Assert.AreEqual(TimeOfDayPeriod.Midnight, timeCtrl.CurrentPeriod);

            Object.DestroyImmediate(timeGo);
        }

        [Test]
        public void SeasonManager_DispatchesSeasonChangeSignals()
        {
            GameObject seasonGo = new GameObject("TestSeason");
            SeasonManager seasonMgr = seasonGo.AddComponent<SeasonManager>();

            IndianSeason receivedSeason = IndianSeason.Summer;
            seasonMgr.OnSeasonChanged += s => receivedSeason = s;

            seasonMgr.SetSeason(IndianSeason.Monsoon);
            Assert.AreEqual(IndianSeason.Monsoon, seasonMgr.CurrentSeason);
            Assert.AreEqual(IndianSeason.Monsoon, receivedSeason);

            Object.DestroyImmediate(seasonGo);
        }
    }
}

