using Microsoft.Maui.Controls;
using OMDb.Maui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Threading;

namespace OMDb.Maui.MyControls
{
    /// <summary>
    /// 轮播图控件 - 显示在分类页顶部的推荐轮播图
    ///
    /// 主要功能：
    /// 1. 自动轮播（每 8 秒切换一次）
    /// 2. 手动选择图片
    /// 3. 显示标题和描述
    /// 4. 支持查看详情按钮
    /// 5. 支持鼠标悬停放大效果（MAUI 中暂未实现）
    ///
    /// 使用示例：
    /// <code>
    /// // XAML 中定义
    /// &lt;mycontrols:Banner
    ///     ItemsSource="{Binding BannerItemsSource}"
    ///     DetailCommand="{Binding BannerDetailCommand}"
    ///     HeightRequest="300" /&gt;
    ///
    /// // 数据源
    /// BannerItemsSource = new List&lt;BannerItem&gt;
    /// {
    ///     new BannerItem { Title = "电影 1", Description = "描述 1", Img = "image1.jpg", PreviewImg = "thumb1.jpg" },
    ///     new BannerItem { Title = "电影 2", Description = "描述 2", Img = "image2.jpg", PreviewImg = "thumb2.jpg" }
    /// };
    /// </code>
    ///
    /// 注意：MAUI 版本简化了动画效果，使用 CarouselView 实现
    /// </summary>
    public class Banner : ContentView
    {
        /// <summary>
        /// ItemsSource 绑定属性
        /// 轮播图数据源
        /// </summary>
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable<BannerItem>),
            typeof(Banner),
            null,
            propertyChanged: OnItemsSourceChanged);

        /// <summary>
        /// DetailCommand 绑定属性
        /// 点击"查看详情"按钮时执行的命令
        /// </summary>
        public static readonly BindableProperty DetailCommandProperty = BindableProperty.Create(
            nameof(DetailCommand),
            typeof(ICommand),
            typeof(Banner),
            null);

        /// <summary>
        /// 轮播图数据源
        /// </summary>
        public IEnumerable<BannerItem> ItemsSource
        {
            get => (IEnumerable<BannerItem>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// 查看详情命令
        /// 点击按钮时执行
        /// </summary>
        public ICommand DetailCommand
        {
            get => (ICommand)GetValue(DetailCommandProperty);
            set => SetValue(DetailCommandProperty, value);
        }

        /// <summary>
        /// 主布局
        /// </summary>
        private Grid _mainGrid;

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
        /// 轮播视图
        /// </summary>
        private CarouselView _carouselView;

        /// <summary>
        /// 指示器
        /// </summary>
        private IndicatorView _indicatorView;

        /// <summary>
        /// 自动轮播计时器
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// 当前选中项
        /// </summary>
        private BannerItem _currentItem;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Banner()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化组件
        /// 创建 UI 元素并设置布局
        /// </summary>
        private void InitializeComponent()
        {
            // 主布局
            _mainGrid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Auto }
                }
            };

            // 轮播视图
            _carouselView = new CarouselView
            {
                HeightRequest = 250,
                Loop = true,
                IsSwipeEnabled = true,
                ItemTemplate = new DataTemplate(() =>
                {
                    var image = new Image
                    {
                        Aspect = Aspect.AspectFill,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill
                    };
                    image.SetBinding(Image.SourceProperty, nameof(BannerItem.Img));
                    return new Frame
                    {
                        Content = image,
                        Padding = 0,
                        HasShadow = false,
                        IsClippedToBounds = true,
                        CornerRadius = 0
                    };
                })
            };
            _carouselView.SetBinding(CarouselView.ItemsSourceProperty, nameof(ItemsSource));
            _carouselView.CurrentItemChanged += OnCurrentItemChanged;

            // 指示器
            _indicatorView = new IndicatorView
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                IndicatorSize = 8,
                SelectedIndicatorColor = Colors.White
            };
            _indicatorView.SetBinding(IndicatorView.ItemsSourceProperty, nameof(ItemsSource));
            _carouselView.IndicatorView = _indicatorView;

            // 标题
            _titleLabel = new Label
            {
                FontSize = 24,
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(16, 0, 0, 0)
            };

            // 描述
            _descriptionLabel = new Label
            {
                FontSize = 14,
                TextColor = Colors.LightGray,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(16, 4, 0, 0)
            };

            // 详情按钮
            _detailButton = new Button
            {
                Text = "查看详情",
                BackgroundColor = Colors.Blue,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(16, 10, 0, 10),
                WidthRequest = 120,
                Command = DetailCommand,
                CommandParameter = _currentItem
            };

            // 信息面板
            var infoPanel = new Grid
            {
                BackgroundColor = Color.FromArgb("#80000000"),
                Children =
                {
                    new VerticalStackLayout
                    {
                        Children = { _titleLabel, _descriptionLabel, _detailButton }
                    }
                }
            };

            // 添加到主布局
            _mainGrid.Children.Add(_carouselView);
            _mainGrid.Children.Add(infoPanel);
            _mainGrid.Children.Add(_indicatorView);

            // 设置内容
            Content = _mainGrid;

            // 初始化计时器
            InitializeTimer();
        }

        /// <summary>
        /// 初始化自动轮播计时器
        /// </summary>
        private void InitializeTimer()
        {
            _timer = new Timer(state =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (_carouselView.ItemsSource != null &&
                        _carouselView.ItemsSource is IList<BannerItem> list &&
                        list.Count > 0)
                    {
                        int nextIndex = (_carouselView.Position + 1) % list.Count;
                        _carouselView.Position = nextIndex;
                    }
                });
            }, null, TimeSpan.FromSeconds(8), TimeSpan.FromSeconds(8));
        }

        /// <summary>
        /// ItemsSource 变更回调
        /// </summary>
        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Banner banner)
            {
                banner.UpdateItemsSource();
            }
        }

        /// <summary>
        /// 更新数据源
        /// </summary>
        private void UpdateItemsSource()
        {
            if (ItemsSource != null && ItemsSource.Any())
            {
                _carouselView.Position = 0;
                // 重启计时器
                _timer?.Change(TimeSpan.FromSeconds(8), TimeSpan.FromSeconds(8));
            }
            else
            {
                _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        /// <summary>
        /// 当前项变更事件处理
        /// </summary>
        private void OnCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            _currentItem = e.CurrentItem as BannerItem;

            if (_currentItem != null)
            {
                // 更新标题和描述
                _titleLabel.Text = _currentItem.Title;
                _descriptionLabel.Text = _currentItem.Description;

                // 更新按钮命令参数
                _detailButton.CommandParameter = _currentItem;

                // 重置计时器
                _timer?.Change(TimeSpan.FromSeconds(8), TimeSpan.FromSeconds(8));
            }
        }

        /// <summary>
        /// 启动自动轮播
        /// </summary>
        public void StartAutoPlay()
        {
            _timer?.Change(TimeSpan.FromSeconds(8), TimeSpan.FromSeconds(8));
        }

        /// <summary>
        /// 停止自动轮播
        /// </summary>
        public void StopAutoPlay()
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}
