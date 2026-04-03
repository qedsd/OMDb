using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Maui.Controls;
using OMDb.Maui.Models;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 编辑条目名称控件
/// 支持多个名称条目，可添加和删除
/// </summary>
public class EditEntryName : Border
{
    public static readonly BindableProperty ItemSourceProperty = BindableProperty.Create(
        nameof(ItemSource), typeof(ObservableCollection<EntryName>), typeof(EditEntryName), null,
        BindingMode.TwoWay, propertyChanged: OnItemSourceChanged);

    private static void OnItemSourceChanged(BindableObject d, object oldValue, object newValue)
    {
        if (d is EditEntryName control && newValue is ObservableCollection<EntryName> items)
        {
            control.UpdateItems(items);
        }
    }

    public ObservableCollection<EntryName> ItemSource
    {
        get => (ObservableCollection<EntryName>)GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    private readonly VerticalStackLayout _itemsLayout;
    private readonly Button _addButton;

    public EditEntryName()
    {
        Stroke = new SolidColorBrush(Color.FromArgb("#40FFFFFF"));
        StrokeThickness = 0;
        BackgroundColor = Colors.Transparent;
        Padding = 0;

        _itemsLayout = new VerticalStackLayout
        {
            Spacing = 8
        };

        _addButton = new Button
        {
            Text = "+ 添加名称",
            HorizontalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0, 8, 0, 0),
            BackgroundColor = Colors.Transparent,
            BorderColor = Colors.Gray,
            BorderWidth = 1,
            CornerRadius = 4
        };
        _addButton.Clicked += AddButton_Clicked;

        Content = new VerticalStackLayout
        {
            Children = { _itemsLayout, _addButton }
        };
    }

    private void UpdateItems(ObservableCollection<EntryName> items)
    {
        _itemsLayout.Children.Clear();

        if (items == null)
            return;

        foreach (var item in items)
        {
            _itemsLayout.Children.Add(CreateItemRow(item));
        }
    }

    private Grid CreateItemRow(EntryName entryName)
    {
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var nameEntry = new Entry
        {
            Placeholder = entryName.IsDefault ? "名称" : "别称",
            Text = entryName.Name
        };
        nameEntry.TextChanged += (s, e) => entryName.Name = nameEntry.Text;
        Grid.SetColumn(nameEntry, 0);
        grid.Children.Add(nameEntry);

        var markEntry = new Entry
        {
            Placeholder = "备注",
            Text = entryName.Mark
        };
        markEntry.TextChanged += (s, e) => entryName.Mark = markEntry.Text;
        Grid.SetColumn(markEntry, 1);
        grid.Children.Add(markEntry);

        var deleteButton = new Button
        {
            Text = "\uE74D",
            FontFamily = "Segoe MDL2 Assets",
            Margin = new Thickness(10, 0, 0, 0),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = Colors.Transparent,
            BorderColor = Colors.Transparent,
            BorderWidth = 0
        };
        ToolTipProperties.SetText(deleteButton, "删除");

        // IsDefault 为 true 时隐藏删除按钮
        deleteButton.IsVisible = !entryName.IsDefault;
        deleteButton.Clicked += (s, e) =>
        {
            if (ItemSource != null && ItemSource.Contains(entryName))
            {
                ItemSource.Remove(entryName);
            }
        };

        Grid.SetColumn(deleteButton, 2);
        grid.Children.Add(deleteButton);

        return grid;
    }

    private void AddButton_Clicked(object sender, EventArgs e)
    {
        if (ItemSource == null)
        {
            ItemSource = new ObservableCollection<EntryName>();
        }

        ItemSource.Add(new EntryName
        {
            Name = string.Empty,
            Mark = string.Empty,
            IsDefault = false
        });
    }
}
