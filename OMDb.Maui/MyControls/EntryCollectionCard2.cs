using System.Windows.Input;
using Microsoft.Maui.Controls;
using OMDb.Maui.Models;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 片单卡片控件 2 - 带悬停动画效果
/// </summary>
public class EntryCollectionCard2 : Border
{
    public static readonly BindableProperty EntryCollectionProperty = BindableProperty.Create(
        nameof(EntryCollection), typeof(EntryCollection), typeof(EntryCollectionCard2), null,
        propertyChanged: OnEntryCollectionChanged);

    private static void OnEntryCollectionChanged(BindableObject d, object oldValue, object newValue)
    {
        var card = d as EntryCollectionCard2;
        var collection = newValue as EntryCollection;
        if (collection != null)
        {
            card._titleLabel.Text = collection.Title;
            card._descLabel.Text = collection.Description;
            card._lastUpdateLabel.Text = collection.LastUpdateTime.ToString("yyyy-MM-dd");
            card._watchedCountLabel.Text = collection.WatchedCount.ToString();
            card._totalCountLabel.Text = collection.Items == null ? "0" : collection.Items.Count.ToString();
            card._watchingCountLabel.Text = collection.WatchingCount.ToString();

            if (collection.CoverImage != null)
            {
                card._bgImage.Source = ImageSource.FromFile(collection.CoverImage.ToString());
            }
        }
    }

    public EntryCollection EntryCollection
    {
        get => (EntryCollection)GetValue(EntryCollectionProperty);
        set => SetValue(EntryCollectionProperty, value);
    }

    public static readonly BindableProperty DetailCommandProperty = BindableProperty.Create(
        nameof(DetailCommand), typeof(ICommand), typeof(EntryCollectionCard2), null);

    public ICommand DetailCommand
    {
        get => (ICommand)GetValue(DetailCommandProperty);
        set => SetValue(DetailCommandProperty, value);
    }

    private readonly Image _bgImage;
    private readonly Label _titleLabel;
    private readonly Label _descLabel;
    private readonly Label _lastUpdateLabel;
    private readonly Label _watchedCountLabel;
    private readonly Label _watchingCountLabel;
    private readonly Label _totalCountLabel;
    private readonly VerticalStackLayout _animationArea;
    private readonly BoxView _bgOverlay;

    public EntryCollectionCard2()
    {
        HeightRequest = 200;
        Stroke = new SolidColorBrush(Colors.Transparent);
        StrokeThickness = 0;
        Padding = 0;

        // 背景图片
        _bgImage = new Image
        {
            Aspect = Aspect.AspectFill,
            Opacity = 1.0
        };

        // 背景遮罩
        _bgOverlay = new BoxView
        {
            BackgroundColor = Colors.Black,
            Opacity = 0.2
        };

        // 标题
        _titleLabel = new Label
        {
            FontSize = 20,
            VerticalOptions = LayoutOptions.Center,
            MaxLines = 2,
            LineBreakMode = LineBreakMode.TailTruncation,
            TextColor = Colors.White
        };

        // 描述
        _descLabel = new Label
        {
            FontSize = 14,
            FontAttributes = FontAttributes.None,
            Margin = new Thickness(0, 4, 0, 0),
            MaxLines = 3,
            LineBreakMode = LineBreakMode.TailTruncation,
            TextColor = Colors.White,
            IsVisible = false // 初始隐藏，悬停时显示
        };

        // 动画区域（描述）
        _animationArea = new VerticalStackLayout
        {
            Children = { _titleLabel, _descLabel },
            Margin = new Thickness(0, 20, 0, 0)
        };

        // 统计信息
        _watchedCountLabel = new Label { FontSize = 12, FontAttributes = FontAttributes.None, TextColor = Colors.White };
        _watchingCountLabel = new Label { FontSize = 12, FontAttributes = FontAttributes.None, TextColor = Colors.White };
        _totalCountLabel = new Label { FontSize = 12, FontAttributes = FontAttributes.None, TextColor = Colors.White };

        var watchedLabel = new Label { Text = "看过", FontSize = 12, FontAttributes = FontAttributes.None, TextColor = Colors.White };
        var watchingLabel = new Label { Text = "在看", FontSize = 12, FontAttributes = FontAttributes.None, TextColor = Colors.White };
        var totalLabel = new Label { Text = "总", FontSize = 12, FontAttributes = FontAttributes.None, TextColor = Colors.White };

        var statsLayout = new HorizontalStackLayout
        {
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 10, 0, 4),
            Spacing = 10,
            Children =
            {
                watchedLabel, _watchedCountLabel,
                watchingLabel, _watchingCountLabel,
                totalLabel, _totalCountLabel
            }
        };

        // 更新时间
        _lastUpdateLabel = new Label { FontSize = 12, FontAttributes = FontAttributes.None, TextColor = Colors.White };
        var lastUpdateLabel = new Label { Text = "更新于", FontSize = 12, FontAttributes = FontAttributes.None, TextColor = Colors.White };

        var lastUpdateLayout = new HorizontalStackLayout
        {
            HorizontalOptions = LayoutOptions.End,
            Children = { lastUpdateLabel, _lastUpdateLabel }
        };

        // 主内容布局
        var contentLayout = new Grid
        {
            Margin = new Thickness(20, 0, 20, 10),
            VerticalOptions = LayoutOptions.Fill
        };
        contentLayout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
        contentLayout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var topContent = new Grid
        {
            Margin = new Thickness(0, 20, 0, 0),
            Children = { _animationArea, statsLayout }
        };
        Grid.SetRow(topContent, 0);
        Grid.SetRow(lastUpdateLayout, 1);

        contentLayout.Children.Add(topContent);
        contentLayout.Children.Add(lastUpdateLayout);

        // 点击手势
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) =>
        {
            DetailCommand?.Execute(EntryCollection);
        };
        GestureRecognizers.Add(tapGesture);

        // 悬停效果（MAUI 移动端不支持 PointerEntered/Exited，桌面端可通过 Effects 实现）
        // 这里使用简单的 IsVisible 切换

        Content = new Grid
        {
            Children =
            {
                _bgImage,
                _bgOverlay,
                contentLayout
            }
        };
    }
}
