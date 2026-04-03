using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 片单卡片控件
/// 用于在片单页面展示单个片单的卡片视图
/// </summary>
public class EntryCollectionCard : Border
{
    /// <summary>
    /// 片单数据绑定属性
    /// </summary>
    public static readonly BindableProperty EntryCollectionProperty =
        BindableProperty.Create(
            nameof(EntryCollection),
            typeof(object),
            typeof(EntryCollectionCard),
            null,
            propertyChanged: OnEntryCollectionChanged);

    /// <summary>
    /// 片单数据
    /// </summary>
    public object EntryCollection
    {
        get => GetValue(EntryCollectionProperty);
        set => SetValue(EntryCollectionProperty, value);
    }

    private readonly Image _coverImage;
    private readonly Label _titleLabel;
    private readonly Label _descLabel;
    private readonly Label _countLabel;

    /// <summary>
    /// 创建片单卡片控件
    /// </summary>
    public EntryCollectionCard()
    {
        // MAUI 10 uses StrokeShape with RoundRectangle or direct CornerRadius
        BackgroundColor = Colors.Transparent;
        Stroke = Colors.Transparent;
        Padding = 0;
        HeightRequest = 200;

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
            HeightRequest = 120
        };

        // 垂直布局
        var contentLayout = new VerticalStackLayout
        {
            Padding = 10,
            Spacing = 5
        };

        // 标题
        _titleLabel = new Label
        {
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            MaxLines = 1,
            LineBreakMode = LineBreakMode.TailTruncation
        };

        // 描述
        _descLabel = new Label
        {
            FontSize = 12,
            TextColor = Colors.Gray,
            MaxLines = 2,
            LineBreakMode = LineBreakMode.TailTruncation
        };

        // 数量
        _countLabel = new Label
        {
            FontSize = 12,
            TextColor = Colors.Blue,
            HorizontalOptions = LayoutOptions.End
        };

        contentLayout.Children.Add(_titleLabel);
        contentLayout.Children.Add(_descLabel);
        contentLayout.Children.Add(_countLabel);

        grid.Children.Add(_coverImage);
        Grid.SetRow(_coverImage, 0);

        grid.Children.Add(contentLayout);
        Grid.SetRow(contentLayout, 1);

        Content = grid;
    }

    private static void OnEntryCollectionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EntryCollectionCard card && newValue != null)
        {
            card.UpdateContent();
        }
    }

    private void UpdateContent()
    {
        if (EntryCollection == null)
            return;

        var type = EntryCollection.GetType();

        // 获取片单属性
        var titleProp = type.GetProperty("Title");
        var descProp = type.GetProperty("Description");
        var coverProp = type.GetProperty("CoverImg");
        var countProp = type.GetProperty("EntryCount");

        if (titleProp != null)
            _titleLabel.Text = titleProp.GetValue(EntryCollection)?.ToString();

        if (descProp != null)
            _descLabel.Text = descProp.GetValue(EntryCollection)?.ToString();

        if (coverProp != null)
        {
            var coverImg = coverProp.GetValue(EntryCollection)?.ToString();
            if (!string.IsNullOrEmpty(coverImg))
                _coverImage.Source = ImageSource.FromFile(coverImg);
        }

        if (countProp != null)
            _countLabel.Text = $"{countProp.GetValue(EntryCollection)} 个条目";
    }
}
