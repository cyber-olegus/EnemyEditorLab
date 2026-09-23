using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using EnemyEditor.Core;
using Microsoft.Win32;

namespace EnemyEditor.Wpf;

public partial class MainWindow : Window
{
    private readonly CEnemyTemplateList _enemyTemplates = new();
    private readonly EnemyIconCatalog _iconCatalog = new();

    public MainWindow()
    {
        InitializeComponent();
        SearchEnemyTextBox.TextChanged += SearchEnemyTextBox_TextChanged;
        RefreshEnemyList();
        Loaded += MainWindow_Loaded;
    }

    private void SearchEnemyTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        RefreshEnemyList();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        string sampleIconsFolder = Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "Monsters");
        if (Directory.Exists(sampleIconsFolder))
        {
            LoadIconFolder(sampleIconsFolder);
        }
    }

    private void BrowseIcons_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Выберите папку с PNG-иконками",
            Multiselect = false
        };

        if (Directory.Exists(IconsFolderTextBox.Text))
        {
            dialog.InitialDirectory = IconsFolderTextBox.Text;
        }

        if (dialog.ShowDialog(this) == true)
        {
            LoadIconFolder(dialog.FolderName);
        }
    }

    private void IconsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (IconsListBox.SelectedItem is not EnemyIcon icon)
        {
            return;
        }

        SelectIcon(icon);
        StatusText.Text = $"Выбрана иконка «{icon.Name}».";
    }

    private void SaveEnemyList_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Title = "Сохранение списка противников",
            FileName = "enemy-templates.json",
            DefaultExt = ".json",
            AddExtension = true,
            Filter = "JSON-файлы (*.json)|*.json|Все файлы (*.*)|*.*",
            OverwritePrompt = true
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        try
        {
            _enemyTemplates.SaveToJson(dialog.FileName);
            StatusText.Text = $"Сохранено противников: {_enemyTemplates.Count}.";
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException)
        {
            ShowFileError(exception.Message);
        }
    }

    private void LoadEnemyList_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Загрузка списка противников",
            DefaultExt = ".json",
            Filter = "JSON-файлы (*.json)|*.json|Все файлы (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        try
        {
            _enemyTemplates.LoadFromJson(dialog.FileName);
            SearchEnemyTextBox.Clear();
            RefreshEnemyList();
            ClearForm();

            if (_enemyTemplates.Count > 0)
            {
                EnemiesListBox.SelectedIndex = 0;
            }

            StatusText.Text = $"Загружено противников: {_enemyTemplates.Count}.";
        }
        catch (Exception exception) when (
            exception is IOException
            or UnauthorizedAccessException
            or JsonException
            or ArgumentException)
        {
            ShowFileError(exception.Message);
        }
    }

    private void AddEnemy_Click(object sender, RoutedEventArgs e)
    {
        if (!TryReadForm(out EnemyFormData data))
        {
            return;
        }

        try
        {
            _enemyTemplates.AddEnemy(
                data.Name,
                data.IconName,
                data.BaseLife,
                data.LifeModifier,
                data.BaseGold,
                data.GoldModifier,
                data.SpawnChance);
            SearchEnemyTextBox.Clear();
            RefreshEnemyList(data.Name);
            StatusText.Text = $"Противник «{data.Name}» добавлен.";
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ShowValidationError(exception.Message);
        }
    }

    private void UpdateEnemy_Click(object sender, RoutedEventArgs e)
    {
        if (EnemiesListBox.SelectedItem is not CEnemyTemplate selectedEnemy)
        {
            ShowValidationError("Сначала выберите противника в списке.");
            return;
        }

        int selectedIndex = _enemyTemplates.GetEnemyIndexByName(selectedEnemy.Name);
        if (selectedIndex < 0)
        {
            ShowValidationError("Выбранный противник больше не существует.");
            return;
        }

        if (!TryReadForm(out EnemyFormData data))
        {
            return;
        }

        try
        {
            _enemyTemplates.ReplaceEnemyByIndex(
                selectedIndex,
                data.Name,
                data.IconName,
                data.BaseLife,
                data.LifeModifier,
                data.BaseGold,
                data.GoldModifier,
                data.SpawnChance);
            SearchEnemyTextBox.Clear();
            RefreshEnemyList(data.Name);
            StatusText.Text = $"Противник «{data.Name}» обновлён.";
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ShowValidationError(exception.Message);
        }
    }

    private void DeleteEnemy_Click(object sender, RoutedEventArgs e)
    {
        if (EnemiesListBox.SelectedItem is not CEnemyTemplate enemy)
        {
            ShowValidationError("Сначала выберите противника для удаления.");
            return;
        }

        MessageBoxResult result = MessageBox.Show(
            this,
            $"Удалить противника «{enemy.Name}»?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        _enemyTemplates.DeleteEnemyByName(enemy.Name);
        RefreshEnemyList();
        ClearForm();
        StatusText.Text = $"Противник «{enemy.Name}» удалён.";
    }

    private void NewEnemy_Click(object sender, RoutedEventArgs e)
    {
        EnemiesListBox.SelectedIndex = -1;
        ClearForm();
        EnemyNameTextBox.Focus();
        StatusText.Text = "Заполните параметры нового противника.";
    }

    private void EnemiesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (EnemiesListBox.SelectedItem is not CEnemyTemplate enemy)
        {
            return;
        }

        EnemyNameTextBox.Text = enemy.Name;
        IconNameTextBox.Text = enemy.IconName;
        BaseLifeTextBox.Text = enemy.BaseLife.ToString(CultureInfo.CurrentCulture);
        LifeModifierTextBox.Text = enemy.LifeModifier.ToString(CultureInfo.CurrentCulture);
        BaseGoldTextBox.Text = enemy.BaseGold.ToString(CultureInfo.CurrentCulture);
        GoldModifierTextBox.Text = enemy.GoldModifier.ToString(CultureInfo.CurrentCulture);
        SpawnChanceTextBox.Text = enemy.SpawnChance.ToString(CultureInfo.CurrentCulture);
        SelectIconByName(enemy.IconName);
        StatusText.Text = $"Выбран противник «{enemy.Name}».";
    }

    private bool TryReadForm(out EnemyFormData data)
    {
        data = default;

        if (!TryReadInt(BaseLifeTextBox, "Базовое здоровье", out int baseLife)
            || !TryReadDouble(LifeModifierTextBox, "Модификатор здоровья", out double lifeModifier)
            || !TryReadInt(BaseGoldTextBox, "Базовое золото", out int baseGold)
            || !TryReadDouble(GoldModifierTextBox, "Модификатор золота", out double goldModifier)
            || !TryReadDouble(SpawnChanceTextBox, "Шанс появления", out double spawnChance))
        {
            return false;
        }

        data = new EnemyFormData(
            EnemyNameTextBox.Text,
            IconNameTextBox.Text,
            baseLife,
            lifeModifier,
            baseGold,
            goldModifier,
            spawnChance);
        return true;
    }

    private bool TryReadInt(TextBox textBox, string fieldName, out int value)
    {
        if (int.TryParse(textBox.Text, out value))
        {
            return true;
        }

        ShowValidationError($"Поле «{fieldName}» должно содержать целое число.");
        FocusInvalidField(textBox);
        return false;
    }

    private bool TryReadDouble(TextBox textBox, string fieldName, out double value)
    {
        string input = textBox.Text.Trim();
        bool parsed = double.TryParse(input, NumberStyles.Float, CultureInfo.CurrentCulture, out value)
            || double.TryParse(input.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out value);

        if (parsed)
        {
            return true;
        }

        ShowValidationError($"Поле «{fieldName}» должно содержать число.");
        FocusInvalidField(textBox);
        return false;
    }

    private void RefreshEnemyList(string? selectedName = null)
    {
        string searchText = SearchEnemyTextBox.Text.Trim();
        List<CEnemyTemplate> visibleEnemies = _enemyTemplates.Enemies
            .Where(enemy => searchText.Length == 0
                || enemy.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .ToList();

        EnemiesListBox.ItemsSource = null;
        EnemiesListBox.ItemsSource = visibleEnemies;
        EnemyCountText.Text = searchText.Length == 0
            ? $"Противников: {_enemyTemplates.Count}"
            : $"Показано: {visibleEnemies.Count} из {_enemyTemplates.Count}";
        UpdateEnemyStatistics();

        if (selectedName is not null)
        {
            EnemiesListBox.SelectedItem = visibleEnemies.FirstOrDefault(enemy =>
                string.Equals(enemy.Name, selectedName, StringComparison.OrdinalIgnoreCase));
        }
    }

    private void UpdateEnemyStatistics()
    {
        if (_enemyTemplates.Count == 0)
        {
            EnemyStatisticsText.Text = "Среднее здоровье: —";
            return;
        }

        double averageLife = _enemyTemplates.Enemies.Average(enemy => enemy.BaseLife);
        double averageGold = _enemyTemplates.Enemies.Average(enemy => enemy.BaseGold);
        EnemyStatisticsText.Text = $"Среднее: здоровье {averageLife:0.#}, золото {averageGold:0.#}";
    }

    private void LoadIconFolder(string path)
    {
        try
        {
            _iconCatalog.LoadIconsFromFolder(path);
            IconsFolderTextBox.Text = Path.GetFullPath(path);
            IconsListBox.ItemsSource = null;
            IconsListBox.ItemsSource = _iconCatalog.Icons;

            if (_iconCatalog.Icons.Count > 0)
            {
                IconsListBox.SelectedIndex = 0;
                StatusText.Text = $"Загружено иконок: {_iconCatalog.Icons.Count}.";
            }
            else
            {
                ClearSelectedIcon();
                StatusText.Text = "В выбранной папке PNG-файлы не найдены.";
            }
        }
        catch (Exception exception) when (exception is ArgumentException or IOException or UnauthorizedAccessException)
        {
            ShowValidationError(exception.Message);
        }
    }

    private void SelectIconByName(string iconName)
    {
        EnemyIcon? icon = _iconCatalog.FindByName(iconName);
        if (icon is null)
        {
            IconsListBox.SelectedIndex = -1;
            MainEnemyIcon.Source = null;
            SelectedIconText.Text = $"Файл {iconName} не найден в текущей папке";
            return;
        }

        IconsListBox.SelectedItem = icon;
        SelectIcon(icon);
    }

    private void SelectIcon(EnemyIcon icon)
    {
        IconNameTextBox.Text = icon.Name;
        MainEnemyIcon.Source = LoadBitmap(icon.ImagePath);
        SelectedIconText.Text = icon.Name;
    }

    private static BitmapImage LoadBitmap(string path)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = new System.Uri(Path.GetFullPath(path));
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }

    private void ClearForm()
    {
        EnemyNameTextBox.Clear();
        IconNameTextBox.Clear();
        BaseLifeTextBox.Text = "100";
        LifeModifierTextBox.Text = "1,15";
        BaseGoldTextBox.Text = "10";
        GoldModifierTextBox.Text = "1,05";
        SpawnChanceTextBox.Text = "0,5";
        ClearSelectedIcon();
    }

    private void ClearSelectedIcon()
    {
        IconsListBox.SelectedIndex = -1;
        MainEnemyIcon.Source = null;
        SelectedIconText.Text = "Не выбрана";
        IconNameTextBox.Clear();
    }

    private void ShowValidationError(string message)
    {
        StatusText.Text = message;
        MessageBox.Show(this, message, "Проверьте данные", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void ShowFileError(string message)
    {
        StatusText.Text = message;
        MessageBox.Show(this, message, "Ошибка работы с файлом", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private static void FocusInvalidField(TextBox textBox)
    {
        textBox.Focus();
        textBox.SelectAll();
    }

    private readonly record struct EnemyFormData(
        string Name,
        string IconName,
        int BaseLife,
        double LifeModifier,
        int BaseGold,
        double GoldModifier,
        double SpawnChance);
}
