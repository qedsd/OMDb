using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using OMDb.Core.Models;
using OMDb.Core.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OMDb.WinUI3.Models
{
    public class EntryCollection : ObservableObject
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

        public ObservableCollection<EntryCollectionItem> Items;

        public int TotalCount
        {
            get => Items == null ? 0 : Items.Count;
        }

        /// <summary>
        /// 已观看数量
        /// </summary>
        public int WatchedCount { get; set; }

        /// <summary>
        /// 在看数量
        /// </summary>
        public int WatchingCount { get; set; }

        public ImageSource CoverImage { get; set; }

        public EntryCollection()
        {

        }

        public EntryCollection(Core.Models.EntryCollection entryCollection)
        {
            Id = entryCollection.Id;
            Title = entryCollection.Title;
            Description = entryCollection.Description;
            if(entryCollection.Items != null)
            {
                Items = entryCollection.Items.Select(p => EntryCollectionItem.Create(p)).ToObservableCollection();
            }
            CreateTime = entryCollection.CreateTime;
            LastUpdateTime = entryCollection.LastUpdateTime;
        }

        public Core.DbModels.EntryCollectionDb ToEntryCollectionDb()
        {
            return new Core.DbModels.EntryCollectionDb()
            {
                Id = Id,
                Title = Title,
                Description = Description,
                CreateTime = CreateTime,
                LastUpdateTime = LastUpdateTime,
            };
        }

        public static async Task<EntryCollection> CreateBaseAsync(Core.Models.EntryCollection entryCollection)
        {
            EntryCollection result = new EntryCollection(entryCollection);
            if(result.Items != null && result.Items.Count != 0)
            {
                foreach(var g in result.Items.GroupBy(p => p.DbId).ToList())
                {
                    var histories = await Core.Services.EntryWatchHistoryService.QueryWatchHistoriesAsync(g.Select(p => p.Id).ToList(), g.Key);
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
