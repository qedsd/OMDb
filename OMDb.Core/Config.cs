using OMDb.Core.Helpers;
using OMDb.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core
{
    public static class Config
    {
        public static bool AddConnectionString(string str, string configId, bool needCodeFirst)
        {
            return DbService.AddDb(str, configId, needCodeFirst);
        }
        public static bool AddDbFile(string filePath, string configId,bool needCodeFirst)
        {
            return DbService.AddDb(@"DataSource=" + filePath, configId, needCodeFirst);
        }
        public static bool RemoveDb(string key)
        {
            return DbService.RepositoryDbs.Remove(key);
        }
        public static void ClearDb()
        {
            DbService.RepositoryDbs.Clear();
        }

        public static bool InitConfigDb(string filePath)
        {
            return DbService.SetConfigDb($"DataSource={filePath}");
        }

        /// <summary>
        /// 设置ffmpeg执行文件路径
        /// </summary>
        /// <param name="path"></param>
        public static void SetFFmpegExecutablesPath(string path)
        {
            FFmpegHelper.SetExecutablesPath(path);
        }
    }
}
