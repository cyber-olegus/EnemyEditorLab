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
}
