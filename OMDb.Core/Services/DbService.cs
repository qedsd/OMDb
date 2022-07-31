using OMDb.Core.DbModels;
using OMDb.Core.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class DbService
    {
        /// <summary>
        /// 管理所有数据库连接
        /// </summary>
        internal static readonly Dictionary<string,SqlSugarScope> Dbs = new();
        /// <summary>
        /// 是否有数据库连接
        /// </summary>
        public static bool IsEmpty
        {
            get=>Dbs.Count ==0;
        }
        static DbService()
        {
            
        }
        /// <summary>
        /// 按数据库连接字符串创建多数据库
        /// </summary>
        /// <param name="connet"></param>
        /// <param name="configId"></param>
        public static bool AddDb(string connet,string configId, bool needCodeFirst)
        {
            if (!Dbs.ContainsKey(configId))
            {
                Dbs.Add(configId, new SqlSugarScope(new ConnectionConfig()
                {
                    ConnectionString = connet,
                    DbType = DbType.Sqlite,
                    IsAutoCloseConnection = true,
                    ConfigId = configId,
                    ConfigureExternalServices = new ConfigureExternalServices
                    {
                        EntityService = (c, p) =>
                        {
                            if (c.PropertyType.IsGenericType &&c.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                p.IsNullable = true;
                            }
                        }
                    },
                    MoreSettings = new ConnMoreSettings()
                    {
                        IsAutoRemoveDataCache = true
                    }
                }
                ));
                if (needCodeFirst)
                {
                    if (CodeFirst(configId))
                    {
                        return true;
                    }
                    else
                    {
                        Dbs.Remove(configId);
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 自动建表建库
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        private static bool CodeFirst(string dbId)
        {
            if (GetConnection(dbId).DbMaintenance.CreateDatabase())
            {
                var types = typeof(Entry).Assembly.GetTypes().Where(p => p.FullName.Contains("DbModels")).ToArray();
                GetConnection(dbId).CodeFirst.InitTables(types);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取Scope实例
        /// 不要自己到Dbs拿，统一使用此方法，避免以后改回ORM的多租户模式
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static SqlSugarScope GetConnection(string dbId)
        {
            if(Dbs.TryGetValue(dbId, out SqlSugarScope scope))
            {
                return scope;
            }
            else
            {
                return null;
            }
        }
    }
}
