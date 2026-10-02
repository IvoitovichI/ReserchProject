# Researcher Handoffs

Добавляй новые записи сверху.

## HANDOFF-RS-001 — Research setup review for vertical slice

- Date: 2026-09-30
- From: Researcher
- To: Orchestrator, User, Programmer
- Related hypothesis/task: Next-milestone research setup; P1 vertical slice
- Status: Proposed — ready for a pilot-only instrumentation slice, not for collection from real participants.
- Outcome:
  - **Facts established.** `DEC-002` requires conditions, IDs, event semantics and schema/build version before field testing. The current contract specifies a pseudonymous event envelope and JSONL + manifest export. The master baseline defines a two-run counterbalanced `ResearchObservation` protocol, fixed research settings, local append-only telemetry and no remote upload; its final research gate also requires approved protocol/consent, content/build/schema lock, seed-pool review, analysis plan and export rehearsal.
  - **Facts established.** The selected vertical-slice candidate is room → combat → reward → exit. No approved primary question/condition or human-subject protocol is present. `references/research/` contains only an index; consent text/version, questionnaire/scoring, analysis plan and approved protocol are explicitly absent. No participant JSONL/CSV was supplied, so no data-quality or statistical result is claimed.
  - **Conflict requiring a decision before implementation.** `METRICS_CONTRACT.md` calls time fields `timestamp_utc`/`session_time_ms` and events `session_started`, `run_started`, `run_ended`, `room_completed`, `weapon_fired`, `hit_dealt`. The master baseline uses `occurred_at_utc`/`monotonic_ms` and `session_start`, `run_start`, `run_end`, `room_cleared`, `shot_fired`, `damage_dealt`. They cannot both be the canonical version-1 schema. Do not alias or silently translate in raw logs; accept one registry/envelope (or record an explicit versioned mapping) before Programmer implementation.
- Contract/schema version: Existing contract has no declared active `schema_version`; proposed implementation must not claim `1.0.0` until the conflict above and user questions are resolved.
- User decisions required (one primary and at most three secondary):
  1. **Primary:** Which single confirmatory question is the study for, including its intervention/condition and one primary outcome? The current RQ1–RQ5 are project working questions, not a selected primary question. Recommendation for the vertical-slice pilot: make the primary objective telemetry feasibility/completeness, not a claim about player motivation.
  2. **Secondary 1:** Which, if any, secondary questions/outcomes (maximum three) will be retained, and are they exploratory or confirmatory?
  3. **Secondary 2:** What participant eligibility, consent/protocol version, allowed pre/post measures and approved stop/discomfort rule apply? These cannot be inferred or collected by the client.
  4. **Secondary 3:** What data-management policy applies: retention period, access/transfer, encryption/backup/deletion and withdrawal procedure? The master explicitly leaves these to an approved plan.
- Minimal safe vertical-slice participant flow (recommendation):
  1. Researcher selects **Pilot** (not ResearchObservation) and supplies a preassigned pseudonymous `participant_id`; client generates `session_id`. No name, contact data, OS username, hardware identifier or free text is accepted/logged.
  2. Show approved consent text/version; if not accepted, write only `consent_recorded { consent_version, accepted:false }` locally when allowed by protocol, then end without gameplay. If accepted, lock config (`build_id`, schema version, `condition_id`, seed and standardized profile where applicable).
  3. Emit, in order, `consent_recorded` → `session_started` → `condition_assigned` → `run_started`; then complete one fixed-seed room/combat/reward/exit run and emit terminal `run_ended` → `session_ended`. The client must offer pause/withdrawal and encode a neutral terminal reason; it must not request a medical explanation.
  4. Write one UTF-8 append-only JSONL and one manifest per session; flush at room/run/session transitions, pause/focus loss and quit. Keep raw logs separate from profile save and questionnaire data. Pilot export remains local and controlled.
