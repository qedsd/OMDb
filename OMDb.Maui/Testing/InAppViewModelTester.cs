using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using OMDb.Maui.ViewModels;
using CommunityToolkit.Mvvm.Input;

namespace OMDb.Maui.Testing;

/// <summary>
/// 在应用程序内测试 ViewModel 命令
/// </summary>
public static class InAppViewModelTester
{
    public class TestResult
    {
        public string ViewModelName { get; set; }
        public string CommandName { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public static async Task<List<TestResult>> RunAllTests()
    {
        var results = new List<TestResult>();

        // 测试 HomeViewModel
        results.AddRange(await TestHomeViewModel());

        // 测试 ShellViewModel
        results.AddRange(TestShellViewModel());

        // 测试 CollectionsViewModel
        results.AddRange(TestCollectionsViewModel());

        // 测试 ClassificationViewModel
        results.AddRange(await TestClassificationViewModel());

        // 测试 SettingViewModel
        results.AddRange(TestSettingViewModel());

        // 测试 EntryCollectionDetailViewModel
        results.AddRange(TestEntryCollectionDetailViewModel());

        // 测试 LabelCollectionViewModel
        results.AddRange(TestLabelCollectionViewModel());

        return results;
    }

    private static async Task<List<TestResult>> TestHomeViewModel()
    {
        var results = new List<TestResult>();
        var viewModel = new HomeViewModel();

        await TestCommand(results, "HomeViewModel", "RefreshCommand", async () =>
        {
            if (viewModel.RefreshCommand is IAsyncRelayCommand asyncCommand)
            {
                await asyncCommand.ExecuteAsync(null);
            }
            else
            {
                viewModel.RefreshCommand.Execute(null);
            }
        });

        return results;
    }

    private static List<TestResult> TestShellViewModel()
    {
        var results = new List<TestResult>();
        var viewModel = new ShellViewModel();

        TestCommand(results, "ShellViewModel", "NavClickCommand", () =>
        {
            viewModel.NavClickCommand.Execute(null);
        });

        return results;
    }

    private static List<TestResult> TestCollectionsViewModel()
    {
        var results = new List<TestResult>();
        var viewModel = new CollectionsViewModel();

        TestCommand(results, "CollectionsViewModel", "AddNewCollectionCommand", () =>
        {
            viewModel.AddNewCollectionCommand.Execute(null);
        });

        TestCommand(results, "CollectionsViewModel", "RefreshCommand", () =>
        {
            viewModel.RefreshCommand.Execute(null);
        });

        TestCommand(results, "CollectionsViewModel", "SuggestionChosenCommand", () =>
        {
            viewModel.SuggestionChosenCommand.Execute(null);
        });

        return results;
    }

    private static async Task<List<TestResult>> TestClassificationViewModel()
    {
        var results = new List<TestResult>();
        var viewModel = new ClassificationViewModel();

        TestCommand(results, "ClassificationViewModel", "RefreshCommand", () =>
        {
            viewModel.RefreshCommand.Execute(null);
        });

        TestCommand(results, "ClassificationViewModel", "ChangeShowTypeCommand", () =>
        {
            viewModel.ChangeShowTypeCommand.Execute("True");
            viewModel.ChangeShowTypeCommand.Execute("False");
        });

        await TestCommand(results, "ClassificationViewModel", "BannerDetailCommand", async () =>
        {
            if (viewModel.BannerDetailCommand is IAsyncRelayCommand asyncCommand)
            {
                await asyncCommand.ExecuteAsync(null);
            }
            else
            {
                viewModel.BannerDetailCommand.Execute(null);
            }
        });

        await TestCommand(results, "ClassificationViewModel", "LabelDetailCommand", async () =>
        {
            if (viewModel.LabelDetailCommand is IAsyncRelayCommand asyncCommand)
            {
                await asyncCommand.ExecuteAsync(null);
            }
            else
            {
                viewModel.LabelDetailCommand.Execute(null);
            }
        });

        return results;
    }

    private static List<TestResult> TestSettingViewModel()
    {
        var results = new List<TestResult>();
        var viewModel = new SettingViewModel();

        TestCommand(results, "SettingViewModel", "DbSelector_RefreshCommand", () =>
        {
            viewModel.DbSelector_RefreshCommand.Execute(null);
        });

        return results;
    }

    private static List<TestResult> TestEntryCollectionDetailViewModel()
    {
        var results = new List<TestResult>();
        var viewModel = new EntryCollectionDetailViewModel();

        TestCommand(results, "EntryCollectionDetailViewModel", "CancelEditCommand", () =>
        {
            viewModel.CancelEditCommand.Execute(null);
        });

        return results;
    }

    private static List<TestResult> TestLabelCollectionViewModel()
    {
        var results = new List<TestResult>();
        var viewModel = new LabelCollectionViewModel();

        TestCommand(results, "LabelCollectionViewModel", "ItemClickCommand", () =>
        {
            viewModel.ItemClickCommand.Execute(null);
        });

        return results;
    }

    private static async Task TestCommand(List<TestResult> results, string viewModelName, string commandName, Func<Task> action)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            await action();
            results.Add(new TestResult
            {
                ViewModelName = viewModelName,
                CommandName = commandName,
                Success = true,
                Duration = stopwatch.Elapsed
            });
        }
        catch (Exception ex)
        {
            results.Add(new TestResult
            {
                ViewModelName = viewModelName,
                CommandName = commandName,
                Success = false,
                ErrorMessage = ex.Message,
                Duration = stopwatch.Elapsed
            });
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    private static void TestCommand(List<TestResult> results, string viewModelName, string commandName, Action action)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            action();
            results.Add(new TestResult
            {
                ViewModelName = viewModelName,
                CommandName = commandName,
                Success = true,
                Duration = stopwatch.Elapsed
            });
        }
        catch (Exception ex)
        {
            results.Add(new TestResult
            {
                ViewModelName = viewModelName,
                CommandName = commandName,
                Success = false,
                ErrorMessage = ex.Message,
                Duration = stopwatch.Elapsed
            });
        }
        finally
        {
            stopwatch.Stop();
        }
    }
}
