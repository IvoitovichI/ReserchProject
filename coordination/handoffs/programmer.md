# Programmer Handoffs

Добавляй новые записи сверху.

## HANDOFF-PR-002 — Исправление ошибки сборки NavMesh

- Date: 2026-10-02
- From: Programmer
- To: Orchestrator; Game Designer
- Status: Verified in the open Unity Editor log.
- Implemented: Removed an undeclared `Unity.AI.Navigation` dependency (`NavMeshSurface`) from the Editor-only TenRoom scene generator. The package is absent, so this restores compilation without a package change. The generated scene retains `TenRoomDungeonZone`; enemy movement uses its existing visible fallback when no baked NavMesh exists.
- Changed files: `Assets/DungeonTrace/Editor/DungeonWalkthroughSceneGenerator.cs`; `references/game-design/UPLOAD_REFERENCES_HERE.md`.
- Checks: Unity Editor recompiled successfully: `Tundra build success (3.81 seconds)` and reloaded `DungeonTrace.Editor`, `DungeonTrace.EditModeTests`, and runtime assemblies. `git diff --check` passed.
- Known non-project log messages: Unity AI Assistant emits `NoSubscription` for `generators.ai.unity.com`; this is an account/service warning, not a DungeonTrace compile error. The log also records a Unity licensing 404; the Editor subsequently resolved entitlement details.
- Design input needed: `NEED-003` and `NEED-004` remain open. Upload references into `references/game-design/` and register each targeted mechanic in `REFERENCE_INDEX.md`.
- Next step: Run EditMode tests from Unity Test Runner; then Game Designer reviews the first 1–3 registered references.

## HANDOFF-PR-001 — DT-POLISH-01 TenRoom content-profile preflight

- Date: 2026-09-30
- From: Programmer
- To: Game Designer; Researcher; Orchestrator
- Related task/decision: `DT-POLISH-01`; `BLK-003`; `HANDOFF-ORCH-004`.
- Status: Implemented with verification limitation. The diagnostic intentionally reports a failure for the current content; it does not select or change a canonical profile.
- Implemented:
  - Added **Dungeon Trace > Validation > Preflight TenRoom Content Profile**.
  - The read-only report has ten authored roles and prints `room-role → prefab path → room_id → encounter ID(s) → definition content_version → prefab-root content_version`, plus the TenRoom session version parsed from `GameBootstrap`.
  - Validation fails for missing role/definition/prefab/root-version data and whenever the gathered profile has more than one content version.
  - Added two EditMode tests: the repository profile must expose `prototype-01, prototype-02, prototype-03`; an omitted role mapping must fail with an actionable error.
- Changed files:
  - `Assets/DungeonTrace/Editor/DungeonTrace.Editor.asmdef` and `TenRoomContentProfilePreflight.cs` (+ `.meta` files).
  - `Assets/DungeonTrace/Tests/EditMode/DungeonTrace.EditModeTests.asmdef` and `TenRoomContentProfilePreflightTests.cs` (+ `.meta`).
  - `Assets/DungeonTrace/Documentation/TenRoomContentProfilePreflight.md` (+ `.meta`).
- APIs / data contracts / IDs:
  - Editor-only public API: `TenRoomContentProfilePreflight.InspectTenRoom()` and `Validate(TenRoomContentProfileSnapshot)` return `TenRoomContentProfileReport`; no runtime API or telemetry contract changed.
  - Authored room labels and encounter IDs mirror `DungeonWalkthroughSceneGenerator`; this is an audit mapping, not a new canonical content manifest.
- Tests and manual checks:
  - Passed: `git diff --check` (only existing line-ending warning for the edited asmdef); source/static discovery confirms diagnostic, tests and usage note are present.
  - Not run: Unity compilation and EditMode Test Runner. `unity command run_tests` reported no connected Pipeline instance; a later Pipeline check was manually stopped rather than waited on. Therefore the test assertions and new named Editor assembly still require verification in Unity.
- Telemetry verification: Not applicable. No telemetry event, schema, facade, JSONL, participant data, or research-session behavior changed.
- Not verified:
  - Unity import of the new `DungeonTrace.Editor` assembly and its reference from `DungeonTrace.EditModeTests`.
  - Console output of the menu command and both EditMode tests.
- Known issues / placeholders:
  - The expected current result is `FAIL`: prefab roots are `prototype-01`, generated definitions are `prototype-02`, and the TenRoom bootstrap declaration is `prototype-03`.
  - The preflight reads the bootstrap declaration and authoring assets; it does not mutate or regenerate `TenRoomDungeonZone`.
- Needs opened or closed: None. `BLK-003` remains open and is now mechanically reproducible.
- Requested next action: In an available Unity Editor, run the menu command and `TenRoomContentProfilePreflightTests`. Give the report to Game Designer and Researcher; then the owning decision-maker chooses one canonical content profile before any player-visible polish or research build.

## HANDOFF-PR-000 — Шаблон

- Date:
- From: Programmer
- To:
- Related task/decision:
- Status: Proposed
- Implemented:
- Changed files:
- APIs / data contracts / IDs:
- Tests and manual checks:
- Telemetry verification:
- Not verified:
- Known issues / placeholders:
- Needs opened or closed:
- Requested next action:
