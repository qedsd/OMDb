using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using OMDb.Core.Extensions;
using OMDb.WinUI3.Helpers;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Services;
using OMDb.WinUI3.ViewModels;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OMDb.WinUI3.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddEntryBatchDialog : Page
    {
        public AddEntryBatchViewModel VM=new AddEntryBatchViewModel();
        public AddEntryBatchDialog()
        {
            this.InitializeComponent();
        }

        public static async Task<string> ShowDialog()
        {
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = "词条批量新增";
            dialog.PrimaryButton.Content = "确认";
            dialog.CancelButton.Content = "取消";
            AddEntryBatchDialog content = new AddEntryBatchDialog();
            dialog.ContentFrame.Content = content;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                return null;
            }
            else
            {
                return null;
            }
        }

        private void SelectFolders_Click(object sender, RoutedEventArgs e)
        {
            var path = @"D:\Data\Data\SRC.AV-Nippon";
            this.ei.treeFolders.ItemsSource= FileHelper.FindFolderItems(path);
            this.flyGrid.Visibility= Visibility.Visible;
        }

        private void ReturnFolder_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in (List<string>)this.eic.ItemsSource)
            {
                EntryDetail ed = new EntryDetail();
                ed.FullCoverImgPath = CommonService.GetCoverByPath(item);
                VM.EntryDetailCollection.Add(ed);
            }
            this.ei.treeFolders.SelectedItems.Clear();
            this.btn_FolderPicker.Flyout.Hide();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.ei.treeFolders.SelectedItems.Clear();
            this.btn_FolderPicker.Flyout.Hide();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            List<string> lstEic= new List<string>();
            foreach (var item in this.ei.treeFolders.SelectedItems)
            {
                var ei = (ExplorerItem)item;
                lstEic.Add(ei.FullName);

            }
            this.eic.ItemsSource = lstEic;
            this.ei.treeFolders.SelectedItems.Clear();
            
        }
    }
}
