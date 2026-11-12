using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.EnvironmentSystem;
using IndianBikeLife.TrafficSystem;
using NUnit.Framework;
using UnityEngine;

namespace IndianBikeLife.Tests.Editor
{
    public class TrafficAndEnvTests
    {
        [Test]
        public void TrafficConfig_ReturnsValidVehicleParameters()
        {
            TrafficConfiguration config = ScriptableObject.CreateInstance<TrafficConfiguration>();

            VehicleTypeConfig autoConfig = config.GetConfigForType(IndianVehicleType.AutoRickshaw);
            Assert.IsNotNull(autoConfig);
            Assert.AreEqual(IndianVehicleType.AutoRickshaw, autoConfig.vehicleType);
            Assert.IsTrue(autoConfig.maxSpeedKmph > 20f);

            Assert.AreEqual(8, config.GetMaxVehicles(TrafficDensity.Low));
            Assert.AreEqual(16, config.GetMaxVehicles(TrafficDensity.Medium));
            Assert.AreEqual(28, config.GetMaxVehicles(TrafficDensity.High));

            Object.DestroyImmediate(config);
        }

        [Test]
        public void ObjectPoolManager_SpawnsAndRecyclesObjects()
        {
            GameObject poolGo = new GameObject("TestPool");
            ObjectPoolManager pool = poolGo.AddComponent<ObjectPoolManager>();

            GameObject prefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prefab.name = "TestPrefab";

            GameObject spawned = pool.Spawn(prefab, Vector3.zero, Quaternion.identity);
            Assert.IsNotNull(spawned);
            Assert.IsTrue(spawned.activeSelf);

            pool.Despawn(spawned);
            Assert.IsFalse(spawned.activeSelf);

            Object.DestroyImmediate(prefab);
            Object.DestroyImmediate(poolGo);
        }
    }
}
