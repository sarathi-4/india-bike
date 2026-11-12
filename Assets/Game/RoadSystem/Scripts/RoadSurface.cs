using System;
using UnityEngine;

namespace IndianBikeLife.RoadSystem
{
    public enum RoadSurfaceType
    {
        SmoothAsphalt = 0,   // National Highways (Grip: 1.0)
        WornTarmac = 1,      // State Highways & City Roads (Grip: 0.9)
        WetTarmac = 2,       // Monsoon Rain Conditions (Grip: 0.65)
        Concrete = 3,        // Expressways & Bridges (Grip: 0.95)
        DirtTrack = 4,       // Village & Rural Roads (Grip: 0.7)
        Gravel = 5,          // Hill Stations & Shoulders (Grip: 0.55)
        MudTrail = 6         // Off-Road Trails (Grip: 0.45)
    }

    /// <summary>
    /// Surface friction and physical response properties for motorcycle tires.
    /// </summary>
    [Serializable]
    public class RoadSurface
    {
        public RoadSurfaceType surfaceType = RoadSurfaceType.WornTarmac;
        [Range(0.2f, 1.2f)] public float frictionCoefficient = 0.9f;
        [Range(0.01f, 0.2f)] public float rollingResistance = 0.02f;
        [Range(0f, 1f)] public float bumpiness = 0.05f;
        public float defaultSpeedLimitKmph = 60f;

        public static RoadSurface GetDefault(RoadSurfaceType type)
        {
            RoadSurface surface = new RoadSurface { surfaceType = type };
            switch (type)
            {
                case RoadSurfaceType.SmoothAsphalt:
                    surface.frictionCoefficient = 1.0f;
                    surface.rollingResistance = 0.015f;
                    surface.bumpiness = 0.02f;
                    surface.defaultSpeedLimitKmph = 100f;
                    break;
                case RoadSurfaceType.WornTarmac:
                    surface.frictionCoefficient = 0.9f;
                    surface.rollingResistance = 0.02f;
                    surface.bumpiness = 0.06f;
                    surface.defaultSpeedLimitKmph = 60f;
                    break;
                case RoadSurfaceType.WetTarmac:
                    surface.frictionCoefficient = 0.65f;
                    surface.rollingResistance = 0.025f;
                    surface.bumpiness = 0.05f;
                    surface.defaultSpeedLimitKmph = 50f;
                    break;
                case RoadSurfaceType.Concrete:
                    surface.frictionCoefficient = 0.95f;
                    surface.rollingResistance = 0.018f;
                    surface.bumpiness = 0.04f;
                    surface.defaultSpeedLimitKmph = 80f;
                    break;
                case RoadSurfaceType.DirtTrack:
                    surface.frictionCoefficient = 0.70f;
                    surface.rollingResistance = 0.04f;
                    surface.bumpiness = 0.15f;
                    surface.defaultSpeedLimitKmph = 40f;
                    break;
                case RoadSurfaceType.Gravel:
                    surface.frictionCoefficient = 0.55f;
                    surface.rollingResistance = 0.05f;
                    surface.bumpiness = 0.20f;
                    surface.defaultSpeedLimitKmph = 35f;
                    break;
                case RoadSurfaceType.MudTrail:
                    surface.frictionCoefficient = 0.45f;
                    surface.rollingResistance = 0.08f;
                    surface.bumpiness = 0.25f;
                    surface.defaultSpeedLimitKmph = 25f;
                    break;
            }
            return surface;
        }
    }
}
