# Task Board

Правило: одна задача имеет одного владельца записи. Другие роли дают вход через handoff или комментарий в `dependencies`.

## Active

| task_id | milestone | title | owner | dependencies | deliverable | acceptance | status |
|---|---|---|---|---|---|---|---|
| DT-001 | Research setup | Утвердить канонический telemetry-контракт и pilot-only research inputs | Researcher | DEC-003..004; BLK-004; instrument/rater protocol gates | versioned envelope/event registry + preregistration-lite + golden-log QA criteria | schema `1.0.0`; privacy-safe Pilot flow; operationalized PRQ/SQ; no ambiguous raw-event aliases | In Progress |
| DT-POLISH-01 | Prototype polish | Content-profile preflight для `TenRoomDungeonZone` | Programmer | BLK-003; audit HANDOFF-ORCH-004 | diagnostic Editor/EditMode preflight + test + usage note | воспроизводимо находит `prototype-01/02/03`; выводит role→prefab→room/encounter ID→content version; не меняет gameplay/schema/scene/assets | Review — Unity menu/Test Runner pending |

Статусы: `Backlog`, `Ready`, `In Progress`, `Review`, `Blocked`, `Done`.

## Next milestone candidates

| priority | task | recommended owner |
|---|---|---|
| P0 | Зафиксировать исследовательский вопрос и условия | Researcher + User |
| P0 | Инвентаризация Unity-проекта | Programmer |
| P0 | Разобрать 1–3 ключевых игровых референса | Game Designer |
| P1 | Создать asset list и первый visual brief | 3D Modeler |
| P1 | Вертикальный срез: комната → бой → награда → выход | Programmer |

## Done

Переноси сюда завершенные строки, сохраняя ID и ссылку на handoff/commit.

| task_id | milestone | title | owner | dependencies | deliverable | acceptance | status |
|---|---|---|---|---|---|---|---|
| DT-000 | Setup | Проверить и адаптировать пакет под реальный Unity-репозиторий | Programmer | путь к проекту, Unity version | inventory + [HANDOFF-ORCH-001](handoffs/orchestrator.md) | фактический root/version/packages подтверждены; роли получили baseline | Done |
