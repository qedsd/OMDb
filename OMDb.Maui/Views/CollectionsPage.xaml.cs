using Microsoft.Maui.Controls;
using OMDb.Maui.ViewModels;

namespace OMDb.Maui.Views;

public partial class CollectionsPage : ContentPage
{
    public CollectionsPage(CollectionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void NewCollectionButton_Clicked(object sender, EventArgs e)
    {
        var title = await DisplayPromptAsync("新建片单", "请输入片单标题：");
        if (!string.IsNullOrWhiteSpace(title))
        {
            var desc = await DisplayPromptAsync("片单描述", "请输入片单描述（可选）", initialValue: "");
            var vm = BindingContext as CollectionsViewModel;
            if (vm != null)
            {
                vm.NewCollectionTitle = title;
                vm.NewCollectionDesc = desc;
                vm.AddNewCollectionCommand.Execute(null);
            }
        }
    }
}
