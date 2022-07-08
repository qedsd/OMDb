using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// 深度克隆
        /// 通过json序列化对象实现
        /// 出错时返回类型默认值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">原始版本对象</param>
        /// <returns>深度克隆后的对象</returns>
        public static T DepthClone<T>(this object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return default(T);
            }
            try
            {
                var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj), deserializeSettings);
            }
            catch (JsonException)
            {
                return default(T);
            }
        }
    }
}
