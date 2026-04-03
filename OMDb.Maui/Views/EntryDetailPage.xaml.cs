using Microsoft.Maui.Controls;
using OMDb.Maui.ViewModels;

namespace OMDb.Maui.Views;

public partial class EntryDetailPage : ContentPage
{
    public EntryDetailPage(EntryDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
