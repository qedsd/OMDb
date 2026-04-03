using OMDb.Core.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class StorageService
    {

        private static bool IsLocalDbValid()
        {
            return DbService.DCDb != null;
        }


        /// <summary>
        /// 获取全部仓库
        /// </summary>
        /// <returns></returns>
        public static async Task<List<StorageDb>> GetAllStorageAsync()
        {
            if (IsLocalDbValid())
            {
                return await DbService.DCDb.Queryable<StorageDb>().ToListAsync();
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 获取全部仓库
        /// </summary>
        /// <returns></returns>
        public static async Task<List<StorageDb>> GetAllStorageAsync(string DbCenterId)
        {
            if (IsLocalDbValid())
            {
                return await DbService.DCDb.Queryable<StorageDb>().Where(a=>a.DbCenterId== DbCenterId).ToListAsync();
            }
            else
            {
                return null;
            }
        }

        public static void AddStorage(StorageDb storageDb)
        {
            if (string.IsNullOrEmpty(storageDb.Id))
            {
                storageDb.Id = Guid.NewGuid().ToString();
            }
            DbService.DCDb.Insertable(storageDb).ExecuteCommand();
        }
        public static void RemoveStorage(string DbCenterId,string storageName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Delete from Storage where StorageName in ('{0}') and DbCenterId='{1}'", storageName, DbCenterId);
            DbService.DCDb.Ado.ExecuteCommand(sb.ToString());
            //DbService.LocalDb.Deleteable<StorageDb>().Where(a => a.DbCenterId == DbCenterId&&a.StorageName==storageName);
        }

        public static void RemoveStorage(string DbCenterId, List<string> storageName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Delete from Storage where StorageName in '{0}' and DbCenterId='{1}'", string.Join("','",storageName), DbCenterId);
            DbService.DCDb.Ado.ExecuteCommand(sb.ToString());
        }
    }
}
