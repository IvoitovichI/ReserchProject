# Testing and Definition of Done

## 1. Verification matrix

| Level | What belongs here |
| --- | --- |
| EditMode | deterministic graph/budgets, damage/modifier rules, unique IDs, serialization, schema validation, feature formulas |
| PlayMode | scene flow, cursor/pause, camera/aim, doors/room lifecycle, death/retry, prefab interactions, flush on transitions |
| Content batch | room rules, NavMesh/clearance/LoS, all weapons, supported encounters, 1000 generated seeds |
| Build smoke | clean-PC launch without Editor, manifest/export paths, full session, quit/crash recovery |
| Performance | target hardware frame-time, memory across two runs, telemetry on/off comparison, GC spikes |
| Human pilot | controls/telegraphs, sensitivity, motion discomfort, session duration, no researcher assistance |
| Data QA | synthetic known sessions, event order, expected features, participant-safe splits |

## 2. Core automated scenarios

### Determinism

- same seed + same content version ⇒ identical graph and selected stable IDs;
- different cosmetic random calls do not change gameplay substreams;
- invalid layout retry uses deterministic derived seed and is recorded;
- candidate enumeration order does not alter selection.

### FP player/combat

- cursor locks/unlocks on play/pause/focus change;
- yaw/pitch clamps and settings are applied/recorded;
- camera ray and projectile muzzle correction agree;
- close obstruction prevents firing through cover;
- cooldown/dash invulnerability are frame-rate independent;
- duplicate damage/death cannot duplicate rewards or room clear.

### Room/dungeon

- room state transitions only through legal sequence;
- clear unlocks doors once;
- every selected definition has a valid prefab and version;
- start/boss/reward requirements hold;
- retry same seed recreates the intended layout/profile state.

### Telemetry

- event envelope has all required fields;
- sequence is monotonic and IDs unique;
- session/run/room terminal events are idempotent;
- periodic and boundary flush preserve complete JSONL lines;
- sink failure does not alter gameplay;
- no forbidden participant/device/path fields;
- synthetic session produces expected normalized features.

## 3. Research build DoD

- Build launches on a clean target PC without Unity Editor.
- Researcher assigns a participant code and sees the assigned counterbalanced seed order before play.
- Consent/protocol gate is completed as required by the study.
- Research Mode fixes or records every relevant setting: FOV, sensitivity, invert-Y, input device and performance.
- Same seed/content version reproduces graph, item offers and boss assignment.
- Two balanced seeds meet agreed path length, budget and choice-opportunity tolerances.
- Alt+F4, death, normal completion and retry do not corrupt JSONL.
- Telemetry can be disabled/replaced by a null sink without gameplay failure.
- Profile reset affects the selected test profile only and does not delete exported logs.
- Data dictionary covers every field and unit.
- Pilot and main-study builds have distinct build IDs/export locations.
- After content lock, gameplay/content changes require a new content version and revalidation.

## 4. Feature-specific DoD

A gameplay feature is complete when:

- acceptance criteria are observable;
- definition/runtime/presentation responsibilities are separated;
- invalid configuration is diagnosed;
- lifecycle cleanup is correct;
- relevant tests pass;
- Research Mode restrictions still hold;
- telemetry/opportunity impact is documented;
- content/schema version impact is decided.

A content asset is complete when:

- stable ID is unique;
- validator has no errors;
- it works in isolated harness and generated context;
- it is compatible with allowed weapons/enemies;
- it is included only in appropriate content versions/seed pools.

A telemetry change is complete when:

- a research/QA question justifies it;
- trigger and payload are unambiguous;
- units, IDs, cardinality and denominator are documented;
- schema/order/privacy tests pass;
- offline parser/feature code is updated if necessary;
- gameplay remains functional when the sink fails.

## 5. Evidence in agent handoff

Report commands/tests actually run and their result. If Unity Editor, package restore, platform build, profiler or human pilot could not be run, state that directly. Do not replace evidence with “should work.” Include reproduction steps for remaining manual checks.

## 6. Stop conditions

Pause and ask the user when:

- existing repository architecture conflicts with this target architecture in a way that requires migration;
- a task changes the study condition, seed assignment, primary outcomes or data handling;
- a package/Unity upgrade is required;
- a serialized migration risks asset loss;
- the bug cannot be reproduced and multiple fixes imply materially different behavior;
- a test would overwrite or delete participant data.
