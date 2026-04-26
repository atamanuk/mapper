---
title: Mapper.Demo Problem Statement
doc_kind: domain
doc_function: canonical
purpose: Назначение demo-приложения Mapper.Demo, его ограниченный сценарий и стартовый пример Json Patch.
derived_from:
  - ../problem.md
status: active
audience: humans_and_agents
canonical_for:
  - mapper_demo_problem_statement
  - mapper_demo_scenario
---

# Mapper.Demo Problem Statement

`Mapper.Demo` подключается к `src/Mapper.sln`, поэтому его сборка входит в обычный `dotnet build src/Mapper.sln`.

Для локального запуска demo используйте:
- `dotnet run --project src/Mapper.Demo/Mapper.Demo.csproj`

Для запуска из редактора используйте:
- `.vscode/launch.json` -> `Launch Mapper.Demo`

Назначение demo-проекта:
- показать на одной странице demo-модели `Source` и `Target` в формате `name: type`;
- дать ввести или отредактировать Json Patch для `Source`;
- прогнать `JsonPatchDocument<Source>` через локальную библиотеку `Mapper`;
- отобразить сериализованный результат как `JsonPatchDocument<Target>`.

Ограниченный demo-сценарий:
- `Source`: `Name: string`, `Age: int`;
- `Target`: `DisplayName: string`, `Age: int`;
- демонстрируется same-name mapping `Age -> Age`;
- демонстрируется `MapFrom(...)`-переименование `Name -> DisplayName`.

Стартовое состояние страницы:
- при первой загрузке показываются поля `Source` и `Target` в формате `name: type`;
- поле ввода уже заполнено обязательным примером patch-а;
- результат остаётся на той же странице после нажатия `Map`.

Стартовый пример patch-а:
```json
[
  { "op": "replace", "path": "/Name", "value": "Alice" },
  { "op": "replace", "path": "/Age", "value": 42 }
]
```

Ожидаемый результат для этого примера:
```json
[
  { "op": "replace", "path": "/DisplayName", "value": "Alice" },
  { "op": "replace", "path": "/Age", "value": 42 }
]
```
