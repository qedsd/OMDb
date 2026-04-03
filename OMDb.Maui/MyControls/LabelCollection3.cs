using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 标签集合控件 3
/// 用于展示标签集合的封面，带悬停动画效果
/// </summary>
public class LabelCollection3 : Border
{
    /// <summary>
    /// 标题绑定属性
    /// </summary>
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(LabelCollection3),
            null,
            propertyChanged: OnTitleChanged);

    /// <summary>
    /// 描述绑定属性
    /// </summary>
    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(
            nameof(Description),
            typeof(string),
            typeof(LabelCollection3),
            null,
            propertyChanged: OnDescriptionChanged);

    /// <summary>
    /// 详情命令绑定属性
    /// </summary>
    public static readonly BindableProperty DetailCommandProperty =
        BindableProperty.Create(
            nameof(DetailCommand),
            typeof(ICommand),
            typeof(LabelCollection3),
            null);

    /// <summary>
    /// 背景图片绑定属性
    /// </summary>
    public static readonly BindableProperty BgImageSourceProperty =
        BindableProperty.Create(
            nameof(BgImageSource),
            typeof(string),
            typeof(LabelCollection3),
            null,
            propertyChanged: OnBgImageSourceChanged);

    /// <summary>
    /// ID 绑定属性
    /// </summary>
    public static readonly BindableProperty IdProperty =
        BindableProperty.Create(
            nameof(Id),
            typeof(string),
            typeof(LabelCollection3),
            null);

    private readonly Label _titleLabel;
    private readonly Label _descLabel;
    private readonly Image _bgImage;
    private readonly BoxView _bgOverlay;

    /// <summary>
    /// 标题
    /// </summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// 详情命令
    /// </summary>
    public ICommand DetailCommand
    {
        get => (ICommand)GetValue(DetailCommandProperty);
        set => SetValue(DetailCommandProperty, value);
    }

    /// <summary>
    /// 背景图片路径
    /// </summary>
    public string BgImageSource
    {
        get => (string)GetValue(BgImageSourceProperty);
        set => SetValue(BgImageSourceProperty, value);
    }

    /// <summary>
    /// ID
    /// </summary>
    public string Id
    {
        get => (string)GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }

    /// <summary>
    /// 创建标签集合控件 3
    /// </summary>
    public LabelCollection3()
    {
        HorizontalOptions = LayoutOptions.Fill;
        VerticalOptions = LayoutOptions.Fill;
        BackgroundColor = Colors.Transparent;
        Stroke = Colors.Transparent;
        Padding = 0;

        var grid = new Grid();

        // 背景图片
        _bgImage = new Image
        {
            Aspect = Aspect.AspectFill,
            Opacity = 1
        };

        // 背景遮罩
        _bgOverlay = new BoxView
        {
            Color = Colors.Black,
            Opacity = 0.2
        };

        // 底部内容
        var bottomPanel = new VerticalStackLayout
        {
            Margin = new Thickness(20),
            VerticalOptions = LayoutOptions.End
        };

        // 标题
        _titleLabel = new Label
        {
            FontSize = 24,
            TextColor = Colors.White
        };

        // 描述（默认隐藏）
        _descLabel = new Label
        {
            FontSize = 16,
            FontAttributes = FontAttributes.None,
            TextColor = Colors.White,
            HeightRequest = 0,
            IsVisible = false
        };

        bottomPanel.Children.Add(_titleLabel);
        bottomPanel.Children.Add(_descLabel);

        grid.Children.Add(_bgImage);
        grid.Children.Add(_bgOverlay);
        grid.Children.Add(bottomPanel);

        Content = grid;

        // 添加手势
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnGridTapped;
        GestureRecognizers.Add(tapGesture);

        var pointerGesture = new PointerGestureRecognizer();
        pointerGesture.PointerEntered += OnPointerEntered;
        pointerGesture.PointerExited += OnPointerExited;
        GestureRecognizers.Add(pointerGesture);
    }

    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LabelCollection3 control)
        {
            control._titleLabel.Text = newValue as string;
        }
    }

    private static void OnDescriptionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LabelCollection3 control)
        {
            control._descLabel.Text = newValue as string;
        }
    }

    private static void OnBgImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LabelCollection3 control && newValue != null)
        {
            control._bgImage.Source = ImageSource.FromFile(newValue as string);
        }
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        // 悬停时显示描述，背景变暗
        _descLabel.HeightRequest = 20;
        _descLabel.IsVisible = true;
        _bgOverlay.Opacity = 0.6;
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        // 离开时隐藏描述，恢复背景
        _descLabel.HeightRequest = 0;
        _descLabel.IsVisible = false;
        _bgOverlay.Opacity = 0.2;
    }

    private void OnGridTapped(object? sender, TappedEventArgs e)
    {
        DetailCommand?.Execute(Id);
    }
}
