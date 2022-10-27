using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Helpers
{
    public static class RandomHelper
    {
        public static List<T> RandomList<T>(IEnumerable<T> items,int count)
        {
            if (items == null)
            {
                return null;
            }
            List<T> results = new List<T>();
            if (items.Count() <= count)
            {
                count = items.Count();
            }
            Random random = new Random();
            HashSet<int> indexs = new HashSet<int>();
            while (indexs.Count < count)
            {
                var index = random.Next(0, items.Count());
                indexs.Add(index);
            }
            foreach (var i in indexs)
            {
                results.Add(items.ElementAt(i));
            }
            return results;
        }

        /// <summary>
        /// 带权重的随机集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="weight"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<T> RandomList<T>(IEnumerable<T> items, int[] weight, int count)
        {
            if (items == null || weight.Length != items.Count())
            {
                return null;
            }
            List<T> results = new List<T>();
            if (items.Count() <= count)
            {
                results = items.ToList();
            }
            else
            {
                List<KeyValuePair<int, int>> wlist = new List<KeyValuePair<int, int>>();//第一个int为list下标索引、第一个int为权重排序值
                int sum = weight.Sum();
                Random random = new Random();
                for (int i = 0; i < weight.Length; i++)
                {
                    int w = (weight[i] + 1) + random.Next(0, sum);   // （权重+1） + 从0到（总权重-1）的随机数
                    wlist.Add(new KeyValuePair<int, int>(i, w));
                }
                wlist.Sort(delegate (KeyValuePair<int, int> kvp1, KeyValuePair<int, int> kvp2)
                {
                    return kvp2.Value - kvp1.Value;
                });
                for (int i = 0; i < count; i++)
                {
                    T entiy = items.ElementAt(wlist[i].Key);
                    results.Add(entiy);
                }
            }
            return results;
        }

        public static T RandomOne<T>(IEnumerable<T> items)
        {
            if(items == null)
            {
                return default(T);
            }
            if (items.Count() <= 1)
            {
                return items.FirstOrDefault();
            }
            else
            {
                Random random = new Random();
                var index = random.Next(0, items.Count());
                return items.ElementAt(index);
            }
        }

        /// <summary>
        /// 随机种子值
        /// </summary>
        /// <returns></returns>
        //private static int GetRandomSeed()
        //{
        //    byte[] bytes = new byte[4];
        //    //RandomNumberGenerator
        //    System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        //    rng.GetBytes(bytes);
        //    return BitConverter.ToInt32(bytes, 0);
        //}
    }
}
