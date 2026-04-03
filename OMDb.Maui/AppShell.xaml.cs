using Microsoft.Maui.Controls;

namespace OMDb.Maui;

/// <summary>
/// 应用程序导航壳
/// 定义应用的主导航结构和侧边栏菜单
///
/// 主要功能：
/// 1. 管理应用的页面导航
/// 2. 提供侧边栏菜单（Flyout）
/// 3. 注册应用路由
/// 4. 管理菜单按钮行为
///
/// 继承自：Microsoft.Maui.Controls.Shell
/// </summary>
public partial class AppShell : Shell
{
    /// <summary>
    /// 构造函数
    /// 初始化导航壳并注册应用路由
    /// </summary>
    public AppShell()
    {
        InitializeComponent();

        // 注册 EntryDetailPage 路由
        // 这样可以通过 Shell 导航到词条详情页
        // 路由格式：//EntryDetailPage
        Routing.RegisterRoute(nameof(Views.EntryDetailPage), typeof(Views.EntryDetailPage));
    }

    /// <summary>
    /// 菜单按钮点击事件处理
    /// 当用户点击侧边栏菜单按钮时触发
    /// 显示/隐藏 Flyout 菜单
    /// </summary>
    /// <param name="sender">事件源对象</param>
    /// <param name="e">事件参数</param>
    private void MenuButton_Clicked(object sender, EventArgs e)
    {
        // 显示 Flyout 菜单
        // 侧边栏菜单包含应用的主要导航项
        Current.FlyoutIsPresented = true;
    }


    /// <summary>
    /// 处理器变更时的回调方法
    /// 当控件的 Handler 被设置或更改时调用
    /// 用于在初始化后自动隐藏 Flyout 菜单
    /// </summary>
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        // 延迟 100ms 后隐藏 Flyout 菜单
        // 确保应用启动时菜单是收起状态
        Task.Delay(100).ContinueWith(_ =>
        {
            FlyoutIsPresented = false;
        });
    }
}
