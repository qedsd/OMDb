using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using OMDb.Maui.Models;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 资源管理器项控件
/// 以树形结构显示文件和文件夹
/// </summary>
public class ExplorerItemControl : Grid
{
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(ObservableCollection<ExplorerItem>), typeof(ExplorerItemControl), null,
        propertyChanged: OnItemsSourceChanged);

    private static void OnItemsSourceChanged(BindableObject d, object oldValue, object newValue)
    {
        if (d is ExplorerItemControl control && newValue is ObservableCollection<ExplorerItem> items)
        {
            control.UpdateItems(items);
        }
    }

    public ObservableCollection<ExplorerItem> ItemsSource
    {
        get => (ObservableCollection<ExplorerItem>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty OpenItemCommandProperty = BindableProperty.Create(
        nameof(OpenItemCommand), typeof(ICommand), typeof(ExplorerItemControl), null);

    public ICommand OpenItemCommand
    {
        get => (ICommand)GetValue(OpenItemCommandProperty);
        set => SetValue(OpenItemCommandProperty, value);
    }

    public static readonly BindableProperty DeleteItemCommandProperty = BindableProperty.Create(
        nameof(DeleteItemCommand), typeof(ICommand), typeof(ExplorerItemControl), null);

    public ICommand DeleteItemCommand
    {
        get => (ICommand)GetValue(DeleteItemCommandProperty);
        set => SetValue(DeleteItemCommandProperty, value);
    }

    public static readonly BindableProperty CancelCopyCommandProperty = BindableProperty.Create(
        nameof(CancelCopyCommand), typeof(ICommand), typeof(ExplorerItemControl), null);

    public ICommand CancelCopyCommand
    {
        get => (ICommand)GetValue(CancelCopyCommandProperty);
        set => SetValue(CancelCopyCommandProperty, value);
    }

    private readonly CollectionView _collectionView;

    public ExplorerItemControl()
    {
        _collectionView = new CollectionView
        {
            SelectionMode = SelectionMode.None,
            ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
        };

        _collectionView.ItemTemplate = new DataTemplate(() =>
        {
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // 主行
            var itemGrid = new Grid();
            itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            itemGrid.HeightRequest = 32;

            var nameLabel = new Label { VerticalOptions = LayoutOptions.Center };
            nameLabel.SetBinding(Label.TextProperty, "Name");
            Grid.SetColumn(nameLabel, 0);
            itemGrid.Children.Add(nameLabel);

            var sizeLabel = new Label
            {
                Margin = new Thickness(0, 0, 16, 0),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center
            };
            sizeLabel.SetBinding(Label.TextProperty, "Length", converter: new ByteSizeConverter());
            Grid.SetColumn(sizeLabel, 1);
            itemGrid.Children.Add(sizeLabel);

            var cancelButton = new Button
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Text = "\uE10A",
                FontFamily = "Segoe MDL2 Assets"
            };
            cancelButton.SetBinding(Button.CommandProperty, "CancelCopyCommand");
            cancelButton.SetBinding(Button.IsVisibleProperty, "IsCopying", converter: new BoolToVisibilityConverter());
            Grid.SetColumn(cancelButton, 2);
            itemGrid.Children.Add(cancelButton);

            // 右键菜单模拟（MAUI 使用 MenuFlyout）
            var menuFlyout = new MenuFlyout();
            var openItem = new MenuFlyoutItem { Text = "打开" };
            openItem.SetBinding(MenuItem.IsEnabledProperty, "IsCopying", converter: new InverseBoolConverter());
            openItem.Clicked += (s, e) =>
            {
                var vm = (s as MenuItem)?.BindingContext as ExplorerItem;
                if (vm != null)
                {
                    OpenItem(vm);
                }
            };
            menuFlyout.Add(openItem);

            var deleteItem = new MenuFlyoutItem { Text = "删除" };
            deleteItem.SetBinding(MenuItem.IsEnabledProperty, "IsCopying", converter: new InverseBoolConverter());
            menuFlyout.Add(deleteItem);

            itemGrid.GestureRecognizers.Add(new TapGestureRecognizer());
            itemGrid.GestureRecognizers.Add(new TapGestureRecognizer { NumberOfTapsRequired = 2 });

            Grid.SetRow(itemGrid, 0);
            grid.Children.Add(itemGrid);

            // 复制进度条
            var copyProgress = new ProgressBar
            {
                Margin = new Thickness(0, 0, 16, 0)
            };
            copyProgress.SetBinding(ProgressBar.ProgressProperty, "CopyPercent");
            copyProgress.SetBinding(ProgressBar.IsVisibleProperty, "IsCopying", converter: new BoolToVisibilityConverter());
            ToolTipProperties.SetText(copyProgress, "{Binding CopyPercent}");
            Grid.SetRow(copyProgress, 1);
            grid.Children.Add(copyProgress);

            // 删除进度条 - 使用 ActivityIndicator 表示不确定进度
            var deleteProgress = new ActivityIndicator
            {
                Margin = new Thickness(0, 0, 16, 0),
                IsRunning = true,
                Color = Colors.Blue
            };
            deleteProgress.SetBinding(ActivityIndicator.IsVisibleProperty, "IsDeleting", converter: new BoolToVisibilityConverter());
            ToolTipProperties.SetText(deleteProgress, "删除中");
            Grid.SetRow(deleteProgress, 2);
            grid.Children.Add(deleteProgress);

            // 校验进度条 - 使用 ActivityIndicator 表示不确定进度
            var verifyProgress = new ActivityIndicator
            {
                Margin = new Thickness(0, 0, 16, 0),
                IsRunning = true,
                Color = Colors.Green
            };
            verifyProgress.SetBinding(ActivityIndicator.IsVisibleProperty, "IsVerifying", converter: new BoolToVisibilityConverter());
            ToolTipProperties.SetText(verifyProgress, "校验中");
            Grid.SetRow(verifyProgress, 3);
            grid.Children.Add(verifyProgress);

            return grid;
        });

        Children.Add(_collectionView);
    }

    private void UpdateItems(ObservableCollection<ExplorerItem> items)
    {
        _collectionView.ItemsSource = items;
    }

    private void OpenItem(ExplorerItem item)
    {
        if (!string.IsNullOrEmpty(item.FullName))
        {
            try
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = item.FullName,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"打开文件失败：{ex.Message}");
            }
        }
    }
}

/// <summary>
/// 字节大小转换器
/// </summary>
public class ByteSizeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {sizes[order]}";
        }
        return value?.ToString() ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 布尔值转可见性转换器
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b;
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 布尔值取反转换器
/// </summary>
public class InverseBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return !b;
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
