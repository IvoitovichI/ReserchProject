# Enemy AI and movement — future upgrade notice

The Phase 5 encounter slice uses NavMesh pathing with a visible direct-movement fallback. It is sufficient for the current prototype, but it is not the final combat-AI implementation.

Before pilot or research lock, upgrade and validate: local avoidance under crowding, charger collision/stun handling, Warder ally-targeting and shield duration, Spitter cover selection, audio telegraphs, obstacle-aware line-of-sight recovery, and per-archetype fairness metrics. Any change that affects movement speed, pathing, spawn positions, attack timing, or encounter composition requires a new content version and renewed fairness validation.
