---
title: Project Architecture
doc_kind: domain
doc_function: canonical
purpose: Каноническая структура репозитория, расположение solution и базовые команды сборки через dotnet.
derived_from:
  - ../dna/governance.md
status: active
audience: humans_and_agents
---

# Project Architecture

## Repository Structure

Каноническая структура репозитория:
- `src/Mapper.sln` — основное solution проекта;
- `src/Mapper` — библиотека;
- `src/Mapper.Tests` — отдельный тестовый проект;
- `src/Mapper.Demo` — демонстрационный Razor Pages проект для ручной проверки мэппинга.

## Build And Test

Для обычной сборки достаточно стандартных CLI-команд `dotnet`.
Отдельный build-скрипт для повседневной сборки в репозитории не нужен и не используется как основной путь.

Основные команды:
- `dotnet build src/Mapper.sln` - сборка проекта;
- `dotnet test src/Mapper.sln` - запуск тестов.
