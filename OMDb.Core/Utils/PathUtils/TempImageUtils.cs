using OMDb.Core.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Utils.PathUtils
{
    public static class TempImageUtils
    {
        private const string _defaultImageName = @"temp_img.jpg";

        public static void CreateTempImage(string fileName, MemoryStream ms)
        {
            TempPathUtils.CreateTempFile(Path.Combine(Path.GetTempPath(), fileName.IsNullOrEmptyOrWhiteSpazeOrCountZero() ? fileName : _defaultImageName), ms);
        }
        public static void CreateTempImage(MemoryStream ms)
        {
            TempPathUtils.CreateTempFile(Path.Combine(Path.GetTempPath(), _defaultImageName), ms);
        }

        public static void CreateTempImage(string fileName)
        {
            TempPathUtils.CreateTempFile(Path.Combine(Path.GetTempPath(), fileName.IsNullOrEmptyOrWhiteSpazeOrCountZero() ? fileName : _defaultImageName), null);
        }
        public static void CreateTempImage()
        {
            TempPathUtils.CreateTempFile(Path.Combine(Path.GetTempPath(), _defaultImageName), null);
        }

        public static string GetDefaultTempImage()
        {
            var path_DefaultTempImage = Path.Combine(Path.GetTempPath(), _defaultImageName);
            return path_DefaultTempImage;
        }
    }
}
