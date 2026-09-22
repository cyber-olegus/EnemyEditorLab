# Enemy Editor Lab

WPF-редактор шаблонов противников для будущей игры-кликера. Проект соответствует лабораторной работе № 1: классы игровых данных, загрузка PNG-иконок, добавление и удаление противников, сериализация и ручная десериализация JSON.

## Возможности по веткам

| Ветка | Готовая версия |
| --- | --- |
| `main` | Базовый редактор: создание, изменение и удаление противников; имя иконки вводится вручную |
| `feature/enemy-icons` | Версия участника № 1: выбор папки, галерея PNG, единый размер и главная иконка |
| `feature/json-persistence` | Версия участника № 2: полная программа с сохранением и загрузкой JSON |

Ветки являются последовательными: `feature/json-persistence` построена поверх `feature/enemy-icons`. Сначала сливается Pull Request с иконками, затем Pull Request с JSON.

## Запуск

Требования:

- Windows 10/11;
- Visual Studio 2022 с компонентом **Разработка классических приложений .NET**;
- .NET 8 SDK.

Откройте `EnemyEditorLab.sln`, назначьте `EnemyEditor.Wpf` запускаемым проектом и нажмите `F5`.

Или используйте PowerShell:

```powershell
dotnet restore
dotnet build EnemyEditorLab.sln
dotnet run --project src/EnemyEditor.Wpf
```

`System.Text.Json` входит в .NET 8, поэтому отдельный NuGet-пакет устанавливать не требуется.

## Структура

```text
EnemyEditorLab/
├── src/
│   ├── EnemyEditor.Core/       # игровые классы, каталог иконок, JSON
│   └── EnemyEditor.Wpf/        # интерфейс редактора и приложенные PNG
├── examples/                   # пример списка противников
├── docs/GIT-WORKFLOW.md        # пошаговая работа вдвоём
├── docs/CONTROL-QUESTIONS.md   # ответы на контрольные вопросы
└── EnemyEditorLab.sln
```

## Основные классы

- `CEnemyTemplate` — данные одного противника с приватными сеттерами.
- `CEnemyTemplateList` — добавление, поиск, изменение, удаление и JSON-файлы.
- `EnemyIcon` — имя файла и абсолютный путь к изображению.
- `EnemyIconCatalog` — поиск PNG-файлов в выбранной папке и вложенных каталогах.

Полный сценарий публикации двух версий описан в [docs/GIT-WORKFLOW.md](docs/GIT-WORKFLOW.md).

