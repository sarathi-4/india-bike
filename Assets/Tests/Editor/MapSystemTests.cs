using IndianBikeLife.MapSystem;
using NUnit.Framework;
using UnityEngine;

namespace IndianBikeLife.Tests.Editor
{
    public class MapSystemTests
    {
        [Test]
        public void MapCatalogue_HierarchyLookupsSucceed()
        {
            MapCatalogue catalogue = ScriptableObject.CreateInstance<MapCatalogue>();
            StateData tn = ScriptableObject.CreateInstance<StateData>();

            Assert.IsNotNull(catalogue);
            Assert.AreEqual(0, catalogue.TotalStatesCount);

            Object.DestroyImmediate(tn);
            Object.DestroyImmediate(catalogue);
        }

        [Test]
        public void DestinationData_StoresElevationAndSceneRouting()
        {
            DestinationData dest = ScriptableObject.CreateInstance<DestinationData>();

            Assert.IsNotNull(dest);
            Assert.AreEqual("RegionalRideTest", dest.SceneName);
            Assert.IsTrue(dest.IsPlayableInStage2);

            Object.DestroyImmediate(dest);
        }

        [Test]
        public void MapSelectionManager_ToggleFavouritesWorks()
        {
            GameObject go = new GameObject("TestSelectionManager");
            MapSelectionManager manager = go.AddComponent<MapSelectionManager>();

            string destId = "ooty_ghat";
            Assert.IsFalse(manager.IsFavourite(destId));

            bool added = manager.ToggleFavourite(destId);
            Assert.IsTrue(added);
            Assert.IsTrue(manager.IsFavourite(destId));

            bool removed = manager.ToggleFavourite(destId);
            Assert.IsFalse(removed);
            Assert.IsFalse(manager.IsFavourite(destId));

            Object.DestroyImmediate(go);
        }
    }
}

