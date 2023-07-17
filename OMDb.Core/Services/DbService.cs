using OMDb.Core.DbModels;
using OMDb.Core.DbModels.ManagerCenterDb;
using OMDb.Core.Models;
using OMDb.Core.Utils;
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
        /// 管理所有仓库数据库连接
        /// </summary>
        internal static readonly Dictionary<string, SqlSugarScope> Dbs = new();
        /// <summary>
        /// 是否有数据库连接
        /// </summary>
        internal static bool IsEmpty
        {
            get => Dbs.Count == 0;
        }

        /// <summary>
        /// 保存标签、集锦等需要关联多个仓库的数据
        /// </summary>
        internal static SqlSugarScope MCDb;

        /// <summary>
        /// 保存标签、集锦等需要关联多个仓库的数据
        /// </summary>
        internal static SqlSugarScope DCDb;
        static DbService()
        {

        }
        /// <summary>
        /// 按数据库连接字符串创建多仓库数据库
        /// </summary>
        /// <param name="connet"></param>
        /// <param name="configId"></param>
        internal static bool AddDb(string connet, string configId, bool needCodeFirst)
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
                }
                ));
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
                types.Add(typeof(EntrySourceDb));
                types.Add(typeof(EntryWatchHistoryDb));
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
            if (!string.IsNullOrEmpty(dbId) && Dbs.TryGetValue(dbId, out SqlSugarScope scope))
            {
                return scope;
            }
            else
            {
                throw new Exception("Database does not exist");
            }
        }


        /// <summary>
        /// 数据中心数据库 建表建库
        /// </summary>
        /// <param name="connet"></param>
        /// <returns></returns>
        internal static bool SetMCDb(string connet)
        {
            try
            {
                MCDb = new SqlSugarScope(new ConnectionConfig()
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
                MCDb.DbMaintenance.CreateDatabase();
                List<Type> types = new List<Type>();
                types.Add(typeof(DbCenterDb));
                MCDb.CodeFirst.InitTables(types.ToArray());
                var dbCenters = DbCenterService.GetAllDbCenter();
                if (dbCenters.Count == 0)
                    DbCenterService.AddDbCenter("DC01");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        internal static bool SetDCDb(string connet)
        {
            try
            {
                DCDb = new SqlSugarScope(new ConnectionConfig()
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
                DCDb.DbMaintenance.CreateDatabase();
                List<Type> types = new List<Type>();

                types.Add(typeof(LabelClassDb));
                types.Add(typeof(LabelPropertyDb));
                types.Add(typeof(LabelPropertyLinkDb));
                types.Add(typeof(EntryLabelClassLinkDb));
                types.Add(typeof(EntryLabelPropertyLinkDb));
                types.Add(typeof(EntryCollectionDb));
                types.Add(typeof(EntryCollectionItemDb));
                types.Add(typeof(StorageDb));

                DCDb.CodeFirst.InitTables(types.ToArray());

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

    }
}
