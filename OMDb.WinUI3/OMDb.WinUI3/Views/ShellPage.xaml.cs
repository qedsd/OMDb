using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;

namespace OMDb.WinUI3.Views
{
    public sealed partial class ShellPage : Page
    {
        public ViewModels.ShellViewModel ViewModel { get; set; } = new ViewModels.ShellViewModel();
        public ShellPage()
        {
            this.InitializeComponent();
            Helpers.InfoHelper.InfoBar = InfoBar;
            Helpers.InfoHelper.WaitingGrid = WaitingGrid;
            Helpers.InfoHelper.WaitingProgressRing = WaitingProgressRing;
            Helpers.InfoHelper.DialogFrame = DialogFrame;
            ViewModel.Init(shellFrame);
        }

        private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.GoBack();
        }
    }
}
