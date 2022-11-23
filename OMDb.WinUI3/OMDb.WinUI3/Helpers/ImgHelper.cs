using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Helpers
{
    public static class ImgHelper
    {
        private static List<string> SupportImgs = new List<string>()
        {
            "jpeg",
            "jpg",
            "png",
            "bmp",
            "gif",
            "tiff",
            "ico",
            "svg"
        };
        public static bool IsSupportImg(string file)
        {
            if(!string.IsNullOrEmpty(file))
            {
                var ext = System.IO.Path.GetExtension(file).Replace(".","");
                return SupportImgs.Contains(ext.ToLower());
            }
            else
            {
                return false;
            }
        }

        public static async Task<BitmapImage> CreateBitmapImageAsync(MemoryStream stream)
        {
            try
            {
                var bitmapImage = new BitmapImage();
                stream.Seek(0,SeekOrigin.Begin);
                await bitmapImage.SetSourceAsync(stream.AsRandomAccessStream());
                return bitmapImage;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static BitmapImage CreateBitmapImage(MemoryStream stream)
        {
            try
            {
                var bitmapImage = new BitmapImage();
                stream.Seek(0, SeekOrigin.Begin);
                bitmapImage.SetSource(stream.AsRandomAccessStream());
                return bitmapImage;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
