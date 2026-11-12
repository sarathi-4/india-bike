using System.Collections.Generic;
using UnityEngine;

namespace IndianBikeLife.MapSystem
{
    /// <summary>
    /// Represents an administrative district within an Indian State or Union Territory.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDistrict", menuName = "IndianBikeLife/Maps/District Data")]
    public class DistrictData : ScriptableObject
    {
        [SerializeField] private string _districtId;
        [SerializeField] private string _districtName;
        [SerializeField] private List<CityData> _cities = new List<CityData>();

        public string DistrictId => _districtId;
        public string DistrictName => _districtName;
        public IReadOnlyList<CityData> Cities => _cities;
    }
}
