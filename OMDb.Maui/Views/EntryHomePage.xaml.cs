using Microsoft.Maui.Controls;
using OMDb.Maui.ViewModels;

namespace OMDb.Maui.Views;

public partial class EntryHomePage : ContentPage
{
    public HomeViewModel VM { get; set; }

    public EntryHomePage() : this(new HomeViewModel()) { }

    public EntryHomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        VM = viewModel;
        BindingContext = VM;
        VM.Init();
    }
}
