using OMDb.Core.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class DbSourceService
    {

        private static bool IsLocalDbValid()
        {
            return DbService.LocalDb != null;
        }


        /// <summary>
        /// 获取DbSource
        /// </summary>
        /// <returns></returns>
        public static  List<DbSourceDb> GetAllDbSource()
        {
            if (IsLocalDbValid())
            {
                return  DbService.LocalDb.Queryable<DbSourceDb>().ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 新增仓库
        /// </summary>
        /// <param name="dbSourceDb"></param>
        public static void AddDbSource(DbSourceDb dbSourceDb)
        {
            if (string.IsNullOrEmpty(dbSourceDb.Id)) dbSourceDb.Id = Guid.NewGuid().ToString();
            DbService.LocalDb.Insertable(dbSourceDb).ExecuteCommand();
        }
        public static void AddDbSource(string dbName)
        {
            var dbSourceDb = new DbSourceDb() {DbName= dbName};
            if (string.IsNullOrEmpty(dbSourceDb.Id)) dbSourceDb.Id = Guid.NewGuid().ToString();
            dbSourceDb.CreateTime = DateTime.Now;
            dbSourceDb.ModifyTime = DateTime.Now;
            DbService.LocalDb.Insertable(dbSourceDb).ExecuteCommand();
        }

        /// <summary>
        /// 移除仓库
        /// </summary>
        /// <param name="storageName"></param>
        public  static void RemoveDbSource(string dbId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Delete from DbSource where ID='{0}'", dbId);
            DbService.LocalDb.Ado.ExecuteCommand(sb.ToString());
        }

        /// <summary>
        /// 批量移除仓库
        /// </summary>
        /// <param name="storageNameCollection"></param>
        public  static void RemoveDbSource(List<string> storageNameCollection)
        {
            StringBuilder sb = new StringBuilder();
            var storageNameCollectionStr=string.Join(",", storageNameCollection);
            sb.AppendFormat("Delete from DbSource where ID in '{0}'", storageNameCollectionStr);
            DbService.LocalDb.Ado.ExecuteCommand(sb.ToString());
        }
    }
}
