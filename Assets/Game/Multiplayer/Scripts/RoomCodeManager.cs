using System;
using System.Text;

namespace IndianBikeLife.Multiplayer
{
    /// <summary>
    /// Generates and validates readable 6-character room codes.
    /// Excludes confusing glyphs (0/O, 1/I) for seamless mobile input.
    /// </summary>
    public static class RoomCodeManager
    {
        private const string AllowedCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private const int CodeLength = 6;
        private static readonly Random _rng = new Random();

        public static string GenerateRoomCode()
        {
            var sb = new StringBuilder(CodeLength);
            for (int i = 0; i < CodeLength; i++)
            {
                int index = _rng.Next(AllowedCharacters.Length);
                sb.Append(AllowedCharacters[index]);
            }
            return sb.ToString();
        }

        public static bool IsValidCodeFormat(string code)
        {
            if (string.IsNullOrEmpty(code) || code.Length != CodeLength) return false;

            string upper = code.ToUpperInvariant();
            for (int i = 0; i < upper.Length; i++)
            {
                if (!AllowedCharacters.Contains(upper[i].ToString()))
                    return false;
            }
            return true;
        }

        public static string SanitizeCode(string rawCode)
        {
            if (string.IsNullOrEmpty(rawCode)) return string.Empty;
            return rawCode.Trim().ToUpperInvariant();
        }
    }
}

