# Telemetry specification

## 1. Purpose and boundary

Telemetry captures an ordered, reconstructable account of meaningful play. It supports offline feature extraction, comparison between controlled seeds and quality control. It is not the game state, an online classification service, or a reason for gameplay to fail.

MVP storage:

- one append-only JSONL event stream per session;
- one session manifest containing assignment, versions and initial settings;
- optional validation/report file generated after collection;
- local export controlled by the researcher.

## 2. Event envelope

Every line is one JSON object. Required fields:

| Field | Type | Meaning |
| --- | --- | --- |
| `event_id` | GUID/string | globally unique deduplication ID |
| `event_name` | string | stable snake_case event type |
| `schema_version` | string/int | meaning/shape version |
| `session_id` | GUID/string | pseudonymous session |
| `participant_id` | string | researcher-assigned code; never a name |
| `run_id` | GUID/string/null | current run if applicable |
| `sequence_no` | long | strictly increasing within session |
| `occurred_at_utc` | ISO-8601 | wall-clock correlation |
| `monotonic_ms` | long | ordering/duration immune to clock changes |
| `build_id` | string | exact player build |
| `content_version` | string | content/balance interpretation |
| `mode` | enum/string | ResearchObservation, Pilot, Sandbox |
| `condition_id` | string/null | assigned condition |
| `context` | object | seed, room, phase and common IDs |
| `payload` | object | event-specific data only |

Example:

```json
{"event_id":"4bc7...","event_name":"shot_fired","schema_version":1,"session_id":"s-...","participant_id":"P017","run_id":"r-...","sequence_no":487,"occurred_at_utc":"2026-09-17T10:12:31.284Z","monotonic_ms":351284,"build_id":"pilot-0.4.1","content_version":"0.4","mode":"ResearchObservation","condition_id":"G1","context":{"seed":184205,"room_id":"combat_l_03","encounter_id":"enc-07"},"payload":{"shot_id":"sh-...","weapon_id":"pulse_pistol","origin":[1.2,1.65,-4.1],"direction":[0.01,-0.04,0.999]}}
```

Use documented coordinate system and units. Prefer numeric arrays for vectors and degrees for angles. Avoid locale-dependent numbers or dates.

## 3. Core event catalog

### Session and settings

- `session_start`, `session_end`, `app_focus_changed`
- `settings_changed`
- `performance_sample`
- `telemetry_recovery`

Manifest/settings include build, consent/protocol version, input device, screen/resolution if approved, FOV, sensitivity, invert-Y and target frame settings. Do not collect hardware serials, account names or IP addresses.

### Run and generation

- `run_configured`, `run_start`, `run_end`, `retry_start`
- `dungeon_generated`, `generation_retry`
- `room_entered`, `room_activated`, `room_cleared`, `room_exited`
- `branch_entered`, `backtrack_detected`

Record assigned seed, actual derived seed if retry occurs, content IDs, graph summary, outcome, reason and duration.

### Exploration

- `position_sample`, `view_sample`
- `telemetry_zone_entered`, `telemetry_zone_exited`
- `clue_observed`, `secret_revealed`, `secret_entered`
- `interaction_completed`

Position/view samples are low-frequency and should support route/view coverage without generating per-frame data. `clue_observed` requires a documented visibility rule, distance/angle threshold and dwell threshold; do not equate one raycast hit with attention.

### Combat

- `weapon_equipped`
- `shot_fired`, `shot_resolved`, `projectile_hit`
- `damage_dealt`, `damage_taken`
- `dodge_started`
- `enemy_killed`, `player_died`
- `boss_phase_started`

Use `shot_id` to correlate fire and resolution. Record stable target/enemy/archetype IDs, distance, aim error where defined, damage, health before/after and threat direction where needed.

### Economy, items and hub

- `reward_granted`, `currency_changed`
- `offer_presented`, `offer_selected`, `offer_skipped`
- `item_acquired`
- `hub_upgrade_purchased`
- `hub_item_placed`, `hub_item_removed`, `hub_action_undone`

Every choice event must preserve the offer set and opportunity counts, not only the selected ID.

## 4. Sampling

- Semantic transitions are emitted once at the source-of-truth transition.
- `position_sample` and `view_sample`: start around 4 Hz; lower if pilot shows adequate reconstruction. Use one scheduler, not one sampler per object.
- `performance_sample`: around 1 Hz with frame-time aggregates, not every frame.
- Flush event buffer approximately every 10 seconds, at room boundary, `run_end`, focus loss/pause and application quit.
- Compression/sequence filtering happens offline after raw collection; preserve original order and segment boundaries.

Exact rates are protocol/config values and must be written to the manifest.

## 5. Ordering, durability and recovery

- Allocate `sequence_no` centrally and monotonically.
- Serialize event data before enqueueing if source objects may mutate.
- Append complete lines; flush so a crash loses a bounded tail rather than the whole session.
- On launch, detect an incomplete previous session and write a recovery marker/report without rewriting original lines.
- Duplicate `event_id` is invalid. Sequence gaps require an explicit recovery/drop marker.
- `session_end` and `run_end` are idempotent; only one terminal event per lifecycle entity.

## 6. Privacy and separation

Allowed identity: researcher-assigned participant code and random session/run IDs.

Forbidden in gameplay logs: real name, email, phone, free-text participant answers, IP address, OS username, machine/device serial, exact personal file path, voice/video, raw questionnaire data.

Keep the re-identification key and questionnaire mapping outside the game export. Logs should be stored under a researcher-controlled location with retention/deletion procedures defined by the study.

## 7. Feature derivation guidance

Raw logs remain the source. Derived data is reproducible and versioned separately.

| Construct | Candidate features | Normalize/control by |
| --- | --- | --- |
| Exploration | optional-room ratio, secret discovery, route entropy, backtrack ratio, normalized path length, view coverage | opportunities, play time, seed, FOV/sensitivity |
| Combat approach | weapon share, mean engagement distance, dodge timing, damage exposure | encounters, enemy types, weapon availability |
| Resource behavior | spend/save ratio, skipped offers, risk before purchase | affordable offers, earned currency, condition |
| Hub expression | cosmetic/function choice ratio, placement revisions | unlocked items, slots, available currency |
| Skill/confounds | hit rate, angular aim error, acquisition latency, clear time, deaths, tutorial errors | prior FPS experience, input device, frame rate |

Do not treat skill features as motivation by default. Prefer continuous values, confidence and test-retest stability across seeds.

## 8. Schema/version rules

- Adding an optional backwards-compatible field may keep the major schema version if documented.
- Changing meaning, units, trigger, required fields or identifier semantics advances schema version.
- Gameplay/content/balance changes affecting opportunities or outcomes advance content version.
- Analysis code records its own feature-pipeline version.
- Never merge sessions across incompatible schema/content versions without an explicit migration or analysis decision.

## 9. Validation gates

The validator checks:

- required fields and enum/ID validity;
- unique event IDs and monotonic sequence/time;
- manifest/event version agreement;
- legal lifecycle order (`session_start`, run boundaries, room boundaries, terminal event);
- correlation pairs such as shot fire/resolution;
- no forbidden fields or accidental absolute user paths;
- known stable IDs and coordinate/unit constraints;
- final flush or explicit recovery marker.

Maintain synthetic sessions with known actions and expected features. Prevent participant leakage when splitting data for ML: all runs from one participant stay in one fold.
