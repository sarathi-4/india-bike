using IndianBikeLife.Core.Audio;
using IndianBikeLife.Core.Configuration;
using NUnit.Framework;
using UnityEngine;

namespace IndianBikeLife.Tests.Editor
{
    public class ConfigurationTests
    {
        [Test]
        public void AudioConfig_LinearToDecibels_CalculatesCorrectly()
        {
            AudioConfiguration config = ScriptableObject.CreateInstance<AudioConfiguration>();

            // Full volume (1.0) is 0 dB
            float maxDb = config.LinearToDecibels(1.0f);
            Assert.AreEqual(0f, maxDb, 0.001f);

            // Zero volume is min decibels (-80 dB)
            float minDb = config.LinearToDecibels(0.0f);
            Assert.AreEqual(-80f, minDb, 0.001f);

            // 50% volume is approximately -6 dB
            float halfDb = config.LinearToDecibels(0.5f);
            Assert.IsTrue(halfDb < 0f && halfDb > -10f);

            Object.DestroyImmediate(config);
        }

        [Test]
        public void GraphicsConfig_PresetTargetFps_MatchesAndroidRequirements()
        {
            GraphicsConfiguration config = ScriptableObject.CreateInstance<GraphicsConfiguration>();

            Assert.AreEqual(30, config.GetTargetFpsForPreset(MobileQualityPreset.Low));
            Assert.AreEqual(60, config.GetTargetFpsForPreset(MobileQualityPreset.Medium));
            Assert.AreEqual(60, config.GetTargetFpsForPreset(MobileQualityPreset.High));

            Object.DestroyImmediate(config);
        }
    }
}
