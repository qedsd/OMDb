using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OMDb.Core.Utils
{
    public static class ListUtil
    {
        public static DataTable ConbertToDataTable<T>(ICollection<T> elementList)
        {
            DataTable dtDataSource = new DataTable();
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo item in properties)
            {
                dtDataSource.Columns.Add(item.Name, item.PropertyType);
            }
            foreach (T elementItem in elementList)
            {
                DataRow drItem = dtDataSource.NewRow();
                foreach (var item in properties)
                {
                    drItem[item.Name] = item.GetValue(elementItem, null);
                }
                dtDataSource.Rows.Add(drItem);
            }
            return dtDataSource;
        }

        public static ICollection<T> ConvertFromDataTable<T>(DataTable dtDataSource)
            where T : new()
        {
            ICollection<T> elementList=new List<T>();
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (DataRow drItem in dtDataSource.Rows)
            {
                T item = new T();
                foreach (var property in properties)
                {
                    property.SetValue(item, drItem[property.Name], null);
                }
                elementList.Add(item);
            }
            return elementList;
        }

        /// <summary>
        /// 检测集合是否为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this IEnumerable<T> col)
        {
            if (null == col)
            {
                return true;
            }
            ICollection<T> temp= col as ICollection<T>;
            if (null !=temp ) 
            {
                return temp.Count == 0;
            }
            ICollection temp2=col as ICollection;
            if (null != temp2)
            {
                return temp2.Count == 0;
            }
            return !col.Any();
        }

        /// <summary>
        /// 获取双引号或者花括号内的关键字（用于提示支持提示信息中的关键字）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<string> GetAlertMsgKeyword(string input)
        {
            //抓取key比较麻烦，使用向前匹配、向后匹配，条件用法，保证取到正确的key
            const string quotePattern = "(?<=([\"]))[\\w]+(?=\\1)";
            const string bracePattern = "(?<=([\\{]))[\\w]+(?=(?(1)\\}))";

            string pattern = bracePattern;
            if (input.Contains('"'))
            {
                pattern = quotePattern;
            }
            Regex regex = new Regex(pattern);
            var matchResult = regex.Matches(input);
            List<string> resultList = new List<string>();
            foreach (Match result in matchResult)
            {
                if (!resultList.Contains(result.Value, StringComparer.OrdinalIgnoreCase))
                {
                    resultList.Add(result.Value);
                }
            }
            return resultList;
        }

    }
}
