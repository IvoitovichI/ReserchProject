# Game Designer Handoffs

Добавляй новые записи сверху.

## HANDOFF-GD-001 — Baseline: измеримый vertical slice «комната → бой → награда → выход»

- Date: 2026-09-30
- From: Game Designer
- To: Programmer; Researcher (telemetry review)
- Related task/decision: P1 candidate «Вертикальный срез»; DEC-002; master §§6, 8, 15–16, 18, 20, 27–28.
- Status: Proposed — готово с ограничениями по отсутствующим reference и research inputs.
- Outcome:
  - **Факт.** `references/game-design/REFERENCE_INDEX.md` содержит только pending-шаблон `GD-REF-001`; видео, изображения, ссылка и timestamp отсутствуют. Следовательно, ниже не анализ референса, а минимальная design-baseline, выведенная из master specification.
  - **Рекомендуемый вариант (Must).** Одна authored start/combat-комната, в которой вход активирует один детерминированный encounter; после единственного clear создаётся одна достижимая награда; её получение открывает/разрешает выход. Цепочка должна завершаться без генератора, магазина, предметных offers, boss или secret. Это изолирует полный наблюдаемый цикл и не раздувает scope.
  - **Не включать сейчас (Won't).** Случайный граф комнат, выбор пути, secret, elite, boss, adaptive difficulty, persistent economy и дополнительные оружия. Они вводят новые причины изменения времени/урона/выбора и мешают диагностике среза.
- Player fantasy / loop:
  - Игрок входит в читаемую короткую арену, видит безопасный сектор и противников, стреляет и перемещается, получает явный сигнал о завершении угрозы, забирает награду и физически покидает комнату.
  - Input: WASD + mouse; primary fire; Interact только для награды/выхода, если это требуется реализацией.
  - Feedback: distinct spawn/encounter activation, confirmed hit, enemy death, room-clear cue, visible reachable reward, exit-unlocked cue. Конкретные звук/VFX/анимации не предписаны, так как visual reference отсутствует.
- Minimal content and tunable parameters (authored/data-driven, не хардкод):
  - `room_id=vs_combat_01`, `room_type=combat`, `difficulty_tier=1`, один `room_instance_id` на запуск; ориентир expected clear time: 30–90 s.
  - `encounter_id=vs_encounter_01`; один enemy archetype `enemy_id=pursuer`; default 2 экземпляра, допустимый диапазон 1–3. Spawn points: 2–3, вне прямого контакта при входе и с доступным retreat/safe sector.
  - `weapon_id=pulse_pistol`; для среза один стартовый weapon definition. Значения damage/fire-rate не устанавливаются дизайн-baseline без существующей balance sheet; их нужно versioned-authored и проверить тем, что encounter очищается в 30–90 s без softlock.
  - `reward_id=vs_currency_reward_01` (если reward definition разделена от currency), `currency_id=run_currency`, amount default 10, допустимый диапазон 5–15; выдаётся ровно один раз по stable idempotency key.
  - `exit_id=vs_exit_01`; state sequence `locked → unlocked_on_reward_claim → exited`. Если технически выход не интерактивен, переход через его trigger всё равно обязан быть однозначным и измеримым.
- Required stable IDs / context:
  - Контент: `room_id`, `encounter_id`, `enemy_id`, `weapon_id`, `reward_id`, `currency_id`, `exit_id`, плюс unique `zone_id` для entry/combat/reward/exit при наличии TelemetryZone.
  - Runtime: `room_instance_id`, `run_id`, `shot_id`, reward idempotency key. Не использовать GameObject names, asset filenames, Unity instance IDs или array indices.
  - Исследовательский envelope уже требует: `participant_id`, `session_id`, `build_id`, `content_version`, `schema_version`, `condition_id`, `seed`, `mode`, sequence/time. Для sandbox и обычного dev-run сохранять корректный mode, не смешивать с ResearchObservation.
- Telemetry impact (запрос на использование уже заданного каталога; **не изменение schema**):
  - Lifecycle: `run_configured → run_start → room_entered → room_activated → enemy_spawned* → [weapon_equipped] → shot_fired/shot_resolved* → damage_dealt* → enemy_killed* → room_cleared → reward_granted → currency_changed → room_exited → run_end`.
  - Для урона игроку добавить `damage_taken`; при смерти — `player_died` и terminal `run_end(reason=death)`. Каждое событие несёт available room/encounter context, `shot_fired`/`shot_resolved` коррелируют по `shot_id`; clear/reward/run_end не дублируются.
  - Минимальная проверка записи: synthetic successful run содержит ровно по одному `room_activated`, `room_cleared`, `reward_granted`, `currency_changed`, `room_exited`, `run_end`; число `enemy_killed` равно spawned enemies; failed/death run не даёт reward или exit.
- Acceptance criteria:
  1. При фиксированных `seed`, `build_id` и `content_version` запускается та же authored room/encounter и один valid `room_instance_id`.
  2. Игрок безопасно появляется, может перемещаться/смотреть/стрелять; два Pursuer достижимы и комната очищается Pulse Pistol в 30–90 s при ручном smoke test.
  3. До clear награда и выход не дают completion; после последнего valid enemy death награда появляется/становится доступна один раз; после claim выход доступен и достижим без прыжков/клиппинга.
  4. После `room_exited` срез завершает run с нейтральным success outcome; повторный interact/trigger не удваивает reward, currency или lifecycle event.
  5. Content validation проходит: unique stable IDs/zones, entry clearance, spawn NavMesh/safe distance, reward и exit reachable; no softlock after pause/re-entry/death.
  6. PlayMode/manual telemetry trace соответствует указанному порядку, envelope complete, `sequence_no` monotonic; отключённый sink не меняет gameplay-result.
- Research risk:
  - Основной исследовательский вопрос и condition definition отсутствуют (NEED-002), поэтому нельзя интерпретировать completion/time/damage как эффект механики или назначать experimental parameter. На этом этапе допустима только проверка полноты и жизненного цикла данных.
  - Референсные материалы отсутствуют (NEED-004); нельзя утверждать similarity target, требовать конкретный tempo/feedback или менять баланс ради референса. Параметры vertical slice должны попасть в content version/config до pilot.
- Priority: Must. Альтернатива A — reward выдаётся автоматически после clear (дешевле, но не проверяет interaction); альтернатива B — две комнаты и branch (проверяет навигацию, но вводит confound). Рекомендован основной вариант: одна комната с explicit reward claim.
- Files: `coordination/handoffs/game-designer.md` only. Runtime, telemetry schema, assets и общие coordination files не менялись.
- Requested next action:
  1. Programmer: создать data-driven definitions/prefab integration и тест/ручной smoke trace по acceptance criteria, не меняя telemetry schema без Researcher review.
  2. Researcher/User: закрыть NEED-002 формулировкой primary question/conditions и NEED-004 реальными reference link/file + timestamp + target mechanic; после этого Game Designer сделает reference-grounded tuning proposal.

## HANDOFF-GD-000 — Шаблон

- Date:
- From: Game Designer
- To:
- Related task/decision:
- Status: Proposed
- Outcome:
- Files:
- Parameters / IDs:
- Telemetry impact:
- Acceptance criteria:
- Risks / placeholders:
- Requested next action:
