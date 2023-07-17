using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;

namespace OMDb.WinUI3.Views
{
    public sealed partial class ShellPage : Page
    {
        public ViewModels.ShellViewModel VM { get; set; } = new ViewModels.ShellViewModel();
        public ShellPage()
        {
            this.InitializeComponent();
            Loaded += ShellPage_Loaded;
            Helpers.InfoHelper.InfoBar = InfoBar;
            Helpers.InfoHelper.WaitingGrid = WaitingGrid;
            Helpers.InfoHelper.WaitingProgressRing = WaitingProgressRing;
            Helpers.InfoHelper.DialogFrame = DialogFrame;
            VM.Init(ShellFrame);
        }

        private void ShellPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Helpers.WindowHelper.GetWindowForElement(this).SetTitleBar(CustomDragRegion);
        }

        private void ContentTabView_AddTabButtonClick(TabView sender, object args)
        {
            TabViewItem item = new TabViewItem()
            {
                Header = "Test",
                IsSelected = true,
            };
            sender.TabItems.Add(item);
        }

        private void ContentTabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((sender as TabView).SelectedItem == null)
            {
                VM.IsInTabView = false;
            }
            else
            {
                VM.IsInTabView = true;
            }
        }
    }
}
