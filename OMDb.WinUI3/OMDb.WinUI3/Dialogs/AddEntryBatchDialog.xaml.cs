using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Extensions;
using OMDb.WinUI3.Models;
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
    public sealed partial class AddEntryBatchDialog : Page
    {
        public AddEntryBatchViewModel VM=new AddEntryBatchViewModel();
        public AddEntryBatchDialog()
        {
            this.InitializeComponent();
            EntryDetail ed = new EntryDetail();
            ed.FullCoverImgPath = @"C:\Users\26712\Desktop\新建文件夹\微信图片_20230308211158.jpg";
            VM.EntryDetailCollection.Add(ed);
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
    }
}
