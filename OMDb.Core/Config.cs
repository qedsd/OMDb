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
        internal static readonly List<string> SqliteConnectionStrings = new();
        public static bool AddConnectionString(string str, string configId, bool needCodeFirst)
        {
            SqliteConnectionStrings.Add(str);
            return Services.DbService.AddDb(str, configId, needCodeFirst);
        }
        public static bool AddDbFile(string filePath, string configId,bool needCodeFirst)
        {
            SqliteConnectionStrings.Add(@"DataSource=" + filePath);
            return Services.DbService.AddDb(@"DataSource=" + filePath, configId, needCodeFirst);
        }
        public static bool RemoveDb(string key)
        {
            return Services.DbService.Dbs.Remove(key);
        }
        public static void ClearDb()
        {
            Services.DbService.Dbs.Clear();
        }

        public static bool InitLocalDb(string filePath)
        {
            return DbService.SetLocalDb($"DataSource={filePath}");
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
