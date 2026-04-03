using OMDb.Maui.ViewModels;

namespace OMDb.Maui.Tests;

/// <summary>
/// ViewModel 初始化测试
/// </summary>
public class ViewModelInitializationTests
{
    [Fact]
    public void HomeViewModel_Init_ShouldNotThrow()
    {
        // Arrange & Act
        var viewModel = new HomeViewModel();
        var exception = Record.Exception(() => {
            viewModel.Init();
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void ClassificationViewModel_Init_ShouldNotThrow()
    {
        // Arrange & Act
        var viewModel = new ClassificationViewModel();
        var exception = Record.Exception(() => {
            viewModel.RefreshCommand.Execute(null);
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void CollectionsViewModel_Init_ShouldNotThrow()
    {
        // Arrange & Act
        var viewModel = new CollectionsViewModel();
        var exception = Record.Exception(() => {
            viewModel.RefreshCommand.Execute(null);
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void SettingViewModel_Init_ShouldNotThrow()
    {
        // Arrange & Act
        var viewModel = new SettingViewModel();
        var exception = Record.Exception(() => {
            viewModel.DbSelector_RefreshCommand.Execute(null);
        });

        // Assert
        Assert.Null(exception);
    }
}
