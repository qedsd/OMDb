using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OMDb.Core.Utils
{
    public class CharCheckUtil
    {
        private static Regex RegPhone = new Regex("^[0-9]+[-]?[0-9]+[-]?[0-9]$");   //电话
        private static Regex RegEmail = new Regex("^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);//w 英文字母或数字的字符串，和 [a-zA-Z0-9] 语法一样 
        private static Regex RegCHZN = new Regex("[\u4e00-\u9fa5]", RegexOptions.Compiled | RegexOptions.IgnoreCase);               //中文
        private static Regex RegUrl = new Regex(@"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        public static bool IsUrl(string inputData)
        {
            var isUrl=RegUrl.IsMatch(inputData);
            return isUrl;
        }
        public static bool IsEmail(string inputData)
        {
            var isEmail = RegEmail.IsMatch(inputData);
            return isEmail;
        }
        public static bool IsPhone(string inputData)
        {
            var isPhone = RegPhone.IsMatch(inputData);
            return isPhone;
        }

    }
}
