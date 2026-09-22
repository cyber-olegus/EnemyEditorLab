namespace EnemyEditor.Core;

/// <summary>
/// Описывает один PNG-файл, доступный для выбора в редакторе.
/// </summary>
public sealed class EnemyIcon
{
    public string Name { get; }

    public string ImagePath { get; }

    public EnemyIcon(string name, string imagePath)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Имя файла иконки не должно быть пустым.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(imagePath))
        {
            throw new ArgumentException("Путь к иконке не должен быть пустым.", nameof(imagePath));
        }

        Name = name;
        ImagePath = Path.GetFullPath(imagePath);
    }
}

