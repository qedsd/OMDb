using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OMDb.Core.Enums;
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

        private IList<EntryCollection> suggetions;
        public IList<EntryCollection> Suggetions
        {
            get => suggetions;
            set => SetProperty(ref suggetions, value);
        }

        private string suggestText;
        public string SuggestText
        {
            get => suggestText;
            set
            {
                SetProperty(ref suggestText, value);
                UpdateSuggestions();
            }
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
            }
        }

        private void UpdateSuggestions()
        {
            if(EntryCollections!= null)
            {
                if(!string.IsNullOrEmpty(SuggestText))
                {
                    Suggetions = EntryCollections.Where(p => p.Title.Contains(SuggestText)).ToList();
                }
                else
                {
                    Suggetions = null;
                }
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
                Core.Services.EntryCollectionService.AddCollection(entryCollectionDb);
                EntryCollections.Add(new EntryCollection(Core.Models.EntryCollection.Create(entryCollectionDb)));
                NewCollectionTitle = string.Empty;
                NewCollectionDesc = string.Empty;
                Helpers.InfoHelper.ShowSuccess("已添加");
            }
        });

        public ICommand CollectionDetailCommand => new RelayCommand<EntryCollection>((selectedItem) =>
        {
            if (selectedItem != null)
            {
                Services.TabViewService.AddItem(new Views.EntryCollectionDetailPage(selectedItem));
            }
        });

        public ICommand EditCommand => new RelayCommand<EntryCollection>((selectedItem) =>
        {

        });
        public ICommand RemoveCommand => new RelayCommand<EntryCollection>(async(selectedItem) =>
        {
            if(selectedItem != null)
            {
                if(await Dialogs.QueryDialog.ShowDialog("删除片单", $"是否删除 {selectedItem.Title}"))
                {
                    Core.Services.EntryCollectionService.RemoveCollection(selectedItem.Id);
                    EntryCollections.Remove(selectedItem);
                    Helpers.InfoHelper.ShowSuccess("已删除");
                }
                else
                {
                    Helpers.InfoHelper.ShowSuccess("已取消");
                }
            }
        });
        public ICommand RefreshCommand => new RelayCommand(() =>
        {
            InitAsync();
        });
        public ICommand SuggestionChosenCommand => new RelayCommand<EntryCollection>((item) =>
        {
            CollectionDetailCommand.Execute(item);
            SuggestText = null;
        });
    }
}
