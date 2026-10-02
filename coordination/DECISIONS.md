# Decision Log

Решения здесь имеют приоритет над старыми предположениями. Не редактируй историю решения задним числом: добавляй superseding decision.

## Шаблон

### DEC-000 — Короткое название

- Date:
- Status: `Proposed / Accepted / Rejected / Superseded`
- Decider:
- Context:
- Options considered:
- Decision:
- Rationale:
- Consequences:
- Affected files/contracts:
- Supersedes / superseded by:

## Начальные решения

### DEC-001 — Четыре специализированные роли

- Date: 2026-09-29
- Status: Accepted
- Decider: User
- Context: Для прототипа нужны дизайн, исследование, 3D и программирование.
- Decision: Использовать Game Designer, Researcher, 3D Modeler и Programmer с одним оркестратором.
- Consequences: Каждая задача имеет одного владельца; интеграцию кода выполняет Programmer.

### DEC-002 — Research-first telemetry

- Date: 2026-09-29
- Status: Accepted
- Decider: Project baseline
- Context: Прототип тестируется на реальных участниках.
- Decision: Условия, IDs и события определяются до полевого теста; schema/build version входят в экспорт.
- Consequences: Изменение смысла события требует review Researcher и version bump.

### DEC-003 — Исследовательская рамка SDK мотивационных профилей

- Date: 2026-09-30
- Status: Accepted
- Decider: User
- Context: Для DT-001 нужно зафиксировать research question до утверждения telemetry schema и реализации исследовательского режима.
- Decision: Primary Research Question — оценить, насколько точно и интерпретируемо SDK выводит непрерывные мотивационные профили из телеметрии контролируемого FP3D roguelite-прототипа и преобразует их в применимые design recommendations. Secondary questions: (SQ1) надежность telemetry-derived поведенческих признаков/последовательностей; (SQ2) соответствие профилей самоотчётам Hexad и Bartle с контролем игрового опыта и performance; (SQ3) полезность рекомендаций для exploration, combat pacing, rewards, secrets и hub progression.
- Rationale: User определил исследовательскую цель и ограничил интерпретируемые измерения одиночной игрой.
- Consequences: В текущем протоколе измеряются только `achievement`, `exploration_free_spirit`, `reward_orientation` и `system_testing`. `socialiser` и `philanthropist` исключены из inferential claims до появления валидных социальных механик. SQ2 требует отдельного утверждения формы/лицензии/порядка Hexad и Bartle; SQ3 требует заранее определённого rubric и оценивающих. PRQ не разрешает сбор данных без схемы, consent и analysis plan.
- Affected files/contracts: `coordination/METRICS_CONTRACT.md`, preregistration-lite, analysis plan, research-mode UI, telemetry event registry.
- Supersedes / superseded by: None.

### DEC-004 — Schema v1 и дизайн Pilot для мотивационных профилей

- Date: 2026-09-30
- Status: Accepted
- Decider: User
- Context: DEC-003 требует единого telemetry vocabulary, privacy-safe Pilot protocol и pre-specified comparison до реализации или сбора.
- Decision: `coordination/METRICS_CONTRACT.md` является каноническим registry/envelope для `schema_version: 1.0.0`; его имена `timestamp_utc`, `session_time_ms`, `session_started`, `run_started`, `room_completed`, `weapon_fired`, `hit_dealt` и `run_ended` заменяют конфликтующие варианты в старых примерах master-документа. Pilot использует `PILOT-CONSENT-v1.0`, локальный защищённый export, pseudonymous IDs, две counterbalanced seed runs в `GAME_STANDARD_V1` и paired offline comparison `A0_BASELINE` vs `A1_SDK_SEQUENCE`.
- Rationale: Нужны воспроизводимые данные без адаптации сложности и без утечки questionnaire/free-text в игровой event log.
- Consequences: Primary accuracy outcome — Macro Motivation Profile Convergence: Fisher-z mean четырёх Spearman correlations SDK↔Hexad (Achievement, Free Spirit, Player/Reward Orientation, Disruptor/System Testing). Interpretability/actionability сравнивается отдельно между blinded `D0_METRICS_ONLY` и `D1_SDK_CARD`; их нельзя объединять в один accuracy score. Hexad/Bartle raw answers и designer free text не входят в telemetry JSONL и требуют отдельного защищённого, protocol-approved store. `Socialiser` и `Philanthropist` не входят в primary inference.
- Affected files/contracts: `coordination/METRICS_CONTRACT.md`, `coordination/PILOT_PROTOCOL.md`, preregistration-lite, research-mode UI, telemetry implementation and analysis plan.
- Supersedes / superseded by: Resolves BLK-002; supersedes conflicting master examples only for schema-v1 field/event vocabulary.
