using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Utils
{
    public static class LinqUtil
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys =new HashSet<TKey>();
            foreach (TSource item in source)
            {
                if (seenKeys.Add(keySelector(item)))
                {
                    yield return item;
                }
            }
        }
    }
}
