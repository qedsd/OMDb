using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OMDb.WinUI3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellPage : Page
    {
        public ViewModels.ShellViewModel ViewModel { get; set; } = new ViewModels.ShellViewModel();
        public ShellPage()
        {
            this.InitializeComponent();
            ViewModel.Init(shellFrame);
            //var tiWtleBar = Microsoft.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            //tiWtleBar.BackgroundColor = Colors.Transparent;
            //tiWtleBar.ButtonBackgroundColor = Colors.Transparent;
            //tiWtleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            //Microsoft.UI.Xaml.Window.Current.SetTitleBar(AppTitleBar);
        }
    }
}
