# .prompts

Каноническое место для prompt-шаблонов, используемых в workflow реализации фич.

## Соглашение использования
- Имя команды соответствует имени файла в `.prompts/`.
- `start_task` вызывается в форме `start_task issue_number`.
- Остальные команды получают `issue_number` из имени текущей Git-ветки по шаблону `issue/{issue_number}` и не требуют передачи номера в команде.
- Во всех шаблонах используется единый плейсхолдер `issue_number`.

Пример:

```text
start_task 9
```

```text
make_spec
```

## Шаблоны
- `start_task` — читает GitHub issue и создаёт `brief.md`.
- `review_brief` — ревьюит `brief.md`.
- `make_spec` — строит `spec.md` из `brief.md`.
- `review_spec` — ревьюит `spec.md`.
- `make_plan` — строит `plan.md` из `spec.md`.
- `review_plan` — проверяет `plan.md` на соответствие `spec.md`.
- `ground_plan` — проверяет `plan.md` на соответствие кодовой базе.
- `implement` — реализует `plan.md`.
- `check_links` — проверяет достижимость файлов `memory-bank` из `memory-bank/README.md` и отсутствие битых markdown-ссылок.

## Артефакты
Все шаблоны работают относительно директории:

```text
memory-bank/features/{issue_number}/
```

В зависимости от шага workflow там создаются или проверяются:
- `brief.md`
- `spec.md`
- `plan.md`
