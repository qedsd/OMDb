using Microsoft.Maui.Controls;
using OMDb.Maui.ViewModels;
using OMDb.Maui.MyControls;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 条目详情控件
/// 显示和编辑条目详细信息
/// </summary>
public class EntryDetailControl : Grid
{
    private readonly Image _coverImage;
    private readonly Entry _nameEntry;
    private readonly Label _pathLabel;
    private readonly Label _sourcePathLabel;
    private readonly DatePicker _releaseDatePicker;
    private readonly RatingControl _ratingControl;
    private readonly LabelsControl _labelsControl;

    public EditEntryHomeViewModel VM { get; set; }

    public EntryDetailControl()
    {
        VM = new EditEntryHomeViewModel(null);
        BindingContext = VM;

        Margin = new Thickness(0, 20, 0, 0);

        // 封面图片按钮
        _coverImage = new Image
        {
            WidthRequest = 91,
            HeightRequest = 128,
            Aspect = Aspect.AspectFill
        };

        var addCoverLabel = new Label
        {
            Text = "+ 添加封面图",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        var coverGrid = new Grid
        {
            HeightRequest = 128,
            WidthRequest = 91,
            BackgroundColor = Colors.Transparent,
            Children = { addCoverLabel, _coverImage }
        };

        var coverButton = new Button
        {
            BackgroundColor = Colors.Transparent,
            BorderColor = Colors.Transparent,
            VerticalOptions = LayoutOptions.Start
        };
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += CoverButton_Clicked;
        coverGrid.GestureRecognizers.Add(tapGesture);
        coverButton.Command = new Command(() => CoverButton_Clicked(coverButton, EventArgs.Empty));
        coverButton.Clicked += CoverButton_Clicked;

        // 名称输入
        _nameEntry = new Entry
        {
            WidthRequest = 350,
            HeightRequest = 40,
            Placeholder = "输入条目名称",
            Margin = new Thickness(4, 0, 12, 0)
        };
        _nameEntry.TextChanged += (s, e) =>
        {
            if (VM != null)
                VM.EntryName = _nameEntry.Text;
        };

        var nameLayout = new HorizontalStackLayout
        {
            Children = { _nameEntry }
        };

        // 路径显示
        _pathLabel = new Label
        {
            WidthRequest = 960,
            Margin = new Thickness(20, 10, 0, 0)
        };
        var pathTitleLabel = new Label
        {
            WidthRequest = 87,
            Text = "条目路径：",
            Margin = new Thickness(8, 10, 20, 0)
        };

        var pathLayout = new HorizontalStackLayout
        {
            Children = { pathTitleLabel, _pathLabel }
        };

        // 源路径显示
        _sourcePathLabel = new Label
        {
            WidthRequest = 960,
            Margin = new Thickness(0, 10, 0, 0)
        };
        var sourcePathTitleLabel = new Label
        {
            WidthRequest = 87,
            Text = "源路径：",
            Margin = new Thickness(8, 10, 20, 0)
        };

        var sourcePathLayout = new HorizontalStackLayout
        {
            Children = { sourcePathTitleLabel, _sourcePathLabel }
        };

        var pathStack = new VerticalStackLayout
        {
            Children = { pathLayout, sourcePathLayout }
        };

        // 上映日期和评分
        _releaseDatePicker = new DatePicker
        {
            WidthRequest = 180,
            Margin = new Thickness(0, 0, 0, 0),
            Scale = 0.92
        };
        var releaseDateLabel = new Label
        {
            Text = "上映日期：",
            Margin = new Thickness(8, 5, 17, 0)
        };

        var releaseDateLayout = new HorizontalStackLayout
        {
            Margin = new Thickness(0, 0, 0, 5),
            Children = { releaseDateLabel, _releaseDatePicker }
        };

        _ratingControl = new RatingControl
        {
            Margin = new Thickness(0, 3, 0, 0)
        };
        var ratingLabel = new Label
        {
            WidthRequest = 87,
            Text = "评分：",
            Margin = new Thickness(29, 5, 5, 0)
        };

        var ratingLayout = new HorizontalStackLayout
        {
            Children = { ratingLabel, _ratingControl }
        };

        var rightStack = new VerticalStackLayout
        {
            Children = { releaseDateLayout, ratingLayout }
        };

        // 标签
        var labelsTitle = new Label { Text = "标签" };
        _labelsControl = new LabelsControl
        {
            Margin = new Thickness(0, 4, 0, 0),
            HorizontalOptions = LayoutOptions.Start
        };

        var labelsStack = new VerticalStackLayout
        {
            Children = { labelsTitle, _labelsControl }
        };

        // 主布局
        var topRow = new HorizontalStackLayout
        {
            Children =
            {
                coverButton,
                new VerticalStackLayout
                {
                    Children = { nameLayout, pathStack }
                },
                rightStack,
                labelsStack
            }
        };

        Children.Add(topRow);
    }

    private async void CoverButton_Clicked(object sender, EventArgs e)
    {
        // 打开文件选择器选择封面图片
        try
        {
            var file = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "选择封面图片",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DeviceInfo.Platform, new[] { ".jpg", ".jpeg", ".png", ".webp" } }
                })
            });

            if (file != null)
            {
                var stream = await file.OpenReadAsync();
                _coverImage.Source = ImageSource.FromStream(() => stream);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"选择封面失败：{ex.Message}");
        }
    }
}
