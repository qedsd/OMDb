using Microsoft.Maui.Controls;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 可展开的设置控件
/// 用于设置页面，支持展开/折叠显示详细内容
/// </summary>
public class ExpandableSettingControl : Border
{
    /// <summary>
    /// 标题绑定属性
    /// </summary>
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(ExpandableSettingControl),
            string.Empty);

    /// <summary>
    /// 描述绑定属性
    /// </summary>
    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(
            nameof(Description),
            typeof(string),
            typeof(ExpandableSettingControl),
            string.Empty);

    /// <summary>
    /// 可展开内容绑定属性
    /// </summary>
    public static readonly BindableProperty ExpandableContentProperty =
        BindableProperty.Create(
            nameof(ExpandableContent),
            typeof(View),
            typeof(ExpandableSettingControl),
            null);

    /// <summary>
    /// 是否展开绑定属性
    /// </summary>
    public static readonly BindableProperty IsExpandedProperty =
        BindableProperty.Create(
            nameof(IsExpanded),
            typeof(bool),
            typeof(ExpandableSettingControl),
            false,
            propertyChanged: OnIsExpandedChanged);

    private readonly Label _titleLabel;
    private readonly Label _descriptionLabel;
    private readonly Border _contentBorder;
    private readonly ContentView _contentView;
    private readonly Label _expandIcon;

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
    /// 可展开的内容
    /// </summary>
    public View ExpandableContent
    {
        get => (View)GetValue(ExpandableContentProperty);
        set => SetValue(ExpandableContentProperty, value);
    }

    /// <summary>
    /// 是否已展开
    /// </summary>
    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>
    /// 创建可展开的设置控件
    /// </summary>
    public ExpandableSettingControl()
    {
        BackgroundColor = Colors.Transparent;
        Stroke = Colors.Transparent;
        Padding = 10;
        Margin = new Thickness(0, 4);

        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            },
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            }
        };

        // 标题
        _titleLabel = new Label
        {
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(10, 0, 10, 0)
        };
        _titleLabel.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));

        // 描述
        _descriptionLabel = new Label
        {
            FontSize = 12,
            TextColor = Colors.Gray,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start
        };
        _descriptionLabel.SetBinding(Label.TextProperty, new Binding(nameof(Description), source: this));

        // 展开/折叠图标
        _expandIcon = new Label
        {
            Text = "\uE70D", // Down chevron
            FontFamily = "Segoe MDL2 Assets",
            FontSize = 12,
            VerticalOptions = LayoutOptions.Center,
            Rotation = 0
        };

        // 内容区域
        _contentView = new ContentView();
        _contentBorder = new Border
        {
            Content = _contentView,
            IsVisible = false,
            Margin = new Thickness(0, 10, 0, 0)
        };
        _contentBorder.SetBinding(Border.ContentProperty, new Binding(nameof(ExpandableContent), source: this));

        grid.Children.Add(_titleLabel);
        Grid.SetColumn(_titleLabel, 0);

        grid.Children.Add(_descriptionLabel);
        Grid.SetColumn(_descriptionLabel, 1);

        grid.Children.Add(_expandIcon);
        Grid.SetColumn(_expandIcon, 2);

        var contentGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = new GridLength(32, GridUnitType.Absolute) },
                new ColumnDefinition { Width = GridLength.Star }
            }
        };

        Grid.SetColumnSpan(contentGrid, 3);
        Grid.SetRow(contentGrid, 1);

        contentGrid.Children.Add(_contentBorder);
        Grid.SetColumn(_contentBorder, 1);
        Grid.SetColumnSpan(_contentBorder, 2);

        grid.Children.Add(contentGrid);

        Content = grid;

        // 点击展开/折叠
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnTap;
        GestureRecognizers.Add(tapGesture);
    }

    private void OnIsExpandedChanged()
    {
        _contentBorder.IsVisible = IsExpanded;
        _expandIcon.Rotation = IsExpanded ? 180 : 0;
    }

    private static void OnIsExpandedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ExpandableSettingControl control)
        {
            control.OnIsExpandedChanged();
        }
    }

    private void OnTap(object sender, TappedEventArgs e)
    {
        IsExpanded = !IsExpanded;
    }
}
