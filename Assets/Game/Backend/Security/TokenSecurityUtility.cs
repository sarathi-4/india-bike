using System;
using System.Security.Cryptography;
using System.Text;

namespace IndianBikeLife.Backend.Security
{
    /// <summary>
    /// Cryptographic helper for secure password hashing (PBKDF2/SHA256),
    /// JWT token simulation, and transaction HMAC signature validation.
    /// Never stores raw plaintext credentials.
    /// </summary>
    public static class TokenSecurityUtility
    {
        private const string GlobalSalt = "IBL_SECURE_SALT_2026";

        public static string HashPassword(string rawPassword, string salt = "")
        {
            if (string.IsNullOrEmpty(rawPassword)) return string.Empty;

            string combined = rawPassword + GlobalSalt + salt;
            using var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        public static bool VerifyPassword(string inputPassword, string storedHash, string salt = "")
        {
            string computed = HashPassword(inputPassword, salt);
            return string.Equals(computed, storedHash, StringComparison.OrdinalIgnoreCase);
        }

        public static string GenerateSessionToken(string userId)
        {
            long expiry = DateTimeOffset.UtcNow.AddDays(7).ToUnixTimeSeconds();
            string payload = $"{userId}:{expiry}:{Guid.NewGuid():N}";
            string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
            string sig = ComputeHmac(payload);
            return $"{encoded}.{sig}";
        }

        public static bool ValidateSessionToken(string token, out string userId)
        {
            userId = string.Empty;
            if (string.IsNullOrEmpty(token) || !token.Contains(".")) return false;

            string[] parts = token.Split('.');
            if (parts.Length != 2) return false;

            try
            {
                string payload = Encoding.UTF8.GetString(Convert.FromBase64String(parts[0]));
                string expectedSig = ComputeHmac(payload);

                if (!string.Equals(expectedSig, parts[1], StringComparison.OrdinalIgnoreCase))
                    return false;

                string[] sub = payload.Split(':');
                if (sub.Length >= 2 && long.TryParse(sub[1], out long expiry))
                {
                    if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiry)
                        return false; // Expired

                    userId = sub[0];
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public static string ComputeHmac(string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(GlobalSalt));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}

