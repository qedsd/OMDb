using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Dialogs
{
    public sealed partial class EditEntryDialog : Page
    {
        public ViewModels.EditEntryViewModel VM { get; set; } = new ViewModels.EditEntryViewModel();
        public EditEntryDialog(Core.Models.Entry entry)
        {
            if(entry == null)
            {
                VM.Entry = new Core.Models.Entry();
            }
            else
            {
                VM.Entry = entry.DepthClone<Core.Models.Entry>();
                Image_CoverImg.Source = new BitmapImage(new Uri(Converters.EntryCoverImgConverter.Convert(VM.Entry)));
            }
            
            this.InitializeComponent();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = ComboBox_Names.SelectedItem as Models.EntryName;
            VM.EntryNames.ForEach(p =>
            {
                if (p.IsDefault && p != selected)
                {
                    p.IsDefault = false;
                }
            });
        }

        public static async Task<Core.Models.Entry> ShowDialog(Core.Models.Entry entry = null)
        {
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = entry == null ? "新建词条" : "编辑词条";
            dialog.PrimaryButton.Content = "保存";
            dialog.CancelButton.Content = "取消";
            EditEntryDialog content = new EditEntryDialog(entry);
            dialog.ContentFrame.Content = content;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                {
                    entry.CopyFrom(content.VM.Entry);
                }
                return entry;
            }
            else
            {
                return null;
            }
        }
        
        private async void Button_Path_Click(object sender, RoutedEventArgs e)
        {
            var folder = await Helpers.PickHelper.PickFolderAsync();
            if(folder != null)
            {
                if(!folder.Path.StartsWith(Path.GetDirectoryName(VM.SelectedEnrtyStorage.StoragePath), StringComparison.OrdinalIgnoreCase))
                {
                    Helpers.DialogHelper.ShowError("请选择位于仓库下的路径");
                }
                else
                {
                    VM.SelectedEntryDicPath = folder.Path;
                }
            }
        }

        private async void Button_CoverImg_Click(object sender, RoutedEventArgs e)
        {
            var file = await Helpers.PickHelper.PickImgAsync();
            if(file != null)
            {
                VM.Entry.CoverImg = file.Path;
                Image_CoverImg.Source = new BitmapImage(new Uri(file.Path));
            }
        }
        /// <summary>
        /// 编辑词条名的时候同步修改仓库路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (VM.EntryName.IsDefault)
            {
                VM.SetEntryPath((sender as TextBox).Text);
            }
        }
    }
}
