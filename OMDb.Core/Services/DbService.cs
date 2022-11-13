using OMDb.Core.DbModels;
using OMDb.Core.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace OMDb.Core.Services
{
    public static class DbService
    {
        /// <summary>
        /// 管理所有仓库数据库连接
        /// </summary>
        internal static readonly Dictionary<string,SqlSugarScope> Dbs = new();
        /// <summary>
        /// 是否有数据库连接
        /// </summary>
        internal static bool IsEmpty
        {
            get=>Dbs.Count ==0;
        }
        /// <summary>
        /// 保存标签、集锦等需要关联多个仓库的数据
        /// </summary>
        internal static SqlSugarScope LocalDb;
        static DbService()
        {
            
        }
        /// <summary>
        /// 按数据库连接字符串创建多仓库数据库
        /// </summary>
        /// <param name="connet"></param>
        /// <param name="configId"></param>
        internal static bool AddDb(string connet,string configId, bool needCodeFirst)
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
        /// 仓库数据库自动建表建库
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        private static bool CodeFirst(string dbId)
        {
            if (GetConnection(dbId).DbMaintenance.CreateDatabase())
            {
                List<Type> types = new List<Type>();
                types.Add(typeof(EntryDb));
                types.Add(typeof(EntryNameDb));
                types.Add(typeof(WatchHistoryDb));
                GetConnection(dbId).CodeFirst.InitTables(types.ToArray());
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取仓库数据库Scope实例
        /// 不要自己到Dbs拿，统一使用此方法，避免以后改回ORM的多租户模式
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        internal static SqlSugarScope GetConnection(string dbId)
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

        internal static bool SetLocalDb(string connet)
        {
            try
            {
                LocalDb = new SqlSugarScope(new ConnectionConfig()
                {
                    ConnectionString = connet,
                    DbType = DbType.Sqlite,
                    IsAutoCloseConnection = true,
                    ConfigId = Guid.NewGuid(),
                    ConfigureExternalServices = new ConfigureExternalServices
                    {
                        EntityService = (c, p) =>
                        {
                            if (c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                p.IsNullable = true;
                            }
                        }
                    },
                    MoreSettings = new ConnMoreSettings()
                    {
                        IsAutoRemoveDataCache = true
                    }
                });
                LocalDb.DbMaintenance.CreateDatabase();
                List<Type> types = new List<Type>();
                types.Add(typeof(LabelDb));
                types.Add(typeof(EntryLabelDb));
                types.Add(typeof(EntryCollectionDb));
                types.Add(typeof(EntryCollectionItemDb));
                LocalDb.CodeFirst.InitTables(types.ToArray());
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
