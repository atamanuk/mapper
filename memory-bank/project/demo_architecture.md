# Mapper.Demo Architecture

`Mapper.Demo` — демонстрационное ASP.NET Core приложение на Razor Pages, расположенное в `src/Mapper.Demo`.
Его задача — дать минимальный интерактивный сценарий для ручной проверки библиотеки `Mapper`:
пользователь вводит `Json Patch` для модели `Source`, приложение преобразует его в
`JsonPatchDocument<Target>` через локальную библиотеку, а затем по отдельному действию применяет
этот mapped patch к фиксированному экземпляру `Target`. На странице остаются наблюдаемыми как
сериализованный target patch, так и итоговое состояние объекта или диагностическая ошибка.

## Роль в репозитории

- Проект не расширяет архитектуру библиотеки отдельным доменным слоем, а выступает thin UI-shell над `Mapper`.
- Основная зависимость — локальный проект `src/Mapper`, подключённый через `ProjectReference`.
- Технологическая база: `net8.0`, ASP.NET Core Razor Pages, `Microsoft.AspNetCore.JsonPatch`, `Newtonsoft.Json`.

## Архитектурные слои

### 1. Composition root

Файл `src/Mapper.Demo/Program.cs` собирает всё приложение:

- регистрирует Razor Pages;
- создаёт singleton `Mapper.Mapper` с профилем `DemoProfile`;
- настраивает стандартный HTTP pipeline (`UseExceptionHandler`, `UseHsts`, `UseHttpsRedirection`,
  `UseStaticFiles`, `UseRouting`, `MapRazorPages`).

Важно, что demo не использует отдельный DI-слой для профилей или сервисов мэппинга:
экземпляр `Mapper` создаётся прямо в startup-коде, потому что сценарий фиксированный и малый.

### 2. UI-слой

Главный пользовательский поток сосредоточен в одной странице:

- `src/Mapper.Demo/Pages/Index.cshtml` — Razor-разметка главного экрана;
- `src/Mapper.Demo/Pages/Index.cshtml.cs` — `PageModel`, управляющая вводом, вызовом mapper и выводом результата.

`IndexModel` выполняет три задачи:

- хранит и биндингует текстовое поле `PatchInput`;
- предоставляет метаданные `SourceFields` и `TargetFields` для отображения текущих моделей;
- обрабатывает два явных POST-сценария: `Map` и `Apply`.

Поведение `IndexModel` теперь разделено так:

- `OnPostMap()` десериализует source patch, вызывает `_mapper.Map<Source, Target>(...)` и подготавливает
  сериализованный `JsonPatchDocument<Target>` в `ResultJson`;
- `OnPostApply()` в рамках того же HTTP-запроса заново строит mapped patch из текущего `PatchInput`,
  применяет его к новому фиксированному экземпляру `Target` и сохраняет итоговое состояние в `ApplyResultJson`;
- ошибки построения mapped patch попадают в `MapErrorMessage`, а ошибки применения и ошибки `ApplyTo(...)`
  фиксируются отдельно в `ApplyErrorMessage`.

В UI нет отдельного API-контроллера или клиентского SPA: весь цикл запроса выполняется обычным HTML form post
в пределах одной страницы.

### 3. Mapping configuration

Файл `src/Mapper.Demo/Mapping/DemoProfile.cs` содержит единственный профиль мэппинга:

- `Age -> Age` работает по same-name mapping библиотеки;
- `Name -> DisplayName` задаётся явно через `ForMember(...).MapFrom(...)`.

Этот профиль кодирует демонстрационный сценарий и служит живым примером того, как потребитель библиотеки
описывает правила соответствия между source и target моделями.

### 4. Demo domain models

Модели лежат в `src/Mapper.Demo/Models`:

- `Source`: `Name`, `Age`;
- `Target`: `DisplayName`, `Age`.

Они намеренно маленькие и нужны не как бизнес-сущности, а как фиксированный контракт для примера.
Архитектурно это DTO для демонстрации возможностей библиотеки.

### 5. Infrastructure helpers

В `src/Mapper.Demo/Services` находятся два статических вспомогательных компонента:

- `DemoMetadata` — читает публичные свойства модели через reflection и формирует строки вида `Property: type`
  для отображения на странице;
- `PatchJsonSerializer` — преобразует сырой JSON в `JsonPatchDocument<T>` и обратно через список `Operation<T>`.

Оба сервиса не содержат бизнес-логики мэппинга. Их роль — поддержка demo UX:
показ структуры моделей и стабильная сериализация/десериализация patch-операций.

## Поток данных

Последовательность запроса в demo такая:

1. Пользователь открывает `/` и получает страницу с примером patch-документа.
2. `IndexModel.OnGet()` отдаёт начальное состояние без вычислений.
3. При нажатии `Map` или `Apply` `IndexModel` получает строку `PatchInput`.
4. `PatchJsonSerializer.Deserialize<Source>()` превращает JSON-массив операций в `JsonPatchDocument<Source>`.
5. Singleton `Mapper.Mapper` применяет профиль `DemoProfile` и строит `JsonPatchDocument<Target>`.
6. Шаг `Map` сериализует target patch для вывода в `<pre>`.
7. Шаг `Apply` заново использует этот mapped patch в том же запросе, создаёт фиксированный экземпляр
   `Target` со стартовым состоянием `DisplayName = "Before patch"` и `Age = 7`, затем вызывает `ApplyTo(...)`.
8. После успешного `Apply` страница показывает уже не список операций, а сериализованное состояние объекта `Target`.
9. Ошибки JSON, мэппинга и применения переводятся в наблюдаемые сообщения на той же странице, без silent failure.

## Архитектурные ограничения и решения

- Приложение intentionally monolithic: один UI endpoint, один профиль, один mapper instance.
- Нет разделения на application/domain/infrastructure слои, потому что demo обслуживает один ручной сценарий.
- Статические helper-сервисы достаточны, так как у них нет состояния и расширяемых контрактов.
- `Mapper` зарегистрирован singleton'ом, потому что профиль фиксирован и не зависит от запроса.
- Шаг `Apply` не использует неявное межзапросное состояние: mapped patch каждый раз заново вычисляется из текущего `PatchInput`.
- Вся интерактивность построена вокруг server-rendered Razor Pages, без REST API и без JavaScript-логики мэппинга.

## Что менять, если demo будет расти

Если `Mapper.Demo` начнёт покрывать несколько сценариев или профилей, текущую архитектуру стоит расширять так:

- вынести регистрацию профилей и mapper-конфигурации в отдельный extension для DI;
- разделить demo-сценарии по нескольким Razor Pages или handler'ам;
- выделить отдельный application service над `Mapper`, если появится логика подготовки patch-документов;
- перевести metadata/serialization helpers на интерфейсы только если появятся альтернативные реализации или тестовая подмена.
