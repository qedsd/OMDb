using System;
using System.Threading.Tasks;
using OMDb.Maui.ViewModels;

namespace OMDb.Maui.Tests;

/// <summary>
/// 全局异常捕获测试工具
/// </summary>
public static class ExceptionHandlingHelper
{
    /// <summary>
    /// 捕获并记录异步操作中的异常
    /// </summary>
    public static async Task<(bool Success, Exception? Exception)> TryExecuteAsync(Func<Task> action)
    {
        try
        {
            await action();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex);
        }
    }

    /// <summary>
    /// 捕获并记录同步操作中的异常
    /// </summary>
    public static (bool Success, Exception? Exception) TryExecute(Action action)
    {
        try
        {
            action();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex);
        }
    }
}

/// <summary>
/// 异常捕获测试 - 测试各种边界条件
/// </summary>
public class ExceptionHandlingTests
{
    [Fact]
    public void NullCommandParameter_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new HomeViewModel();

        // Act
        var (success, exception) = ExceptionHandlingHelper.TryExecute(() => {
            viewModel.RefreshCommand.Execute(null);
        });

        // Assert - 应该不抛出异常或者优雅地处理 null
        Assert.True(success || exception == null, $"Command should handle null parameter gracefully: {exception?.Message}");
    }

    [Fact]
    public void EmptyStringCommandParameter_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new ClassificationViewModel();

        // Act
        var (success, exception) = ExceptionHandlingHelper.TryExecute(() => {
            viewModel.ChangeShowTypeCommand.Execute("");
        });

        // Assert
        Assert.True(success || exception == null, $"Command should handle empty string gracefully: {exception?.Message}");
    }

    [Fact]
    public void InvalidCommandParameter_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new ClassificationViewModel();

        // Act
        var (success, exception) = ExceptionHandlingHelper.TryExecute(() => {
            viewModel.ChangeShowTypeCommand.Execute("InvalidValue");
        });

        // Assert
        Assert.True(success || exception == null, $"Command should handle invalid parameter gracefully: {exception?.Message}");
    }

    [Fact]
    public void MultipleRapidCommandExecutions_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new HomeViewModel();

        // Act
        var (success, exception) = ExceptionHandlingHelper.TryExecute(() => {
            // 快速多次执行命令
            for (int i = 0; i < 10; i++)
            {
                viewModel.RefreshCommand.Execute(null);
            }
        });

        // Assert
        Assert.True(success || exception == null, $"Rapid command execution should not crash: {exception?.Message}");
    }

    [Fact]
    public async Task ViewModelInitialization_ShouldNotThrow()
    {
        // Arrange & Act
        var (success, exception) = await ExceptionHandlingHelper.TryExecuteAsync(async () => {
            var vm1 = new HomeViewModel();
            var vm2 = new ClassificationViewModel();
            var vm3 = new CollectionsViewModel();
            var vm4 = new SettingViewModel();

            // 等待片刻让异步初始化完成
            await Task.Delay(100);
        });

        // Assert
        Assert.True(success, $"ViewModel initialization should not throw: {exception?.Message}");
    }
}
