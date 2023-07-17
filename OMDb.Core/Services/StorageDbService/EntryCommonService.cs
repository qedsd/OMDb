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
                sb.AppendFormat(@"select 
t13.Eid,NameStr,ReleaseDate,MyRating,SaveType,path_entry,path_cover,path_source from 
(select distinct t1.EntryId as Eid,t1.Path as path_entry,t1.SaveType,t1.CoverImg as path_cover,t1.ReleaseDate,t1.MyRating,group_concat(t3.path,'>,<') as path_source from Entry t1 
    left join EntrySource t3 on t1.EntryId=t3.EntryId and ((t1.SaveType=t3.PathType) or (t1.SaveType=2 and t3.PathType<>1) or (t1.SaveType=3))
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

        public static string GetEidBySameEntryPath(string ep, string dbId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($@"select EntryId from Entry where Path='{ep}'");
            var result = DbService.GetConnection(dbId).Ado.SqlQuery<dynamic>(sb.ToString());
            if (result.Count > 0)
            {
                var data = (IDictionary<String, Object>)result.FirstOrDefault();
                data.TryGetValue("EntryId", out object eidObj);
                return eidObj.ToString();
            }
            else return String.Empty;
        }


    }
}
