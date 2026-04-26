---
title: Build
doc_kind: engineering
doc_function: canonical
purpose: Базовая команда сборки проекта и статус отдельного build-скрипта.
derived_from:
  - ../domain/architecture.md
status: active
audience: humans_and_agents
---

# Build

Для обычной сборки достаточно стандартных CLI-команд `dotnet`.
Отдельный build-скрипт для повседневной сборки в репозитории не нужен и не используется как основной путь.

Основная команда:
- `dotnet build src/Mapper.sln` — сборка проекта.