- Minimal event/field implementation contract (recommendation, using the current Metrics Contract vocabulary only until canon is resolved):
  - Every event: `schema_version`, UUID/ULID `event_id`, `event_name`, UTC timestamp, monotonic session time, strictly increasing `sequence_no`, pseudonymous `participant_id`, `session_id`, immutable `build_id`, `condition_id`, contextual `run_id`/`seed`, and object `payload`.
  - Slice events: `consent_recorded(consent_version, accepted)`, `session_started(locale, platform, target_fps)`, `condition_assigned(assignment_method)`, `run_started(dungeon_profile_id, starting_weapon_id)`, `room_entered(room_instance_id, room_id, room_type, depth)`, `weapon_fired(weapon_id, shot_id, ammo_cost)`, `hit_dealt(shot_id, weapon_id, target_type_id, damage)`, `damage_taken(source_type_id, amount, hp_after)`, `enemy_killed(enemy_id, enemy_instance_id, weapon_id)`, `room_completed(room_instance_id, duration_ms, damage_taken)`, reward as `currency_earned(amount, reason, balance_after)` when present, `player_died(...)` if applicable, terminal `run_ended(reason, duration_ms, rooms_completed, currency_earned)`, `session_ended(reason, duration_ms, valid_run_count)`, and `telemetry_error(error_code, affected_event_name)`.
  - Required lifecycle/QA: no duplicated `event_id`; sequence and monotonic time strictly increase within session; exactly one terminal `run_ended` per started run and one terminal `session_ended` or explicit recovery marker; all `hit_dealt.shot_id` refer to an earlier `weapon_fired.shot_id`; `room_instance_id` is unique within a run; manifest event count/hash/version/time range agrees with JSONL; no PII/free text. Record missing events as missing, never as zero.
- Pilot feasibility preregistration-lite (recommendation only; not a human-subject hypothesis):

  | field | proposal |
  |---|---|
  | hypothesis_id | `PILOT-TELEM-001` |
  | question | Can one prescribed vertical-slice session produce a schema-valid, reconstructable run log without changing gameplay? |
  | independent variable | None; implementation/QA feasibility check |
  | dependent variable | Session/run lifecycle validity and required-event completeness (each denominator: started pilot session/run) |
  | covariates | build_id, schema_version, seed, condition_id, target_fps/platform; no inference from them |
  | unit of analysis | Session for lifecycle validity; run for run completeness |
  | source | JSONL envelope/events and adjacent manifest |
  | quality criterion | 100% required envelope validity; one legal terminal lifecycle; manifest agreement; zero detected PII for scripted synthetic golden session |
  | confounds | crash/forced quit, telemetry I/O error, unfinalized naming conflict, unverified build integration |
  | exclusions fixed before review | Exclude only sessions with `accepted:false`, withdrawal before `run_started`, or `technical_abort` that lacks a recoverable complete raw lifecycle; report excluded count/reason separately and do not delete raw logs |

- QA evidence: Read-only review of `AGENTS.md`, `agents/researcher/AGENTS.md`, master specification, decisions, task board, blockers/needs, current metrics contract and research reference index. No Unity project/build inspection, code execution, synthetic log or real participant dataset was available; implementation and schema conformance are unverified.
- Privacy/validity risks: (1) unresolved event/field registry conflict; (2) no selected primary outcome or intervention; (3) no approved consent/retention/withdrawal procedure; (4) vertical slice does not establish two-seed stability or motivation claims; (5) inconsistent terms Pilot/ResearchObservation must be visible in `mode` and excluded from any later main-study aggregation unless protocol permits otherwise.
- Requested next action: User selects the primary and up to three secondary questions and approves protocol/data-management inputs. Researcher then issues an accepted versioned contract plus golden-log expectations; Programmer implements only that canonical contract and returns a synthetic JSONL+manifest for validation.

## HANDOFF-RS-000 — Шаблон

- Date:
- From: Researcher
- To:
- Related hypothesis/task:
- Status: Proposed
- Outcome:
- Contract/schema version:
- Events/fields/metrics:
- QA evidence:
- Privacy/validity risks:
- Requested next action:
