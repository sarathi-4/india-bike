using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.MapSystem
{
    /// <summary>
    /// Represents an Indian city, town, or taluk holding specific destinations.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCity", menuName = "IndianBikeLife/Maps/City Data")]
    public class CityData : ScriptableObject
    {
        [SerializeField] private string _cityId;
        [SerializeField] private string _cityName;
        [SerializeField] private bool _isMajorHub = true;
        [SerializeField] private List<DestinationData> _destinations = new List<DestinationData>();

        public string CityId => _cityId;
        public string CityName => _cityName;
        public bool IsMajorHub => _isMajorHub;
        public IReadOnlyList<DestinationData> Destinations => _destinations;
    }
}

