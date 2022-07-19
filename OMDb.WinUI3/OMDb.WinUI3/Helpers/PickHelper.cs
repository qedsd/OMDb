using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace OMDb.WinUI3.Helpers
{
    public static class PickHelper
    {
        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static async Task<StorageFile> PickImgAsync()
        {
            List<string> ps = new List<string>()
            {
                ".jpg",
                ".jpeg",
                ".png"
            };
            return await PickFileAsync(ps);
        }
        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static async Task<StorageFile> PickFileAsync(string filter)
        {
            return await PickFileAsync(new List<string>() { filter });
        }
        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static async Task<StorageFile> PickFileAsync(List<string> filter = null)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow.Instance);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            if (filter != null)
            {
                filter.ForEach(p => openPicker.FileTypeFilter.Add(p));
            }
            else
            {
                openPicker.FileTypeFilter.Add("*");
            }
            return await openPicker.PickSingleFileAsync();
        }
        /// <summary>
        /// 选择文件夹
        /// </summary>
        /// <returns></returns>
        public static async Task<StorageFolder> PickFolderAsync()
        {
            FolderPicker openPicker = new FolderPicker();
            openPicker.FileTypeFilter.Add("*");
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow.Instance);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            return await openPicker.PickSingleFolderAsync();
        }
    }
}
