using System;
using UnityEngine;

namespace IndianBikeLife.PassengerSystem
{
    public enum CharacterGender
    {
        Male = 0,
        Female = 1
    }

    public enum PassengerHoldStyle
    {
        Waist = 0,       // Holds rider's waist (couples style)
        GrabHandles = 1, // Holds motorcycle rear grab rail (standard style)
        OneHanded = 2    // Relaxed one hand on grab rail
    }

    /// <summary>
    /// ScriptableObject defining passenger character profile, gender, clothing, and accessories.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPassengerData", menuName = "IndianBikeLife/Passenger/Passenger Data")]
    public class PassengerData : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string _passengerId = "passenger_priya";
        [SerializeField] private string _displayName = "Priya";
        [SerializeField] private CharacterGender _gender = CharacterGender.Female;

        [Header("Appearance & Customization")]
        [SerializeField] private string _outfitStyle = "Casual Kurti / Jeans";
        [SerializeField] private bool _hasHelmet = true;
        [SerializeField] private string _helmetStyle = "Full-Face Matte Black";
        [SerializeField] private string _accessories = "Backpack, Sunglasses";
        [SerializeField] private Color _clothingTint = new Color(0.9f, 0.3f, 0.3f);

        [Header("Riding Dynamics")]
        [SerializeField] private PassengerHoldStyle _preferredHoldStyle = PassengerHoldStyle.Waist;
        [Range(30f, 120f)]
        [SerializeField] private float _weightKg = 55f;
        [Range(0.5f, 1.5f)]
        [SerializeField] private float _leanReactionMultiplier = 1.0f;

        public string PassengerId => _passengerId;
        public string DisplayName => _displayName;
        public CharacterGender Gender => _gender;
        public string OutfitStyle => _outfitStyle;
        public bool HasHelmet => _hasHelmet;
        public string HelmetStyle => _helmetStyle;
        public string Accessories => _accessories;
        public Color ClothingTint => _clothingTint;
        public PassengerHoldStyle PreferredHoldStyle => _preferredHoldStyle;
        public float WeightKg => _weightKg;
        public float LeanReactionMultiplier => _leanReactionMultiplier;
    }
}
