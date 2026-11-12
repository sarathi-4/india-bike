using System.Collections.Generic;
using IndianBikeLife.NavigationSystem;
using IndianBikeLife.RoadSystem;
using NUnit.Framework;
using UnityEngine;

namespace IndianBikeLife.Tests.Editor
{
    public class RoadAndNavTests
    {
        [Test]
        public void RoadSpline_EvaluatesSmoothPointsAndTangents()
        {
            GameObject go = new GameObject("TestSpline");
            RoadSpline spline = go.AddComponent<RoadSpline>();

            spline.AddNode(new Vector3(0, 0, 0));
            spline.AddNode(new Vector3(0, 0, 50));
            spline.AddNode(new Vector3(20, 5, 100));

            Vector3 midPoint = spline.EvaluatePosition(0.5f);
            Assert.IsTrue(midPoint.z > 0f && midPoint.z < 100f);

            Vector3 tangent = spline.EvaluateTangent(0.5f);
            Assert.IsTrue(tangent.sqrMagnitude > 0.5f);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void RoadSurface_ReturnsCorrectDefaultGripValues()
        {
            RoadSurface asphalt = RoadSurface.GetDefault(RoadSurfaceType.SmoothAsphalt);
            RoadSurface wetTarmac = RoadSurface.GetDefault(RoadSurfaceType.WetTarmac);
            RoadSurface gravel = RoadSurface.GetDefault(RoadSurfaceType.Gravel);

            Assert.AreEqual(1.0f, asphalt.frictionCoefficient);
            Assert.AreEqual(0.65f, wetTarmac.frictionCoefficient);
            Assert.AreEqual(0.55f, gravel.frictionCoefficient);

            Assert.IsTrue(wetTarmac.frictionCoefficient < asphalt.frictionCoefficient);
        }

        [Test]
        public void RouteGenerator_CalculatesAccurateDistance()
        {
            Vector3 start = Vector3.zero;
            Vector3 end = new Vector3(0, 0, 1000);

            RouteData route = RouteGenerator.BuildRoute(start, end, "Highway Milestone", null);

            Assert.IsNotNull(route);
            Assert.AreEqual("Highway Milestone", route.destinationName);
            Assert.AreEqual(1000f, route.totalDistanceMeters, 0.01f);
            Assert.IsTrue(route.estimatedTravelTimeSeconds > 0f);
        }
    }
}
