using NPOI.HPSF;
using OMDb.Core.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Utils.PathUtils
{
    public static class TempPathUtils
    {
        private const string _defaultImageName = @"temp_img.jpg";
        private const string _defaultFileName = @"temp_file";

        private static List<string> lstFullTempFilePath = new List<string>();

        public static void CreateTempImage(string fileName , Stream stream )
        {
            CreateTempFile(Path.Combine(Path.GetTempPath(), fileName.IsNullOrEmptyOrWhiteSpazeOrCountZero() ? fileName : _defaultImageName), stream);
        }
        public static void CreateTempImage(Stream stream)
        {
            CreateTempFile(Path.Combine(Path.GetTempPath(), _defaultImageName), stream);
        }
        public static void CreateTempImage(string fileName)
        {
            CreateTempFile(Path.Combine(Path.GetTempPath(), fileName.IsNullOrEmptyOrWhiteSpazeOrCountZero() ? fileName : _defaultImageName), null);
        }
        public static void CreateTempImage()
        {
            CreateTempFile(Path.Combine(Path.GetTempPath(),  _defaultImageName), null);
        }

        public static void CreateTempFile(string? fileName = null, Stream? stream = null)
        {
            var newTempFile = Path.Combine(Path.GetTempPath(), fileName ?? _defaultFileName);
            //删除原临时文件
            if (lstFullTempFilePath.Contains(newTempFile))
                DeleteTempFile(newTempFile);
            using (var fs = File.Create(newTempFile))
                stream?.CopyTo(fs);
        }


        public static List<string> GetTempFileAll()
        {
            return lstFullTempFilePath;
        }

        public static string GetTempFile(string fileName)
        {
            var fullTempFilePath = Path.Combine(Path.GetTempPath(), fileName ?? _defaultFileName);
            if (lstFullTempFilePath.Contains(fullTempFilePath))
                return fullTempFilePath;
            else
                return string.Empty;
        }

        public static string GetDefaultTempImage()
        {
            var path_DefaultTempImage = Path.Combine(Path.GetTempPath(), _defaultImageName);
            if (lstFullTempFilePath.Contains(path_DefaultTempImage))
                return path_DefaultTempImage;
            else
                return string.Empty;
        }


        public static void DeleteTempFileAll()
        {
            if (lstFullTempFilePath.IsNullOrEmptyOrWhiteSpazeOrCountZero())
                return;
            foreach (var file in lstFullTempFilePath)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                    lstFullTempFilePath.Remove(file);
                }
            }
        }


        public static void DeleteTempFile(string file)
        {
            if (File.Exists(file))
                File.Delete(file);
        }
        public static void DeleteTempFile(List<string> files)
        {
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                    lstFullTempFilePath.Remove(file);
                }
            }
        }
    }
}
