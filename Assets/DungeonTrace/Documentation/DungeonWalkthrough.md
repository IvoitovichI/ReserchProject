# Dungeon Trace — Dungeon Walkthrough

## Назначение

`Assets/Scenes/DungeonWalkthrough.unity` — ручная playable-проверка authored placeholder content, а не полноценный generated run и не research build. Сцена позволяет пройти через три соединённые prefab-комнаты без смены сцен:

`Room_Empty → Room_Table → Room_Crypt`

Комнаты используют generated placeholder content `prototype-02`. Их нельзя смешивать в сравнении с данными `prototype-01`.

## Запуск

1. Откройте `Assets/Scenes/DungeonWalkthrough.unity`.
2. Войдите в Play Mode.
3. `GameBootstrap` создаст FP-игрока в позиции `(0, 0, -4)` перед первой комнатой, установит `ResearchMode.Standard`, seed `12345` и `content_version` `prototype-02`.
4. Пройдите через две открытые арки в следующие комнаты.

Текущие controls: `WASD`/левый стик — движение, mouse/правый стик — обзор, `E` — interaction в пределах prototype room. Jump временно существует в коде, но не является поддерживаемой частью MVP walkthrough и должен быть удалён при завершении Phase 1.

## Границы проверки

Сцена подтверждает только непрерывный first-person traversal и стартовую интеграцию prefab content. Она не подтверждает combat, encounters, награды, secrets, NavMesh AI, telemetry, save/load, counterbalancing или валидность research session.

## Регенерация

Если scene-asset требуется восстановить, выполните Unity menu command **Dungeon Trace > Scenes > Create Dungeon Walkthrough**. Генератор находится в `DungeonWalkthroughSceneGenerator`; он создаёт новую сцену из `Assets/DungeonTrace/GeneratedDungeon`.
