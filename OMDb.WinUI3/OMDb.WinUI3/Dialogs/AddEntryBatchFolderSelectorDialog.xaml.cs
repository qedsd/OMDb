using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Extensions;
using OMDb.WinUI3.Helpers;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Services;
using OMDb.WinUI3.ViewModels;
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
    public sealed partial class AddEntryBatchFolderSelectorDialog : Page
    {
        public AddEntryBatchFolderSelectorDialog(string path)
        {
            this.InitializeComponent();
            var items=FileHelper.FindFolderItems(path);

        }

        public static async Task<string> ShowDialog(string path)
        {
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = "文件夹选择器";
            dialog.PrimaryButton.Content = "返回数据";
            dialog.CancelButton.Content = "取消";
            AddEntryBatchFolderSelectorDialog content = new AddEntryBatchFolderSelectorDialog(path);
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

        private async void SelectFolders_Click(object sender, RoutedEventArgs e)
        {
            var paths = await Helpers.PickHelper.PickFolderAsync();
            /*foreach (var item in collection)
            {

            }*/
            EntryDetail ed = new EntryDetail();
            ed.FullCoverImgPath = CommonService.GetCoverByPath(paths.Path );
        }
    }
}
