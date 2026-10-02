# TenRoom Content Profile Preflight

## Purpose

`TenRoomContentProfilePreflight` is a read-only Editor diagnostic for the authored `TenRoomDungeonZone` baseline. It reports every expected room role with its assigned prefab path, stable `room_id`, encounter ID(s), definition `content_version`, prefab-root `content_version`, and the `content_version` declared for `TenRoomDungeonZone` in `GameBootstrap`.

It does not edit scenes, prefabs, definitions, gameplay values, telemetry, or session configuration. It cannot choose the canonical profile; that remains a team decision after the failure report is reviewed.

## Run

In Unity, select **Dungeon Trace > Validation > Preflight TenRoom Content Profile**. The Console receives a table and either `PASS` or `FAIL`.

Run the EditMode test `TenRoomContentProfilePreflightTests` from Test Runner to confirm that the current repository exposes the known version mismatch and that an omitted role mapping fails.

## Current expected result

The preflight must currently fail. It exposes the confirmed three-version conflict:

- prefab roots: `prototype-01`;
- generated `RoomDefinition` assets: `prototype-02`;
- `TenRoomDungeonZone` session declaration: `prototype-03`.

This failure is intentional diagnostic evidence for `BLK-003`; it does not change any `content_version`. Do not use the authored scene as a research build until the team approves one profile and the preflight passes.
