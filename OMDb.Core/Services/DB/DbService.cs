using OMDb.Core.DbModels;
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
        internal static readonly Dictionary<string, SqlSugarScope> RepositoryDbs = new();
        /// <summary>
        /// 是否有数据库连接
        /// </summary>
        internal static bool IsEmpty
        {
            get => RepositoryDbs.Count == 0;
        }

        /// <summary>
        /// 保存标签、集锦等需要关联多个仓库的数据
        /// </summary>
        internal static SqlSugarScope ConfigDb;

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
            if (!RepositoryDbs.ContainsKey(configId))
            {
                RepositoryDbs.Add(configId, new SqlSugarScope(new ConnectionConfig()
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
                    RepositoryDbs.Remove(configId);
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
                List<Type> types = new List<Type>
                {
                    typeof(EntryDb),
                    typeof(EntryNameDb),
                    typeof(EntryWatchHistoryDb)
                };
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
            if (!string.IsNullOrEmpty(dbId) && RepositoryDbs.TryGetValue(dbId, out SqlSugarScope scope))
            {
                return scope;
            }
            else
            {
                throw new Exception("Database does not exist");
            }
        }

        internal static bool SetConfigDb(string connet)
        {
            try
            {
                ConfigDb = new SqlSugarScope(new ConnectionConfig()
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
                ConfigDb.DbMaintenance.CreateDatabase();
                List<Type> types = new List<Type>
                {
                    typeof(EntryLabelLinkDb),
                    typeof(EntryCollectionDb),
                    typeof(EntryCollectionItemDb),
                    typeof(RepositoryDb)
                };

                ConfigDb.CodeFirst.InitTables(types.ToArray());

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
