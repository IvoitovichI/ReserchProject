# Implementation workflows

Use these procedures as checklists, not as permission to expand scope. Adapt names to the existing repository.

## 1. Implement a gameplay feature

1. Write one observable outcome and acceptance criteria.
2. Locate the current definition, domain, runtime and presentation owners.
3. Decide whether the feature changes player opportunities or a research condition.
4. Add the smallest data contract first; avoid hard-coded asset-specific branches.
5. Implement pure rules before Unity integration when practical.
6. Expose typed gameplay events at semantic state transitions.
7. Add telemetry mapping only when the event answers an approved question.
8. Add EditMode rules tests and only necessary PlayMode integration tests.
9. Validate Research Mode restrictions and version impact.

## 2. Add a weapon

1. Create a stable `weapon_id` and `WeaponDefinition`; do not identify it by prefab name.
2. Reuse the existing fire strategy: hitscan, spread hitscan, charged/ray, or projectile/AOE. Add a new strategy only for genuinely new behavior.
3. Configure damage, cadence, range, projectile speed/radius, view prefab, audio/VFX and gameplay collision mask.
4. Route aiming through `AimProvider`; do not raycast from an arbitrary muzzle forward vector.
5. Keep camera motion disabled by Research Mode policy; viewmodel recoil is allowed.
6. Emit `weapon_equipped`, `shot_fired`, and a resolution event with the same correlation ID.
7. Test cooldown, camera/muzzle correction, obstruction, misses, targets, pause, switching and room completion.
8. Re-run every MVP room with the weapon; a weapon must not create an unavoidable softlock.
9. Review balance/content version and opportunity-normalized metrics.

## 3. Add an enemy archetype

1. Define the behavioral role and distinguish it from existing archetypes.
2. Create stable `enemy_id` and definition with health, speed, ranges, telegraph durations, budget cost and allowed room tags.
3. Implement state transitions explicitly: spawn/idle/acquire/telegraph/attack/recover/dead as applicable.
4. Ensure threats outside initial FOV have spatial audio or sufficient telegraph and safe entry distance.
5. Use deterministic decisions where enemy behavior contributes to compared conditions.
6. Emit semantic combat events, not per-frame AI state dumps.
7. Test navigation failure, target loss, pause, repeated damage, duplicate death and room cleanup.
8. Validate spawn points and encounter budget across seed pool.

## 4. Add or change a boss

1. Fix stable boss/phase/attack IDs.
2. Represent phase progression as an explicit state machine with deterministic thresholds.
3. Avoid unavoidable damage, instant off-screen attacks and camera-disorienting effects.
4. Keep reward emission and boss death idempotent.
5. Record phase start, attack choice, damage, kill/death and completion context.
6. Compare boss assignment and expected difficulty across counterbalanced groups.
7. Treat balance/attack changes as likely `content_version` changes.

## 5. Add a passive item

1. Create stable `item_id` and authored definition.
2. Implement a narrow modifier contract; avoid reaching into unrelated controllers.
3. Specify stacking, uniqueness, order of operations and removal behavior.
4. Record offer set, choice/skip and applied modifier IDs.
5. Test deterministic offers, duplicate exclusion and save/run reset.
6. Update opportunity counts: offered items, affordable choices and eligible slots.

## 6. Add a room

1. Follow [room-editor-guide.md](room-editor-guide.md).
2. Assign stable room/content IDs and compatible sockets.
3. Author entries, encounter points, reward points, telemetry zones and optional secrets.
4. Run room validator, NavMesh checks and first-person readability checks.
5. Test with all four weapons and relevant enemy groups.
6. Run batch generation and same-seed reproducibility checks.
7. Advance content version when the room enters or materially changes in a study pool.

## 7. Add a telemetry event

1. State the research or diagnostic question it answers.
2. Prefer an existing semantic gameplay event as source.
3. Define event name, trigger, payload, units, cardinality, required context and opportunity denominator.
4. Decide whether event is transition-based or sampled; never default to every frame.
5. Add a data-only payload and map it to the shared envelope.
6. Add schema/serialization/order/privacy tests and update the data dictionary.
7. Advance `schema_version` if meaning or required fields change.
8. Check that disabling/failing the sink does not alter gameplay.

## 8. Fix a bug

1. Reproduce and capture the smallest failing scenario, seed, room, build/content version and expected/actual behavior.
2. Identify the broken invariant rather than patching the visible symptom.
3. Add a regression test when deterministic and economical.
4. Make the narrowest fix; do not combine it with a redesign.
5. Verify nearby lifecycle cases: disable/destroy, scene reload, retry same seed, pause and duplicate events.
6. Assess whether earlier research data is affected. If semantic interpretation changed, document the affected version range.

## 9. Refactor

1. Establish characterization tests or concrete invariants.
2. Keep serialized fields, stable IDs, public contracts and event meaning compatible unless change is explicit.
3. Use `FormerlySerializedAs` for serialized field renames.
4. Preserve `.meta` files and asset references.
5. Separate mechanical moves from behavior changes when possible.
6. Re-run determinism and telemetry tests if ordering or lifecycle changed.

## 10. Change the experiment protocol

Stop and request confirmation if the change affects seed assignment, condition, participant flow, primary outcomes, session length, comfort settings, consent, data retention, or adaptation. After approval, update config, manifest fields, protocol documentation, validation and versioning together.
