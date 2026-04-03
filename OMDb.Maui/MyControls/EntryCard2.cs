using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 词条卡片控件 2
/// 用于在片单详情页展示词条卡片，带有添加日期动画效果
/// </summary>
public class EntryCard2 : Border
{
    /// <summary>
    /// 词条项绑定属性
    /// </summary>
    public static readonly BindableProperty EntryCollectionItemProperty =
        BindableProperty.Create(
            nameof(EntryCollectionItem),
            typeof(object),
            typeof(EntryCard2),
            null,
            propertyChanged: OnEntryCollectionItemChanged);

    private readonly Image _coverImage;
    private readonly Label _nameLabel;
    private readonly Label _yearLabel;
    private readonly Label _addDateLabel;
    private readonly VerticalStackLayout _animationArea;

    /// <summary>
    /// 词条项数据
    /// </summary>
    public object EntryCollectionItem
    {
        get => GetValue(EntryCollectionItemProperty);
        set => SetValue(EntryCollectionItemProperty, value);
    }

    /// <summary>
    /// 创建词条卡片控件 2
    /// </summary>
    public EntryCard2()
    {
        BackgroundColor = Colors.Transparent;
        Stroke = Colors.Transparent;
        Padding = 0;
        Margin = new Thickness(8);

        var grid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Auto }
            }
        };

        // 封面图片
        _coverImage = new Image
        {
            Aspect = Aspect.AspectFill,
            BackgroundColor = Colors.LightGray
        };

        // 底部信息区域
        var infoPanel = new VerticalStackLayout
        {
            Padding = new Thickness(0, 4, 0, 0),
            Spacing = 2
        };

        // 名称
        _nameLabel = new Label
        {
            FontSize = 14,
            HorizontalOptions = LayoutOptions.Center,
            MaxLines = 1,
            LineBreakMode = LineBreakMode.TailTruncation
        };

        // 年份
        _yearLabel = new Label
        {
            FontSize = 12,
            FontAttributes = FontAttributes.None,
            HorizontalOptions = LayoutOptions.Center
        };

        // 添加日期动画区域
        _animationArea = new VerticalStackLayout
        {
            HeightRequest = 0,
            Margin = new Thickness(0, 2, 0, 0),
            HorizontalOptions = LayoutOptions.Center,
            IsVisible = false
        };

        var addedLabel = new Label
        {
            Text = "添加于",
            FontSize = 13,
            FontAttributes = FontAttributes.None
        };

        _addDateLabel = new Label
        {
            FontSize = 13,
            FontAttributes = FontAttributes.None,
            Margin = new Thickness(4, 0, 0, 0)
        };

        _animationArea.Children.Add(addedLabel);
        _animationArea.Children.Add(_addDateLabel);

        infoPanel.Children.Add(_nameLabel);
        infoPanel.Children.Add(_yearLabel);
        infoPanel.Children.Add(_animationArea);

        grid.Children.Add(_coverImage);
        Grid.SetRow(_coverImage, 0);

        grid.Children.Add(infoPanel);
        Grid.SetRow(infoPanel, 1);

        Content = grid;

        // 添加手势识别
        var pointerGesture = new TapGestureRecognizer();
        pointerGesture.Tapped += OnPointerEntered;
        GestureRecognizers.Add(pointerGesture);
    }

    private static void OnEntryCollectionItemChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EntryCard2 card && newValue != null)
        {
            card.UpdateContent();
        }
    }

    private void UpdateContent()
    {
        if (EntryCollectionItem == null)
            return;

        var type = EntryCollectionItem.GetType();
        var entryProp = type.GetProperty("Entry");
        var addDateProp = type.GetProperty("AddDate");

        if (entryProp != null)
        {
            var entry = entryProp.GetValue(EntryCollectionItem);
            if (entry != null)
            {
                var entryType = entry.GetType();
                var nameProp = entryType.GetProperty("Name");
                var yearProp = entryType.GetProperty("ReleaseYear");
                var coverProp = entryType.GetProperty("CoverImg");

                if (nameProp != null)
                    _nameLabel.Text = nameProp.GetValue(entry)?.ToString();

                if (yearProp != null)
                    _yearLabel.Text = yearProp.GetValue(entry)?.ToString();

                if (coverProp != null)
                {
                    var coverImg = coverProp.GetValue(entry)?.ToString();
                    if (!string.IsNullOrEmpty(coverImg))
                        _coverImage.Source = ImageSource.FromFile(coverImg);
                }
            }
        }

        if (addDateProp != null)
        {
            var addDate = addDateProp.GetValue(EntryCollectionItem)?.ToString();
            if (!string.IsNullOrEmpty(addDate))
                _addDateLabel.Text = addDate;
        }
    }

    private void OnPointerEntered(object sender, TappedEventArgs e)
    {
        // 显示动画区域
        _animationArea.HeightRequest = 20;
        _animationArea.IsVisible = true;
    }
}
