# Room editor and prefab guide

## 1. Goal

The room tool is a Unity `EditorWindow` plus prefab components and validators. It creates verified preset rooms; it is not a runtime level editor or procedural-geometry system.

Each room has a prefab and a `RoomDefinition` with the same stable `room_id` and content version.

## 2. Required authored elements

| Element | Purpose | Validation |
| --- | --- | --- |
| `RoomRoot` | local origin and room bounds | origin/grid convention and bounds valid |
| `DoorSocket` | connection position/direction/type | on grid, outward forward, compatible type, no overlap |
| `PlayerEntry` | possible arrival pose | capsule/head clearance, not inside collider, view not into wall |
| `EnemySpawnPoint` | allowed spawn and tags | on NavMesh, free radius, minimum entry distance |
| `EncounterDefinition` | enemy groups/budget | valid stable IDs and budget range |
| `RewardSpawnPoint` | reward/shop placement | reachable and does not block exit |
| `TelemetryZone` | named area/ROI | unique zone ID, bounds and label |
| `SecretDefinition` | clue/reveal/entry | clue readable at eye height, secret reachable |
| `RoomBounds` | overlap/culling/debug bounds | contains gameplay geometry and sockets |

Optional: cover markers, interest points, boss anchors, audio zones and lighting probes when supported by the actual project.

## 3. RoomDefinition shape

Conceptual fields:

```text
room_id
content_version
prefab
room_type
difficulty_tier
weight
tags[]
sockets[]
encounter_options[]
telemetry_zones[]
secret_definition?
expected_clear_time_range
first_person_validation_data
```

Do not duplicate transform coordinates in ScriptableObject if the prefab component is already the authoritative authored location. Store stable metadata in the definition and validate correspondence.

## 4. First-person constraints

- Target camera height: 1.65 m unless project config changes it.
- Entry must fit the player capsule and provide head clearance with margin.
- Pitch range is approximately -80° to +80°; required clues/interactions must be visible within comfortable pitch.
- Corridors, doors and cover must account for weapon viewmodel and gameplay collision separately.
- Prevent camera clipping into thin walls, low ceilings and decorative meshes.
- Avoid required information only visible from a top-down viewpoint.
- Off-screen enemy activation needs safe entry, spatial audio and/or readable telegraph.
- Secret clues should support a documented observation opportunity; do not hide them behind random VFX or lighting variation in compared seeds.

## 5. Validation levels

### Authoring-time

- missing/duplicate stable IDs;
- prefab/definition version mismatch;
- socket grid, orientation, type and obstruction;
- collider/layer/static flag policy;
- player capsule/head clearance;
- spawn free radius, NavMesh and entry distance;
- required exits/rewards/zones;
- clue angle, distance, line of sight and dwell opportunity;
- room bounds and geometry overlap.

### Batch content

- every room opens in isolated test harness;
- every required weapon can clear it;
- supported encounter groups can spawn and finish;
- all rewards and exits are reachable;
- unique IDs across the content registry.

### Generated dungeon

- start→boss and required rooms reachable;
- socket types compatible and doors unblocked;
- no overlap beyond allowed connectors;
- budget and path-length constraints satisfied;
- assigned seed reproduces identical graph/content IDs;
- 1000-seed pre-lock batch has no softlocks.

## 6. Authoring workflow

1. Duplicate an approved template while preserving only template references, not its stable IDs.
2. Set room ID/type/tier/tags and content version.
3. Block out navigable space around player capsule and camera.
4. Add sockets and all possible entries.
5. Bake/configure navigation using the project's chosen solution.
6. Add spawn points and encounter options; keep immediate threats outside unsafe entry region.
7. Add rewards, telemetry zones and optional secret/clue.
8. Run local validator until there are no errors. Warnings require an explicit design decision.
9. Test isolated room with all weapons and representative enemies.
10. Save prefab/definition, run global ID and generation batches, then include it in the study pool.

## 7. Validator output

Errors block use in research builds. Each result includes severity, rule code, room ID, object path, concise explanation and suggested correction. Do not auto-fix transforms or IDs when that could silently change authored intent.

Useful rule codes: `ROOM_ID_DUPLICATE`, `SOCKET_OFF_GRID`, `SOCKET_BLOCKED`, `ENTRY_CAPSULE_BLOCKED`, `ENTRY_HEAD_CLEARANCE`, `SPAWN_TOO_CLOSE`, `SPAWN_OFF_NAVMESH`, `CLUE_NO_LOS`, `ZONE_ID_DUPLICATE`, `REWARD_UNREACHABLE`, `VERSION_MISMATCH`.

## 8. Content-lock rule

After content lock, geometry, sockets, entry poses, encounter layouts, secrets, clue visibility, rewards or navigation changes require a reviewed content-version change and seed-pool revalidation. Purely cosmetic changes still require review if they can affect visibility, performance or attention.
