# DungeonTrace Multi-Agent Pack

Готовый набор инструкций для четырех ролей Codex:

1. `dungeontrace_game_designer` — анализ игровых референсов и измеримые дизайн-рекомендации.
2. `dungeontrace_researcher` — исследовательский дизайн, телеметрия, QA и анализ CSV/JSON.
3. `dungeontrace_3d_modeler` — разбор визуальных референсов, asset briefs и 3D QA.
4. `dungeontrace_programmer` — Unity/C# имплементация, тесты и обязательный список недостающих входов.

## Быстрый старт

1. Скопируй содержимое пакета в корень Git-репозитория Unity.
2. Проверь, что master-документ доступен как `docs/DungeonTrace_CODEX_MASTER.md`.
3. Положи игровые референсы в `references/game-design/`, исследовательские источники — в `references/research/`, визуальные — в `references/3d-modeler/images/`.
4. Заполни соответствующие `REFERENCE_INDEX.md`.
5. Открой `TEAM_LAUNCH_GUIDE.md` и выбери режим запуска.
6. Для первого запуска используй `prompts/ORCHESTRATOR_START.md`.

## Главные файлы

| Файл | Назначение |
|---|---|
| `AGENTS.md` | общие правила проекта |
| `.codex/agents/*.toml` | project-scoped определения четырех custom agents |
| `agents/*/AGENTS.md` | подробные правила ролей для отдельных сессий |
| `TEAM_LAUNCH_GUIDE.md` | запуск, изоляция и коммуникация |
| `coordination/TASK_BOARD.md` | задачи и владельцы |
| `coordination/DECISIONS.md` | архитектурные и исследовательские решения |
| `coordination/NEEDS_FROM_TEAM.md` | обязательный список того, чего не хватает программисту |
| `coordination/METRICS_CONTRACT.md` | словарь событий и envelope телеметрии |
| `coordination/handoffs/` | отдельный outbox каждой роли |

## Рекомендованный режим

- Для быстрых обзоров: одна сессия Orchestrator + четыре subagents.
- Для длительной параллельной работы с файлами: отдельный Git worktree на роль.
- Для самого первого vertical slice: один Programmer пишет код; остальные агенты сначала готовят требования и review, не меняя Unity assets параллельно.

