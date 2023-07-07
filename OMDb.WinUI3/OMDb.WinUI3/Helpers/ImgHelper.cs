using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using OMDb.Core.Const;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Helpers
{
    public static class ImgHelper
    {

        public static async Task<BitmapImage> CreateBitmapImageAsync(MemoryStream stream)
        {
            try
            {
                var bitmapImage = new BitmapImage();
                stream.Seek(0, SeekOrigin.Begin);
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
