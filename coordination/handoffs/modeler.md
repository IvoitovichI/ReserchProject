# 3D Modeler Handoffs

Добавляй новые записи сверху.

## HANDOFF-3D-001 — Read-only art baseline: первый room → combat → reward → exit

- Date: 2026-09-30
- From: 3D Modeler
- To: Programmer, Game Designer, Researcher
- Related task/asset brief: P1 candidate from `coordination/TASK_BOARD.md`; minimum visual set for the first vertical slice. This entry is a brief only: no Unity asset, scene, prefab, code, or shared coordination document was changed.
- Status: Proposed / BLOCKOUT-first

### Evidence and constraints

- Existing visual content is primitive placeholder content only: `Assets/DungeonTrace/GeneratedDungeon/Room_Empty.prefab`, `Room_Table.prefab`, and `Room_Crypt.prefab`; all are marked/documented as `prototype-02`. The authored walkthrough is `Empty → Table → Crypt` and expressly does **not** validate combat, rewards, exit logic, or research readiness.
- Existing debug presentation assets are `Assets/Resources/DungeonTrace/Combat/WeaponViewmodel.prefab`, `CombatDummy.prefab`, and `EnemyProjectile.prefab`. They are functional/blockout aids, not final art.
- The reusable room envelope currently established by the generated placeholders is 12 × 12 × 3 m, with four door sockets at the bounds and an opening contract of 1.5 m wide × 2 m high. Player camera height is 1.65 m; therefore all critical affordances must read from a 1.65 m eye level at 2–12 m, without crouch/jump.
- No visual reference has been supplied. `references/3d-modeler/REFERENCE_INDEX.md` contains only pending `ART-REF-001` and `images/` has only its instruction file. `LowPolyArtPolishIdeas.md` is an internal direction note, **not** an approved external reference.
- Provisional visual language, until reference approval: cool blue-grey stone, oxidised teal metal, warm gold for reward, and one high-value emissive colour per gameplay meaning. Do not use a look-alike of any named game. Emissive is reserved for interaction/reward/telegraph states; decoration must not obscure doorways, cover, or lines of sight.

### Minimum asset list / individual briefs

| asset_id / proposed source name | purpose and FPS reading | size, pivot, orientation | materials / budget / collision | LOD, export and acceptance |
|---|---|---|---|---|
| `ENV_DUN_Floor_03m` — modular floor tile | Makes the room perimeter and walkable area legible; repeat across the 12 m room, viewed 0–10 m. | 3 × 0.15 × 3 m; pivot at floor-centre; Y-up, +Z north/socket-forward. | 1 shared non-metal stone material (base colour, normal, ORM; 512 px atlas region); ≤120 tris/tile, 16 instances/room; BoxCollider only. | No LOD required. FBX with applied scale 1. Preview: top, seam at 1.65 m, 12 m room scale. |
| `ENV_DUN_Wall_03m` plus `ENV_DUN_Corner_03m` | Defines the boundary but keeps the four 1.5 × 2 m sockets visually and physically clear. Read at 1–12 m. | Wall: 3 × 3 × 0.25 m; corner: 0.25 × 3 × 0.25 m; pivot on floor at module/socket grid. | Same stone material, optional teal inset as a second shared slot only where used; ≤220 / ≤100 tris; BoxCollider per module. | No LOD. Must match the existing 12 × 12 × 3 m envelope without changing markers; screenshots front/side/seam/in-room. |
| `ENV_DUN_DoorFrame_15x20` — exit frame | Establishes a readable route after room clear. The frame is visual-only; programmer retains open/lock/collision state. Read from 2–12 m. | Outside dimensions max 2.1 × 2.5 × 0.35 m; inner aperture exactly 1.5 × 2 m; pivot at aperture-floor centre; +Z points through exit. | 2 shared slots: stone + teal metal/emissive trim; ≤450 tris, 1–2 visible/room; no collider on trim, or one simple frame BoxCollider only after programmer confirms traversal contract. | No LOD. Export FBX; separate optional `ExitGlow` mesh/socket for presentation. Acceptance: route colour distinguishes this from reward, aperture remains unobstructed at camera height. |
| `PRP_RWD_Pedestal_01` — reward pedestal | Announces the post-combat claim location and supports a 2.2 m interaction distance; focal read at 2–8 m. | 0.9 × 1.1 × 0.9 m; pivot on floor; +Z is preferred player-approach side. | 2 slots: dark stone/metal and warm-gold emissive cap; ≤650 tris, one per room; BoxCollider no larger than plinth silhouette. | No LOD. FBX and 512 px shared/atlas-ready textures. Must leave a 1 m clear ring and keep room geometry/spawn layout unchanged. |
| `PRP_RWD_Glyph_01` — neutral reward visual | Floating, item-agnostic glyph above pedestal; it must not imply a particular item effect before design data exists. Read at 2–8 m. | max 0.35 × 0.35 × 0.08 m; pivot centred; billboard/rotation behaviour is programmer-owned. | One emissive gold material; ≤180 tris; no collision. | No LOD. Separate FBX, no baked VFX. Acceptance: legible against blue-grey wall under room lighting, distinguishable from exit teal. |
| `ENM_PURSUER_BLOCKOUT_01` — first combat target | Single melee pressure silhouette for the vertical slice: broad low body, oversized forward arms, clear raised-arm wind-up. Read at 3–12 m; distinct from the red debug dummy. | Approx. 0.9 × 1.9 × 0.8 m; floor pivot, +Z forward. | 2 slots: dark body and high-value warm/red telegraph patch; ≤1,500 tris; one simple capsule collider **only after** programmer maps it to existing enemy collision/hitbox rules. | No LOD for one instance. FBX, no animation unless a skeleton/socket contract is supplied. Acceptance: static front/side/back and 1.65 m in-room view; wind-up zone remains visible above cover. |
| `VFX_ENM_WindupMarker_01` — optional mesh/VFX brief | Supplements, never replaces, the pursuer's physical wind-up. Read at 3–12 m; disabled outside attack state by programmer. | 0.8 m max radius; pivot at enemy floor/root or named hand socket (pending animation contract). | One additive/emissive material; ≤80 tris if mesh; no collision. | No LOD; keep separate from enemy FBX. Acceptance: colour/value visible without bloom and does not mask the enemy silhouette. |
| `WPN_PulsePistol_View_BLOCKOUT` — existing debug asset disposition | Current cyan cuboid viewmodel is adequate for a temporary hit-scan proof only; it is not a final weapon brief. The player sees it at <1 m. | Preserve existing viewmodel transform until programmer supplies camera/viewmodel socket and clipping limits. | Existing primitive material; proposed final limit ≤1,200 tris, ≤2 shared slots, no collider. | No action in this slice. Replace only under a dedicated weapon brief so aiming/readability telemetry remains comparable. |

