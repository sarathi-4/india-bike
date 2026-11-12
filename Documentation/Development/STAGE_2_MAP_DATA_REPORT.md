# Stage 2: Indian Map Data & Geographical Architecture Report
**Project:** Indian Bike Life (Android 3D Motorcycle Simulator)  
**Author:** Senior Unity Game Architect  
**Date:** 2026-11-12  

---

## 1. Geographical Data Sources & Licensing

To adhere strictly to commercial game distribution ethics, copyright laws, and the project guidelines:
1. **No Google Maps Extraction**:
   - Google Maps vector, satellite, or elevation data extraction is strictly prohibited and was **not** used. No unauthorized scraping was performed.
2. **Permissible Geographical Sources**:
   - Road network topologies and waypoint vectors are modeled after publicly available **OpenStreetMap (ODbL)** geographic schemas.
   - Elevation data respects open public elevation models (such as **NASA SRTM 30m** elevation benchmarks).
   - In-game GPS positions are processed through `GPSPositionSimulator`, which maps local engine vectors to geographic coordinates mathematically without external API calls or user data privacy exposure.

---

## 2. National Catalogue Coverage

The national architecture in [IndiaMapCatalogue.asset](file:///run/media/sarathi/2-250/indianbike/Assets/Game/MapSystem/Data/IndiaMapCatalogue.asset) registers **all 28 States and 8 Union Territories**:

### 28 Indian States Registered:
1. Andhra Pradesh (Priority State)
2. Arunachal Pradesh
3. Assam
4. Bihar
5. Chhattisgarh
6. Goa
7. Gujarat
8. Haryana
9. Himachal Pradesh
10. Jharkhand
11. Karnataka (Priority State)
12. Kerala (Priority State)
13. Madhya Pradesh
14. Maharashtra
15. Manipur
16. Meghalaya
17. Mizoram
18. Nagaland
19. Odisha
20. Punjab
21. Rajasthan
22. Sikkim
23. Tamil Nadu (Priority State)
24. Telangana (Priority State)
25. Tripura
26. Uttar Pradesh
27. Uttarakhand
28. West Bengal

### 8 Union Territories Registered:
1. Andaman and Nicobar Islands
2. Chandigarh
3. Dadra and Nagar Haveli and Daman and Diu
4. Delhi NCR
5. Jammu and Kashmir
6. Ladakh
7. Lakshadweep
8. Puducherry

---

## 3. Priority States & Implemented Regional Data

The five priority states have deep structured hierarchies down to destinations and road environments:

### 1. Tamil Nadu
- **Ooty Hill Ghat**: 36 hairpin bends through eucalyptus ridges (Elevation: 2,240m).
- **ECR Coastal Highway**: Scenic seaside road from Chennai to Mahabalipuram (Elevation: 10m).
- **Kodaikanal Ghat**: Misty pine forests and lake loop (Elevation: 2,133m).
- **Yercaud Loop**: 20 winding hairpin bends through the Shevaroy Hills (Elevation: 1,515m).
- **Pamban Sea Bridge**: Ocean expressway leading to Rameswaram Island (Elevation: 5m).

### 2. Kerala
- **Munnar Tea Estate Ghat**: Rolling emerald hill roads and fog corridors (Elevation: 1,600m).
- **Thamarassery Ghat Pass (Wayanad)**: 9 hairpin bends under dense rainforest canopy (Elevation: 900m).
- **Varkala Cliff Road**: Coastal road overlooking the Arabian Sea (Elevation: 30m).

### 3. Karnataka
- **Coorg Coffee Trails**: Winding roads through coffee plantations (Elevation: 1,150m).
- **Hampi Boulder Highway**: Historic road cutting through granite boulders (Elevation: 467m).

### 4. Andhra Pradesh
- **Araku Valley Ghat**: Eastern Ghats curves through tribal valleys (Elevation: 910m).

### 5. Telangana
- **Hyderabad Outer Ring Road**: 8-lane modern expressway (Elevation: 540m).

---

## 4. Playable Road Coverage in Stage 2

In `RegionalRideTest.unity`, a composite regional road segment is fully rideable:
- **Highway Corridor (NH44)**: 2-lane smooth asphalt road with left-hand traffic markings.
- **Village Road Section**: Narrower tarmac with roadside dhaba stalls, huts, and banyan trees.
- **Hill-Road Curve Section**: Elevated banked turns with road sign warnings.
- **Intersection**: T-junction with traffic lights and branching routes.

---

## 5. Future Map Expansion Roadmap (Stage 3+)

- **Streaming OpenStreetMap GeoJSON**: Procedural road mesh generation from licensed GeoJSON vectors.
- **Additive Scene Chunks**: As the player travels from Chennai to Ooty, intermediate districts will stream in via `MapStreamingManager`.

