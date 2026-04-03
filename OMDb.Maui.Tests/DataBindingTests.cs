using System;
using System.Collections.ObjectModel;
using OMDb.Maui.ViewModels;

namespace OMDb.Maui.Tests;

/// <summary>
/// 数据绑定测试 - 测试 ViewModel 属性和绑定
/// </summary>
public class DataBindingTests
{
    [Fact]
    public void ShellViewModel_Properties_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new ShellViewModel();

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            var isInTabView = viewModel.IsInTabView;
            viewModel.IsInTabView = true;
            viewModel.IsInTabView = false;

            var selectedPage = viewModel.SelectedPage;
            viewModel.SelectedPage = "TestPage";
            viewModel.SelectedPage = null;
        });

        Assert.Null(exception);
    }

    [Fact]
    public void HomeViewModel_Properties_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new HomeViewModel();

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            // 测试可访问性
            var refreshCommand = viewModel.RefreshCommand;
            Assert.NotNull(refreshCommand);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void CollectionsViewModel_Properties_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new CollectionsViewModel();

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            var addCommand = viewModel.AddNewCollectionCommand;
            var refreshCommand = viewModel.RefreshCommand;
            var suggestionCommand = viewModel.SuggestionChosenCommand;

            Assert.NotNull(addCommand);
            Assert.NotNull(refreshCommand);
            Assert.NotNull(suggestionCommand);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void ClassificationViewModel_Properties_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new ClassificationViewModel();

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            var refreshCommand = viewModel.RefreshCommand;
            var changeShowTypeCommand = viewModel.ChangeShowTypeCommand;
            var bannerDetailCommand = viewModel.BannerDetailCommand;
            var labelDetailCommand = viewModel.LabelDetailCommand;

            Assert.NotNull(refreshCommand);
            Assert.NotNull(changeShowTypeCommand);
            Assert.NotNull(bannerDetailCommand);
            Assert.NotNull(labelDetailCommand);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void SettingViewModel_Properties_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new SettingViewModel();

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            var dbSelectorRefreshCommand = viewModel.DbSelector_RefreshCommand;
            Assert.NotNull(dbSelectorRefreshCommand);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void EntryCollectionDetailViewModel_Properties_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new EntryCollectionDetailViewModel();

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            var cancelEditCommand = viewModel.CancelEditCommand;
            Assert.NotNull(cancelEditCommand);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void LabelCollectionViewModel_Properties_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new LabelCollectionViewModel();

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            var itemClickCommand = viewModel.ItemClickCommand;
            Assert.NotNull(itemClickCommand);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void ObservableCollectionOperations_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new CollectionsViewModel();

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            // 测试集合属性访问
            var items = viewModel.EntryCollections;
            Assert.NotNull(items);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void HomeViewModel_BindingContext_ShouldNotThrow()
    {
        // Arrange
        var viewModel = new HomeViewModel();

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            viewModel.Init();
        });

        Assert.Null(exception);
    }
}
