using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class EntryCommonService
    {
        /// <summary>
        /// Entry三表联查
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static List<dynamic> SelectEntry(string dbId)
        {
            if (!string.IsNullOrEmpty(dbId))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(@"select t13.Eid,CoverImg,ReleaseDate,PathStr,NameStr,MyRating from 
(select distinct t1.EntryId as Eid,t1.CoverImg,t1.ReleaseDate,t1.MyRating,group_concat(t3.path,'/') as PathStr from Entry t1 
    inner join EntrySource t3 on t1.EntryId=t3.EntryId 
    group by t1.EntryId) t13
inner join 
(select distinct t1.EntryId as Eid,group_concat(t2.Name,'/') as NameStr from Entry t1 
    inner join EntryName t2 on t1.EntryId=t2.EntryId 
    group by t1.EntryId) t12
on t13.Eid=t12.Eid;");
                return DbService.GetConnection(dbId).Ado.SqlQuery<dynamic>(sb.ToString());
            }
            return null;
        }



    }
}
