using System.Collections;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 词条仓库选择控件
/// 用于显示多个仓库选项，支持多选
/// </summary>
public class EntryStoragesControl : VerticalStackLayout
{
    /// <summary>
    /// 仓库列表绑定属性
    /// </summary>
    public static readonly BindableProperty EntryStoragesProperty =
        BindableProperty.Create(
            nameof(EntryStorages),
            typeof(System.Collections.IEnumerable),
            typeof(EntryStoragesControl),
            null,
            propertyChanged: OnEntryStoragesChanged);

    /// <summary>
    /// 选中变更命令绑定属性
    /// </summary>
    public static readonly BindableProperty CheckChangedCommandProperty =
        BindableProperty.Create(
            nameof(CheckChangedCommand),
            typeof(ICommand),
            typeof(EntryStoragesControl),
            null);

    private readonly WrapLayout _checkBoxesLayout;

    /// <summary>
    /// 仓库列表
    /// </summary>
    public System.Collections.IEnumerable EntryStorages
    {
        get => (System.Collections.IEnumerable)GetValue(EntryStoragesProperty);
        set => SetValue(EntryStoragesProperty, value);
    }

    /// <summary>
    /// 选中变更命令
    /// </summary>
    public ICommand CheckChangedCommand
    {
        get => (ICommand)GetValue(CheckChangedCommandProperty);
        set => SetValue(CheckChangedCommandProperty, value);
    }

    /// <summary>
    /// 创建词条仓库选择控件
    /// </summary>
    public EntryStoragesControl()
    {
        Spacing = 10;

        _checkBoxesLayout = new WrapLayout();

        Children.Add(_checkBoxesLayout);
    }

    private static void OnEntryStoragesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EntryStoragesControl control && newValue != null)
        {
            control.UpdateContent();
        }
    }

    private void UpdateContent()
    {
        _checkBoxesLayout.Children.Clear();

        foreach (var storage in EntryStorages)
        {
            var type = storage.GetType();
            var nameProp = type.GetProperty("StorageName");
            var isCheckedProp = type.GetProperty("IsChecked");

            var name = nameProp?.GetValue(storage)?.ToString() ?? "未知仓库";
            var isChecked = (bool)(isCheckedProp?.GetValue(storage) ?? false);

            var checkBox = new CheckBox
            {
                IsChecked = isChecked,
                VerticalOptions = LayoutOptions.Center
            };
            checkBox.CheckedChanged += (s, e) =>
            {
                isCheckedProp?.SetValue(storage, checkBox.IsChecked);
                if (CheckChangedCommand?.CanExecute(null) == true)
                {
                    CheckChangedCommand.Execute(null);
                }
            };

            var label = new Label
            {
                Text = name,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(-5, 0, 10, 0)
            };

            var horizontalLayout = new HorizontalStackLayout
            {
                Spacing = 5
            };
            horizontalLayout.Children.Add(checkBox);
            horizontalLayout.Children.Add(label);

            _checkBoxesLayout.Children.Add(horizontalLayout);
        }
    }
}

/// <summary>
/// 流式布局容器
/// </summary>
public class WrapLayout : FlexLayout
{
    public WrapLayout()
    {
        Direction = FlexDirection.Row;
        Wrap = FlexWrap.Wrap;
        AlignContent = FlexAlignContent.Start;
        AlignItems = FlexAlignItems.Center;
    }
}
