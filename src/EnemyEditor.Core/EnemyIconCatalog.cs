namespace EnemyEditor.Core;

/// <summary>
/// Загружает сведения о PNG-иконках из выбранной директории.
/// </summary>
public sealed class EnemyIconCatalog
{
    private readonly List<EnemyIcon> _icons = [];

    public IReadOnlyList<EnemyIcon> Icons => _icons.AsReadOnly();

    public void LoadIconsFromFolder(string path, bool includeSubdirectories = true)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Путь к папке не должен быть пустым.", nameof(path));
        }

        string fullPath = Path.GetFullPath(path);
        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException($"Папка «{fullPath}» не найдена.");
        }

        SearchOption searchOption = includeSubdirectories
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

        List<EnemyIcon> loadedIcons = Directory
            .EnumerateFiles(fullPath, "*", searchOption)
            .Where(file => string.Equals(Path.GetExtension(file), ".png", StringComparison.OrdinalIgnoreCase))
            .OrderBy(file => Path.GetFileName(file), StringComparer.OrdinalIgnoreCase)
            .Select(file => new EnemyIcon(Path.GetFileName(file), file))
            .ToList();

        _icons.Clear();
        _icons.AddRange(loadedIcons);
    }

    public EnemyIcon? FindByName(string iconName)
    {
        return _icons.FirstOrDefault(icon =>
            string.Equals(icon.Name, iconName, StringComparison.OrdinalIgnoreCase));
    }
}

