# Low-poly 3D art polish ideas

Use a restrained dungeon palette: cool stone blue/grey, oxidized teal metal, warm treasure gold, and clear enemy telegraph colours. Keep silhouettes readable from first-person view and avoid decorative geometry that blocks doors, NavMesh, or combat sight-lines.

## Rooms and traversal

- Modular floor, wall, ceiling, corner, arch, and doorway pieces.
- Three pillar families: square ruin, round crypt column, and broken support.
- Rubble clusters, cracked slabs, wall alcoves, hanging chains, barred windows, and torch brackets.
- Table, chair, bookshelf, sarcophagus, altar, brazier, treasure chest, merchant stall, and locked gate.
- Clear room-role props: combat banner, choice statue, elite skull pile, secret rune wall, treasure pedestal, and boss throne.

## Enemies and bosses

- Pursuer: hunched melee guardian with oversized arms and a visible windup pose.
- Spitter: floating masked caster with a glowing projectile core.
- Charger: broad-shouldered beast with horn or shield silhouette and bright charge telegraph.
- Warder: robed support enemy with orbiting shield shards.
- Warden boss: armoured sentinel, large weapon, shoulder lanterns, and readable phase-two glow.
- Hoarder boss: bulky collector with chest/backpack silhouette, coin props, decoy rewards, and hazard markers.

## Pickups and interaction

- Passive-item pedestals, consumable bottles, skeleton key, reroll token, heal station, and coin piles.
- Secret clue tablet, concealed lever, revealable wall panel, and one-time reward cache.
- Weapon viewmodels: Pulse Pistol, scatter blaster, arc rifle, and orb launcher.
- Door sockets, encounter spawn markers, cover blocks, and reward stands should have editor-only gizmo versions and unobtrusive in-game meshes.

## Hub and rewards

- Fixed-slot carpet, lamp, plant, poster, wall panel, shelf, small statue, and archive terminal.
- Warden and Hoarder trophy stands.
- Starting-module console for Heal, Reroll, and +5 Coins.

## Production rules

- Prefer shared materials, atlas-friendly textures, and LODs for large props.
- Use emissive colour only for interactables, hazards, and combat telegraphs.
- Keep colliders simple; never use decorative meshes as detailed collision.
- Any geometry or encounter-layout change that alters play metrics requires a new `content_version`.
