using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using OMDb.Maui.ViewModels;

namespace OMDb.Maui.Tests;

/// <summary>
/// ViewModel 命令测试 - 测试所有 ICommand 不会崩溃
/// </summary>
public class ViewModelCommandTests
{
    #region HomeViewModel Tests

    [Fact]
    public async Task HomeViewModel_RefreshCommand_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new HomeViewModel();
        var command = viewModel.RefreshCommand;

        // Act & Assert
        var exception = await Record.ExceptionAsync(async () => {
            if (command is IRelayCommand relayCommand)
            {
                if (relayCommand is IAsyncRelayCommand asyncCommand)
                {
                    await asyncCommand.ExecuteAsync(null);
                }
                else
                {
                    relayCommand.Execute(null);
                }
            }
        });

        // Assert
        Assert.Null(exception);
    }

    #endregion

    #region ShellViewModel Tests

    [Fact]
    public void ShellViewModel_NavClickCommand_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new ShellViewModel();
        var command = viewModel.NavClickCommand;

        // Act & Assert
        var exception = Record.Exception(() => {
            command.Execute(null);
        });

        // Assert
        Assert.Null(exception);
    }

    #endregion

    #region CollectionsViewModel Tests

    [Fact]
    public void CollectionsViewModel_AddNewCollectionCommand_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new CollectionsViewModel();
        var command = viewModel.AddNewCollectionCommand;

        // Act & Assert
        var exception = Record.Exception(() => {
            command.Execute(null);
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void CollectionsViewModel_RefreshCommand_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new CollectionsViewModel();
        var command = viewModel.RefreshCommand;

        // Act & Assert
        var exception = Record.Exception(() => {
            command.Execute(null);
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void CollectionsViewModel_SuggestionChosenCommand_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new CollectionsViewModel();
        var command = viewModel.SuggestionChosenCommand;

        // Act & Assert
        var exception = Record.Exception(() => {
            command.Execute(null);
        });

        // Assert
        Assert.Null(exception);
    }

    #endregion

    #region ClassificationViewModel Tests

    [Fact]
    public void ClassificationViewModel_RefreshCommand_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new ClassificationViewModel();
        var command = viewModel.RefreshCommand;

        // Act & Assert
        var exception = Record.Exception(() => {
            command.Execute(null);
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void ClassificationViewModel_ChangeShowTypeCommand_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new ClassificationViewModel();
        var command = viewModel.ChangeShowTypeCommand;

        // Act & Assert
        var exception = Record.Exception(() => {
            command.Execute("True");
            command.Execute("False");
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void ClassificationViewModel_BannerDetailCommand_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new ClassificationViewModel();
        var command = viewModel.BannerDetailCommand;

        // Act & Assert
        var exception = Record.Exception(() => {
            command.Execute(null);
        });

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void ClassificationViewModel_LabelDetailCommand_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new ClassificationViewModel();
        var command = viewModel.LabelDetailCommand;

        // Act & Assert
        var exception = Record.Exception(() => {
            command.Execute(null);
        });

        // Assert
        Assert.Null(exception);
    }

    #endregion

    #region SettingViewModel Tests

    [Fact]
    public void SettingViewModel_DbSelector_RefreshCommand_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new SettingViewModel();
        var command = viewModel.DbSelector_RefreshCommand;

        // Act & Assert
        var exception = Record.Exception(() => {
            command.Execute(null);
        });

        // Assert
        Assert.Null(exception);
    }

    #endregion

    #region EntryCollectionDetailViewModel Tests

    [Fact]
    public void EntryCollectionDetailViewModel_CancelEditCommand_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new EntryCollectionDetailViewModel();
        var command = viewModel.CancelEditCommand;

        // Act & Assert
        var exception = Record.Exception(() => {
            command.Execute(null);
        });

        // Assert
        Assert.Null(exception);
    }

    #endregion

    #region LabelCollectionViewModel Tests

    [Fact]
    public void LabelCollectionViewModel_ItemClickCommand_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new LabelCollectionViewModel();
        var command = viewModel.ItemClickCommand;

        // Act & Assert
        var exception = Record.Exception(() => {
            command.Execute(null);
        });

        // Assert
        Assert.Null(exception);
    }

    #endregion
}
