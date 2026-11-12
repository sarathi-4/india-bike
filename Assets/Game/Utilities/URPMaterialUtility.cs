using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace IndianBikeLife.Core.Utilities
{
    /// <summary>
    /// Utility for creating and applying Universal Render Pipeline (URP) compatible materials.
    /// Prevents magenta / pink unlit shader fallback on procedurally spawned 3D primitives.
    /// </summary>
    public static class URPMaterialUtility
    {
        private static readonly Dictionary<Color, Material> _materialCache = new Dictionary<Color, Material>();
        private static Shader _urpShader;

        public static Shader GetURPShader()
        {
            if (_urpShader != null && _urpShader.isSupported) return _urpShader;

            // Probe standard shaders in priority order based on active pipeline
            string[] shaderCandidates = new[]
            {
                "Universal Render Pipeline/Lit",
                "Universal Render Pipeline/Simple Lit",
                "Universal Render Pipeline/Unlit",
                "Mobile/Diffuse",
                "Standard",
                "Unlit/Color",
                "Sprites/Default"
            };

            foreach (var name in shaderCandidates)
            {
                var s = Shader.Find(name);
                if (s != null && s.isSupported && !s.name.Contains("Error") && s.name != "Hidden/InternalErrorShader")
                {
                    _urpShader = s;
                    return _urpShader;
                }
            }

            _urpShader = Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Color");
            return _urpShader;
        }

        public static Material CreateURPMaterial(string name, Color color, float metallic = 0.2f, float smoothness = 0.3f)
        {
            Shader shader = GetURPShader();
            Material mat = new Material(shader)
            {
                name = name,
                color = color
            };

            // Support both URP (_BaseColor) and Legacy Standard (_Color)
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);

            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);

            return mat;
        }

        public static Material GetCachedColorMaterial(Color color, string name = "URP_ColorMat")
        {
            if (_materialCache.TryGetValue(color, out var cached) && cached != null)
            {
                return cached;
            }

            Material newMat = CreateURPMaterial(name, color);
            _materialCache[color] = newMat;
            return newMat;
        }

        public static void ApplyURPColor(GameObject go, Color color, string name = "URP_PrimitiveMat")
        {
            if (go == null) return;
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = GetCachedColorMaterial(color, name);
                renderer.shadowCastingMode = ShadowCastingMode.On;
                renderer.receiveShadows = true;
            }
        }
    }
}

