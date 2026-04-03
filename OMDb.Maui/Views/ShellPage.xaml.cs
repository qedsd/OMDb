using Microsoft.Maui.Controls;
using OMDb.Maui.ViewModels;
using OMDb.Maui.Views;

namespace OMDb.Maui.Views;

/// <summary>
/// 主窗口导航页面
/// 提供应用程序的主要导航框架，包括顶部菜单栏和内容区域
/// </summary>
public partial class ShellPage : ContentPage
{
    /// <summary>
    /// 创建主窗口导航页
    /// </summary>
    public ShellPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    /// <summary>
    /// 菜单按钮点击事件处理
    /// 显示导航菜单
    /// </summary>
    private async void MenuButton_Clicked(object sender, EventArgs e)
    {
        var action = await DisplayActionSheet("导航", "取消", null,
            "主页", "分类", "片单", "词条", "管理", "工具", "设置");

        if (action != "取消")
        {
            // 简单导航实现
            switch (action)
            {
                case "主页":
                    MainContentView.Content = new HomePage().Content;
                    break;
                case "分类":
                    MainContentView.Content = new ClassificationPage().Content;
                    break;
                case "片单":
                    MainContentView.Content = new CollectionsPage().Content;
                    break;
                case "词条":
                    MainContentView.Content = new EntryHomePage().Content;
                    break;
                case "管理":
                    MainContentView.Content = new ManagementPage().Content;
                    break;
                case "工具":
                    MainContentView.Content = new ToolsPage().Content;
                    break;
                case "设置":
                    MainContentView.Content = new SettingPage().Content;
                    break;
            }
        }
    }

    /// <summary>
    /// 显示信息提示
    /// </summary>
    /// <param name="message">提示信息</param>
    /// <param name="severity">信息类型</param>
    public void ShowInfo(string message, string severity = "Informational")
    {
        InfoBarLabel.Text = message;
        InfoBarGrid.IsVisible = true;

        // 3 秒后自动隐藏
        Device.StartTimer(TimeSpan.FromSeconds(3), () =>
        {
            InfoBarGrid.IsVisible = false;
            return false;
        });
    }

    /// <summary>
    /// 显示等待遮罩
    /// </summary>
    public void ShowWaiting()
    {
        WaitingGrid.IsVisible = true;
    }

    /// <summary>
    /// 隐藏等待遮罩
    /// </summary>
    public void HideWaiting()
    {
        WaitingGrid.IsVisible = false;
    }
}
