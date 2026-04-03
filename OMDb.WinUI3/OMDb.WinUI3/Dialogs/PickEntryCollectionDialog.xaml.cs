using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Dialogs
{
    public sealed partial class PickEntryCollectionDialog : Page
    {
        public PickEntryCollectionDialog()
        {
            this.InitializeComponent();
            Init();
        }
        private async void Init()
        {
            ItemsListBox.ItemsSource = await Core.Services.EntryCollectionService.GetAllCollectionsAsync();
        }
        public static async Task<IList<Core.Models.EntryCollection>> ShowDialog()
        {
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = "选择片单";
            dialog.PrimaryButton.Content = "确定";
            dialog.CancelButton.Content = "取消";
            PickEntryCollectionDialog content = new PickEntryCollectionDialog();
            dialog.ContentFrame.Content = content;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                if(content.ItemsListBox.SelectedItems.Count > 0)
                {
                    List<Core.Models.EntryCollection> entryCollections = new List<Core.Models.EntryCollection>(content.ItemsListBox.SelectedItems.Count);
                    foreach(var item in content.ItemsListBox.SelectedItems)
                    {
                        entryCollections.Add(item as Core.Models.EntryCollection);
                    }
                    return entryCollections;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if(string.IsNullOrEmpty(sender.Text))
            {
                sender.ItemsSource = null;
            }
            else
            {
                var items = ItemsListBox.ItemsSource as List<Core.Models.EntryCollection>;
                if (items != null && items.Count != 0)
                {
                    sender.ItemsSource = items.Where(p => p.Title.Contains(sender.Text, StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            ItemsListBox.ScrollIntoView(args.SelectedItem);
            ItemsListBox.SelectedItem = args.SelectedItem;
            sender.Text = (args.SelectedItem as Core.Models.EntryCollection).Title;
        }
    }
}
