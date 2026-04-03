using Microsoft.Maui.Controls;
using OMDb.Maui.ViewModels;

namespace OMDb.Maui.Views;

public partial class HomePage : ContentPage
{
    public static HomePage Current { get; private set; }
    public HomeViewModel VM { get; set; }

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        Current = this;
        VM = viewModel;
        BindingContext = VM;
        VM.Init();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // 页面出现时刷新数据
        // VM.Init();
    }
}
