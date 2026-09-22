# Работа двух участников через GitHub

## 1. Восстановление репозитория из bundle

Положите `EnemyEditorLab.bundle` в отдельную папку и выполните:

```powershell
git clone .\EnemyEditorLab.bundle EnemyEditorLab
cd EnemyEditorLab
git branch feature/enemy-icons origin/feature/enemy-icons
git branch feature/json-persistence origin/feature/json-persistence
git switch main
```

Проверьте ветки и историю:

```powershell
git branch
git log --graph --oneline --decorate --all
```

## 2. Настройка авторов

Каждый участник на своём компьютере указывает личные данные GitHub:

```powershell
git config user.name "Имя Фамилия"
git config user.email "email-с-GitHub@example.com"
```

Подготовительные коммиты имеют нейтрального автора `Enemy Editor Team`. Чтобы в истории были личные изменения, каждый участник должен проверить свою часть и сделать хотя бы одно настоящее улучшение программы под собственным именем.

Примеры небольших осмысленных изменений:

- участник № 1 может изменить размер миниатюр или текст состояния после выбора иконки;
- участник № 2 может изменить предлагаемое имя JSON-файла или добавить ещё один пример противника.

Не создавайте пустые или фиктивные коммиты.

## 3. Создание GitHub-репозитория

Один участник создаёт пустой репозиторий `EnemyEditorLab` без автоматического README и добавляет второго участника через **Settings → Collaborators**.

Замените временный адрес bundle на URL GitHub:

```powershell
git remote set-url origin https://github.com/USERNAME/EnemyEditorLab.git
git push -u origin main
git push -u origin feature/enemy-icons
git push -u origin feature/json-persistence
```

## 4. Изменение участника № 1

Первый участник переключается на свою ветку:

```powershell
git switch feature/enemy-icons
```

Он запускает программу, проверяет выбор папки и иконок, затем вносит небольшое реальное улучшение. После проверки:

```powershell
git add путь-к-изменённому-файлу
git commit -m "style(icons): improve icon gallery"
git push
```

На GitHub создаётся Pull Request:

- `base`: `main`;
- `compare`: `feature/enemy-icons`;
- заголовок: `Добавлена галерея иконок противников`.

Второй участник просматривает **Files changed**, запускает программу и оставляет review. Затем PR сливается вариантом **Create a merge commit**.

## 5. Изменение участника № 2

После слияния первого PR второй участник выполняет:

```powershell
git fetch origin
git switch feature/json-persistence
```

Он проверяет сохранение и загрузку `examples/enemies.example.json`, вносит своё небольшое улучшение и фиксирует его:

```powershell
git add путь-к-изменённому-файлу
git commit -m "feat(json): improve enemy file workflow"
git push
```

На GitHub создаётся второй Pull Request:

- `base`: `main`;
- `compare`: `feature/json-persistence`;
- заголовок: `Добавлено сохранение и загрузка JSON`.

Так как ветка JSON построена поверх ветки иконок, после первого merge GitHub покажет во втором PR только новые JSON-изменения. Первый участник проверяет PR, после чего он сливается через **Create a merge commit**.

## 6. Получение полной версии

После двух Pull Request оба участника обновляют `main`:

```powershell
git switch main
git pull origin main
dotnet build EnemyEditorLab.sln
```

Итоговый `main` должен уметь:

1. выбирать папку с PNG;
2. показывать иконки одинакового размера;
3. создавать, изменять и удалять противников;
4. сохранять список в JSON;
5. загружать список из JSON;
6. показывать понятные ошибки для неверных данных.

## 7. Что показать преподавателю

- работающий редактор;
- три исходные ветки и итоговый `main`;
- два Pull Request, созданные разными участниками;
- review каждого участника;
- личные коммиты обоих участников;
- JSON-файл со списком противников;
- граф истории `git log --graph --oneline --decorate --all`.

