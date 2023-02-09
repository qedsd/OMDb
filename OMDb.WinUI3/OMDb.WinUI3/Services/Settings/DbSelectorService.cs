using Newtonsoft.Json;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Views.Homes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OMDb.WinUI3.Services.Settings
{
    internal static class DbSelectorService
    {
        private const string Key = "DbSelector";
        public static string dbCurrentId = string.Empty;
        public static List<DbSource> dbsCollection = new List<DbSource>();
        public static void Initialize()
        {
            LoadAllDbs();
            LoadFromSettings();
        }
        public static async Task SetAsync(string dbSwich)
        {
            dbCurrentId = dbSwich;
            await SaveInSettingsAsync(dbCurrentId);
        }


        //配置Json获取当前Db源Id
        private static async void LoadFromSettings()
        {
            dbCurrentId = Convert.ToString(SettingService.GetValue(Key));

            //数据库读取所有Db源
            LoadAllDbs();

            //找不到 或 未配置 -> 抽一个赋值
            if ((!dbsCollection.Select(a => a.DbSourceDb.Id).ToList().Contains(dbCurrentId)) || string.IsNullOrEmpty(dbCurrentId))
            {
                dbCurrentId = dbsCollection.FirstOrDefault().DbSourceDb.Id;
            }

            dbsCollection.Where(a => a.DbSourceDb.Id == dbCurrentId).FirstOrDefault().IsChecked = true;
            await SetAsync(dbCurrentId);
        }

        //保存
        private static async Task SaveInSettingsAsync(string dbc)
        {
            await SettingService.SetValueAsync(Key, dbc.ToString());
        }

        //添加
        public static void AddDbAsync(string DbName)
        {
            Core.Services.DbSourceService.AddDbSource(DbName);
        }

        //移除
        public static void RemoveDbAsync(string DbId)
        {
            Core.Services.DbSourceService.RemoveDbSource(DbId);
        }

        //修改Db名称源
        public static void EditDbAsync(Core.DbModels.DbSourceDb db)
        {
            Core.Services.DbSourceService.EditDbSource(db);
        }


        /// <summary>
        /// 获取所有Db源
        /// </summary>
        private static void LoadAllDbs()
        {
            var result = Core.Services.DbSourceService.GetAllDbSource();
            dbsCollection.Clear();
            foreach (var item in result)
            {
                var db = new DbSource(item);
                dbsCollection.Add(db);
            }
        }



    }
}
