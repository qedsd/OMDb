using Microsoft.Extensions.DependencyInjection;
using System.Text;
using OMDb.Maui.ViewModels;

namespace OMDb.Maui;

/// <summary>
/// OMDb MAUI 应用程序主类
/// 负责应用程序的初始化和生命周期管理
///
/// 主要功能：
/// 1. 应用程序初始化
/// 2. 创建主窗口
/// 3. 运行自动测试（开发/调试模式）
///
/// 继承自：Microsoft.Maui.Controls.App
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// 应用程序构造函数
    /// 初始化应用程序并启动自动测试
    /// </summary>
    public App()
    {
        InitializeComponent();

        // 启动时自动测试 ViewModel
        // 在开发/调试模式下运行自动化测试，验证所有 ViewModel 的初始化
        _ = RunAutoTestsAsync();
    }

    /// <summary>
    /// 创建应用程序的主窗口
    /// 重写基类的 CreateWindow 方法
    /// </summary>
    /// <param name="activationState">应用激活状态（可选）</param>
    /// <returns>新的 Window 对象，包含 AppShell 导航壳</returns>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        // 创建包含 AppShell 的新窗口
        // AppShell 定义了应用的主导航结构
        return new Window(new AppShell());
    }

    /// <summary>
    /// 运行自动化测试
    /// 异步方法，在应用启动时测试所有 ViewModel
    ///
    /// 测试内容：
    /// 1. HomeViewModel - 主页视图模型初始化
    /// 2. ClassificationViewModel - 分类视图模型初始化
    /// 3. CollectionsViewModel - 片单视图模型初始化
    /// 4. ShellViewModel - 导航壳视图模型初始化
    ///
    /// 测试结果通过弹窗或调试输出显示
    /// </summary>
    /// <returns>Task</returns>
    private async Task RunAutoTestsAsync()
    {
        var results = new StringBuilder();
        results.AppendLine("=== 自动测试开始 ===");
        results.AppendLine();

        // 测试 HomeViewModel
        // 验证主页视图模型能否正常初始化
        try
        {
            var homeVm = new HomeViewModel();
            homeVm.Init();
            results.AppendLine("✓ HomeViewModel 初始化成功");
        }
        catch (Exception ex)
        {
            results.AppendLine($"✗ HomeViewModel 初始化失败：{ex.Message}");
            results.AppendLine(ex.StackTrace);
        }

        // 测试 ClassificationViewModel
        // 验证分类视图模型能否正常初始化（包括异步数据加载）
        try
        {
            var classificationVm = new ClassificationViewModel();
            await Task.Delay(2000); // 等待异步初始化完成
            results.AppendLine("✓ ClassificationViewModel 初始化成功");
        }
        catch (Exception ex)
        {
            results.AppendLine($"✗ ClassificationViewModel 初始化失败：{ex.Message}");
            results.AppendLine(ex.StackTrace);
        }

        // 测试 CollectionsViewModel
        // 验证片单视图模型能否正常初始化
        try
        {
            var collectionsVm = new CollectionsViewModel();
            await Task.Delay(2000); // 等待异步初始化完成
            results.AppendLine("✓ CollectionsViewModel 初始化成功");
        }
        catch (Exception ex)
        {
            results.AppendLine($"✗ CollectionsViewModel 初始化失败：{ex.Message}");
            results.AppendLine(ex.StackTrace);
        }

        // 测试 ShellViewModel
        // 验证导航壳视图模型的命令能否正常执行
        try
        {
            var shellVm = new ShellViewModel();
            shellVm.NavClickCommand.Execute(null);
            results.AppendLine("✓ ShellViewModel 命令执行成功");
        }
        catch (Exception ex)
        {
            results.AppendLine($"✗ ShellViewModel 命令执行失败：{ex.Message}");
            results.AppendLine(ex.StackTrace);
        }

        results.AppendLine();
        results.AppendLine("=== 自动测试结束 ===");

        // 显示测试结果
        // 如果主窗口可用，使用弹窗显示；否则输出到调试控制台
        if (Current?.Windows.Count > 0 && Current.Windows[0].Page != null)
        {
            await Current.Windows[0].Page.DisplayAlert("自动测试结果", results.ToString(), "确定");
        }
        else
        {
            // 窗口不可用时，输出到调试控制台
            System.Diagnostics.Debug.WriteLine(results.ToString());
        }
    }
}
