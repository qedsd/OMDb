using OMDb.Core.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class RepositoryService
    {
        private static bool IsLocalDbValid()
        {
            return DbService.ConfigDb != null;
        }

        /// <summary>
        /// 获取全部仓库
        /// </summary>
        /// <returns></returns>
        public static async Task<List<RepositoryDb>> GetAllRepositoryAsync()
        {
            if (IsLocalDbValid())
            {
                return await DbService.ConfigDb.Queryable<RepositoryDb>().ToListAsync();
            }
            else
            {
                return null;
            }
        }

        public static void AddRepository(RepositoryDb repositoryDb)
        {
            if (string.IsNullOrEmpty(repositoryDb.Id))
            {
                repositoryDb.Id = Guid.NewGuid().ToString();
            }
            DbService.ConfigDb.Insertable(repositoryDb).ExecuteCommand();
        }
        public static void RemoveRepository(string repId)
        {
            DbService.ConfigDb.Deleteable<RepositoryDb>().In(repId).ExecuteCommand();
        }
    }
}
