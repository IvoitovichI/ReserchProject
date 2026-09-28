# Unity architecture

## 1. Цели архитектуры

- детерминированная генерация и воспроизводимые условия;
- разделение authored data, runtime state, presentation и telemetry;
- возможность тестировать правила без загрузки сцены;
- явный lifecycle session/run/room;
- безопасное расширение контента без `switch` по каждому asset;
- локальная работа без backend-зависимости.

Перед реализацией сверяйся с фактической структурой проекта. Имена ниже — целевая модель, а не разрешение создавать дубликаты существующих систем.

## 2. Слои и направление зависимостей

```text
Definitions ──> Domain ──> Runtime ──> Presentation
                    │          │
                    └──── events ────> Telemetry
Bootstrap/Research configuration composes all layers.
```

- **Definitions**: `ScriptableObject` assets и сериализуемые config types.
- **Domain**: plain C# rules, seeds, graph, damage calculation, modifiers, feature formulas.
- **Runtime**: Unity adapters, state machines, scene lifecycle, spawning, persistence.
- **Presentation**: camera rig, weapon view, VFX, audio, UI.
- **Telemetry**: typed event capture, envelope, JSONL sink, manifest, validation/export.
- **Editor**: Room authoring, validators и batch tests; не зависит от runtime telemetry.

Запрещённые направления: Domain → MonoBehaviour, gameplay → concrete JSON writer, Definitions → scene objects, Telemetry → gameplay decision.

## 3. Сцены

| Сцена | Ответственность |
| --- | --- |
| `Bootstrap` | Composition root, persistent services, mode/config selection, scene transitions |
| `Frontend` | Participant/session setup, consent gate, settings calibration |
| `Hub` | Fixed slots, currency spending, loadout/progression presentation |
| `Dungeon` | Runtime assembler, player, combat, room lifecycle, boss |
| `RoomAuthoring` | Editor-only шаблон проверки prefab; не включать в player flow |

Не делать менеджеры persistent автоматически. Объект переживает смену сцены только если его lifecycle действительно равен приложению/session.

## 4. Рекомендуемые assembly boundaries

```text
Game.Definitions
Game.Domain
Game.Runtime
Game.Presentation
Game.Telemetry
Game.Editor
Game.Tests.EditMode
Game.Tests.PlayMode
```

Если проект ещё мал, не дробить его механически. Добавлять asmdef, когда граница уменьшает нежелательные зависимости или ускоряет тестирование. `Game.Editor` никогда не должен входить в runtime player build.

## 5. Основные контракты

### Application/research

- `GameBootstrap`: строит composition root и запускает flow.
- `GameFlowController`: явные состояния frontend, hub, loading, dungeon, results.
- `ExperimentManager`: читает `SessionConfig`, назначает condition и seed order; не классифицирует игрока online.
- `ResearchModePolicy`: фиксирует разрешённые settings/features для режима.
- `SessionManifestWriter`: один раз фиксирует immutable session context и изменения настроек отдельными событиями.

### First-person player

- `FirstPersonMotor`: CharacterController movement, gravity, strafe, dash. Не управляет камерой, оружием или телеметрией.
- `PlayerLookController`: yaw тела, pitch camera root, sensitivity/invert-Y, cursor lock.
- `AimProvider`: camera ray, aim point, near-muzzle obstruction и corrected shot direction.
- `PlayerCombatController`: equip/fire orchestration и damage pipeline.
- `InteractionController`: screen-center raycast, focus state и interaction command.
- `PlayerHealth`: runtime health и typed damage/death events.

### Dungeon

- `DungeonGraphBuilder`: pure deterministic graph from seed/config.
- `DungeonBudgetPlanner`: budgets/tags/reward/boss constraints.
- `DungeonAssembler`: instantiates selected prefabs and connects sockets.
- `DungeonValidator`: graph reachability, socket compatibility, blocked door, required room checks.
- `RoomController`: state machine `Unvisited → Entered → Active → Cleared`.
- `DoorController`: reacts to room state; does not decide combat completion.
- `EncounterDirector`: spawns pre-authored encounter groups under the assigned budget.

### Content

- `RoomDefinition`, `WeaponDefinition`, `EnemyDefinition`, `BossDefinition`, `ItemDefinition`, `HubUpgradeDefinition` are authored immutable definitions.
- `RuntimeWeapon`, modifiers, health, cooldowns and inventory are per-run state, not stored back into assets.
- Stable IDs are serialized explicitly and validated for uniqueness.

### Telemetry

- `IGameEventSource<T>` or typed C# events publish semantic facts.
- `TelemetryCollector` maps events to data-only payloads.
- `EventEnvelopeFactory` adds order/time/version/context.
- `ITelemetrySink` accepts envelopes; MVP implementation is `JsonlTelemetrySink`.
- `TelemetryFlushController` flushes on interval, room boundary, `run_end`, pause/quit.
- `SessionManifestWriter` writes configuration separately from high-volume stream.

Gameplay may run with a `NullTelemetrySink`; failure to write telemetry must not change gameplay outcome.

## 6. Determinism

- Create an explicit run RNG service from the assigned seed. Do not depend on ambient `UnityEngine.Random` state.
- Derive named substreams (`graph`, `rooms`, `encounters`, `loot`, `boss`) so adding a cosmetic random call does not reshuffle the dungeon.
- Sort candidate collections by stable ID before seeded selection. Never rely on asset discovery or dictionary enumeration order.
- Persist seed, derived-seed policy, content version, chosen definition IDs and generation retry count.
- On invalid generation, use a deterministic derived seed and record the retry. Do not silently generate a different layout.

## 7. Room and combat flow

```text
Player crosses entry trigger
→ RoomController enters Active
→ doors lock and encounter starts
→ enemy lifecycle events update remaining count
→ room emits RoomCleared once
→ doors unlock and rewards become available
→ telemetry records semantic transitions
```

Idempotency is required. Re-entering, disabling an enemy, duplicate death events, pause, or scene unload must not emit `room_clear` or rewards twice.

## 8. Aiming pipeline

1. Cast from gameplay camera through reticle using gameplay collision mask.
2. Resolve aim point at hit or configured maximum distance.
3. For hitscan, evaluate along camera ray unless the design explicitly models muzzle obstruction.
4. For projectile, cast from muzzle to aim point. If blocked near weapon, target the blocking point.
5. Spawn visual projectile from muzzle using corrected direction.
6. Record camera ray, muzzle position, target/hit IDs and resolution as separate fire/resolution events when applicable.

Weapon recoil animation may move the viewmodel, but Research Mode must not move the gameplay camera through shake or dynamic FOV.

## 9. State and persistence

- Session state: participant pseudonym, condition, build/schema/content versions, assigned run order.
- Run state: seed, current room, health, inventory, currency earned, chosen content IDs.
- Profile state: hub unlocks/currency outside standardized research profiles.
- Exported research data: append-only JSONL + manifest; it is not a gameplay save.

Use versioned DTOs for disk formats. Validate before load; use atomic replacement for mutable saves. Profile reset affects only the selected test profile and never exported logs.

## 10. Folder model

```text
Assets/_Project/
  Art/
  Audio/
  Definitions/
  Prefabs/
    Characters/
    Rooms/
    Weapons/
  Scenes/
  Scripts/
    Bootstrap/
    Domain/
    Dungeon/
    Player/
    Combat/
    Hub/
    Telemetry/
    UI/
  Editor/
  Tests/
    EditMode/
    PlayMode/
```

Follow the current project if it already has a coherent structure. Do not reorganize the repository as collateral work.
