using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 双滑块控件
/// 用于选择范围值（如日期范围、数值范围）
/// </summary>
public class DoubleThumbSlider : Grid
{
    /// <summary>
    /// 最小值绑定属性
    /// </summary>
    public static readonly BindableProperty MinimumProperty =
        BindableProperty.Create(nameof(Minimum), typeof(int), typeof(DoubleThumbSlider), 0);

    /// <summary>
    /// 最大值绑定属性
    /// </summary>
    public static readonly BindableProperty MaximumProperty =
        BindableProperty.Create(nameof(Maximum), typeof(int), typeof(DoubleThumbSlider), 275);

    /// <summary>
    /// 当前最小值绑定属性
    /// </summary>
    public static readonly BindableProperty MinValueProperty =
        BindableProperty.Create(nameof(MinValue), typeof(int), typeof(DoubleThumbSlider), 0, propertyChanged: OnMinValueChanged);

    /// <summary>
    /// 当前最大值绑定属性
    /// </summary>
    public static readonly BindableProperty MaxValueProperty =
        BindableProperty.Create(nameof(MaxValue), typeof(int), typeof(DoubleThumbSlider), 0, propertyChanged: OnMaxValueChanged);

    /// <summary>
    /// 选中变更命令绑定属性
    /// </summary>
    public static readonly BindableProperty CheckChangedCommandProperty =
        BindableProperty.Create(nameof(CheckChangedCommand), typeof(ICommand), typeof(DoubleThumbSlider), null);

    private readonly BoxView _trackBox;
    private readonly BoxView _highlightBox;
    private readonly Border _minThumb;
    private readonly Border _maxThumb;
    private readonly Label _minLabel;
    private readonly Label _maxLabel;
    private readonly PanGestureRecognizer _minPanGesture;
    private readonly PanGestureRecognizer _maxPanGesture;

    /// <summary>
    /// 最小值
    /// </summary>
    public int Minimum
    {
        get => (int)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// 最大值
    /// </summary>
    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// 当前最小值
    /// </summary>
    public int MinValue
    {
        get => (int)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    /// <summary>
    /// 当前最大值
    /// </summary>
    public int MaxValue
    {
        get => (int)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
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
    /// 创建双滑块控件
    /// </summary>
    public DoubleThumbSlider()
    {
        MinimumHeightRequest = 16;
        HorizontalOptions = LayoutOptions.Fill;
        VerticalOptions = LayoutOptions.Center;

        ColumnDefinitions = new ColumnDefinitionCollection
        {
            new ColumnDefinition { Width = new GridLength(60, GridUnitType.Absolute) },
            new ColumnDefinition { Width = GridLength.Star },
            new ColumnDefinition { Width = new GridLength(60, GridUnitType.Absolute) }
        };

        // 最小值标签
        _minLabel = new Label
        {
            FontSize = 12,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0)
        };
        _minLabel.SetBinding(Label.TextProperty, new Binding(nameof(MinValue), source: this));

        // 滑块容器
        var sliderGrid = new Grid
        {
            MinimumHeightRequest = 16,
            HorizontalOptions = LayoutOptions.Fill
        };

        // 轨道背景
        _trackBox = new BoxView
        {
            HeightRequest = 3,
            BackgroundColor = Colors.SkyBlue,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center
        };

        // 高亮区域（两滑块之间）
        _highlightBox = new BoxView
        {
            HeightRequest = 3,
            BackgroundColor = Colors.Orange,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 0
        };

        // 最小值滑块
        _minThumb = new Border
        {
            WidthRequest = 16,
            HeightRequest = 16,
            BackgroundColor = Colors.Orange,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            TranslationX = 0
        };

        // 最大值滑块
        _maxThumb = new Border
        {
            WidthRequest = 16,
            HeightRequest = 16,
            BackgroundColor = Colors.Orange,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            TranslationX = 0
        };

        // 滑动手势
        _minPanGesture = new PanGestureRecognizer();
        _minPanGesture.PanUpdated += OnMinPanUpdated;
        _minThumb.GestureRecognizers.Add(_minPanGesture);

        _maxPanGesture = new PanGestureRecognizer();
        _maxPanGesture.PanUpdated += OnMaxPanUpdated;
        _maxThumb.GestureRecognizers.Add(_maxPanGesture);

        sliderGrid.Children.Add(_trackBox);
        sliderGrid.Children.Add(_highlightBox);
        sliderGrid.Children.Add(_minThumb);
        sliderGrid.Children.Add(_maxThumb);

        // 最大值标签
        _maxLabel = new Label
        {
            FontSize = 12,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10, 0, 0, 0)
        };
        _maxLabel.SetBinding(Label.TextProperty, new Binding(nameof(MaxValue), source: this));

        Children.Add(_minLabel);
        Grid.SetColumn(_minLabel, 0);

        Children.Add(sliderGrid);
        Grid.SetColumn(sliderGrid, 1);

        Children.Add(_maxLabel);
        Grid.SetColumn(_maxLabel, 2);

        UpdateLayout();
    }

    private static void OnMinValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is DoubleThumbSlider slider)
        {
            slider.UpdateLayout();
        }
    }

    private static void OnMaxValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is DoubleThumbSlider slider)
        {
            slider.UpdateLayout();
        }
    }

    private void UpdateLayout()
    {
        var range = Maximum - Minimum;
        if (range <= 0)
            return;

        var minPercent = (double)(MinValue - Minimum) / range;
        var maxPercent = (double)(MaxValue - Minimum) / range;

        // 使用固定的可用宽度，因为 MAUI 没有 ActualWidth
        var trackWidth = 200; // 默认估计值
        var thumbOffset = 8; // Padding
        var availableWidth = trackWidth - thumbOffset * 2;

        // 更新滑块位置
        _minThumb.TranslationX = thumbOffset + minPercent * availableWidth;
        _maxThumb.TranslationX = thumbOffset + maxPercent * availableWidth;

        // 更新高亮区域
        _highlightBox.TranslationX = _minThumb.TranslationX + 8;
        _highlightBox.WidthRequest = Math.Max(0, _maxThumb.TranslationX - _minThumb.TranslationX - 16);

        // 当大小改变时重新计算
        SizeChanged += (s, e) =>
        {
            var newTrackWidth = Width > 0 ? Width - 120 : 200; // 减去两侧标签宽度
            var newAvailableWidth = newTrackWidth - thumbOffset * 2;
            _minThumb.TranslationX = thumbOffset + minPercent * newAvailableWidth;
            _maxThumb.TranslationX = thumbOffset + maxPercent * newAvailableWidth;
            _highlightBox.TranslationX = _minThumb.TranslationX + 8;
            _highlightBox.WidthRequest = Math.Max(0, _maxThumb.TranslationX - _minThumb.TranslationX - 16);
        };
    }

    private void OnMinPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Completed:
                CheckChangedCommand?.Execute(null);
                break;
        }
    }

    private void OnMaxPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Completed:
                CheckChangedCommand?.Execute(null);
                break;
        }
    }
}
