using Microsoft.Maui.Controls;
using Entry = OMDb.Core.Models.Entry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace OMDb.Maui.MyControls
{
    /// <summary>
    /// 标签合集展示控件（样式 1）-  horizontal 列表形式展示词条
    ///
    /// 主要功能：
    /// 1. 显示标题和描述
    /// 2. 显示背景图片
    /// 3. 水平列表展示词条卡片
    /// 4. 支持点击查看词条详情
    /// 5. 支持"查看全部"按钮
    ///
    /// 使用示例：
    /// <code>
    /// // XAML 中定义
    /// &lt;mycontrols:LabelCollection1
    ///     Title="{Binding Title}"
    ///     Description="{Binding Description}"
    ///     ItemsSource="{Binding Entries}"
    ///     BgImageSource="{Binding BackgroundImage}"
    ///     DetailCommand="{Binding ViewAllCommand}"
    ///     ClickItemCommand="{Binding ItemClickCommand}"
    ///     HeightRequest="400" /&gt;
    ///
    /// // 数据源
    /// ItemsSource = new List&lt;Entry&gt;
    /// {
    ///     new Entry { Name = "电影 1", CoverImg = "cover1.jpg" },
    ///     new Entry { Name = "电影 2", CoverImg = "cover2.jpg" }
    /// };
    /// </code>
    ///
    /// 注意：MAUI 版本简化了动画效果，使用 CollectionView 实现
    /// </summary>
    public class LabelCollection1 : ContentView
    {
        /// <summary>
        /// ItemsSource 绑定属性 - 词条列表
        /// </summary>
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable<Entry>),
            typeof(LabelCollection1),
            null,
            propertyChanged: OnItemsSourceChanged);

        /// <summary>
        /// Title 绑定属性 - 标题
        /// </summary>
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(LabelCollection1),
            string.Empty,
            propertyChanged: OnTitleChanged);

        /// <summary>
        /// Description 绑定属性 - 描述
        /// </summary>
        public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(
            nameof(Description),
            typeof(string),
            typeof(LabelCollection1),
            string.Empty,
            propertyChanged: OnDescriptionChanged);

        /// <summary>
        /// DetailCommand 绑定属性 - 查看详情命令
        /// </summary>
        public static readonly BindableProperty DetailCommandProperty = BindableProperty.Create(
            nameof(DetailCommand),
            typeof(ICommand),
            typeof(LabelCollection1),
            null);

        /// <summary>
        /// ClickItemCommand 绑定属性 - 点击项命令
        /// </summary>
        public static readonly BindableProperty ClickItemCommandProperty = BindableProperty.Create(
            nameof(ClickItemCommand),
            typeof(ICommand),
            typeof(LabelCollection1),
            null);

        /// <summary>
        /// BgImageSource 绑定属性 - 背景图片
        /// </summary>
        public static readonly BindableProperty BgImageSourceProperty = BindableProperty.Create(
            nameof(BgImageSource),
            typeof(ImageSource),
            typeof(LabelCollection1),
            null,
            propertyChanged: OnBgImageSourceChanged);

        /// <summary>
        /// Id 绑定属性 - 标签合集 ID
        /// </summary>
        public static readonly BindableProperty IdProperty = BindableProperty.Create(
            nameof(Id),
            typeof(string),
            typeof(LabelCollection1),
            string.Empty);

        /// <summary>
        /// 词条列表数据源
        /// </summary>
        public IEnumerable<Entry> ItemsSource
        {
            get => (IEnumerable<Entry>)GetValue(ItemsSourceProperty);
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
        /// 标签合集 ID
        /// </summary>
        public string Id
        {
            get => (string)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        /// <summary>
        /// 查看详情命令（点击"查看全部"按钮时执行）
        /// </summary>
        public ICommand DetailCommand
        {
            get => (ICommand)GetValue(DetailCommandProperty);
            set => SetValue(DetailCommandProperty, value);
        }

        /// <summary>
        /// 点击项命令（点击词条卡片时执行）
        /// </summary>
        public ICommand ClickItemCommand
        {
            get => (ICommand)GetValue(ClickItemCommandProperty);
            set => SetValue(DetailCommandProperty, value);
        }

        /// <summary>
        /// 背景图片
        /// </summary>
        public ImageSource BgImageSource
        {
            get => (ImageSource)GetValue(BgImageSourceProperty);
            set => SetValue(BgImageSourceProperty, value);
        }

        /// <summary>
        /// 主布局网格
        /// </summary>
        private Grid _mainGrid;

        /// <summary>
        /// 背景图片
        /// </summary>
        private Image _bgImage;

        /// <summary>
        /// 标题标签
        /// </summary>
        private Label _titleLabel;

        /// <summary>
        /// 描述标签
        /// </summary>
        private Label _descriptionLabel;

        /// <summary>
        /// 详情按钮
        /// </summary>
        private Button _detailButton;

        /// <summary>
        /// 词条列表
        /// </summary>
        private CollectionView _itemsList;

        /// <summary>
        /// 遮罩层
        /// </summary>
        private BoxView _overlay;

        /// <summary>
        /// 构造函数
        /// </summary>
        public LabelCollection1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitializeComponent()
        {
            // 主布局
            _mainGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(240, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };

            // 背景图片
            _bgImage = new Image
            {
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Opacity = 0.3
            };
            _bgImage.SetBinding(Image.SourceProperty, nameof(BgImageSource));

            // 遮罩层
            _overlay = new BoxView
            {
                Color = Colors.Black,
                Opacity = 0.5,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            // 标题
            _titleLabel = new Label
            {
                FontSize = 28,
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(24, 0, 0, 0)
            };
            _titleLabel.SetBinding(Label.TextProperty, nameof(Title));

            // 描述
            _descriptionLabel = new Label
            {
                FontSize = 14,
                TextColor = Colors.LightGray,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(24, 8, 0, 16)
            };
            _descriptionLabel.SetBinding(Label.TextProperty, nameof(Description));

            // 详情按钮
            _detailButton = new Button
            {
                Text = "查看全部",
                BackgroundColor = Colors.Blue,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(24, 10, 0, 24),
                WidthRequest = 120,
                Command = DetailCommand,
                CommandParameter = Id
            };

            // 左侧信息面板
            var leftPanel = new VerticalStackLayout
            {
                Children = { _titleLabel, _descriptionLabel, _detailButton },
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };

            // 词条列表
            _itemsList = new CollectionView
            {
                ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Horizontal),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(16),
                HeightRequest = 300,
                ItemTemplate = new DataTemplate(() =>
                {
                    var image = new Image
                    {
                        Aspect = Aspect.AspectFill,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill
                    };
                    image.SetBinding(Image.SourceProperty, nameof(Entry.CoverImg));

                    var nameLabel = new Label
                    {
                        TextColor = Colors.White,
                        HorizontalOptions = LayoutOptions.Center,
                        FontSize = 12,
                        MaxLines = 1,
                        LineBreakMode = LineBreakMode.TailTruncation
                    };
                    nameLabel.SetBinding(Label.TextProperty, nameof(Entry.Name));

                    var yearLabel = new Label
                    {
                        TextColor = Colors.LightGray,
                        HorizontalOptions = LayoutOptions.Center,
                        FontSize = 10,
                        FontAttributes = FontAttributes.None
                    };
                    yearLabel.SetBinding(Label.TextProperty, nameof(Entry.ReleaseYear));

                    return new Frame
                    {
                        Content = new Grid
                        {
                            RowDefinitions = new RowDefinitionCollection
                            {
                                new RowDefinition { Height = GridLength.Star },
                                new RowDefinition { Height = GridLength.Auto }
                            },
                            Children =
                            {
                                image,
                                new BoxView
                                {
                                    Color = Colors.Black,
                                    Opacity = 0.6,
                                    HorizontalOptions = LayoutOptions.Fill,
                                    VerticalOptions = LayoutOptions.End,
                                    HeightRequest = 50
                                },
                                new VerticalStackLayout
                                {
                                    Children = { nameLabel, yearLabel },
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.End,
                                    Padding = new Thickness(8)
                                }
                            }
                        },
                        Padding = 0,
                        HasShadow = true,
                        IsClippedToBounds = true,
                        CornerRadius = 4,
                        Margin = new Thickness(8, 4),
                        WidthRequest = 160,
                        HeightRequest = 280,
                        GestureRecognizers =
                        {
                            new TapGestureRecognizer
                            {
                                Command = new Command(async (item) =>
                                {
                                    if (ClickItemCommand?.CanExecute(item) == true)
                                    {
                                        ClickItemCommand.Execute(item);
                                    }
                                    else if (item is Entry entry)
                                    {
                                        // 默认行为：显示提示
                                        await Application.Current.MainPage.DisplayAlert("提示", $"点击了：{entry.Name}", "确定");
                                    }
                                })
                            }
                        }
                    };
                })
            };
            _itemsList.SetBinding(CollectionView.ItemsSourceProperty, nameof(ItemsSource));

            // 添加到主布局
            _mainGrid.Children.Add(_bgImage);
            _mainGrid.Children.Add(_overlay);
            _mainGrid.Children.Add(leftPanel);
            _mainGrid.Children.Add(_itemsList);

            Content = _mainGrid;
        }

        /// <summary>
        /// ItemsSource 变更回调
        /// </summary>
        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is LabelCollection1 control && newValue is IEnumerable<Entry> entries)
            {
                control._itemsList.ItemsSource = entries;
            }
        }

        /// <summary>
        /// Title 变更回调
        /// </summary>
        private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is LabelCollection1 control)
            {
                control._titleLabel.Text = newValue as string;
            }
        }

        /// <summary>
        /// Description 变更回调
        /// </summary>
        private static void OnDescriptionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is LabelCollection1 control)
            {
                control._descriptionLabel.Text = newValue as string;
            }
        }

        /// <summary>
        /// BgImageSource 变更回调
        /// </summary>
        private static void OnBgImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is LabelCollection1 control)
            {
                control._bgImage.Source = newValue as ImageSource;
            }
        }
    }
}
