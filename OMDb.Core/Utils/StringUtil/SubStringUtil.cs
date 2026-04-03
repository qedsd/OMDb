using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OMDb.Core.Utils.StringUtil
{
    public static class SubStringUtil
    {
        public static int NthIndexOf(this string target, string value, int n)
        {
            string pattern = "((" + Regex.Escape(value) + ").*?){" + n + "}";
            Match m = Regex.Match(target, pattern);

            if (m.Success)
                return m.Groups[2].Captures[n - 1].Index;
            else
                return -1;
        }


        public static int NthLastIndexOf(this string target, string value, int n)
        {
            var target_charArray = target.Reverse();
            var target_reverse = new string(target_charArray.ToArray());

            string pattern = "((" + Regex.Escape(value) + ").*?){" + n + "}";
            Match m = Regex.Match(target_reverse, pattern);

            if (m.Success)
                return (target.Length - 1) - (m.Groups[2].Captures[n - 1].Index);
            else
                return -1;
        }


        private static int NIndex(this string target, string value, int n, bool isForward)
        {
            if (isForward)
            {
                return target.NthIndexOf(value, n);
            }
            else
            {
                return target.NthLastIndexOf(value, n);
            }
        }


        public static string SubStringByIndex(this string sourceStr, int startIndex, int endIndex)
        {
            return sourceStr.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// 字符串初始位置截取到指定终止位置
        /// </summary>
        /// <param name="sourceStr">原始字符串</param>
        /// <param name="end_str">指定终止字符串</param>
        /// <param name="end_str_index">指定终止字符串Index</param>
        /// <param name="end_str_index_isOrder">指定终止字符串Index顺倒数</param>
        /// <param name="end_str_index_isContain">返回字符串是否包含该指定终止字符串</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string SubString_StartToPoint(this string sourceStr, string end_str, int end_str_index, bool end_str_index_isOrder, bool end_str_index_isContain)
        {
            //终止字符串为空，返回源字符串
            if (end_str == string.Empty)
                return sourceStr;

            //源字符串不包含终止字符串，报错
            if (!sourceStr.Contains(end_str))
                throw new Exception(string.Format($@"Source string does not exist:[{end_str}]"));

            //源字符串找到终止字符串位置，找不到报错
            var end_str_position = sourceStr.NIndex(end_str, end_str_index, end_str_index_isOrder);
            if (end_str_position == -1)
                throw new Exception(string.Format($@"{end_str}[{end_str_index}] out of index!"));


            if (end_str_index_isContain)
                return sourceStr.SubStringByIndex(0, end_str_position);
            else
                return sourceStr.SubStringByIndex(0, end_str_position - end_str.Length);
        }


        /// <summary>
        /// 字符串指定起始位置截取到终点
        /// </summary>
        /// <param name="sourceStr">原始字符串</param>
        /// <param name="start_str">指定起始字符串</param>
        /// <param name="start_str_index">指定起始字符串Index</param>
        /// <param name="start_str_index_isOrder">指定起始字符串Index顺倒数</param>
        /// <param name="start_str_index_isContain">返回字符串是否包含该指定起始字符串</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string SubString_PointToEnd(this string sourceStr, string start_str, int start_str_index, bool start_str_index_isOrder, bool start_str_index_isContain)
        {
            //起始字符串为空，返回源字符串
            if (start_str == string.Empty)
                return sourceStr;

            //源字符串不包含起始字符串，报错
            if (!sourceStr.Contains(start_str))
                throw new Exception(string.Format($@"Source string does not exist:[{start_str}]"));

            //源字符串找到起始字符串位置，找不到报错
            var start_str_position = sourceStr.NIndex(start_str, start_str_index, start_str_index_isOrder);
            if (start_str_position == -1)
                throw new Exception(string.Format($@"{start_str}[{start_str_index}] out of index!"));


            if (start_str_index_isContain)
                return sourceStr.SubStringByIndex(start_str_position, sourceStr.Length);
            else
                return sourceStr.SubStringByIndex(start_str_position + start_str.Length, sourceStr.Length);

        }


        /// <summary>
        /// 字符串指定起始位置截取到指定终止位置
        /// </summary>
        /// <param name="sourceStr">原始字符串</param>
        /// <param name="start_str">指定起始字符串</param>
        /// <param name="start_str_index">指定起始字符串Index</param>
        /// <param name="start_str_index_isOrder">指定起始字符串Index顺倒数</param>
        /// <param name="start_str_index_isContain">返回字符串是否包含该指定起始字符串</param>
        /// <param name="end_str">指定终止字符串</param>
        /// <param name="end_str_index">指定终止字符串Index</param>
        /// <param name="end_str_index_isOrder">指定终止字符串Index顺倒数</param>
        /// <param name="end_str_index_isContain">返回字符串是否包含该指定终止字符串</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string SubString_PointToPoint(this string sourceStr,
            string start_str, int start_str_index, bool start_str_index_isOrder, bool start_str_index_isContain,
            string end_str, int end_str_index, bool end_str_index_isOrder, bool end_str_index_isContain)
        {
            //起始字符串为空
            if (start_str == string.Empty)
                return sourceStr.SubString_StartToPoint(end_str, end_str_index, end_str_index_isOrder, end_str_index_isContain);
            //终止字符串为空
            if (end_str == string.Empty)
                return sourceStr.SubString_PointToEnd(end_str, end_str_index, end_str_index_isOrder, end_str_index_isContain);

            //源字符串不包含起始字符串，报错
            if (!sourceStr.Contains(start_str))
                throw new Exception(string.Format($@"Source string does not exist:[{start_str}]"));

            //源字符串找到起始字符串位置，找不到报错
            var start_str_position = sourceStr.NIndex(start_str, start_str_index, start_str_index_isOrder);
            if (start_str_position == -1)
                throw new Exception(string.Format($@"{start_str}[{start_str_index}] out of index!"));


            //源字符串不包含终止字符串，报错
            if (!sourceStr.Contains(end_str))
                throw new Exception(string.Format($@"Source string does not exist:[{end_str}]"));

            //源字符串找到终止字符串位置，找不到报错
            var end_str_position = sourceStr.NIndex(end_str, end_str_index, end_str_index_isOrder);
            if (end_str_position == -1)
                throw new Exception(string.Format($@"{end_str}[{end_str_index}] out of index!"));


            if (start_str_index_isContain && end_str_index_isContain)
                return sourceStr.SubStringByIndex(start_str_position, end_str_position + end_str.Length);
            else if (start_str_index_isContain && !end_str_index_isContain)
                return sourceStr.SubStringByIndex(start_str_position, end_str_position);
            else if (end_str_index_isContain && !start_str_index_isContain)
                return sourceStr.SubStringByIndex(start_str_position + start_str.Length, end_str_position + 1);
            else
                return sourceStr.SubStringByIndex(start_str_position + start_str.Length, end_str_position + 1 - end_str.Length);
        }
    }
}
