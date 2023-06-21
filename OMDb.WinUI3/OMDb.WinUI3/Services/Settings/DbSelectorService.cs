using Newtonsoft.Json;
using OMDb.Core.DbModels.ManagerCenterDb;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Views.Homes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.Storage.Search;

namespace OMDb.WinUI3.Services.Settings
{
    internal static class DbSelectorService
    {
        private const string Key = "DbSelector";
        public static string dbCurrentId = string.Empty;
        public static string dbCurrentName = string.Empty;
        public static List<DbCenter> dbsCollection = new List<DbCenter>();
        public static async void Initialize()
        {
            LoadFromSettings();
            dbCurrentName = dbsCollection.Where(a => a.IsChecked == true).FirstOrDefault().DbCenterDb.DbName;
            ConfigService.DefaultEntryFolder = $@"{ConfigService.OMDbFolder}\{dbCurrentName}";
            ConfigService.LoadStorages();
        }
        public static async Task SetAsync(string dbSwich)
        {
            dbCurrentId = dbSwich;
            await SaveInSettingsAsync(dbCurrentId);
        }


        //配置Json获取当前Db源Id
        private static async void LoadFromSettings()
        {
            LoadAllDbs();//数据库读取所有Db源
            dbCurrentId = Convert.ToString(SettingService.GetValue(Key));
            //找不到 或 未配置 -> 抽一个赋值
            if ((!dbsCollection.Select(a => a.DbCenterDb.Id).ToList().Contains(dbCurrentId)) || string.IsNullOrEmpty(dbCurrentId))
                dbCurrentId = dbsCollection.FirstOrDefault().DbCenterDb.Id;
                
            Core.Config.InitDCDb(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", $"DCDb_{DbSelectorService.dbCurrentId}.db"));
            dbsCollection.Where(a => a.DbCenterDb.Id == dbCurrentId).FirstOrDefault().IsChecked = true;
            await SetAsync(dbCurrentId);
        }

        //保存
        private static async Task SaveInSettingsAsync(string dbc)
        {
            await SettingService.SetValueAsync(Key, dbc.ToString());
            dbCurrentName = dbsCollection.Where(a => a.IsChecked == true).FirstOrDefault().DbCenterDb.DbName;
            ConfigService.DefaultEntryFolder = @$"{ConfigService.OMDbFolder}\{dbCurrentName}";
            ConfigService.LoadStorages();

        }

        //添加
        public static void AddDbAsync(string DbName)
        {
            Core.Services.DbCenterService.AddDbCenter(DbName);
        }

        //移除
        public static void RemoveDbAsync(string DbId)
        {
            Core.Services.DbCenterService.RemoveDbCenter(DbId);
        }

        //修改Db名称源
        public static void EditDbAsync(DbCenterDb db)
        {
            Core.Services.DbCenterService.EditDbCenter(db);
        }


        /// <summary>
        /// 获取所有Db源
        /// </summary>
        private static void LoadAllDbs()
        {
            var result = Core.Services.DbCenterService.GetAllDbCenter();
            dbsCollection.Clear();
            foreach (var item in result)
            {
                var db = new DbCenter(item);
                dbsCollection.Add(db);
            }
        }



    }
}
