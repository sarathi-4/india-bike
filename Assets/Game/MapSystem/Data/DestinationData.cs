using System;
using UnityEngine;

namespace IndianBikeLife.MapSystem
{
    public enum RoadEnvironmentType
    {
        CityUrban = 0,
        NationalHighway = 1,
        StateHighway = 2,
        HillStationGhat = 3,
        CoastalExpressway = 4,
        VillageRural = 5,
        ForestSanctuary = 6,
        OffRoadDirt = 7
    }

    /// <summary>
    /// Specific rideable destination, waypoint, or scenic hotspot in an Indian city or region.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDestination", menuName = "IndianBikeLife/Maps/Destination Data")]
    public class DestinationData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string _destinationId;
        [SerializeField] private string _destinationName;
        [SerializeField] private string _description;
        [SerializeField] private RoadEnvironmentType _primaryEnvironment = RoadEnvironmentType.StateHighway;

        [Header("Geography & Distance")]
        [SerializeField] private float _elevationMeters = 50f;
        [SerializeField] private float _distanceFromStateCapitalKm = 120f;
        [SerializeField] private Vector3 _worldAnchorPosition;

        [Header("Road & Scene Route")]
        [SerializeField] private string _sceneName = "RegionalRideTest";
        [SerializeField] private bool _isPlayableInStage2 = true;
        [SerializeField] private string _recommendedBikes = "Classic 350, Himalayan, Duke 390";

        public string DestinationId => _destinationId;
        public string DestinationName => _destinationName;
        public string Description => _description;
        public RoadEnvironmentType PrimaryEnvironment => _primaryEnvironment;
        public float ElevationMeters => _elevationMeters;
        public float DistanceFromStateCapitalKm => _distanceFromStateCapitalKm;
        public Vector3 WorldAnchorPosition => _worldAnchorPosition;
        public string SceneName => _sceneName;
        public bool IsPlayableInStage2 => _isPlayableInStage2;
        public string RecommendedBikes => _recommendedBikes;
    }
}