### Proposed naming, destination, and integration boundary

- Do not create these directories/assets yet. Once Programmer confirms the target project layout, proposed source destination is `Assets/_Project/Art/Environment/Dungeon/`, `.../Props/Rewards/`, `.../Characters/Enemies/`, and `.../Weapons/`; this follows the master document's target layout but is not yet a migration request for the existing `Assets/DungeonTrace` content.
- Source: applied-transform FBX, 1 Unity unit = 1 m, Y-up, no spaces in names; textures PNG/TGA in documented PBR set. Each mesh uses only the stated shared slots. No MeshCollider, no embedded gameplay scripts, and no gameplay prefab/scene edit from art.
- Programmer owns prefab assembly, layers, navmesh, state-driven visibility, collider/hitbox assignment, import settings, and all runtime/VFX binding. Game Designer approves silhouette/readability; Researcher checks that colour and guidance remain fixed across conditions and do not introduce a condition-specific exploration cue.

### Placeholders and missing inputs

1. `Room_Empty`, `Room_Table`, `Room_Crypt`, debug pistol/dummy/projectile, and their primitive materials remain `BLOCKOUT` placeholders. No final 3D models, material library, texture atlas, normal maps, or preview renders exist.
2. Required before final art (but not before a neutral blockout): visual-reference set registered in `references/3d-modeler/REFERENCE_INDEX.md`; confirmed Unity import profile/actual Art destination; target hardware/per-room instance budget; exit state/door collision contract; enemy rig, animation, and hitbox/socket contract; reward item/interaction state contract; lighting/URP exposure baseline.
3. `NEED-003` already records the missing visual references, and `BLK-001` records the project/version uncertainty. No duplicate shared-document entry was made by this read-only task.

### Preview / acceptance checklist for the eventual art handoff

- Screenshots: front, side, top, perspective, and 1.65 m eye-height in a 12 × 12 m room; include reward at 2/8 m and exit at 12 m.
- Player proxy fits through the 1.5 × 2 m aperture; floor/walls preserve a clear line from entry to reward and exit; no decoration intersects sockets, spawn positions, cover, or navigation surface.
- Reward gold, exit teal, and enemy telegraph colour are separable in grayscale value and under the proposed cool dungeon lighting; do not rely only on colour.
- Validate scale/pivots/normals/material-slot count/applied transforms; simple collision only; record mesh and texture counts and any LOD threshold in the asset-specific final handoff.

- Known limitations: This is a provisional brief derived from internal prototype documentation rather than user art references; it does not approve a final art style or alter `content_version`. Current generated assets contain mixed documented/YAML versions (`prototype-02` documentation vs `prototype-01` prefab fields observed in `Room_Empty`); Programmer must reconcile that content-version baseline before research comparisons.
- Requested next action: Programmer confirms integration folder and required prefab sockets/collider boundaries; Game Designer accepts/revises neutral silhouettes and colour meanings; User supplies 1–3 reference images with intended target/use/avoid/scale. Then Modeler can prepare the first neutral blockout source assets without changing gameplay content.

## HANDOFF-3D-000 — Шаблон

- Date:
- From: 3D Modeler
- To: Programmer
- Related task/asset brief:
- Status: Proposed
- Asset IDs and files:
- Scale/pivot/orientation:
- Materials/textures:
- Collider/LOD/animation sockets:
- Import instructions:
- QA screenshots:
- Known limitations:
- Requested next action:
