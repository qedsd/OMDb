using Newtonsoft.Json;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Views.Homes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Services.Settings
{
    internal static class HomeItemConfigsService
    {
        private const string Key = "HomeItemConfig";
        public static List<HomeItemConfig> ActiveItems { get; set; }
        public static List<HomeItemConfig> InactiveItems { get; set; }
        public static List<HomeItemConfig> AllItems { get; set; }
        public static void Initialize()
        {
            LoadFromSettings();
        }

        public static async Task SetAsync(List<HomeItemConfig> activeItems)
        {
            ActiveItems = activeItems.ToList();
            InactiveItems.Clear();
            foreach (var item in AllItems)
            {
                if (ActiveItems.FirstOrDefault(p => p.Name == item.Name) == null)
                {
                    InactiveItems.Add(item);
                }
            }
            await SaveInSettingsAsync(activeItems);
        }

        private static void LoadFromSettings()
        {
            AllItems = new List<HomeItemConfig>();
            AllItems.Add(new HomeItemConfig("摘录台词", typeof(ExtractLinePage)));
            AllItems.Add(new HomeItemConfig("最近观看视频", typeof(RecentlyWatchedFilesPage)));
            AllItems.Add(new HomeItemConfig("最近观看词条", typeof(RecentlyWatchedEntryPage)));
            AllItems.Add(new HomeItemConfig("最近更新词条", typeof(RecentlyUpdatedEntryPage)));
            AllItems.Add(new HomeItemConfig("随机词条", typeof(RandomEntryPage)));
            AllItems.Add(new HomeItemConfig("统计信息", typeof(StatisticsPage)));
            InactiveItems = new List<HomeItemConfig>();
            var json = SettingService.GetValue(Key);
            if(string.IsNullOrEmpty(json))
            {
                ActiveItems = AllItems.ToList();
            }
            else
            {
                ActiveItems = JsonConvert.DeserializeObject<List<HomeItemConfig>>(SettingService.GetValue(Key));
                foreach(var item in AllItems)
                {
                    if(ActiveItems.FirstOrDefault(p=>p.Name == item.Name) == null)
                    {
                        InactiveItems.Add(item);
                    }
                }
            }
        }

        private static async Task SaveInSettingsAsync(List<HomeItemConfig> activeItems)
        {
            await SettingService.SetValueAsync(Key, JsonConvert.SerializeObject(activeItems));
        }
    }
}
