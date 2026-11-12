# Indian Bike Life — Game Economy Design Document

**Date:** 2026-11-12  
**Project:** Indian Bike Life (Android 3D Motorcycle Simulator)  
**Document:** In-Game Economy, Progression Formulas & Reward Rules  

---

## 1. Virtual Currency Specification

| Currency | Symbol | Type | Storage Format | Acquisition Methods | Primary Utility |
|---|:---:|:---:|:---:|---|---|
| **Silver Coins** | 🪙 | Soft Currency | 32-bit Integer ($\ge 0$) | Missions, free-roam riding distance, daily login calendar, daily tasks | Standard bikes, bike maintenance, routine gear, customization |
| **Gold Coins** | 🟡 | Hard / Prestige Currency | 32-bit Integer ($\ge 0$) | Weekly challenges, achievement unlocks, 7-day login streaks | Premium cruiser/superbike unlocks, exclusive helmets, rare cosmetic wraps |

### Safety & Integrity Constraints:
1. **Integer Arithmetic:** All currency operations use strict integer amounts. No floating-point values are permitted in wallet balances.
2. **Non-Negative Clamping:** Debits that exceed the current balance return `false` immediately, leaving the wallet balance untouched. Negative balances are physically impossible.
3. **Audit Ledger:** Every transaction generates an immutable `TransactionRecord` with a UTC ISO-8601 timestamp, delta amount, resulting balance, and purpose description.
4. **Idempotency Keys:** Every reward payout generates a unique transaction key (`reward_<type>_<id>_<timestamp>`). Duplicate keys submitted to `RewardManager` or `CurrencyManager` are rejected.

---

## 2. Mission Reward & Payout Matrix

Base payouts are configured per mission and dynamically adjusted based on **Difficulty Multiplier** and **Star Performance**:

$$\text{FinalPayout} = \text{round}\left(\text{BasePayout} \times \text{DifficultyMultiplier} \times \text{StarMultiplier}\right)$$

### Difficulty Multipliers:
- **Beginner:** $0.8\times$ Silver & XP
- **Easy:** $0.9\times$ Silver & XP
- **Normal:** $1.0\times$ Silver, Gold & XP
- **Hard:** $1.4\times$ Silver, $1.2\times$ Gold, $1.4\times$ XP
- **Expert:** $1.8\times$ Silver, $1.5\times$ Gold, $1.8\times$ XP

### Star Performance Multipliers:
- **1 Star (Completed):** $1.0\times$
- **2 Stars (Great Time / Low Damage):** $1.25\times$
- **3 Stars (Pristine $\le 5\%$ Damage + Optional Objectives):** $1.5\times$

### Baseline Mission Rewards Table:
| Category | Sample Mission | Base Silver | Base Gold | Base XP | Target Time |
|---|---|:---:|:---:|:---:|:---:|
| **Village Riding** | Village Milk Run | 500 | 0 | 150 | 90s |
| **City Riding** | Bengaluru Peak Traffic | 1,200 | 2 | 300 | 150s |
| **Highway** | NH44 Highway Sprint | 1,500 | 3 | 350 | 180s |
| **Hill Station** | Ooty 36 Hairpins | 2,500 | 10 | 600 | 240s |
| **Long Distance** | ECR Coastal Endurance | 2,000 | 5 | 500 | 300s |
| **Passenger** | Temple Pillion Escort | 1,000 | 2 | 250 | 160s |
| **Couple Riding** | Sunset Couple Cruise | 1,800 | 5 | 400 | 200s |
| **Navigation** | Hampi Heritage Waypoints | 1,600 | 4 | 380 | 220s |
| **Exploration** | Wayanad Forest Trail | 1,400 | 3 | 320 | 250s |
| **Time Trial** | Pamban Sea Bridge Sprint | 2,200 | 8 | 550 | 90s |
| **Traffic Challenge**| Auto-Rickshaw Slalom | 3,000 | 15 | 750 | 120s |
| **Distance Challenge**| Century Distance Ride | 3,500 | 20 | 800 | Untimed |
| **Parking Challenge**| Highway Dhaba Parking | 800 | 0 | 200 | 60s |
| **Delivery** | Express Chai Delivery | 2,000 | 5 | 480 | 100s |
| **Special Event** | Diwali Night Parade | 2,500 | 12 | 650 | 180s |

