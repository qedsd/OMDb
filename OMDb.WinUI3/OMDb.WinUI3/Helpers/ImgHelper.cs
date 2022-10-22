using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
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
    }
}
