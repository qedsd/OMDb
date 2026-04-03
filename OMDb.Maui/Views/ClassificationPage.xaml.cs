using Microsoft.Maui.Controls;
using OMDb.Maui.ViewModels;

namespace OMDb.Maui.Views;

public partial class ClassificationPage : ContentPage
{
    public ClassificationPage(ClassificationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
