# Blockers

Блокер — проблема, которую агент не может безопасно решить внутри своей ответственности. Не записывать сюда обычные задачи.

| blocker_id | date | raised_by | blocked_task | description | decision_needed | owner | severity | status |
|---|---|---|---|---|---|---|---|---|
| BLK-001 | 2026-09-29 | Team setup | DT-000 | Путь и версия фактического Unity-проекта еще не указаны в этом пакете | Подключить пакет к репозиторию и подтвердить Unity version | User/Programmer | P0 | Resolved: `C:\Users\Ivan\GameForStudy`, Unity `6000.3.19f1` confirmed 2026-09-30 |
| BLK-002 | 2026-09-30 | Researcher review | DT-001 | `docs/DungeonTrace_CODEX_MASTER.md` и `coordination/METRICS_CONTRACT.md` задают несовместимые имена полей envelope и событий (`session_start`/`session_started`, `monotonic_ms`/`session_time_ms` и др.) | User/Researcher выбирают один versioned canonical registry либо явную новую версию/mapping до runtime implementation | Researcher + User | P0 | Resolved by DEC-004: `METRICS_CONTRACT.md`, schema `1.0.0` |
| BLK-004 | 2026-09-30 | Orchestrator | Pilot participant collection | Pilot protocol states that final retention/consent requirements must be agreed with KBTU; no evidence of that approval is recorded | Confirm applicable supervisor/KBTU or ethics approval before recruiting or collecting real participant data | User/Researcher | P0 | Open |
| BLK-003 | 2026-09-30 | Programmer + 3D Modeler + Game Designer audit | следующий research slice | Version conflict is now confirmed across active content: generated definitions `prototype-02`, prefab roots `prototype-01`, and `TenRoomDungeonZone` session/bootstrap `prototype-03`; room roles also reuse three primitive prefabs without an explicit canonical role→prefab/encounter manifest | DT-POLISH-01 first reports the inconsistency deterministically; Programmer then proposes one canonical content profile before a research build | Programmer | P1 | Open |

Severity: `P0` останавливает milestone, `P1` мешает качеству/сроку, `P2` имеет workaround.

## Правило эскалации

1. Владелец фиксирует blocker и возможные варианты.
2. Ответственная роль предлагает рекомендацию.
3. Оркестратор принимает обратимое решение или запрашивает пользователя.
4. Решение переносится в `DECISIONS.md`, blocker закрывается ссылкой.
