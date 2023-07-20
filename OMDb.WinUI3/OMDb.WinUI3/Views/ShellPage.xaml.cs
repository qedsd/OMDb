using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OMDb.WinUI3.Interfaces;
using OMDb.WinUI3.Services;

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
            TabViewService.Init(ContentTabView);
        }

        private void ShellPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Helpers.WindowHelper.GetWindowForElement(this).SetTitleBar(CustomDragRegion);
        }

        private void ContentTabView_AddTabButtonClick(TabView sender, object args)
        {
            TabViewService.AddItem(new TestTabViewPage());
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

        private void ContentTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            ((args.Item as TabViewItem).Content as ITabViewItemPage)?.Close();
            TabViewService.ReomveItem(args.Item as TabViewItem);
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ContentTabView.SelectedItem = null;
            VM.NavClickCommand.Execute((e.ClickedItem as FrameworkElement).Parent as ListViewItem);
            MenuFlyout.Hide();
        }

        private void Button_BackToMenuPage_Click(object sender, RoutedEventArgs e)
        {
            ContentTabView.SelectedItem = null;
        }

        private void TabViewLeftPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TabStripHeaderGrid.Width = (sender as FrameworkElement).ActualWidth;
        }
    }
}
