# Лабораторная № 1: работа двух участников через GitHub

Ниже описан безопасный сценарий для **нового пустого репозитория**. Обозначения можно поменять местами, но один участник должен отвечать за иконки, второй — за JSON, поиск и статистику.

## Кто что делает

| Участник | Рабочая ветка | Что находится в части | Личный финальный коммит |
| --- | --- | --- | --- |
| Участник № 1 (вы) | `feature/enemy-icons` | загрузка PNG, каталог, галерея, выбор главной иконки | увеличить миниатюры в галерее |
| Участник № 2 (сокомандник) | `feature/json-persistence` | сохранение и ручная загрузка JSON, поиск, статистика | открывать диалоги JSON из «Документов» |

Ветки уже содержат подготовленный командный код с нейтральным автором `Enemy Editor Team`. Личные финальные изменения выполняются каждым участником самостоятельно: так GitHub честно покажет вклад обоих. Пустые коммиты и изменение автора чужих коммитов не нужны.

## Часть A. Действия участника № 1

### Шаг 1. Распакуйте переданный набор

Сохраните `EnemyEditorLab-teamwork.bundle`, например, в `C:\Projects`. Откройте PowerShell в этой папке.

### Шаг 2. Восстановите репозиторий

```powershell
cd C:\Projects
git clone .\EnemyEditorLab-teamwork.bundle EnemyEditorLab
cd EnemyEditorLab
git branch feature/enemy-icons origin/feature/enemy-icons
git branch feature/json-persistence origin/feature/json-persistence
git switch main
```

Если Git сообщает, что ветка уже существует, это не ошибка: пропустите соответствующую команду `git branch`.

### Шаг 3. Проверьте ветки

```powershell
git branch -a
git log --graph --oneline --decorate --all
git status
```

Рабочая папка должна быть чистой. В истории должны присутствовать `main`, `feature/enemy-icons` и `feature/json-persistence`.

### Шаг 4. Настройте своё имя

Укажите именно имя и почту своего аккаунта GitHub:

```powershell
git config user.name "Ваше Имя"
git config user.email "ваша-почта@example.com"
git config --get user.name
git config --get user.email
```

### Шаг 5. Создайте репозиторий на GitHub

На GitHub нажмите **New repository** и задайте имя `EnemyEditorLab`.

- выберите Public или Private по требованиям преподавателя;
- **не** добавляйте README;
- **не** добавляйте `.gitignore`;
- **не** выбирайте лицензию.

После создания откройте **Settings → Collaborators → Add people**, добавьте аккаунт сокомандника и попросите его принять приглашение.

### Шаг 6. Подключите GitHub и отправьте подготовленные ветки

Замените `YOUR_LOGIN` своим логином:

```powershell
git remote set-url origin https://github.com/YOUR_LOGIN/EnemyEditorLab.git
git remote -v
git push -u origin main
git push -u origin feature/enemy-icons
git push -u origin feature/json-persistence
```

Если используется SSH, вместо HTTPS можно указать `git@github.com:YOUR_LOGIN/EnemyEditorLab.git`.

### Шаг 7. Сделайте своё изменение в части иконок

```powershell
git switch feature/enemy-icons
git pull --ff-only origin feature/enemy-icons
```

Откройте `src/EnemyEditor.Wpf/MainWindow.xaml`. В шаблоне элемента `IconsListBox` измените:

- размер `Border`: `Width="98" Height="112"` → `Width="106" Height="120"`;
- размер вложенного `Image`: `Width="72" Height="72"` → `Width="80" Height="80"`.

Это настоящее визуальное улучшение, а не фиктивный коммит. Запустите программу и проверьте, что иконки не обрезаются.

### Шаг 8. Создайте свой коммит

```powershell
git diff
git add src/EnemyEditor.Wpf/MainWindow.xaml
git commit -m "style(icons): enlarge gallery thumbnails"
git push origin feature/enemy-icons
git status
```

В выводе `git log -1` должно быть ваше имя:

```powershell
git log -1 --format="%h | %an | %ae | %s"
```

### Шаг 9. Создайте первый Pull Request

На GitHub откройте **Pull requests → New pull request**:

- `base`: `main`;
- `compare`: `feature/enemy-icons`;
- заголовок: `Добавлена галерея иконок противников`;
- в описании: что реализовано и как проверялось.

Не сливайте PR сами. Отправьте ссылку сокоманднику.

## Часть B. Действия участника № 2

### Шаг 10. Сокомандник клонирует GitHub-репозиторий

После принятия приглашения сокомандник выполняет на своём компьютере:

