using System.Text.Encodings.Web;
using System.Text.Json;

namespace EnemyEditor.Core;

/// <summary>
/// Управляет коллекцией шаблонов противников.
/// </summary>
public sealed class CEnemyTemplateList
{
    private readonly List<CEnemyTemplate> _enemies = [];

    public IReadOnlyList<CEnemyTemplate> Enemies => _enemies.AsReadOnly();

    public int Count => _enemies.Count;

    public void AddEnemy(
        string name,
        string iconName,
        int baseLife,
        double lifeModifier,
        int baseGold,
        double goldModifier,
        double spawnChance)
    {
        EnsureUniqueName(name);
        _enemies.Add(new CEnemyTemplate(
            name,
            iconName,
            baseLife,
            lifeModifier,
            baseGold,
            goldModifier,
            spawnChance));
    }

    public CEnemyTemplate? GetEnemyByName(string name)
    {
        return _enemies.FirstOrDefault(enemy =>
            string.Equals(enemy.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public CEnemyTemplate GetEnemyByIndex(int id)
    {
        if (id < 0 || id >= _enemies.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Противник с таким индексом отсутствует.");
        }

        return _enemies[id];
    }

    public bool DeleteEnemyByName(string name)
    {
        CEnemyTemplate? enemy = GetEnemyByName(name);
        return enemy is not null && _enemies.Remove(enemy);
    }

    public void DeleteEnemyByIndex(int id)
    {
        _enemies.RemoveAt(id);
    }

    public List<string> GetListOfEnemyNames()
    {
        return _enemies.Select(enemy => enemy.Name).ToList();
    }

    public void SaveToJson(string path)
    {
        string fullPath = ValidateJsonPath(path);
        string? directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        string json = JsonSerializer.Serialize(_enemies, options);
        File.WriteAllText(fullPath, json);
    }

    public void LoadFromJson(string path)
    {
        string fullPath = ValidateJsonPath(path);
        string json = File.ReadAllText(fullPath);

        var documentOptions = new JsonDocumentOptions
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip
        };

        using JsonDocument document = JsonDocument.Parse(json, documentOptions);
        if (document.RootElement.ValueKind != JsonValueKind.Array)
        {
            throw new JsonException("Корневой элемент JSON должен быть массивом противников.");
        }

        var loadedList = new CEnemyTemplateList();
        int position = 0;
        foreach (JsonElement element in document.RootElement.EnumerateArray())
        {
            position++;
            if (element.ValueKind != JsonValueKind.Object)
            {
                throw new JsonException($"Элемент №{position} должен быть объектом.");
            }

            try
            {
                loadedList.AddEnemy(
                    ReadString(element, nameof(CEnemyTemplate.Name)),
                    ReadString(element, nameof(CEnemyTemplate.IconName)),
                    ReadInt32(element, nameof(CEnemyTemplate.BaseLife)),
                    ReadDouble(element, nameof(CEnemyTemplate.LifeModifier)),
                    ReadInt32(element, nameof(CEnemyTemplate.BaseGold)),
                    ReadDouble(element, nameof(CEnemyTemplate.GoldModifier)),
                    ReadDouble(element, nameof(CEnemyTemplate.SpawnChance)));
            }
            catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
            {
                throw new JsonException($"Ошибка в элементе №{position}: {exception.Message}", exception);
            }
        }

        // Заменяем текущий список только после успешной проверки всего файла.
        _enemies.Clear();
        _enemies.AddRange(loadedList._enemies);
    }

    public void ReplaceEnemyByIndex(
        int id,
        string name,
        string iconName,
        int baseLife,
        double lifeModifier,
        int baseGold,
        double goldModifier,
        double spawnChance)
    {
        CEnemyTemplate oldEnemy = GetEnemyByIndex(id);
        bool nameChanged = !string.Equals(oldEnemy.Name, name, StringComparison.OrdinalIgnoreCase);
        if (nameChanged)
        {
            EnsureUniqueName(name);
        }

        _enemies[id] = new CEnemyTemplate(
            name,
            iconName,
            baseLife,
            lifeModifier,
            baseGold,
            goldModifier,
            spawnChance);
    }

    private void EnsureUniqueName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Имя противника не должно быть пустым.", nameof(name));
        }

        string normalizedName = name.Trim();
        if (GetEnemyByName(normalizedName) is not null)
        {
            throw new InvalidOperationException($"Противник с именем «{normalizedName}» уже существует.");
        }
    }

    private static string ValidateJsonPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Путь к JSON-файлу не должен быть пустым.", nameof(path));
        }

        return Path.GetFullPath(path);
    }

    private static string ReadString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement value)
            || value.ValueKind != JsonValueKind.String)
        {
            throw new JsonException($"Свойство {propertyName} должно быть строкой.");
        }

        return value.GetString()!;
    }

    private static int ReadInt32(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement value)
            || !value.TryGetInt32(out int result))
        {
            throw new JsonException($"Свойство {propertyName} должно быть целым числом.");
        }

        return result;
    }

    private static double ReadDouble(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out JsonElement value)
            || !value.TryGetDouble(out double result))
        {
            throw new JsonException($"Свойство {propertyName} должно быть числом.");
        }

        return result;
    }
}
