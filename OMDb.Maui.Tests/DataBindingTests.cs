using OMDb.Maui.ViewModels;
using OMDb.Maui.Models;
using System.Collections.ObjectModel;

namespace OMDb.Maui.Tests;

/// <summary>
/// 数据绑定测试 - 测试数据绑定不会崩溃
/// </summary>
public class DataBindingTests
{
    [Fact]
    public void HomeViewModel_BindingContext_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new HomeViewModel();

        // Act & Assert
        var exception = Record.Exception(() => {
            viewModel.Init();
        });

        Assert.Null(exception);
    }

    [Fact]
    public void ShellViewModel_Properties_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new ShellViewModel();

        // Act & Assert
        var exception = Record.Exception(() => {
            viewModel.IsInTabView = true;
            viewModel.IsInTabView = false;
            viewModel.SelectedPage = "Test";
            viewModel.SetSelected(typeof(string));
        });

        Assert.Null(exception);
    }

    [Fact]
    public void CollectionsViewModel_Properties_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new CollectionsViewModel();

        // Act & Assert - 等待初始化完成
        var exception = Record.Exception(() => {
            viewModel.SuggestText = "test";
            viewModel.NewCollectionTitle = "New Collection";
            viewModel.NewCollectionDesc = "Description";
        });

        Assert.Null(exception);
    }

    [Fact]
    public void ClassificationViewModel_Properties_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new ClassificationViewModel();

        // Act & Assert
        var exception = Record.Exception(() => {
            viewModel.IsList = true;
            viewModel.IsList = false;
        });

        Assert.Null(exception);
    }

    [Fact]
    public void SettingViewModel_Properties_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new SettingViewModel();

        // Act & Assert
        var exception = Record.Exception(() => {
            viewModel.SelectedThemeIndex = 0;
            viewModel.PotPlayerPlaylistPath = "test";

            // 测试集合操作
            viewModel.ActiveHomeItems.Add(new HomeItemConfig("Test", typeof(string)));
            viewModel.InactiveHomeItems.Add(new HomeItemConfig("Test2", typeof(string)));
        });

        Assert.Null(exception);
    }

    [Fact]
    public void EntryCollectionDetailViewModel_Properties_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new EntryCollectionDetailViewModel();

        // Act & Assert
        var exception = Record.Exception(() => {
            viewModel.EditTitle = "Test Title";
            viewModel.EditDesc = "Test Description";
            viewModel.SortTypeIndex = 0;
            viewModel.SortWayIndex = 0;
        });

        Assert.Null(exception);
    }

    [Fact]
    public void LabelCollectionViewModel_Properties_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new LabelCollectionViewModel();

        // Act & Assert
        var exception = Record.Exception(() => {
            viewModel.Title = "Test";
            viewModel.Description = "Test Desc";
            viewModel.LabelId = "123";
            viewModel.SortTypeIndex = 0;
            viewModel.SortWayIndex = 0;
        });

        Assert.Null(exception);
    }

    [Fact]
    public void ObservableCollectionOperations_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new SettingViewModel();

        // Act
        var exception = Record.Exception(() => {
            // 测试集合的添加、删除操作
            var item = new HomeItemConfig("Test", typeof(string));
            viewModel.ActiveHomeItems.Add(item);
            viewModel.ActiveHomeItems.RemoveAt(0);
            viewModel.ActiveHomeItems.Clear();
        });

        // Assert
        Assert.Null(exception);
    }
}
