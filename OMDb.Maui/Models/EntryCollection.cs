using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Maui.Models
{
    public class EntryCollection : ObservableObject
    {
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

        public List<Core.Models.EntryCollectionItem> Items { get; set; }

        public int TotalCount
        {
            get => Items == null ? 0 : Items.Count;
        }

        public int WatchedCount { get; set; }

        public int WatchingCount { get; set; }

        public object CoverImage { get; set; }

        public EntryCollection()
        {

        }

        public EntryCollection(Core.Models.EntryCollection entryCollection)
        {
            Id = entryCollection.Id;
            Title = entryCollection.Title;
            Description = entryCollection.Description;
            Items = entryCollection.Items;
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
            if (result.Items != null && result.Items.Count != 0)
            {
                foreach (var g in result.Items.GroupBy(p => p.DbId).ToList())
                {
                    var histories = await Core.Services.EntryWatchHistoryService.QueryWatchHistoriesAsync(g.Select(p => p.Id).ToList(), g.Key);
                    if (histories != null && histories.Count != 0)
                    {
                        result.WatchedCount += histories.GroupBy(p => p.EntryId).Count();
                    }
                }
            }
            return result;
        }
    }
}
