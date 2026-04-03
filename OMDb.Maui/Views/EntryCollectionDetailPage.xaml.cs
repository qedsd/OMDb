using Microsoft.Maui.Controls;
using OMDb.Maui.ViewModels;

namespace OMDb.Maui.Views;

/// <summary>
/// 词条片单详情页面
/// 显示单个片单的详细信息和包含的词条列表
/// </summary>
public partial class EntryCollectionDetailPage : ContentPage
{
    /// <summary>
    /// 视图模型
    /// </summary>
    public EntryCollectionDetailViewModel VM { get; set; }

    /// <summary>
    /// 创建词条片单详情页
    /// </summary>
    /// <param name="viewModel">片单详情视图模型</param>
    public EntryCollectionDetailPage(EntryCollectionDetailViewModel viewModel)
    {
        InitializeComponent();
        VM = viewModel;
        BindingContext = VM;
    }

    /// <summary>
    /// 页面出现时初始化数据
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        // VM 初始化由构造函数处理
    }
}
