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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Dialogs
{
    public sealed partial class EditStorageDialog : Page
    {
        public Models.EnrtyStorage EnrtyStorage { get; set; }
        public EditStorageDialog(Models.EnrtyStorage enrtyStorage)
        {
            if (enrtyStorage == null)
            {
                EnrtyStorage = new Models.EnrtyStorage();
            }
            else
            {
                EnrtyStorage = enrtyStorage.DepthClone<Models.EnrtyStorage>();
            }
            this.InitializeComponent();
            if (EnrtyStorage.CoverImg != null)
                Image_CoverImg.Source = new BitmapImage(new Uri(EnrtyStorage.CoverImg));
        }
        public static async Task<Models.EnrtyStorage> ShowDialog(Models.EnrtyStorage enrtyStorage = null)
        {
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = enrtyStorage == null ? "新建仓库" : "编辑仓库";
            dialog.PrimaryButton.Content = "保存";
            dialog.CancelButton.Content = "取消";
            var content = new EditStorageDialog(enrtyStorage);
            dialog.ContentFrame.Content = content;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                //編輯 or 新增  
                if (enrtyStorage != null)
                    enrtyStorage.Update(content.EnrtyStorage);
                else
                    enrtyStorage = content.EnrtyStorage;
                //封面空 -> 設置默認封面
                if (enrtyStorage.CoverImg == null || enrtyStorage.CoverImg.Length <= 0)
                    enrtyStorage.CoverImg = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "DefaultCover.jpg");

                return enrtyStorage;
            }
            else
            {
                return null;
            }
        }


        private async void Button_CoverImg_Click(object sender, RoutedEventArgs e)
        {
            List<string> ps = new List<string>()
            {
                ".jpg",
                ".jpeg",
                ".png"
            };
            var file = await Helpers.PickHelper.PickFileAsync(ps);
            if (file != null)
            {
                EnrtyStorage.CoverImg = file.Path;
                var bi = new BitmapImage(new Uri(file.Path));
                Image_CoverImg.Source = bi;
            }
        }

        /*private async void Button_PickStorage_Click(object sender, RoutedEventArgs e)
        {
            var file = await Helpers.PickHelper.PickFileAsync(".db");
            if (file != null)
            {
                EnrtyStorage.StoragePath = file.Path;
            }
        }*/

        private async void Button_NewStoragePath_Click(object sender, RoutedEventArgs e)
        {
            var file = await Helpers.PickHelper.PickFolderAsync();
            if (file != null)
            {
                EnrtyStorage.StoragePath = file.Path;
                EnrtyStorage.StorageName = file.Name;
            }
        }
    }
}
