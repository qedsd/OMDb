using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OMDb.Core.Enums;
using OMDb.Core.Services;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Security.Authentication.OnlineId;

namespace OMDb.WinUI3.ViewModels
{
    internal class CollectionsViewModel : ObservableObject
    {
        private ObservableCollection<EntryCollection> entryCollections;
        public ObservableCollection<EntryCollection> EntryCollections
        {
            get => entryCollections;
            set => SetProperty(ref entryCollections, value);
        }

        private string newCollectionTitle;
        public string NewCollectionTitle
        {
            get => newCollectionTitle;
            set => SetProperty(ref newCollectionTitle, value);
        }

        private string newCollectionDesc;
        public string NewCollectionDesc
        {
            get => newCollectionDesc;
            set => SetProperty(ref newCollectionDesc, value);
        }

        public CollectionsViewModel()
        {
            InitAsync();
        }
        private async void InitAsync()
        {
            EntryCollections = new ObservableCollection<EntryCollection>();
            var collections = await Core.Services.EntryCollectionService.GetAllCollectionsAsync();
            if(collections != null && collections.Any())
            {
                foreach (var c in collections)
                {
                    EntryCollections.Add(await EntryCollection.CreateBaseAsync(c));
                }
                //List<EntryCollectionItemDb> entryCollectionItems = new List<EntryCollectionItemDb>();
                //foreach(var c in collections)
                //{
                //    entryCollectionItems.AddRange(c.Items);
                //}
                //var dbGroups = entryCollectionItems.GroupBy(p => p.DbId).ToList();
                //foreach(var dbGroup in dbGroups)
                //{
                //    var entryIds = dbGroup.GroupBy(p => p.EntryId).Select(p => p.Key).ToList();
                //    var histories = await WatchHistoryService.QueryWatchHistoriesAsync(entryIds, dbGroup.Key);
                //    var dic = histories.ToDictionary(p => p.Id);
                //    foreach(var item in dbGroup)
                //    {
                        
                //    }
                //}
            }
        }

        public ICommand AddNewCollectionCommand => new RelayCommand(() =>
        {
            if(!string.IsNullOrWhiteSpace(NewCollectionTitle))
            {
                Core.DbModels.EntryCollectionDb entryCollectionDb = new Core.DbModels.EntryCollectionDb()
                {
                    Title = NewCollectionTitle,
                    Description = NewCollectionDesc,
                    CreateTime = DateTime.Now,
                    LastUpdateTime = DateTime.Now
                };
                EntryCollectionService.AddCollection(entryCollectionDb);
                EntryCollections.Add(new EntryCollection(Core.Models.EntryCollection.Create(entryCollectionDb)));
                NewCollectionTitle = string.Empty;
                NewCollectionDesc = string.Empty;
                Helpers.InfoHelper.ShowSuccess("已添加");
            }
        });

        public ICommand CollectionDetailCommand => new RelayCommand<EntryCollection>(async (selectedItem) =>
        {
            //TODO:借用LabelCollection，后续考虑将两者合并或单独实现
            if (selectedItem != null)
            {
                Helpers.InfoHelper.ShowWaiting();
                LabelCollection labelCollection = new LabelCollection()
                {
                    Title = selectedItem.Title,
                    Description = selectedItem.Description,
                };
                if (selectedItem.Items != null)
                {
                    var entrys = await Core.Services.EntryService.QueryEntryAsync(selectedItem.Items.Select(p => p.ToQueryItem()).ToList());
                    labelCollection.Entries = entrys;
                }
                Services.NavigationService.Navigate(typeof(Views.LabelCollectionPage), labelCollection);
                Helpers.InfoHelper.HideWaiting();
            }
        });

        public ICommand EditCommand => new RelayCommand<EntryCollection>(async (selectedItem) =>
        {

        });
        public ICommand RemoveCommand => new RelayCommand<EntryCollection>((selectedItem) =>
        {
            if(selectedItem != null)
            {
                Core.Services.EntryCollectionService.RemoveCollection(selectedItem.Id);
                EntryCollections.Remove(selectedItem);
            }
        });
    }
}
