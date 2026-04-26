---
title: Project Problem Statement
doc_kind: domain
doc_function: canonical
purpose: Каноничное описание продукта, проблемного пространства и целевых outcomes. Читать перед feature-спеками, чтобы не повторять общий контекст в каждой delivery-единице.
derived_from:
  - ../dna/governance.md
status: active
audience: humans_and_agents
canonical_for:
  - project_problem_statement
  - product_context
  - top_level_outcomes
---

# Project Overview

Библиотека `Mapper` преобразует `JsonPatchDocument<TSource>` в `JsonPatchDocument<TTarget>`. 
Основная задача — перенести операции Json Patch из одной модели в другую с учётом настроенного 
соответствия полей и преобразования значений.

Целевая платформа библиотеки - `netstandard2.1`. 

Исходный код библиотеки находится в `src/Mapper`.

В репозитории также есть демонстрационное приложение `src/Mapper.Demo` на Razor Pages.
Оно нужно для ручной проверки мэппинга patch-документов через локальную библиотеку `Mapper`:
пользователь вводит `Json Patch` для demo-модели `Source`, а приложение показывает
сериализованный результат как `JsonPatchDocument<Target>` или читаемую ошибку.

Ключевые публичные сущности проекта:
- `Mapper` — основной рантайм-компонент, который принимает профили и маппит patch-документы между типами.
- `MapProfile` — базовый класс для конфигурации соответствий между `TSource` и `TTarget`.

## Поддерживаемые правила мэппинга в текущей реализации:
- same-name mapping для одноимённых свойств;
- явное переименование и вычисление через `ForMember(...).MapFrom(...)`;
- исключение целевого поля через `Ignore()`;
- преобразование значения, если типы source и target различаются.

Библиотека работает как runtime-компонент с компиляцией правил в памяти. 

Текущий demo-сценарий в `Mapper.Demo` намеренно ограничен:
- `Source`: `Name: string`, `Age: int`;
- `Target`: `DisplayName: string`, `Age: int`;
- `Age -> Age` демонстрирует same-name mapping;
- `Name -> DisplayName` демонстрирует `ForMember(...).MapFrom(...)`.
