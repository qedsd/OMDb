using OMDb.Core.Models;
using OMDb.Core.Utils.StringUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class CommonService
    {
        public static string GetFileNameNoExFromFullFileName(this string src)
        {
            var result = src.SubString_PointToPoint(@"\", 1, false, false, @".", 1, false, false);
            return result;
        }


        public static string GetFileExFromFullFileName(this string src)
        {
            var result = src.SubString_PointToEnd(@".", 1, false, false);
            return result;
        }


        public static string Delete1stChar(this string src)
        {
            return src.Substring(1, src.Length-1);
        }

        public static string GetDefaultCoverName(this string src)
        {
            return src.Replace(Path.GetFileNameWithoutExtension(src), @"Cover");
        }

        public static string Remove(this string src, string str_remove)
        {
            return src.Replace(str_remove, string.Empty);
        }

        /// <summary>
        /// 根据地址自动获取封面
        /// </summary>
        /// <returns></returns>



    }
}
