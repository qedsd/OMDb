using Microsoft.Maui.Controls;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 词条卡片控件
/// 用于在词条主页展示单个词条的卡片视图
/// </summary>
public class EntryCard : Border
{
    /// <summary>
    /// 词条数据绑定属性
    /// </summary>
    public static readonly BindableProperty EntryProperty =
        BindableProperty.Create(
            nameof(Entry),
            typeof(object),
            typeof(EntryCard),
            null,
            propertyChanged: OnEntryChanged);

    private readonly Image _coverImage;
    private readonly Label _nameLabel;
    private readonly Label _dateLabel;
    private readonly Border _ratingPanel;
    private readonly Label _ratingLabel;

    /// <summary>
    /// 词条数据
    /// </summary>
    public object Entry
    {
        get => GetValue(EntryProperty);
        set => SetValue(EntryProperty, value);
    }

    /// <summary>
    /// 创建词条卡片控件
    /// </summary>
    public EntryCard()
    {
        BackgroundColor = Colors.Transparent;
        Stroke = Colors.Transparent;
        Padding = 0;
        Margin = new Thickness(8);

        var grid = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = new GridLength(160, GridUnitType.Auto) },
                new RowDefinition { Height = GridLength.Auto }
            }
        };

        // 封面图片
        _coverImage = new Image
        {
            Aspect = Aspect.AspectFill,
            BackgroundColor = Colors.LightGray
        };

        // 底部信息面板
        var infoPanel = new VerticalStackLayout
        {
            Padding = 10,
            Spacing = 5,
            BackgroundColor = Colors.White
        };

        // 名称
        _nameLabel = new Label
        {
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            MaxLines = 1,
            LineBreakMode = LineBreakMode.TailTruncation,
            HorizontalOptions = LayoutOptions.Center
        };

        // 日期
        _dateLabel = new Label
        {
            FontSize = 12,
            TextColor = Colors.Gray,
            HorizontalOptions = LayoutOptions.Center
        };

        // 评分面板
        _ratingPanel = new Border
        {
            BackgroundColor = Colors.Gold,
            Padding = 5,
            HorizontalOptions = LayoutOptions.Center,
            IsVisible = false
        };

        _ratingLabel = new Label
        {
            FontSize = 12,
            TextColor = Colors.Black,
            HorizontalOptions = LayoutOptions.Center
        };

        _ratingPanel.Content = _ratingLabel;

        infoPanel.Children.Add(_nameLabel);
        infoPanel.Children.Add(_dateLabel);
        infoPanel.Children.Add(_ratingPanel);

        grid.Children.Add(_coverImage);
        Grid.SetRow(_coverImage, 0);

        grid.Children.Add(infoPanel);
        Grid.SetRow(infoPanel, 1);

        Content = grid;

        // 双击刷新
        var doubleTap = new TapGestureRecognizer();
        doubleTap.Tapped += async (s, e) =>
        {
            if (BindingContext is Microsoft.Maui.Controls.View view)
            {
                if (view.BindingContext is OMDb.Maui.ViewModels.HomeViewModel vm)
                {
                    vm.RefreshCommand.Execute(null);
                }
            }
        };
        GestureRecognizers.Add(doubleTap);
    }

    private static void OnEntryChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EntryCard card && newValue != null)
        {
            card.UpdateContent();
        }
    }

    private void UpdateContent()
    {
        if (Entry == null)
            return;

        var type = Entry.GetType();

        // 获取词条属性
        var nameProp = type.GetProperty("Name");
        var dateProp = type.GetProperty("ReleaseDate");
        var coverProp = type.GetProperty("CoverImg");
        var ratingProp = type.GetProperty("Rank");

        if (nameProp != null)
            _nameLabel.Text = nameProp.GetValue(Entry)?.ToString();

        if (dateProp != null)
        {
            var dateValue = dateProp.GetValue(Entry);
            if (dateValue != null)
            {
                var year = dateValue.GetType().GetProperty("Year")?.GetValue(dateValue);
                _dateLabel.Text = year?.ToString();
            }
            else
            {
                _dateLabel.Text = string.Empty;
            }
        }

        if (coverProp != null)
        {
            var coverImg = coverProp.GetValue(Entry)?.ToString();
            if (!string.IsNullOrEmpty(coverImg))
                _coverImage.Source = ImageSource.FromFile(coverImg);
        }

        if (ratingProp != null)
        {
            var rank = ratingProp.GetValue(Entry);
            if (rank != null && Convert.ToDouble(rank) > 0)
            {
                _ratingPanel.IsVisible = true;
                _ratingLabel.Text = rank.ToString();
            }
            else
            {
                _ratingPanel.IsVisible = false;
            }
        }
    }
}
