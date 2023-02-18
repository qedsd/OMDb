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
            return DbService.LocalDb != null;
        }


        /// <summary>
        /// 获取全部仓库
        /// </summary>
        /// <returns></returns>
        public static async Task<List<StorageDb>> GetAllStorageAsync()
        {
            if (IsLocalDbValid())
            {
                return await DbService.LocalDb.Queryable<StorageDb>().ToListAsync();
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
        public static async Task<List<StorageDb>> GetAllStorageAsync(string dbSourceId)
        {
            if (IsLocalDbValid())
            {
                return await DbService.LocalDb.Queryable<StorageDb>().Where(a=>a.DbSourceId== dbSourceId).ToListAsync();
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
            DbService.LocalDb.Insertable(storageDb).ExecuteCommand();
        }
        public static void RemoveStorage(string dbSourceId,string storageName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Delete from Storage where StorageName in ('{0}') and dbSourceId='{1}'", storageName, dbSourceId);
            DbService.LocalDb.Ado.ExecuteCommand(sb.ToString());
            //DbService.LocalDb.Deleteable<StorageDb>().Where(a => a.DbSourceId == dbSourceId&&a.StorageName==storageName);
        }

        public static void RemoveStorage(string dbSourceId, List<string> storageName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Delete from Storage where StorageName in '{0}' and dbSourceId='{1}'", string.Join("','",storageName), dbSourceId);
            DbService.LocalDb.Ado.ExecuteCommand(sb.ToString());
        }
    }
}
