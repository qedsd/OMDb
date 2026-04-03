using System.Text;
using CommunityToolkit.Mvvm.Input;
using OMDb.Maui.ViewModels;

namespace OMDb.Maui.AutoTest;

/// <summary>
/// OMDb.MaUI 自动化测试程序
///
/// 用途：
/// 1. 测试所有 ViewModel 的初始化是否正常
/// 2. 测试 ViewModel 的命令执行是否报错
/// 3. 验证分类页面等关键功能不会崩溃
///
/// 运行方式：
/// dotnet run --project OMDb.Maui.AutoTest/OMDb.Maui.AutoTest.csproj
///
/// 测试环境：
/// - .NET 10.0
/// - Windows 10.0.19041.0+
/// - 不依赖 UI，纯控制台运行
/// </summary>
public class Program
{
    /// <summary>
    /// 程序入口点
    /// 执行所有 ViewModel 的自动化测试
    /// </summary>
    /// <param name="args">命令行参数（未使用）</param>
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== OMDb.MaUI 自动测试 ===");
        Console.WriteLine();

        // 存储测试结果
        // 每个结果包含：测试名称、是否通过、错误信息
        var results = new List<(string Test, bool Passed, string Error)>();

        // ==================== 测试 HomeViewModel ====================
        // 主页视图模型测试
        // 验证主页初始化和刷新命令是否正常工作
        Console.WriteLine("测试 HomeViewModel...");
        try
        {
            var homeVm = new HomeViewModel();
            homeVm.Init();
            results.Add(("HomeViewModel.Init", true, ""));

            // 测试刷新命令（异步命令）
            if (homeVm.RefreshCommand is IAsyncRelayCommand asyncCmd)
            {
                await asyncCmd.ExecuteAsync(null);
                results.Add(("HomeViewModel.RefreshCommand", true, ""));
            }
        }
        catch (Exception ex)
        {
            results.Add(("HomeViewModel", false, ex.Message));
            Console.WriteLine($"  FAIL: {ex.Message}");
        }
        Console.WriteLine("  OK");

        // ==================== 测试 ClassificationViewModel ====================
        // 分类页视图模型测试
        // 验证分类页初始化和命令执行是否正常工作
        // 分类页是之前崩溃的重点区域，需要特别关注
        Console.WriteLine("测试 ClassificationViewModel...");
        try
        {
            var classificationVm = new ClassificationViewModel();
            await Task.Delay(3000); // 等待异步初始化完成（加载标签、轮播图等）
            results.Add(("ClassificationViewModel.Init", true, ""));

            // 测试刷新命令
            classificationVm.RefreshCommand.Execute(null);
            results.Add(("ClassificationViewModel.RefreshCommand", true, ""));

            // 测试视图切换命令（列表/网格）
            classificationVm.ChangeShowTypeCommand.Execute("True");
            classificationVm.ChangeShowTypeCommand.Execute("False");
            results.Add(("ClassificationViewModel.ChangeShowTypeCommand", true, ""));

            // 测试标签详情命令（模拟点击分类标签）
            // 这是之前崩溃的关键点：点击分类标签时可能因为 null 引用而崩溃
            classificationVm.LabelDetailCommand.Execute(null);
            results.Add(("ClassificationViewModel.LabelDetailCommand", true, ""));
        }
        catch (Exception ex)
        {
            results.Add(("ClassificationViewModel", false, ex.Message));
            Console.WriteLine($"  FAIL: {ex.Message}");
            Console.WriteLine($"  Stack: {ex.StackTrace}");
        }
        Console.WriteLine("  OK");

        // ==================== 测试 CollectionsViewModel ====================
        // 片单页视图模型测试
        // 验证片单管理功能是否正常
        Console.WriteLine("测试 CollectionsViewModel...");
        try
        {
            var collectionsVm = new CollectionsViewModel();
            await Task.Delay(3000); // 等待异步初始化
            results.Add(("CollectionsViewModel.Init", true, ""));

            // 测试添加新片单命令
            collectionsVm.AddNewCollectionCommand.Execute(null);
            results.Add(("CollectionsViewModel.AddNewCollectionCommand", true, ""));

            // 测试刷新命令
            collectionsVm.RefreshCommand.Execute(null);
            results.Add(("CollectionsViewModel.RefreshCommand", true, ""));
        }
        catch (Exception ex)
        {
            results.Add(("CollectionsViewModel", false, ex.Message));
            Console.WriteLine($"  FAIL: {ex.Message}");
        }
        Console.WriteLine("  OK");

