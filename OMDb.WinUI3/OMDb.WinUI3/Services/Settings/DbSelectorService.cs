using Newtonsoft.Json;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Views.Homes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Services.Settings
{
    internal static class DbSelectorService
    {
        private const string Key1 = "DbSelector";
        private const string Key2 = "DbsCollection";
        public static string dbCurrent = string.Empty;
        public static List<string> dbsCollection = new List<string>();
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
            dbCurrent = SettingService.GetValue(Key1);

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
                dbCurrent = dbsCollection[0];
                await SetAsync(dbCurrent);
            }
        }
        private static async void LoadAllDbs()
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
        }
        private static async Task SaveInSettingsAsync(string dbc)
        {
            await SettingService.SetValueAsync(Key1, dbc.ToString());
        }

        public static async Task AddDbAsync(string DbName)
        {
            dbsCollection.Add(DbName);
            var jsonObj = JsonConvert.SerializeObject(dbsCollection);
            await SettingService.SetValueAsync(Key2, jsonObj);
        }

        public static async Task RemoveDbAsync(string DbName)
        {
            dbsCollection.Remove(DbName);
            var jsonObj = JsonConvert.SerializeObject(dbsCollection);
            await SettingService.SetValueAsync(Key2, jsonObj);
        }


    }
}
