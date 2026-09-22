using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using EnemyEditor.Core;

namespace EnemyEditor.Wpf;

public partial class MainWindow : Window
{
    private readonly CEnemyTemplateList _enemyTemplates = new();

    public MainWindow()
    {
        InitializeComponent();
        RefreshEnemyList();
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
        int selectedIndex = EnemiesListBox.SelectedIndex;
        if (selectedIndex < 0)
        {
            ShowValidationError("Сначала выберите противника в списке.");
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
        int selectedIndex = EnemiesListBox.SelectedIndex;
        if (selectedIndex < 0)
        {
            ShowValidationError("Сначала выберите противника для удаления.");
            return;
        }

        CEnemyTemplate enemy = _enemyTemplates.GetEnemyByIndex(selectedIndex);
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

        _enemyTemplates.DeleteEnemyByIndex(selectedIndex);
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
        int selectedIndex = EnemiesListBox.SelectedIndex;
        if (selectedIndex < 0 || selectedIndex >= _enemyTemplates.Count)
        {
            return;
        }

        CEnemyTemplate enemy = _enemyTemplates.GetEnemyByIndex(selectedIndex);
        EnemyNameTextBox.Text = enemy.Name;
        IconNameTextBox.Text = enemy.IconName;
        BaseLifeTextBox.Text = enemy.BaseLife.ToString(CultureInfo.CurrentCulture);
        LifeModifierTextBox.Text = enemy.LifeModifier.ToString(CultureInfo.CurrentCulture);
        BaseGoldTextBox.Text = enemy.BaseGold.ToString(CultureInfo.CurrentCulture);
        GoldModifierTextBox.Text = enemy.GoldModifier.ToString(CultureInfo.CurrentCulture);
        SpawnChanceTextBox.Text = enemy.SpawnChance.ToString(CultureInfo.CurrentCulture);
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
        EnemiesListBox.ItemsSource = null;
        EnemiesListBox.ItemsSource = _enemyTemplates.Enemies;
        EnemyCountText.Text = $"Противников: {_enemyTemplates.Count}";

        if (selectedName is not null)
        {
            EnemiesListBox.SelectedItem = _enemyTemplates.GetEnemyByName(selectedName);
        }
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
    }

    private void ShowValidationError(string message)
    {
        StatusText.Text = message;
        MessageBox.Show(this, message, "Проверьте данные", MessageBoxButton.OK, MessageBoxImage.Warning);
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

