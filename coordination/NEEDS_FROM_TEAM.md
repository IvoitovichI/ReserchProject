# Needs From Team

Главный владелец файла — Programmer. Остальные роли могут добавлять запросы, но не удаляют чужие строки. Этот список — обязательный ответ на вопрос «чего не хватает для следующей реализации».

| need_id | need | blocks | owner | required_format_or_path | priority | milestone | placeholder_allowed | status | updated |
|---|---|---|---|---|---|---|---|---|---|
| NEED-001 | Путь к Unity-репозиторию и версия Editor | Инвентаризация и команды проверки | User | repository path + `ProjectVersion.txt` | P0 | Setup | No | Integrated | 2026-09-30 |
| NEED-002 | Формулировка основного исследовательского вопроса | Финальный metrics contract | User/Researcher | 1 primary + до 3 secondary questions | P0 | Research setup | No | Answered — DEC-003 | 2026-09-30 |
| NEED-003 | Первые визуальные референсы | Art direction и asset briefs | User | файлы в `references/3d-modeler/images/` + index | P1 | Art blockout | Yes | Open | 2026-09-29 |
| NEED-004 | Игровые референсы с целевыми механиками | Reference review | User/Game Designer | link/file + timestamp + target mechanic | P1 | Design baseline | Yes | Open | 2026-09-29 |
| NEED-005 | Канонические имена envelope и event registry для schema v1 | DT-001, BLK-002, telemetry implementation | User/Researcher | явное подтверждение `METRICS_CONTRACT.md` либо versioned mapping/новый registry | P0 | Research setup | No | Integrated — DEC-004 / schema `1.0.0` | 2026-09-30 |
| NEED-006 | Approved pilot consent, retention, withdrawal and data-access rules | research-mode UI, local export, participant collection | User/Researcher | consent version/text or approved reference; retention period; access/transfer; withdrawal handling; stop/discomfort rule | P0 | Research setup | No | Answered — `PILOT-CONSENT-v1.0`; institutional approval remains BLK-004 | 2026-09-30 |
| NEED-007 | Операционализация PRQ/SQ2/SQ3 до сбора | preregistration-lite and analysis plan | User/Researcher | condition/intervention and assignment; primary accuracy/interpretability outcome; Hexad/Bartle form/licensing/timing; recommendation-evaluation rubric and raters | P0 | Research setup | No | Answered — DEC-004; instrument wording/licensing and rater recruitment remain protocol gates | 2026-09-30 |

## Формат новой записи

- `need`: конкретный предмет, не «нужно больше информации»;
- `blocks`: задача, файл, API или решение;
- `owner`: кто способен ответить;
- `required_format_or_path`: как выглядит готовый вход;
- `placeholder_allowed`: `Yes` только с описанием ограничения в handoff;
- `status`: `Open / Answered / Integrated / Rejected`.
