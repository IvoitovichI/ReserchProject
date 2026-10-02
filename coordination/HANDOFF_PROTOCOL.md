# Handoff Protocol

Handoff — это контракт между ролями, а не отчет о занятости.

## Когда нужен handoff

- Game Designer передает механику/баланс Programmer или visual brief Modeler.
- Researcher передает event/analysis contract Programmer или design constraint Game Designer.
- Modeler передает asset + import/integration brief Programmer.
- Programmer передает playable build/data sample на review Designer/Researcher и art integration issue Modeler.

## Обязательная структура

1. `handoff_id`, дата, from, to, related task/decision.
2. Короткий outcome.
3. Измененные/созданные файлы.
4. Контракты, stable IDs и версии.
5. Acceptance criteria и результаты проверки.
6. Неопределенности, ограничения, placeholders.
7. Точный requested action следующего владельца.
8. Статус: `Proposed / Ready / Accepted / Changes Requested / Integrated`.

## Правила параллельной работы

- В одной рабочей копии каждый агент пишет только в свои файлы и согласованные outputs.
- В разных worktree handoff становится доступен другим после commit/merge; оркестратор является интегратором.
- Не использовать общий markdown как «live chat», если четыре worktree пишут в него одновременно.
- Конфликт не решается удалением чужой версии. Остановить интеграцию и определить владельца.

