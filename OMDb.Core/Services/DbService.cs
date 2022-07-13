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
        internal static SqlSugarClient Db;
        internal static readonly HashSet<string> DbConfigIds = new();
        static DbService()
        {
            
        }
        /// <summary>
        /// 按数据库连接字符串创建多租户
        /// </summary>
        /// <param name="connet"></param>
        /// <param name="configId"></param>
        public static bool AddDb(string connet,string configId, bool needCodeFirst)
        {
            if (DbConfigIds.Add(configId))
            {
                if (Db == null)
                {
                    Db = new SqlSugarClient(new ConnectionConfig()
                    {
                        ConnectionString = connet,
                        DbType = DbType.Sqlite,
                        IsAutoCloseConnection = true,
                        ConfigId = configId,
                        ConfigureExternalServices = new ConfigureExternalServices
                        {
                            EntityService = (c, p) =>
                            {
                                // int?  decimal?这种 isnullable=true
                                if (c.PropertyType.IsGenericType &&
                                c.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    p.IsNullable = true;
                                }
                            }
                        }
                    });
                }
                else
                {
                    Db.AddConnection(new ConnectionConfig()
                    {
                        ConnectionString = connet,
                        DbType = DbType.Sqlite,
                        IsAutoCloseConnection = true,
                        ConfigId = configId,
                        ConfigureExternalServices = new ConfigureExternalServices
                        {
                            EntityService = (c, p) =>
                            {
                                // int?  decimal?这种 isnullable=true
                                if (c.PropertyType.IsGenericType &&
                                c.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    p.IsNullable = true;
                                }
                            }
                        }
                    });
                }
                if (needCodeFirst)
                {
                    if (CodeFirst(configId))
                    {
                        return true;
                    }
                    else
                    {
                        DbConfigIds.Remove(configId);
                        //按理说这里还得移除Connection，但是没找到相应接口
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
            if (Db.GetConnection(dbId).DbMaintenance.CreateDatabase())
            {
                var types = typeof(Entry).Assembly.GetTypes().Where(p => p.FullName.Contains("DbModels")).ToArray();
                Db.GetConnection(dbId).CodeFirst.InitTables(types);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