```powershell
cd C:\Projects
git clone https://github.com/YOUR_LOGIN/EnemyEditorLab.git EnemyEditorLab
cd EnemyEditorLab
git config user.name "Имя Сокомандника"
git config user.email "почта-сокомандника@example.com"
git fetch origin
git branch -a
```

### Шаг 11. Сокомандник проверяет первый Pull Request

Он открывает PR на GitHub, просматривает вкладку **Files changed**, скачивает ветку и запускает её:

```powershell
git switch --track origin/feature/enemy-icons
dotnet build EnemyEditorLab.sln
dotnet run --project src/EnemyEditor.Wpf
```

На GitHub сокомандник выбирает **Review changes → Approve**. После этого участник № 1 нажимает **Merge pull request → Create a merge commit**. Ветку пока можно не удалять.

### Шаг 12. Сокомандник подготавливает свою ветку

```powershell
git fetch origin
git switch --track origin/feature/json-persistence
git merge origin/main
```

Если локальная ветка уже создана, используйте `git switch feature/json-persistence` вместо команды с `--track`.

Если открылся редактор сообщения merge-коммита, оставьте стандартный текст, сохраните файл и закройте редактор. Затем:

```powershell
git push origin feature/json-persistence
```

### Шаг 13. Сокомандник делает своё изменение

Откройте `src/EnemyEditor.Wpf/MainWindow.xaml.cs`. В объектах `SaveFileDialog` и `OpenFileDialog`, сразу после строки `Title = ...`, добавьте одинаковую настройку:

```csharp
InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
```

В результате оба диалога сохранения и загрузки будут изначально открывать папку «Документы». Сокомандник должен проверить:

1. создание двух или трёх противников;
2. фильтрацию по части имени;
3. изменение статистики;
4. сохранение в JSON;
5. очистку/перезапуск программы и загрузку сохранённого файла;
6. загрузку `examples/enemies.example.json`.

### Шаг 14. Сокомандник создаёт свой коммит

```powershell
git diff
git add src/EnemyEditor.Wpf/MainWindow.xaml.cs
git commit -m "feat(json): open file dialogs in Documents"
git push origin feature/json-persistence
git log -1 --format="%h | %an | %ae | %s"
```

В последней строке должны отображаться имя и почта сокомандника.

### Шаг 15. Создайте второй Pull Request

Сокомандник создаёт PR:

- `base`: `main`;
- `compare`: `feature/json-persistence`;
- заголовок: `Добавлены JSON, поиск и статистика противников`.

Участник № 1 просматривает код, запускает программу и оставляет **Approve**. После проверки сокомандник или владелец репозитория выполняет **Create a merge commit**.

## Часть C. Финальная проверка обоими участниками

### Шаг 16. Обновите итоговую ветку

На обоих компьютерах:

```powershell
git switch main
git pull --ff-only origin main
dotnet restore
dotnet build EnemyEditorLab.sln
dotnet run --project src/EnemyEditor.Wpf
```

### Шаг 17. Проверьте результат

Итоговый `main` должен уметь:

1. выбирать папку с PNG;
2. показывать галерею и главную иконку;
3. создавать, изменять и удалять противников;
4. искать противников по имени;
5. показывать среднее здоровье и золото;
6. сохранять список в JSON;
7. вручную разбирать и загружать JSON;
8. показывать понятные сообщения при неверных данных.

Проверьте историю:

```powershell
git log --graph --oneline --decorate --all
git shortlog -sne --all
```

## Что показать преподавателю

- работающий редактор;
- ветки `main`, `feature/enemy-icons`, `feature/json-persistence`;
- два Pull Request от разных участников;
- два взаимных review;
- личный коммит каждого участника;
- JSON-файл со списком противников;
- граф `git log --graph --oneline --decorate --all`;
- таблицу разделения ответственности из этого документа.

## Частые ошибки

### `failed to push some refs`

Сначала проверьте адрес:

```powershell
git remote -v
```

Он должен указывать на GitHub, а не на локальный `.bundle`. Если указан bundle:

```powershell
git remote set-url origin https://github.com/YOUR_LOGIN/EnemyEditorLab.git
```

### `src refspec ... does not match any`

Ветка не создана локально. Выполните `git branch -a`, затем создайте её из удалённой:

```powershell
git switch --track origin/feature/json-persistence
```

### Git просит выбрать автора

Настройте имя и почту командами из шагов 4 или 10, затем повторите `git commit`.

### Возник конфликт при `git merge origin/main`

Не удаляйте маркеры наугад. Выполните `git status`, сохраните сообщение и скриншот конфликта и разберите его вместе. После исправления каждого файла:

```powershell
git add путь-к-исправленному-файлу
git commit
git push
```
