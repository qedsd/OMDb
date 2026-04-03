using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 标签集合控件 2
/// 用于展示标签集合的封面和词条列表，带背景图片效果
/// </summary>
public class LabelCollection2 : Border
{
    /// <summary>
    /// 词条列表绑定属性
    /// </summary>
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(
            nameof(ItemsSource),
            typeof(System.Collections.IEnumerable),
            typeof(LabelCollection2),
            null,
            propertyChanged: OnItemsSourceChanged);

    /// <summary>
    /// 标题绑定属性
    /// </summary>
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(LabelCollection2),
            null,
            propertyChanged: OnTitleChanged);

    /// <summary>
    /// 描述绑定属性
    /// </summary>
    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(
            nameof(Description),
            typeof(string),
            typeof(LabelCollection2),
            null,
            propertyChanged: OnDescriptionChanged);

    /// <summary>
    /// 详情命令绑定属性
    /// </summary>
    public static readonly BindableProperty DetailCommandProperty =
        BindableProperty.Create(
            nameof(DetailCommand),
            typeof(ICommand),
            typeof(LabelCollection2),
            null);

    /// <summary>
    /// 点击项命令绑定属性
    /// </summary>
    public static readonly BindableProperty ClickItemCommandProperty =
        BindableProperty.Create(
            nameof(ClickItemCommand),
            typeof(ICommand),
            typeof(LabelCollection2),
            null);

    /// <summary>
    /// ID 绑定属性
    /// </summary>
    public static readonly BindableProperty IdProperty =
        BindableProperty.Create(
            nameof(Id),
            typeof(string),
            typeof(LabelCollection2),
            null);

    /// <summary>
    /// 背景图片绑定属性
    /// </summary>
    public static readonly BindableProperty BgImageSourceProperty =
        BindableProperty.Create(
            nameof(BgImageSource),
            typeof(string),
            typeof(LabelCollection2),
            null,
            propertyChanged: OnBgImageSourceChanged);

    private readonly Label _titleLabel;
    private readonly Label _descLabel;
    private readonly Image _bgImage;
    private readonly CollectionView _itemsList;
    private readonly Button _viewAllButton;

    /// <summary>
    /// 词条列表
    /// </summary>
    public System.Collections.IEnumerable ItemsSource
    {
        get => (System.Collections.IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

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
    /// 点击项命令
    /// </summary>
    public ICommand ClickItemCommand
    {
        get => (ICommand)GetValue(ClickItemCommandProperty);
        set => SetValue(ClickItemCommandProperty, value);
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
    /// 背景图片路径
    /// </summary>
    public string BgImageSource
    {
        get => (string)GetValue(BgImageSourceProperty);
        set => SetValue(BgImageSourceProperty, value);
    }

    /// <summary>
    /// 创建标签集合控件 2
    /// </summary>
    public LabelCollection2()
    {
        HeightRequest = 420;
        BackgroundColor = Colors.Black;
        Stroke = Colors.Transparent;

        var mainGrid = new Grid();

        // 背景图片
        _bgImage = new Image
        {
            Aspect = Aspect.AspectFill,
            Opacity = 0.3
        };

        // 内容网格
        var contentGrid = new Grid
        {
            Margin = new Thickness(32, 20, 20, 0)
        };

        var topPanel = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star }
            },
            VerticalOptions = LayoutOptions.Start
        };

        // 标题
        _titleLabel = new Label
        {
            FontSize = 24,
            TextColor = Colors.White,
            VerticalOptions = LayoutOptions.Center
        };

        // 描述
        _descLabel = new Label
        {
            FontSize = 14,
            FontAttributes = FontAttributes.None,
            TextColor = Colors.White,
            Margin = new Thickness(10, 0, 0, 0),
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(_descLabel, 1);

        // 查看全部按钮
        _viewAllButton = new Button
        {
            Text = "查看全部",
            FontSize = 14,
            TextColor = Colors.White,
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 100
        };
        _viewAllButton.Clicked += OnViewAllClicked;
        Grid.SetColumn(_viewAllButton, 2);

        topPanel.Children.Add(_titleLabel);
        topPanel.Children.Add(_descLabel);
        topPanel.Children.Add(_viewAllButton);

        // 词条列表
        _itemsList = new CollectionView
        {
            HeightRequest = 320,
            Margin = new Thickness(20, 20, 20, 0),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            SelectionMode = SelectionMode.None
        };
        _itemsList.ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Horizontal)
        {
            ItemSpacing = 10
        };
        _itemsList.ItemTemplate = new DataTemplate(() =>
        {
            var grid = new Grid
            {
                WidthRequest = 180,
                HeightRequest = 300,
                HorizontalOptions = LayoutOptions.Center
            };

            var coverImage = new Image
            {
                Aspect = Aspect.AspectFill
            };
            coverImage.SetBinding(Image.SourceProperty, new Binding("CoverImg"));

            var overlayPanel = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.End,
                Padding = new Thickness(0, 8),
                BackgroundColor = new Color(0, 0, 0, 0.5f)
            };

            var nameLabel = new Label
            {
                Text = "{Binding Name}",
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Colors.White
            };

            var yearLabel = new Label
            {
                Text = "{Binding ReleaseYear}",
                HorizontalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.None,
                TextColor = Colors.White
            };

            overlayPanel.Children.Add(nameLabel);
            overlayPanel.Children.Add(yearLabel);

            grid.Children.Add(coverImage);
            grid.Children.Add(overlayPanel);

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnItemTapped;
            grid.GestureRecognizers.Add(tapGesture);

            return grid;
        });

        contentGrid.Children.Add(topPanel);
        contentGrid.Children.Add(_itemsList);

        mainGrid.Children.Add(_bgImage);
        mainGrid.Children.Add(contentGrid);

        Content = mainGrid;
    }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LabelCollection2 control)
        {
            control._itemsList.ItemsSource = newValue as System.Collections.IEnumerable;
        }
    }

    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LabelCollection2 control)
        {
            control._titleLabel.Text = newValue as string;
        }
    }

    private static void OnDescriptionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LabelCollection2 control)
        {
            control._descLabel.Text = newValue as string;
        }
    }

    private static void OnBgImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LabelCollection2 control && newValue != null)
        {
            control._bgImage.Source = ImageSource.FromFile(newValue as string);
        }
    }

    private void OnViewAllClicked(object sender, EventArgs e)
    {
        DetailCommand?.Execute(Id);
    }

    private void OnItemTapped(object sender, TappedEventArgs e)
    {
        if (sender is Grid grid && grid.BindingContext != null)
        {
            ClickItemCommand?.Execute(grid.BindingContext);
        }
    }
}
