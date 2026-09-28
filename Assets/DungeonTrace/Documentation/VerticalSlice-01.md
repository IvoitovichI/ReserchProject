# Dungeon Trace — Vertical Slice 01

## Status

Implemented: a playable FP3D prototype room, a three-room walkthrough, game-session bootstrap, basic research-session policy, health, camera-centre interaction, prefab-based room authoring/validation, and deterministic logical dungeon generation. Unity version and packages were not changed.

## How to run

Open `Assets/Scenes/SampleScene.unity` and enter Play mode. `GameBootstrap` creates the prototype content after the scene loads. For the authored walkthrough, open `Assets/Scenes/DungeonWalkthrough.unity`; see [DungeonWalkthrough.md](DungeonWalkthrough.md).

| Input | Result |
| --- | --- |
| WASD / left stick | Move the CharacterController player |
| Mouse / right stick | Look around |
| E | Interact with the yellow trace console in the room |

The console changes between yellow and cyan, proving that the interaction ray from the centre of the player camera reached an `IInteractable` target.

## Runtime structure

`DungeonTrace.Runtime` is the gameplay assembly. Its dependencies flow from domain definitions and rules to Unity runtime components:

`Domain` → `Flow`, `Health`, `Player`, `Interaction`, `Rooms` → runtime presentation

`GameBootstrap` owns the composition root. In `SampleScene` it starts a deterministic `SessionConfig` (seed `12345`, content version `prototype-01`), creates the prototype room and player, and sends player death to `GameFlowController`. In `DungeonWalkthrough` it starts `prototype-02` and places the player before the first authored room.

## Current components

- `SessionConfig` and `ResearchModePolicy`: session mode, seed and the no-adaptation policy for `ResearchObservation`.
- `HealthState` / `Health`: bounded damage state and Unity lifecycle/events.
- `PlayerInputReader`: the only device-polling component; it emits per-frame move, look and interaction commands. The current prototype still contains a temporary jump action, which conflicts with the MVP movement policy and is not part of the documented walkthrough controls.
- `FirstPersonMotor`, `PlayerLookController`, `AimProvider`: CharacterController movement, look and centre-camera aim ray.
- `InteractionRaycaster` / `IInteractable`: raycast interaction. The trace console is the current placeholder target.
- `RoomDefinition` / `RoomBuilder`: authored room data and its placeholder geometry constructor. This supports manually authored rooms; it is not a dungeon generator.
- `RoomRoot`, `DoorSocket`, `PlayerEntry`, `EnemySpawnPoint`, `CoverMarker` / `LoSMarker`, `RewardSpawnPoint`, `TelemetryZone`, and `RoomBounds`: prefab markers for future room content. They do not implement combat, rewards, or telemetry collection.
- `RoomValidator`: validates authored room identity/versioning, sockets, player space, spawn placement, telemetry IDs, and bounds. Open **Dungeon Trace > Rooms > Room Authoring** to create a safe placeholder prefab, scan its markers, and inspect validation results.
- `IDamageable`, `DamageContext`, `DamageResult`, `Health`, `WeaponDefinition`, and `WeaponController`: the first Phase 4 combat slice. `GameBootstrap` currently equips a runtime placeholder Pulse Pistol (left mouse/right trigger; 20 energy damage; 0.25 s interval; 35 m camera-ray range). A scene target must implement `IDamageable` for the shot to affect it.

## Tests

- EditMode: health clamping, research-mode policy, game-flow transition, room-definition validation, and room-validator rules.
- PlayMode: health death event, camera-centre interaction, room construction, and an isolated-room validation test.

Run them from Unity Test Runner after the Editor has imported changes.

## Deterministic dungeon generation

`DungeonGenerationConfig` defines the room/path limits, budgets, allowed types, optional branch chances and reward policy. `DungeonGraphBuilder` builds the logical graph first; it records `OriginalSeed`, derived `ActualSeed`, and attempt. `DungeonValidator` reports reachability, required rooms, socket availability, path length and budget errors. `DungeonAssembler` is the Unity-only presentation stage.

Primitive authoring assets are in `Assets/DungeonTrace/GeneratedDungeon`: `Room_Empty`, `Room_Table`, and `Room_Crypt`, with typed `RoomDefinition` assets and `DungeonGenerationConfig`. The generated geometry is `prototype-02` and must not be compared with `prototype-01` runs.

`DungeonGenerationTests` also verifies that the same seed and definition order produce the same graph, retry seeds are reproducible, mandatory rooms are reachable, and 1,000 generated logical graphs pass the current validator. This closes the Phase 3 gate for the placeholder vertical slice; it does not yet establish research-ready content coverage or telemetry export.

## Deliberately deferred

Encounters, secret-room mechanics, items, AI, weapons, hub, and full telemetry remain outside this slice. Phase 4 begins with shared damage contracts and a minimal Pulse Pistol implementation.

## Authored room content version

`RoomDefinition.content_version` and `RoomRoot.contentVersion` must match. Changing room geometry, spawn layout, balance, or other authored content that affects research metrics requires a new `content_version`. The generated placeholder uses `prototype-01`; it does not change the session-wide prototype content version by itself.
