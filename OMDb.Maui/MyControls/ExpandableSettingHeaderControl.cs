using Microsoft.Maui.Controls;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 可展开设置头部控件
/// 用于设置页面的分组标题
/// </summary>
public class ExpandableSettingHeaderControl : Border
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(ExpandableSettingHeaderControl), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(
        nameof(Description), typeof(string), typeof(ExpandableSettingHeaderControl), string.Empty);

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon), typeof(View), typeof(ExpandableSettingHeaderControl), null);

    public View Icon
    {
        get => (View)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty SettingActionableElementProperty = BindableProperty.Create(
        nameof(SettingActionableElement), typeof(View), typeof(ExpandableSettingHeaderControl), null);

    public View SettingActionableElement
    {
        get => (View)GetValue(SettingActionableElementProperty);
        set => SetValue(SettingActionableElementProperty, value);
    }

    private readonly View _iconPresenter;
    private readonly Label _titleLabel;
    private readonly Label _descriptionLabel;
    private readonly View _actionableElement;
    private readonly Grid _mainPanel;

    public ExpandableSettingHeaderControl()
    {
        Stroke = new SolidColorBrush(Colors.Transparent);
        StrokeThickness = 0;
        BackgroundColor = Colors.Transparent;
        Padding = new Thickness(0);
        HorizontalOptions = LayoutOptions.Fill;
        VerticalOptions = LayoutOptions.Center;

        // 图标
        _iconPresenter = new ContentView
        {
            WidthRequest = 20,
            Margin = new Thickness(2, 0, 20, 0),
            VerticalOptions = LayoutOptions.Center,
            Content = new Label { Text = "\uE74D" } // 默认图标
        };

        // 标题
        _titleLabel = new Label
        {
            FontSize = 16,
            TextColor = Colors.White,
            VerticalOptions = LayoutOptions.Center
        };

        // 描述
        _descriptionLabel = new Label
        {
            FontSize = 12,
            TextColor = Colors.Gray,
            VerticalOptions = LayoutOptions.Center
        };

        var descriptionPanel = new VerticalStackLayout
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Children = { _titleLabel, _descriptionLabel }
        };

        // 可操作元素
        _actionableElement = new ContentView
        {
            Margin = new Thickness(24, 0, 0, 0),
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center
        };

        // 主面板
        _mainPanel = new Grid();
        _mainPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        _mainPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        _mainPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        _mainPanel.Children.Add(_iconPresenter);
        _mainPanel.Children.Add(descriptionPanel);
        _mainPanel.Children.Add(_actionableElement);

        Grid.SetColumn(_iconPresenter, 0);
        Grid.SetColumn(descriptionPanel, 1);
        Grid.SetColumn(_actionableElement, 2);

        Content = _mainPanel;

        // 监听大小变化
        _mainPanel.SizeChanged += MainPanel_SizeChanged;
    }

    private void MainPanel_SizeChanged(object sender, EventArgs e)
    {
        if (_actionableElement == null)
            return;

        var grid = sender as Grid;
        // MAUI 使用 Width 而不是 ActualWidth
        var gridWidth = grid.Width;
        var actionableWidth = _actionableElement.Width;

        if (gridWidth > 0 && actionableWidth > gridWidth / 3)
        {
            // 紧凑状态：操作元素移到下方
            Grid.SetColumn(_actionableElement, 1);
            Grid.SetRow(_actionableElement, 1);
            _actionableElement.Margin = new Thickness(0, 4, 0, 0);
        }
        else
        {
            // 正常状态：操作元素在右侧
            Grid.SetColumn(_actionableElement, 2);
            Grid.SetRow(_actionableElement, 0);
            _actionableElement.Margin = new Thickness(24, 0, 0, 0);
        }
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext != null)
        {
            // 更新图标 - 使用 ContentView 包装
            if (Icon != null && _iconPresenter is ContentView cv)
            {
                cv.Content = Icon;
            }

            // 更新标题
            if (!string.IsNullOrEmpty(Title))
            {
                _titleLabel.Text = Title;
                _titleLabel.IsVisible = true;
            }
            else
            {
                _titleLabel.IsVisible = false;
            }

            // 更新描述
            if (!string.IsNullOrEmpty(Description))
            {
                _descriptionLabel.Text = Description;
                _descriptionLabel.IsVisible = true;
            }
            else
            {
                _descriptionLabel.IsVisible = false;
            }

            // 更新可操作元素 - 使用 ContentView 包装
            if (SettingActionableElement != null && _actionableElement is ContentView cav)
            {
                cav.Content = SettingActionableElement;
                cav.IsVisible = true;
            }
            else
            {
                _actionableElement.IsVisible = false;
            }
        }
    }
}
