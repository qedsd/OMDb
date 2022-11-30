using Microsoft.UI.Xaml;
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
        public static async Task<StorageFile> PickImgAsync(Window window = null)
        {
            List<string> ps = new List<string>()
            {
                ".jpg",
                ".jpeg",
                ".png"
            };
            return await PickFileAsync(ps, window);
        }
        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static async Task<StorageFile> PickFileAsync(string filter, Window window = null)
        {
            return await PickFileAsync(new List<string>() { filter }, window);
        }
        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static async Task<StorageFile> PickFileAsync(List<string> filter = null, Window window = null)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window ?? MainWindow.Instance);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            if (filter != null && filter.Count != 0)
            {
                filter.Where(p=>!string.IsNullOrWhiteSpace(p)).ToList().ForEach(p => openPicker.FileTypeFilter.Add(p));
            }
            if(openPicker.FileTypeFilter.Count == 0)
            {
                openPicker.FileTypeFilter.Add("*");
            }
            return await openPicker.PickSingleFileAsync();
        }
        /// <summary>
        /// 选择文件夹
        /// </summary>
        /// <returns></returns>
        public static async Task<StorageFolder> PickFolderAsync(Window window = null)
        {
            FolderPicker openPicker = new FolderPicker();
            openPicker.FileTypeFilter.Add("*");
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window ?? MainWindow.Instance);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            return await openPicker.PickSingleFolderAsync();
        }
    }
}
