using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Views
{
    public sealed partial class CollectionsPage : Page
    {
        public CollectionsPage()
        {
            this.InitializeComponent();
        }

        private void MenuFlyoutItem_Edit_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as CollectionsViewModel).EditCommand.Execute((sender as FrameworkElement).DataContext);
        }

        private void MenuFlyoutItem_Remove_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as CollectionsViewModel).RemoveCommand.Execute((sender as FrameworkElement).DataContext);
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            (DataContext as CollectionsViewModel).SuggestionChosenCommand.Execute(args.SelectedItem);
        }
    }
}