---

## 3. Daily & Weekly Progression Rewards

### A. 7-Day Login Streak Calendar
| Day | Streak Name | Silver Coins | Gold Coins | XP | Notes |
|:---:|---|:---:|:---:|:---:|---|
| **Day 1** | Welcome Rider | 500 | 0 | 100 | Basic daily incentive |
| **Day 2** | Fuel Boost | 750 | 2 | 150 | Introduces first Gold coins |
| **Day 3** | Mid-Week Cruise | 1,000 | 5 | 250 | Consistent rider reward |
| **Day 4** | Gear Up | 1,500 | 5 | 400 | Mid-cycle milestone |
| **Day 5** | Weekend Tour | 2,000 | 10 | 600 | High-tier reward |
| **Day 6** | Veteran Rider | 2,500 | 15 | 800 | Escalated payout |
| **Day 7** | Grand Pillion Celebration | **5,000** | **30** | **1,500** | Full streak completion reward |

### B. Daily Tasks (3 Slots Daily)
- **Slot 1 (Distance):** 5,000 meters riding $\rightarrow$ +1,000 Silver, +200 XP
- **Slot 2 (Checkpoints):** 10 checkpoints $\rightarrow$ +1,500 Silver, +300 XP
- **Slot 3 (Passenger):** 2,000 meters with pillion $\rightarrow$ +2,000 Silver, +400 XP

### C. Weekly Challenge Grand Reward
- 100 Weekly Challenge Points $\rightarrow$ **10,000 Silver Coins, 50 Gold Coins, 2,500 XP**.

---

## 4. Player XP Curve & Level Progression

Progression uses a smoothed exponential-linear curve calibrated for mobile engagement:
$$\text{XP}_{\text{delta}}(L) = \text{round}\left(500 \cdot L^{1.25} + 250 \cdot L\right)$$

### Progression Levels Table (Levels 1–15):
| Level | XP Required for Level Up | Cumulative Total XP | Feature / Category Unlock |
|:---:|:---:|:---:|---|
| **1** | 750 XP | 0 XP | Starter 125cc Commuter, Village Riding Mode |
| **2** | 1,440 XP | 750 XP | City Riding Mode, Bengaluru Missions |
| **3** | 2,223 XP | 2,190 XP | Highway Sprint Missions, NH44 Corridor |
| **4** | 3,082 XP | 4,413 XP | Pillion Passenger Selection (Rahul & Priya) |
| **5** | 4,008 XP | 7,495 XP | Cruiser Motorcycle Tier (Royal Enfield 350cc) |
| **6** | 4,994 XP | 11,503 XP | Time Trial Missions, Rameswaram Bridge |
| **7** | 6,036 XP | 16,497 XP | Traffic Challenge Mode, Auto-Rickshaw Slalom |
| **8** | 7,129 XP | 22,533 XP | Couple Riding Mode, East Coast Road Sunset |
| **9** | 8,271 XP | 29,662 XP | Navigation Waypoint Challenges |
| **10** | 9,459 XP | 37,933 XP | Hill Station Ghat Mode, Ooty 36 Hairpins |
| **11** | 10,690 XP | 47,392 XP | Sport Motorcycle Tier (200cc Streetfighter) |
| **12** | 11,962 XP | 58,082 XP | Wayanad Forest Trail & Mud Tracks |
| **13** | 13,274 XP | 70,044 XP | Night Ride Missions & High-Beam Traffic |
| **14** | 14,624 XP | 83,318 XP | Monsoon Downpour Mode & Wet Road Challenges |
| **15** | 16,011 XP | 97,942 XP | Adventure Tourer Tier (Himalayan Class) |

---

## 5. Known Limitations & Stage 4 Boundaries

1. **Local Authoritative vs. Cloud Authoritative:** All wallet balances and transaction ledgers currently operate locally through `SaveManager` with Schema v2 integrity checksums. Stage 4 will connect these interfaces to a backend authoritative ledger.
2. **Anti-Cheat Boundaries:** Client-side monotonic time checks prevent trivial local system clock rolling. However, full financial cheat-proofing will require server receipt validation (Google Play Billing) when IAP is introduced in Stage 4.
3. **Offline Leaderboards:** Leaderboard rankings represent local device records. No simulated online bot profiles are generated to deceive the player.
