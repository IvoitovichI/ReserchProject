# Orchestrator Handoffs

Добавляй новые записи сверху.

## HANDOFF-ORCH-001 — DT-000 setup consolidation and next safe milestone

- Date: 2026-09-30
- From: Orchestrator
- To: User; Researcher; Programmer; Game Designer; 3D Modeler
- Status: Done with limitations; no gameplay, asset, scene, package, or runtime-code change was made.
- Decision: DT-000 is complete as an inventory/setup task. The next milestone is **not** broad gameplay implementation. It is DT-001: approve one canonical, versioned telemetry contract and pilot-only research inputs, then implement the isolated authored loop `room → combat → explicit reward claim → exit`.
- Verified project baseline:
  - Repository root: `C:\Users\Ivan\GameForStudy`.
  - Unity Editor: `6000.3.19f1`; URP `17.3.0`; Input System `1.19.0`; Test Framework `1.6.0`.
  - Runtime, generated rooms, combat/enemy prototypes, deterministic-generation code and EditMode/PlayMode test assemblies already exist. Build Settings currently includes only `SampleScene`.
- Scope / out of scope:
  - In: inventory, requirements consolidation, pilot telemetry and one controlled authored loop.
  - Out: package/pipeline/Input System changes; research-condition changes; multi-room generation, boss, secret, hub economy, final art, or telemetry implementation before schema decision.
- Role handoffs:
  - Game Designer: [HANDOFF-GD-001](game-designer.md) proposes the controlled room `vs_combat_01`, two `pursuer`, explicit reward and exit, with acceptance criteria and stable IDs.
  - Researcher: [HANDOFF-RS-001](researcher.md) identifies the canonical schema conflict and proposes pilot feasibility question `PILOT-TELEM-001`; no participant collection is authorized.
  - 3D Modeler: [HANDOFF-3D-001](modeler.md) defines neutral blockout asset IDs and FPS constraints; user visual references remain required for final art.
  - Programmer: inventory found legacy telemetry without a facade/gameplay integration, incomplete research identifiers, and unverified Unity compilation/test execution in this turn.
- Research and privacy impact: No participant data was collected. Any later Pilot must keep pseudonymous IDs only, explicit consent/withdrawal, local append-only JSONL plus manifest, and must not make motivational or condition-effect claims.
- Version impact: No `content_version` or `schema_version` changed. `BLK-002` prevents selecting/implementing a schema version. `BLK-003` prevents treating current placeholder content as a comparability baseline.
- Checks run: UTF-8 review of project/coordination documents; `ProjectVersion.txt`; `Packages/manifest.json`; source/test/scene inventory; `git status --short`; `git diff --check` reported clean by role review.
- Checks not run: Unity compile, Test Runner and build were not successfully run in this turn. Unity CLI status was inaccessible in the sandbox; historical test notes are not a current verification.
- Risks / exact next step: User supplies one primary and up to three secondary research questions, approved consent/retention/withdrawal constraints, and confirms whether `METRICS_CONTRACT.md` is the canonical v1 vocabulary. Researcher then produces the accepted versioned contract and Programmer can implement/test it before the authored combat slice.

## HANDOFF-ORCH-002 — User research-question integration

- Date: 2026-09-30
- From: Orchestrator
- To: Researcher; User; Programmer; Game Designer
- Status: Integrated as research framing; not yet an executable participant-study protocol.
- Accepted input: DEC-003 records the primary question and SQ1–SQ3. Inferences are restricted to `achievement`, `exploration_free_spirit`, `reward_orientation`, and `system_testing`; no Socialiser or Philanthropist inference is claimed in the single-player prototype.
- Still required before implementation/collection: canonical schema vocabulary (NEED-005); consent, retention, access and withdrawal rules (NEED-006); and preregistered condition/assignment, accuracy/interpretability criterion, approved Hexad/Bartle instrument workflow, and recommendation-rubric/rater plan (NEED-007).
- Scope effect: SQ1 directly informs telemetry feature coverage. SQ2 is not implementable merely by logging gameplay because it requires authorised self-report collection. SQ3 needs a separately defined actionable-recommendation evaluation, not a post-hoc developer impression.
- Next owner action: Researcher derives preregistration-lite and a schema/event delta only after NEED-005..007 are answered; Programmer then owns a single telemetry-facade implementation.

## HANDOFF-ORCH-003 — Pilot protocol and schema-v1 integration

- Date: 2026-09-30
- Status: Integrated; implementation and participant collection remain separate gates.
- Decision: DEC-004 resolves the field/event-name conflict in favour of `METRICS_CONTRACT.md` schema `1.0.0` and records the user-provided consent, retention, withdrawal, safety, player-study and designer-evaluation plan in `PILOT_PROTOCOL.md`.
- Telemetry boundary: Questionnaire values and designer free text are excluded from gameplay JSONL. The protocol records only completion metadata in telemetry; protected separate stores are required for any approved questionnaire/evaluator data.
- Required next action: Researcher reviews the new canonical v1 contract and produces a golden JSONL/manifest expectation. Programmer may then implement exactly one facade/session configuration path. No recruitment or real data collection occurs until BLK-004 is closed.

## HANDOFF-ORCH-004 — Game Designer read-only audit: TenRoom polish baseline

- Date: 2026-09-30
- From: `dungeontrace_game_designer` audit in isolated worktree `codex/game-designer-audit`
- To: Programmer; Researcher; User
- Status: Integrated as evidence; no root Unity file, scene, asset, telemetry contract, or balance value was changed by the audit.
- Facts: `TenRoomDungeonZone` is an authored linear traversal: Start → Pursuer → Spitter → Choice → Charger → Warder+Pursuer → Treasure → Secret → Spitter+Charger → empty Boss room. It has FP movement/look/interact/fire/pause, Pulse Pistol and four enemy archetypes. Boss, reward/choice/secret outcomes, Results and Hub are logical prototypes not integrated into that run. The deterministic graph generator is separate from this authored scene.
- Verified risks: Three primitive room prefabs are reused; active content versioning is inconsistent (`prototype-01` prefab roots, `prototype-02` definitions, `prototype-03` TenRoom bootstrap). Legacy `TelemetryCore` remains incompatible with schema v1 and unconnected to gameplay.
- Recommended order:
  1. **DT-POLISH-01 / Must:** diagnostic content-profile preflight before any behavioral polish. It reports role→prefab→room/encounter ID→content version and fails on mismatch; it creates no player-visible or schema change.
  2. **Must after the preflight baseline:** a truthful end-of-slice completion signal, not a false boss-victory claim.
  3. **Must after baseline lock:** encounter readability (telegraph/hit/kill feedback) while retaining current grace/telegraph durations; this requires telemetry fairness coverage before Pilot.
  4. **Should:** role readability for Choice/Treasure/Secret without fake rewards; then comfort UI (visible pause, cursor release, reticle and controls hint).
- Research impact: Geometry, spawn layout, values, signage and telegraph visuals can all change time/damage/exploration behavior; each must create a new `content_version` and undergo seed/content review before research use. No `run_ended` or boss-victory research event is authorised until the canonical v1 facade exists.
- Open inputs: BLK-004 institutional approval; NEED-004 real gameplay reference; protocol-approved Hexad/Bartle wording/licensing and designer-rater rules; visual reference pack if signage/telegraphs advance beyond placeholders.
- Exact next step: Programmer claims DT-POLISH-01, implements one Editor/EditMode diagnostic and test, and returns its preflight report before selecting a canonical profile or adding player-visible polish.
