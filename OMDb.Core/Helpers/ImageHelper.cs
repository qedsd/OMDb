using ImageMagick;
using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OMDb.Core.Helpers
{
    public static class ImageHelper
    {
        public static void GetImageSize(string path, out double Wpx, out double Hpx)
        {
            try
            {
                MagickImageInfo image = new MagickImageInfo(path);
                int w = image.Width;//宽
                int h = image.Height;//高
                Wpx = image.Density.X;//分辨率
                Hpx = image.Density.Y;//分辨率
                if (image.Density.Units == DensityUnit.PixelsPerCentimeter)//判断分辨率单位
                {
                    Wpx *= 2.54;
                    Hpx *= 2.54;
                }
            }
            catch
            {
                Wpx = 0; Hpx = 0;
            }
        }
        public static ImageInfo GetImageInfo(string path)
        {
            try
            {
                ImageInfo imageInfo = new ImageInfo();
                MagickImageInfo image = new MagickImageInfo(path);
                int w = image.Width;//宽
                int h = image.Height;//高
                var wpx = image.Density.X;//分辨率
                var hpx = image.Density.Y;//分辨率
                if (image.Density.Units == DensityUnit.PixelsPerCentimeter)//判断分辨率单位
                {
                    wpx *= 2.54;
                    hpx *= 2.54;
                }
                imageInfo.Width = wpx;
                imageInfo.Height = hpx;
                imageInfo.FullPath = path;
                long length = new FileInfo(path).Length;
                imageInfo.Length = length;
                return imageInfo;
            }
            catch
            {
                return null;
            }
        }
    }
}
