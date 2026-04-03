using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using OMDb.Maui.ViewModels;

namespace OMDb.Maui.Tests;

/// <summary>
/// 异常处理测试 - 测试 ViewModel 在各种异常情况下的行为
/// </summary>
public class ExceptionHandlingTests
{
    [Fact]
    public async Task ViewModelInitialization_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        var exception = await Record.ExceptionAsync(async () =>
        {
            var homeViewModel = new HomeViewModel();
            homeViewModel.Init();
            await Task.Delay(100);

            var shellViewModel = new ShellViewModel();
            shellViewModel.Init();

            var collectionsViewModel = new CollectionsViewModel();
            // CollectionsViewModel doesn't have public Init

            var classificationViewModel = new ClassificationViewModel();
            // ClassificationViewModel.Init is protected

            var settingViewModel = new SettingViewModel();
            // SettingViewModel doesn't have public Init
        });

        Assert.Null(exception);
    }

    [Fact]
    public void NullCommandParameter_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new ShellViewModel();
        var command = viewModel.NavClickCommand;

        // Act & Assert
        var exception = Record.Exception(() => {
            command.Execute(null);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void InvalidCommandParameter_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new ShellViewModel();
        var command = viewModel.NavClickCommand;

        // Act & Assert
        var exception = Record.Exception(() => {
            command.Execute("invalid_string");
            command.Execute(123);
            command.Execute(new object());
        });

        Assert.Null(exception);
    }

    [Fact]
    public void EmptyStringCommandParameter_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new ShellViewModel();
        var command = viewModel.NavClickCommand;

        // Act & Assert
        var exception = Record.Exception(() => {
            command.Execute("");
        });

        Assert.Null(exception);
    }

    [Fact]
    public async Task MultipleRapidCommandExecutions_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new HomeViewModel();
        var command = viewModel.RefreshCommand;

        // Act & Assert
        var exception = await Record.ExceptionAsync(async () => {
            for (int i = 0; i < 5; i++)
            {
                if (command is IAsyncRelayCommand asyncCommand)
                {
                    await asyncCommand.ExecuteAsync(null);
                }
                else
                {
                    command.Execute(null);
                }
            }
        });

        Assert.Null(exception);
    }
}