        // ==================== 测试 ShellViewModel ====================
        // 导航壳视图模型测试
        // 验证主导航功能是否正常
        Console.WriteLine("测试 ShellViewModel...");
        try
        {
            var shellVm = new ShellViewModel();
            shellVm.NavClickCommand.Execute(null);
            results.Add(("ShellViewModel.NavClickCommand", true, ""));
        }
        catch (Exception ex)
        {
            results.Add(("ShellViewModel", false, ex.Message));
            Console.WriteLine($"  FAIL: {ex.Message}");
        }
        Console.WriteLine("  OK");

        // ==================== 测试 SettingViewModel ====================
        // 设置页视图模型测试
        // 验证数据库选择器等功能是否正常
        Console.WriteLine("测试 SettingViewModel...");
        try
        {
            var settingVm = new SettingViewModel();
            settingVm.DbSelector_RefreshCommand.Execute(null);
            results.Add(("SettingViewModel.DbSelector_RefreshCommand", true, ""));
        }
        catch (Exception ex)
        {
            results.Add(("SettingViewModel", false, ex.Message));
            Console.WriteLine($"  FAIL: {ex.Message}");
        }
        Console.WriteLine("  OK");

        // ==================== 测试 EntryCollectionDetailViewModel ====================
        // 片单详情页视图模型测试
        // 验证片单编辑和删除功能是否正常
        // 这是之前崩溃的另一个重点区域（CancelEdit 命令）
        Console.WriteLine("测试 EntryCollectionDetailViewModel...");
        try
        {
            var entryVm = new EntryCollectionDetailViewModel();
            // 测试取消编辑命令
            // 之前问题：EntryCollection 为 null 时访问其属性导致崩溃
            entryVm.CancelEditCommand.Execute(null);
            results.Add(("EntryCollectionDetailViewModel.CancelEditCommand", true, ""));
            Console.WriteLine("  OK");
        }
        catch (Exception ex)
        {
            results.Add(("EntryCollectionDetailViewModel", false, ex.Message));
            Console.WriteLine($"  FAIL: {ex.Message}");
            Console.WriteLine($"  Stack: {ex.StackTrace}");
            throw;
        }

        // ==================== 测试 LabelCollectionViewModel ====================
        // 标签合集视图模型测试
        // 验证标签合集展示功能是否正常
        Console.WriteLine("测试 LabelCollectionViewModel...");
        try
        {
            var labelVm = new LabelCollectionViewModel();
            labelVm.ItemClickCommand.Execute(null);
            results.Add(("LabelCollectionViewModel.ItemClickCommand", true, ""));
        }
        catch (Exception ex)
        {
            results.Add(("LabelCollectionViewModel", false, ex.Message));
            Console.WriteLine($"  FAIL: {ex.Message}");
        }
        Console.WriteLine("  OK");

        // ==================== 输出测试结果汇总 ====================
        Console.WriteLine();
        Console.WriteLine("=== 测试结果汇总 ===");
        int passed = results.Count(r => r.Passed);
        int failed = results.Count(r => !r.Passed);
        Console.WriteLine($"通过：{passed}, 失败：{failed}");
        Console.WriteLine();

        // 如果有失败的测试，输出详细信息
        if (failed > 0)
        {
            Console.WriteLine("失败详情:");
            foreach (var result in results.Where(r => !r.Passed))
            {
                Console.WriteLine($"  ✗ {result.Test}: {result.Error}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("测试完成。按任意键退出...");
        try
        {
            Console.ReadKey();
        }
        catch { /* 在非控制台环境下忽略错误 */ }
    }
}
