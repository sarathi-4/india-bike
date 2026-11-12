using System;
using System.Collections.Generic;
using IndianBikeLife.Backend.Security;
using IndianBikeLife.Core.Interfaces;
using IndianBikeLife.Core.Utilities.Logger;
using UnityEngine;

namespace IndianBikeLife.Backend
{
    [Serializable]
    public class UserAccountRecord
    {
        public string userId;
        public string email;
        public string passwordHash;
        public string username;
        public bool isGuest;
        public string createdDateUtc;

        public UserAccountRecord() { }

        public UserAccountRecord(string uid, string mail, string pHash, string name, bool guest)
        {
            userId = uid;
            email = mail;
            passwordHash = pHash;
            username = name;
            isGuest = guest;
            createdDateUtc = DateTime.UtcNow.ToString("o");
        }
    }

    /// <summary>
    /// Handles player identity, guest accounts, credential authentication, session tokens,
    /// and account deletion compliance without storing plaintext passwords.
    /// </summary>
    public class AuthenticationManager
    {
        private const string Tag = "AuthenticationManager";
        private const string GuestIdPrefKey = "IBL_GuestUserId";

        private readonly Dictionary<string, UserAccountRecord> _registeredAccounts = new Dictionary<string, UserAccountRecord>();

        private AuthState _authState = AuthState.Unauthenticated;
        private string _currentUserId = "";
        private string _currentUsername = "";
        private string _sessionToken = "";

        public AuthState State => _authState;
        public string CurrentUserId => _currentUserId;
        public string CurrentUsername => _currentUsername;
        public string SessionToken => _sessionToken;

        public AuthenticationManager()
        {
            // Pre-seed default demo account for testing
            string demoEmail = "demo@indianbike.com";
            string demoPass = "IndianBike2026";
            string demoHash = TokenSecurityUtility.HashPassword(demoPass, demoEmail);
            _registeredAccounts[demoEmail] = new UserAccountRecord("usr_demo_rider_01", demoEmail, demoHash, "IndianRider", guest: false);
        }

        public void LoginAsGuest(Action<bool, string> onComplete)
        {
            string guestId = PlayerPrefs.GetString(GuestIdPrefKey, "");
            if (string.IsNullOrEmpty(guestId))
            {
                guestId = "guest_" + Guid.NewGuid().ToString("N").Substring(0, 8);
                PlayerPrefs.SetString(GuestIdPrefKey, guestId);
                PlayerPrefs.Save();
            }

            _currentUserId = guestId;
            _currentUsername = "Guest Rider";
            _sessionToken = TokenSecurityUtility.GenerateSessionToken(guestId);
            _authState = AuthState.AuthenticatedAsGuest;

            GameLogger.I(Tag, $"Authenticated as guest user: {guestId}");
            onComplete?.Invoke(true, "Guest login successful.");
        }

        public void RegisterAccount(string email, string password, string username, Action<bool, string> onComplete)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                onComplete?.Invoke(false, "Invalid email address.");
                return;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                onComplete?.Invoke(false, "Password must be at least 6 characters.");
                return;
            }

            string cleanEmail = email.Trim().ToLowerInvariant();
            if (_registeredAccounts.ContainsKey(cleanEmail))
            {
                onComplete?.Invoke(false, "An account with this email already exists.");
                return;
            }

            string uid = "usr_" + Guid.NewGuid().ToString("N").Substring(0, 10);
            string passHash = TokenSecurityUtility.HashPassword(password, cleanEmail);

            var record = new UserAccountRecord(uid, cleanEmail, passHash, username, guest: false);
            _registeredAccounts[cleanEmail] = record;

            _currentUserId = uid;
            _currentUsername = username;
            _sessionToken = TokenSecurityUtility.GenerateSessionToken(uid);
            _authState = AuthState.AuthenticatedWithAccount;

            GameLogger.I(Tag, $"Account registered successfully for {cleanEmail} (UID: {uid})");
            onComplete?.Invoke(true, "Registration successful.");
        }

        public void LoginWithEmail(string email, string password, Action<bool, string> onComplete)
        {
            string cleanEmail = email.Trim().ToLowerInvariant();
            if (!_registeredAccounts.TryGetValue(cleanEmail, out var record))
            {
                onComplete?.Invoke(false, "Invalid email or account does not exist.");
                return;
            }

            if (!TokenSecurityUtility.VerifyPassword(password, record.passwordHash, cleanEmail))
            {
                onComplete?.Invoke(false, "Incorrect password.");
                return;
            }

            _currentUserId = record.userId;
            _currentUsername = record.username;
            _sessionToken = TokenSecurityUtility.GenerateSessionToken(record.userId);
            _authState = AuthState.AuthenticatedWithAccount;

            GameLogger.I(Tag, $"Authenticated account user {cleanEmail} (UID: {record.userId})");
            onComplete?.Invoke(true, "Login successful.");
        }

        public void Logout()
        {
            _currentUserId = "";
            _currentUsername = "";
            _sessionToken = "";
            _authState = AuthState.Unauthenticated;
            GameLogger.I(Tag, "Player logged out.");
        }

        public void DeleteAccount(Action<bool, string> onComplete)
        {
            if (string.IsNullOrEmpty(_currentUserId))
            {
                onComplete?.Invoke(false, "No active user session.");
                return;
            }

            string uidToDelete = _currentUserId;
            // Remove from registered accounts
            string emailToRemove = null;
            foreach (var kvp in _registeredAccounts)
            {
                if (kvp.Value.userId == uidToDelete)
                {
                    emailToRemove = kvp.Key;
                    break;
                }
            }
            if (emailToRemove != null) _registeredAccounts.Remove(emailToRemove);

            Logout();
            GameLogger.I(Tag, $"Account and personal data purged for user {uidToDelete} (GDPR / Play Store compliance).");
            onComplete?.Invoke(true, "Account data successfully deleted.");
        }
    }
}

