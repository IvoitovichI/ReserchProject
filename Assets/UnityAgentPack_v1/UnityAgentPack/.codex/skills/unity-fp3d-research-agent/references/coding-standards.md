# C# and Unity coding standards

## 1. Naming and layout

- Types, methods, properties, events: `PascalCase`.
- Private fields: `_camelCase`; serialized private fields remain `_camelCase`.
- Parameters/locals: `camelCase`.
- Interfaces: `IName`; abstract bases only when shared behavior is real.
- Boolean names express a fact: `IsAlive`, `CanFire`, `HasReward`.
- Event names describe completed facts: `RoomEntered`, `DamageApplied`, `RunEnded`.
- One primary public type per file; filename matches type.
- Namespace follows module, not folder depth accidentally: `DungeonTrace.Dungeon`, `DungeonTrace.Telemetry`.

## 2. Serialization and inspector

Prefer:

```csharp
[SerializeField, Min(0f)] private float _cooldownSeconds = 0.25f;
public float CooldownSeconds => _cooldownSeconds;
```

Avoid public mutable fields. Validate authored values in `OnValidate` or a dedicated validator. Do not perform gameplay side effects in `OnValidate`, constructors, or property getters.

Use `FormerlySerializedAs` when renaming an existing serialized field. Never delete or regenerate `.meta` to fix a code issue.

## 3. Unity lifecycle

- `Awake`: validate/cache owned components and establish local invariants.
- `OnEnable`/`OnDisable`: symmetric subscriptions for enabled-object lifecycle.
- `Start`: interactions requiring the composed scene after all `Awake` calls.
- `Update`: input/presentation that must run per frame.
- `FixedUpdate`: only physics work tied to fixed timestep.
- `LateUpdate`: camera/viewmodel follow when ordering requires it.
- `OnDestroy`: release resources whose lifetime is the object, not merely enabled state.

Do not assume lifecycle ordering between unrelated GameObjects. Establish order through the composition root or explicit initialization.

## 4. Dependencies

Preferred order:

1. constructor injection for plain C#;
2. explicit `Initialize` for composed Unity objects when necessary;
3. serialized references within a prefab/scene boundary;
4. stable registry for authored definitions.

Avoid `FindObjectOfType`, `GameObject.Find`, `Resources.Load` by dynamic string, and broad service locators in runtime paths. Cache component references; use `TryGetComponent` at uncertain boundaries.

## 5. Events

Use typed C# events or narrow channels. The publisher owns the event; subscribers cannot invoke it.

```csharp
public event Action<RoomClearedEvent>? RoomCleared;
```

For Unity versions without nullable reference types enabled, omit `?` and keep null-safe invocation. Subscribe and unsubscribe in matching lifecycle methods. Event payloads are immutable data and carry stable IDs, not scene references, when they cross module boundaries.

## 6. Data and runtime state

- `ScriptableObject` definitions contain authored constants and references.
- Copy or instantiate mutable runtime data; never decrement ammo/health directly in a shared asset.
- Prefer readonly structs/records for small payloads where supported by the project's language level.
- Keep serialization DTOs separate from rich runtime objects.
- Never serialize `Transform`, `GameObject`, instance ID, localized label, or display name as a research identifier.

## 7. Time, movement, and randomness

- Frame-dependent gameplay uses `deltaTime`; physics uses fixed time.
- Pause-sensitive and pause-insensitive timers must be explicit.
- Cooldowns compare against one chosen time source; tests should be able to substitute it if logic is non-trivial.
- Deterministic generation uses the project RNG/substreams, not ambient `UnityEngine.Random`.
- Visual-only noise may use non-deterministic random only if it cannot affect collisions, AI, timing, rewards, or telemetry semantics.

## 8. Performance

- Do not allocate LINQ closures, strings, arrays, or new collections every frame in hot paths.
- Do not emit high-volume `Debug.Log` in builds or per-frame loops.
- Reuse physics buffers for frequent overlap queries when profiling shows need.
- Use pooling for repeated projectiles/VFX only after lifecycle is clear; pooling must reset subscriptions/state.
- Profile before large optimization. Preserve clarity in non-hot code.

## 9. Errors and diagnostics

- Invalid required configuration should fail early with asset/type/stable-ID context.
- Expected absence uses `Try...` or a result type; programmer errors should not be silently swallowed.
- Catch exceptions only where recovery or additional context is possible.
- Telemetry IO errors are reported and isolated from gameplay. Never recursively log telemetry failures into the same sink.
- Do not use an empty catch or replace a reproducibility failure with a random fallback.

## 10. Coroutines, async, and cancellation

- Use coroutine for Unity-frame sequencing tied to a component lifecycle.
- Use `async` only when the project already has a supported pattern and cancellation/error ownership is defined.
- Cancel work on scene/object teardown. Do not use `async void` except Unity/UI event entry points with explicit error handling.
- Loading transitions must prevent duplicate starts and emit one state transition.

## 11. Comments and documentation

Document why a constraint exists, especially research/determinism rules. Do not paraphrase obvious code. XML docs are useful for public contracts, data schemas, derived-seed rules and units.

Include units in names or docs: `DurationMs`, `DistanceMeters`, `CooldownSeconds`.

## 12. Tests

- Test plain C# rules in EditMode without loading scenes.
- Use PlayMode for Unity lifecycle, scene flow, input/cursor, camera ray, prefabs and collisions.
- Name tests as behavior: `Build_SameSeedAndVersion_ProducesSameGraph`.
- Fix the clock and RNG in tests. Avoid assertions dependent on frame rate or unordered enumeration.
- A regression fix includes a failing test when the defect is deterministic and testable at reasonable cost.
