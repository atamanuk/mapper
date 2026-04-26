---
title: Project Architecture
doc_kind: domain
doc_function: canonical
purpose: Каноническая структура репозитория и расположение solution.
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

Инженерные команды вынесены в [Engineering Documentation Index](../engineering/README.md).
