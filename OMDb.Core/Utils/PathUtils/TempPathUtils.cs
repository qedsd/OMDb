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
        private const string _defaultFileName = @"temp_file";

        private static List<string> lstFullTempFilePath = new List<string>();




        public static void CreateTempFile(string fileName, MemoryStream ms)
        {
            var newTempFile = Path.Combine(Path.GetTempPath(), fileName ?? _defaultFileName);
            //删除原临时文件
            if (lstFullTempFilePath.Contains(newTempFile))
            {
                DeleteTempFile(newTempFile);
            }
            else
            {
                lstFullTempFilePath.Add(newTempFile);
            }

            if (ms != null)
            {
                using (var fs = File.Create(newTempFile))
                    ms.WriteTo(fs);
            }


        }


        public static List<string> GetTempFileAll()
        {
            return lstFullTempFilePath;
        }

        public static string GetTempFile(string fileName)
        {
            var fullTempFilePath = Path.Combine(Path.GetTempPath(), fileName ?? _defaultFileName);
            if (!(lstFullTempFilePath.Contains(fullTempFilePath)))
                lstFullTempFilePath.Add(fullTempFilePath);
            return fullTempFilePath;
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
