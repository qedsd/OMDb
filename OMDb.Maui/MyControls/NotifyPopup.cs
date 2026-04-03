using Microsoft.Maui.Controls;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 通知弹窗控件
/// MAUI 版本使用 Overlay + Animation 实现
/// </summary>
public class NotifyPopup : Grid
{
    private readonly Label _iconLabel;
    private readonly Label _notifyLabel;
    private readonly Border _contentBorder;
    private readonly Grid _overlay;

    private NotifyPopup()
    {
        // 遮罩层
        _overlay = new Grid
        {
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        // 内容边框
        _contentBorder = new Border
        {
            Stroke = new SolidColorBrush(Colors.Gray),
            StrokeThickness = 1,
            BackgroundColor = Colors.WhiteSmoke,
            Padding = new Thickness(20),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            MinimumWidthRequest = 200
        };

        var stackLayout = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        // 图标
        _iconLabel = new Label
        {
            FontFamily = "Segoe MDL2 Assets",
            FontSize = 32,
            TextColor = Colors.IndianRed,
            HorizontalOptions = LayoutOptions.Center
        };

        // 通知文本
        _notifyLabel = new Label
        {
            Margin = new Thickness(0, 10),
            HorizontalOptions = LayoutOptions.Center,
            TextColor = Colors.Black,
            LineBreakMode = LineBreakMode.WordWrap
        };

        stackLayout.Children.Add(_iconLabel);
        stackLayout.Children.Add(_notifyLabel);
        _contentBorder.Content = stackLayout;

        Children.Add(_overlay);
        Children.Add(_contentBorder);

        IsVisible = false;
    }

    public NotifyPopup(string content, string iconStr = "\uE10A", Color? color = null, TimeSpan? showTime = null) : this()
    {
        _notifyLabel.Text = content;
        _iconLabel.Text = iconStr;
        _iconLabel.TextColor = color ?? Colors.IndianRed;

        // 自动隐藏计时器
        Device.StartTimer(showTime ?? TimeSpan.FromSeconds(2), () =>
        {
            Hide();
            return false;
        });
    }

    /// <summary>
    /// 显示成功通知
    /// </summary>
    public static async Task ShowSuccessAsync(string text, Page parent = null)
    {
        await ShowAsync(text, "\uE082", Colors.MediumSeaGreen, parent);
    }

    /// <summary>
    /// 显示错误通知
    /// </summary>
    public static async Task ShowErrorAsync(string text, Page parent = null)
    {
        await ShowAsync(text, "\uE171", Colors.IndianRed, parent);
    }

    /// <summary>
    /// 显示信息通知
    /// </summary>
    public static async Task ShowInformationAsync(string text, Page parent = null)
    {
        await ShowAsync(text, "\uE946", Colors.CornflowerBlue, parent);
    }

    /// <summary>
    /// 显示警告通知
    /// </summary>
    public static async Task ShowWarningAsync(string text, Page parent = null)
    {
        await ShowAsync(text, "\uE7BA", Colors.Orange, parent);
    }

    private static async Task ShowAsync(string text, string icon, Color color, Page? parent = null)
    {
        var popup = new NotifyPopup(text, icon, color);

        // 获取父页面
        var targetPage = parent ?? (Application.Current?.MainPage as NavigationPage)?.CurrentPage ?? Application.Current?.MainPage;
        if (targetPage == null)
            return;

        // 添加到父页面
        var overlayGrid = new Grid
        {
            BackgroundColor = Colors.Black.MultiplyAlpha((float)0.3),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        var container = new Grid
        {
            Children = { overlayGrid, popup }
        };

        // 使用 MAUI 的 Overlay 或添加到当前页面
        // MAUI 中 ContentPage 没有直接的 Content 属性访问，使用 simpler approach
        var mainGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Children = { new Grid(), container }
        };

        // 将 overlay 添加到页面上层
        if (targetPage is ContentPage contentPage && contentPage.Content is Grid existingGrid)
        {
            existingGrid.Children.Add(container);
        }
        else
        {
            // 创建一个覆盖层
            var absoluteLayout = new AbsoluteLayout
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };
            absoluteLayout.Children.Add(container);

            // 创建新页面包装器 - 简化的实现
            var overlay = new Grid
            {
                BackgroundColor = Colors.Transparent,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Children = { container }
            };

            // 尝试添加到页面
            if (targetPage is ContentPage cp)
            {
                var newGrid = new Grid
                {
                    Children = { overlay }
                };
                cp.Content = newGrid;
            }
        }

        popup.IsVisible = true;

        // 淡入动画
        await popup.FadeTo(1, 200, Easing.CubicInOut);

        // 等待自动隐藏后移除
        await Task.Delay(2400);

        await popup.FadeTo(0, 400, Easing.CubicInOut);

        // 从父容器移除
        if (popup.Parent is Grid parentGrid)
        {
            parentGrid.Children.Remove(popup);
            if (parentGrid.Children.Contains(overlayGrid))
                parentGrid.Children.Remove(overlayGrid);
        }
    }

    private async void Hide()
    {
        await this.FadeTo(0, 400, Easing.CubicInOut);
        IsVisible = false;
    }
}
