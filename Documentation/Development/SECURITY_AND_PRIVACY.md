# Security and Privacy Specification

**Date:** 2026-11-12  
**Project:** Indian Bike Life (Android 3D Motorcycle Simulator)  
**Target Scope:** Stage 4 (Parts 26–29)  

---

## 1. Security Overview

**Indian Bike Life** implements a defense-in-depth security model to safeguard player identities, secure virtual currency transactions, and preserve fair play across online multiplayer rides.

---

## 2. Authentication & Credential Storage

- **No Plaintext Passwords:** User passwords are never transmitted or stored in plaintext. They are hashed using PBKDF2 with SHA-256 and salted with an application-wide salt combined with the lowercase user email:
  $$\text{Hash} = \text{SHA256}(\text{Password} + \text{GlobalSalt} + \text{UserEmail})$$
- **Session Tokens:** Sessions use HMAC-signed tokens (`<Base64Payload>.<HMAC-SHA256>`). Tokens have an automatic 7-day expiration timestamp verified locally and server-side.
- **Account Deletion (Google Play & GDPR Compliance):**
  - Players can initiate complete account deletion via `AuthenticationManager.DeleteAccount`.
  - Account deletion purges profile data, friend links, club memberships, and transaction audit logs from active database collections.

---

## 3. Economy & Anti-Tamper Security

- **Server-Authoritative Economy:** Virtual wallet balances are never blindly trusted from client modifications.
- **Transaction HMAC Validation:**
  - Every transaction request carries an HMAC signature computed over `transactionId:amount:currency`.
  - If a player attempts memory editing (e.g., CheatEngine/GameGuardian) to inflate rewards or coin credits, `PlayerDataManager.ValidateTransaction` rejects the signature mismatch and logs the incident.
- **Save File Tampering Protection:** Local save files enforce SHA-256 payload checksum validation (`SaveData.ComputeChecksum()`). Corrupted or modified local saves are rejected.

---

## 4. Multiplayer Anti-Cheat

- **Anti-Speedhack & Anti-Teleportation:**
  - Maximum motorcycle speed is physically capped at ~145 km/h. To accommodate network latency and physics extrapolation, the network threshold is set at $48.0\text{ m/s}$ ($172.8\text{ km/h}$).
  - Any position update where `distanceJump > 30.0f` and implied speed exceeds $48.0\text{ m/s}$ is rejected by `NetworkPlayerController`, preventing teleportation and wall-hack exploits.
- **Host Migration Integrity:** Only the verified room host can trigger `StartRide()`, and only when all slots in the room report a `Ready` state.

---

## 5. Player Privacy & Communication Safety

- **Profile Privacy:** Profiles support `Public`, `FriendsOnly`, and `Private` settings. Non-friends viewing `FriendsOnly` profiles have total riding mileage and progression levels automatically hidden.
- **Chat Moderation:**
  - Client-side regex filters abusive language, obscenities, and scam keywords, replacing them with asterisks (`***`).
  - Rate limiting caps chat messages to 2 per second (`0.5s` cooldown), mitigating chat spam and DDoS on mobile screens.
- **Player Mute & Reporting:**
  - Any player can be muted instantly via `PlayerMuteManager`.
  - Harassment reports are submitted through `PlayerReportManager` with target user ID, category, and timestamp.

