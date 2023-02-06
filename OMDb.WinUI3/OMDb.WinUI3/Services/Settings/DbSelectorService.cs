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
        public static string dbCurrent = string.Empty;
        public static Dictionary<string,string> dbsCollection = new Dictionary<string, string>();
        public static void Initialize()
        {
            LoadAllDbs();
            LoadFromSettings();
        }
        public static async Task SetAsync(string dbSwich)
        {
            dbCurrent = dbSwich;
            await SaveInSettingsAsync(dbCurrent);
        }
        private static async void LoadFromSettings()
        {
            dbCurrent = SettingService.GetValue(Key);

            if (!string.IsNullOrEmpty(dbCurrent))
            {
                dbCurrent.ToString();
            }
            else
            {
                await Task.Run(() =>
                    {
                        LoadAllDbs();
                    });
                dbCurrent = dbsCollection.FirstOrDefault().Key;
                await SetAsync(dbCurrent);
            }
        }

        /*private static async void LoadAllDbs()
        {
            string dbsCollectionStr = SettingService.GetValue(Key2);
            if (!string.IsNullOrEmpty(dbsCollectionStr))
            {
                var jsonObj = JsonConvert.DeserializeObject<List<string>>(dbsCollectionStr);
                dbsCollection.Clear();
                foreach (var item in jsonObj)
                {
                    dbsCollection.Add(item.ToString());
                }
            }
            else
            {
                await AddDbAsync("Default");
            }
        }*/

        private static async Task SaveInSettingsAsync(string dbc)
        {
            await SettingService.SetValueAsync(Key, dbc.ToString());
        }

        public static async Task AddDbAsync(string DbName)
        {
            Core.Services.DbSourceService.AddDbSource(DbName);
        }

        public static async Task RemoveDbAsync(string DbId)
        {
            Core.Services.DbSourceService.RemoveDbSource(DbId);
        }

        /// <summary>
        /// 获取所有Db源
        /// </summary>
        private static async void LoadAllDbs()
        {
            var result =await Core.Services.DbSourceService.GetAllDbSource();
            dbsCollection.Clear();
            foreach (var item in result)
            {
                dbsCollection.Add(item.Id,item.DbName);
            }
        }



    }
}
