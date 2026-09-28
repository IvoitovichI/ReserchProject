---
name: unity-fp3d-research-agent
description: Develop, review, test, or document the Unity first-person 3D research game Dungeon Trace, including deterministic dungeon generation, combat, room authoring, hub progression, research modes, and privacy-aware gameplay telemetry. Use for repository tasks specific to this project; do not invoke for unrelated Unity tutorials or other games.
---

# Unity FP3D Research Agent

Support implementation without breaking experimental reproducibility. Treat gameplay, content, and telemetry as three connected but separately testable systems.

## Start every task

1. Read the nearest `AGENTS.md` and inspect the actual repository before proposing file paths or APIs.
2. Determine whether the task affects gameplay, content, telemetry, the research protocol, or more than one area.
3. Read only the relevant references:
   - Product boundaries and terminology: [project-overview.md](references/project-overview.md)
   - Modules, dependencies, scenes, and data flow: [unity-architecture.md](references/unity-architecture.md)
   - C# and Unity conventions: [coding-standards.md](references/coding-standards.md)
   - Repeated implementation procedures: [workflows.md](references/workflows.md)
   - Event envelope, catalog, sampling, and privacy: [telemetry-spec.md](references/telemetry-spec.md)
   - Room prefab authoring and validation: [room-editor-guide.md](references/room-editor-guide.md)
   - Verification and acceptance criteria: [testing-and-dod.md](references/testing-and-dod.md)
   - Ready-to-use issue and prompt formats: [task-templates.md](references/task-templates.md)
4. Preserve existing user changes. Make the smallest coherent change that satisfies the request.

## Non-negotiable project invariants

- `ResearchObservation` uses fixed rules. It must not adapt difficulty, loot, rooms, or enemy behavior from an inferred player type.
- A seed plus `content_version` must reproduce the dungeon graph, room choices, item offers, and boss assignment.
- Telemetry observes gameplay through typed events; it must not become a hidden dependency of gameplay code.
- Every research event has `event_id`, participant-safe session identifiers, `sequence_no`, monotonic and UTC time, schema/build/content versions, mode, and context.
- Never place a participant's name, email, free text, IP address, or device account name in gameplay logs.
- In Research Mode, default to FOV 90°, no head bob, camera shake, or dynamic FOV. Record sensitivity, invert-Y, input device, and performance samples.
- First-person aiming starts from the gameplay camera. Projectile weapons then correct the muzzle direction toward the camera-derived aim point.
- Do not introduce jumps, crouching, unrestricted physics-based hub placement, runtime procedural geometry, remote telemetry upload, or adaptive difficulty into the MVP unless the user expands scope.
- Do not upgrade Unity, packages, render pipeline, input backend, serialization format, or assembly boundaries incidentally.

## Implementation approach

- Keep pure rules in plain C# where possible. Use `MonoBehaviour` for scene lifecycle and Unity component integration; use `ScriptableObject` for authored definitions, not mutable runtime state.
- Keep orchestration thin. Prefer explicit dependencies, typed interfaces, and C# events over scene searches or broad singletons.
- Use stable authored IDs. Never use display names, array positions, instance IDs, or localized text as research identifiers.
- Version any change that can alter interpretation of collected data. Gameplay/content changes advance `content_version`; event meaning or fields advance `schema_version`.
- When a feature changes player opportunities, add or update the corresponding opportunity counts so later analysis can normalize behavior.
- Avoid per-frame logging. Sample only the defined low-frequency signals and emit semantic events at state transitions.

## Verification and handoff

- Run the narrowest relevant automated checks, then broader checks when available. Do not claim Unity Editor or build verification unless it was actually run.
- For gameplay changes, check both normal play and Research Mode. For content changes, validate deterministic generation and reachability. For telemetry changes, validate schema, ordering, flushing, and privacy.
- Report changed files, behavioral result, tests run, tests not run, and any effect on `content_version`, `schema_version`, seed comparability, or protocol.
- Stop and ask before making a choice that changes the research condition, primary outcomes, participant data handling, or MVP scope.
