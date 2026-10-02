# Metrics Contract

Владелец: Researcher. Реализация: Programmer. Любое изменение семантики требует review обоих владельцев и увеличения `schema_version`.

## Canonical schema status

- Active schema: `1.0.0`.
- `METRICS_CONTRACT.md` is the canonical vocabulary for schema v1. The legacy master examples `occurred_at_utc`, `monotonic_ms`, `session_start`, `run_start`, `room_cleared`, `shot_fired`, `damage_dealt`, and `run_end` are not aliases in raw v1 logs.
- Raw gameplay telemetry must never contain questionnaire answers or designer free-text answers. Those are separate protected research stores, linked only through a pseudonymous code where the approved protocol allows it.

## Session envelope

| field | type | required | rule |
|---|---|---|---|
| schema_version | string | yes | semver-like, например `1.0.0` |
| event_id | string | yes | уникальный UUID/ULID |
| event_name | string | yes | stable snake_case |
| timestamp_utc | string | yes | ISO-8601 UTC |
| session_time_ms | integer | yes | монотонное время сессии |
| sequence_no | integer | yes | строго возрастает в session |
| participant_id | string | yes | псевдонимный код, без PII |
| session_id | string | yes | уникален для запуска исследования |
| build_id | string | yes | неизменен для конкретной сборки |
| content_version | string | yes | неизменен для сравниваемого content/balance set |
| mode | string | yes | `Pilot`, `ResearchObservation` или `Sandbox`; participant collection uses only approved research modes |
| condition_id | string | yes | назначенное условие |
| protocol_version | string | yes | approved protocol identifier, e.g. `PILOT-PROTOCOL-v1.0` |
| run_id | string/null | context | null вне run |
| seed | integer/string/null | context | фиксируется для run |
| payload | object | yes | поля по event registry |

## Event registry

| event_name | trigger | required payload | consumer |
|---|---|---|---|
| consent_recorded | consent gate завершён до gameplay | consent_version, consent_accepted | inclusion audit |
| session_started | research config принят | locale, platform, target_fps | session count |
| condition_assigned | условие зафиксировано | assignment_method | balance check |
| questionnaire_completed | protected questionnaire phase завершена; ответов в event нет | questionnaire_id, phase, completion_status | protocol completion |
| run_started | создан run | dungeon_profile_id, starting_weapon_id | run denominator |
| room_entered | игрок пересек границу комнаты | room_instance_id, room_id, room_type, depth | path/time |
| room_completed | выполнено условие комнаты | room_instance_id, duration_ms, damage_taken | pacing |
| weapon_acquired | оружие стало доступно | weapon_id, source, replaced_weapon_id | choice/exposure |
| weapon_fired | валидный fire action | weapon_id, shot_id, ammo_cost | accuracy denominator |
| hit_dealt | подтвержден урон | shot_id, weapon_id, target_type_id, damage | combat efficacy |
| damage_taken | здоровье игрока уменьшилось | source_type_id, amount, hp_after | difficulty |
| enemy_killed | враг перешел в dead | enemy_id, enemy_instance_id, weapon_id | encounter outcome |
| secret_discovered | секрет стал доступен игроку | room_instance_id, secret_type, discovery_method | exploration |
| boss_started | активирован бой | boss_id, room_instance_id | boss exposure |
| boss_ended | бой завершен | boss_id, outcome, duration_ms, damage_taken | boss outcome |
| currency_earned | баланс увеличен | amount, reason, balance_after | economy source |
| currency_spent | баланс уменьшен | amount, reason, item_or_upgrade_id, balance_after | economy sink |
| hub_upgrade_purchased | покупка подтверждена | upgrade_id, level_before, level_after | progression choice |
| decoration_placed | размещение подтверждено | decoration_id, slot_or_position_id | personalization |
| player_died | player state стал dead | cause_type_id, room_instance_id, elapsed_ms | failure point |
| run_ended | run финализирован | reason, duration_ms, rooms_completed, currency_earned | run outcome |
| session_ended | корректное завершение | reason, duration_ms, valid_run_count | session outcome |
| participant_withdrawal | участник прекращает участие или требует обработки partial data | action, reason_category (optional) | withdrawal audit |
| telemetry_error | событие/запись не удались | error_code, affected_event_name | data quality |

## Правила

- Деньги логируются отдельными транзакциями; `balance_after` используется для QA, но не заменяет transaction sum.
- Урон логируется фактически примененный после правил mitigation; raw damage можно добавить отдельным полем.
- `weapon_fired` и `hit_dealt` связываются `shot_id`; правило multi-hit документируется.
- `room_instance_id` уникален внутри run, `room_id` обозначает preset.
- Запрещен произвольный display name вместо stable ID.
- Запрещено менять значение старого поля, сохраняя прежнюю `schema_version`.
- `consent_recorded.timestamp_utc` в envelope является consent timestamp; не дублировать его в payload. `consent_accepted:false` завершает поток без `session_started` или `run_started`.
- `participant_withdrawal.reason_category` необязателен; разрешены только нейтральные controlled values из Pilot protocol, без диагноза и свободного текста.
- `session_ended.reason` поддерживает как минимум `victory`, `death`, `withdrawal`, `technical_abort`, `timeout`, `researcher_stop`. Technical-exclusion data хранится отдельно и не попадает в основной анализ без заранее установленного правила.

## Export и retention

- Формат по умолчанию: JSONL как канонический event log; CSV — производный экспорт.
- Кодировка UTF-8, даты UTC, числа invariant culture.
- Запись append-only с flush на критических границах.
- Срок хранения и место передачи утверждаются до теста.
- Рядом хранить `manifest.json`: hash файлов, schema/build, число событий и временной диапазон.
