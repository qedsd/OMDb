using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using OMDb.Core.Models;
using OMDb.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OMDb.WinUI3.Models
{
    internal class EntryCollection : ObservableObject
    {
        /// <summary>
        /// 数据库key
        /// </summary>
        public string Id { get; set; }

        private string title;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private string description;
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }
        public DateTime CreateTime { get; set; }

        private DateTime lastUpdateTime;
        public DateTime LastUpdateTime
        {
            get => lastUpdateTime;
            set => SetProperty(ref lastUpdateTime, value);
        }

        private ObservableCollection<Entry> entries;
        public ObservableCollection<Entry> Entries
        {
            get => entries; 
            set => SetProperty(ref entries, value);
        }

        public List<Core.DbModels.EntryCollectionItemDb> Items;

        /// <summary>
        /// 已观看数量
        /// </summary>
        public int WatchedCount { get; set; }

        public ImageSource CoverImage { get; set; }

        public EntryCollection()
        {

        }

        public EntryCollection(Core.Models.EntryCollection entryCollection)
        {
            Title = entryCollection.Title;
            Description = entryCollection.Description;
            Items = entryCollection.Items;
            CreateTime = entryCollection.CreateTime;
            LastUpdateTime = entryCollection.LastUpdateTime;
        }

        /// <summary>
        /// 创建不包含Entries详细信息的实例
        /// </summary>
        /// <param name="entryCollection"></param>
        /// <returns></returns>
        public static async Task<EntryCollection> CreateBaseAsync(Core.Models.EntryCollection entryCollection)
        {
            EntryCollection result = new EntryCollection(entryCollection);
            if(result.Items != null && result.Items.Count != 0)
            {
                foreach(var g in result.Items.GroupBy(p => p.DbId).ToList())
                {
                    var histories = await WatchHistoryService.QueryWatchHistoriesAsync(g.Select(p => p.Id).ToList(), g.Key);
                    if(histories != null && histories.Count != 0)
                    {
                        result.WatchedCount += histories.GroupBy(p => p.EntryId).Count();
                    }
                }
            }
            return result;
        }
    }
}
