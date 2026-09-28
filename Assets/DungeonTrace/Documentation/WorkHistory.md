# Dungeon Trace — история работ

## 2026-09-23 — базовый вертикальный срез

- Проект остаётся Unity 6.3.19f1 с URP; версии Unity, packages и render pipeline не менялись.
- Существует playable FP3D prototype: движение, обзор, прыжок, interaction-ray и trace console.
- Есть domain/runtime основы: сессия с детерминированным seed, политика ResearchObservation, health/death flow и placeholder room builder.
- Имеются EditMode и PlayMode тесты для текущего вертикального среза.

## 2026-09-23 — authoring комнат

- `RoomDefinition` расширен authored-данными: `room_id`, `content_version`, тип, tier, weight, tags, prefab, door sockets, encounter variants, telemetry zones и диапазон ожидаемого времени.
- Добавлены prefab markers и `RoomValidator`; они задают и проверяют данные комнат, но не добавляют dungeon generation, AI, награды или телеметрию.
- Добавлено окно **Dungeon Trace > Rooms > Room Authoring**: оно через Unity API создаёт безопасный placeholder prefab/definition, сканирует маркеры и показывает результаты проверки с переходом к проблемному объекту.
- Добавлены EditMode-тесты валидатора и PlayMode-тест изолированной комнаты. Их необходимо выполнить в Unity Test Runner после импорта.

## Текущий следующий шаг

Unity CLI 1.0.0-beta.11 установлен. В `Packages/manifest.json` добавлен официальный `com.unity.pipeline` 0.7.0-exp.1, а Unity MCP зарегистрирован в Codex и закреплён за `GameForStudy`. После перезапуска Editor Pipeline/MCP server подтверждён на loopback endpoint; он предоставляет 151 editor-команду. Test Runner через Pipeline: 8/8 EditMode и 4/4 PlayMode тестов прошли.

## 2026-09-26 — аудит статуса и walkthrough-документация

- `DungeonWalkthrough.unity` подтверждён как authored walkthrough из `Room_Empty → Room_Table → Room_Crypt`; `GameBootstrap` выбирает для него `content_version: prototype-02` и создаёт игрока в первой комнате.
- Фактический status Phase 0–3 отражён в master specification. Phase 3 закрыта для placeholder vertical slice: в `DungeonGenerationTests` есть проверка same-seed determinism, independence от порядка definitions, derived retry seed, reachability и 1,000 logical seeds.
- Phase 1 и 2 остаются partial: в prototype ещё есть jump, нет dash/pause/full Input Actions adapter, а room content ограничен тремя placeholder prefab layouts.
- Следующая реализация: минимальный Phase 4 vertical slice — общие damage contracts и Pulse Pistol до добавления других weapon strategies.

## 2026-09-26 — Phase 4, минимальный combat vertical slice

- Добавлены pure contracts `IDamageable`, `DamageContext`, `DamageResult` и `DamageType`. Контекст несёт source/target/correlation IDs, base damage, type и distance; он не зависит от telemetry и не принимает решений gameplay.
- `Health` реализует `IDamageable`; переход в death возвращается только первым применением fatal damage. EditMode tests проверяют нормализацию недопустимого payload и idempotent death result.
- Добавлены `WeaponDefinition` и `WeaponController`. `GameBootstrap` выдаёт игроку runtime placeholder Pulse Pistol: semi-automatic camera-ray hitscan с ЛКМ/right trigger, damage 20, interval 0.25 s, range 35 m.
- Phase 4 имеет статус **In progress**: это ещё не полный combat MVP и не research content. Перед продолжением нужны Unity import/Test Runner и ручная smoke-проверка оружия по `IDamageable` цели.
- Unity batch import после добавления combat slice завершился без C# ошибок и Unity создал необходимые `.meta` files. Отдельный EditMode Test Runner запуск не создал XML-результат: локальный `LicenseClient-Ivan` не прошёл validation и не получил access token. Это инфраструктурный blocker, поэтому новый результат тестов не заявляется как pass/fail.

## Правило версий контента

Текущий authored placeholder использует `content_version: prototype-01`. Изменение геометрии, spawn layout, баланса или другого room content, способного повлиять на исследовательские метрики, требует новой версии content. Изменение схемы телеметрии требует отдельной schema version.

## 2026-09-23 — детерминированный граф подземелья

- Добавлены чистые `DungeonGraphBuilder` и `DungeonValidator`, независимые от `UnityEngine.Random`. Для graph, rooms, encounters, loot и boss используются отдельные derived RNG-потоки.
- Генератор создаёт обязательный основной путь Start → 4–5 Combat → Choice → Treasure/Shop → Boss и допускает настроенные Elite/Secret ветви. Фактический seed является детерминированной производной исходного seed и номера попытки.
- `DungeonAssembler` инстанцирует prefab-комнаты, выравнивает противоположные DoorSocket с поворотом, контролирует пересечения `RoomBounds` и назначает стабильные instance IDs.
- Созданы primitive placeholder assets в `Assets/DungeonTrace/GeneratedDungeon`: пустая комната, комната со столом и двумя стульями, крипта с саркофагом и пятью колоннами. Их authored content version — `prototype-02`.
- Полный Unity Test Runner остаётся непроверенным в этом запуске: локальный Unity Licensing Client не выдал test-results XML, а Pipeline server не стал доступен после запуска Editor. C#-проекты Runtime, EditModeTests и PlayModeTests компилируются.

## 2026-09-23 — walkthrough placeholder-подземелья

- Создана сцена `Assets/Scenes/DungeonWalkthrough.unity`: последовательность `Room_Empty → Room_Table → Room_Crypt` с двумя открытыми проходными арками.
- При запуске этой сцены `GameBootstrap` использует `prototype-02` и создаёт FP3D-игрока в начале первой комнаты. Игрок может без загрузок сцен пройти во все три комнаты.
- Добавлен Editor generator `Dungeon Trace > Scenes > Create Dungeon Walkthrough`, который может заново создать сцену из prefab-ассетов `GeneratedDungeon`.
