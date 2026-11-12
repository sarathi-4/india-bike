using IndianBikeLife.Core.SaveSystem.Data;
using UnityEngine;

namespace IndianBikeLife.Core.SaveSystem.Serializers
{
    public interface ISerializer
    {
        string Serialize(SaveData data);
        SaveData Deserialize(string serializedText);
    }

    /// <summary>
    /// JSON serializer utilizing Unity's fast, zero-allocation JsonUtility engine.
    /// </summary>
    public class JsonSaveSerializer : ISerializer
    {
        private readonly bool _prettyPrint;

        public JsonSaveSerializer(bool prettyPrint = true)
        {
            _prettyPrint = prettyPrint;
        }

        public string Serialize(SaveData data)
        {
            if (data == null) return string.Empty;
            data.checksum = data.ComputeChecksum();
            return JsonUtility.ToJson(data, _prettyPrint);
        }

        public SaveData Deserialize(string serializedText)
        {
            if (string.IsNullOrEmpty(serializedText)) return null;
            return JsonUtility.FromJson<SaveData>(serializedText);
        }
    }
}

