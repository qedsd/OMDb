using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Helpers
{
    public static class TempFileHelper
    {
        private const string _imgName = @"temp.jpg";

        public static string fullTempImgPath;

        public static void CreateTempImg(string name=null)
        {
            using(File.Create(Path.Combine(Path.GetTempPath(), name!=null?name:_imgName)));
            fullTempImgPath = Path.Combine(Path.GetTempPath(), name != null ? name : _imgName);
        }


        private static void DeleteTempImg()
        {
            if (fullTempImgPath==null||!File.Exists(fullTempImgPath)) return;
            File.Delete(fullTempImgPath);
            fullTempImgPath = null;
        }
    }
}

