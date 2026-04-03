using Microsoft.Maui.Controls;
using OMDb.Maui.ViewModels;

namespace OMDb.Maui.Views;

public partial class HomePage : ContentPage
{
    public static HomePage Current { get; private set; }
    public HomeViewModel VM { get; set; }

    public HomePage() : this(new HomeViewModel()) { }

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        Current = this;
        VM = viewModel;
        BindingContext = VM;
        VM.Init();

        // 添加双击手势
        ItemContentPanel.GestureRecognizers.Add(new TapGestureRecognizer
        {
            NumberOfTapsRequired = 2,
            Command = new Command(async () =>
            {
                await DisplayAlert("提示", "右键菜单功能请使用长按或右键点击", "确定");
            })
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // 页面出现时刷新数据
        // VM.Init();
    }

    private void OnScrollViewPanUpdated(object sender, EventArgs e)
    {
        // 处理滑动手势
    }
}
