using OMDb.Core.DbModels.ManagerCenterDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class DbCenterService
    {

        private static bool IsLocalDbValid()
        {
            return DbService.MCDb != null;
        }


        /// <summary>
        /// 获取DbCenter
        /// </summary>
        /// <returns></returns>
        public static  List<DbCenterDb> GetAllDbCenter()
        {
            if (IsLocalDbValid())
            {
                return  DbService.MCDb.Queryable<DbCenterDb>().ToList();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 新增Db源
        /// </summary>
        /// <param name="DbCenterDb"></param>
        public static void AddDbCenter(DbCenterDb DbCenterDb)
        {
            if (string.IsNullOrEmpty(DbCenterDb.Id)) DbCenterDb.Id = Guid.NewGuid().ToString();
            DbService.MCDb.Insertable(DbCenterDb).ExecuteCommand();
        }
        public static void AddDbCenter(string dbName)
        {
            var DbCenterDb = new DbCenterDb() {DbName= dbName};
            if (string.IsNullOrEmpty(DbCenterDb.Id)) DbCenterDb.Id = Guid.NewGuid().ToString();
            DbCenterDb.CreateTime = DateTime.Now;
            DbCenterDb.ModifyTime = DateTime.Now;
            DbService.MCDb.Insertable(DbCenterDb).ExecuteCommand();
        }

        /// <summary>
        /// 移除Db源
        /// </summary>
        /// <param name="storageName"></param>
        public  static void RemoveDbCenter(string dbId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Delete from DbCenter where ID='{0}'", dbId);
            DbService.MCDb.Ado.ExecuteCommand(sb.ToString());
        }


        /// <summary>
        /// 修改Db源名称
        /// </summary>
        /// <param name="storageName"></param>
        public static void EditDbCenter(DbCenterDb db)
        {
            db.ModifyTime= DateTime.Now;
            DbService.MCDb.Updateable<DbCenterDb>(db).Where(a=>a.Id== db.Id).ExecuteCommand();
        }

        /// <summary>
        /// 批量移除Db源
        /// </summary>
        /// <param name="storageNameCollection"></param>
        public static void RemoveDbCenter(List<string> storageNameCollection)
        {
            StringBuilder sb = new StringBuilder();
            var storageNameCollectionStr=string.Join(",", storageNameCollection);
            sb.AppendFormat("Delete from DbCenter where ID in '{0}'", storageNameCollectionStr);
            DbService.MCDb.Ado.ExecuteCommand(sb.ToString());
        }
    }
}
