using Microsoft.Maui.Controls;
using OMDb.Maui.ViewModels;

namespace OMDb.Maui.Views;

/// <summary>
/// 标签集合页面
/// 显示单个标签集合下的所有词条
/// </summary>
public partial class LabelCollectionPage : ContentPage
{
    /// <summary>
    /// 视图模型
    /// </summary>
    public LabelCollectionViewModel VM { get; set; }

    /// <summary>
    /// 创建标签集合页
    /// </summary>
    /// <param name="viewModel">标签集合视图模型</param>
    public LabelCollectionPage(LabelCollectionViewModel viewModel)
    {
        InitializeComponent();
        VM = viewModel;
        BindingContext = VM;
    }

    /// <summary>
    /// 页面出现时刷新数据
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        // 页面出现时可执行刷新逻辑
    }
}
