using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OMDb.Core.DbModels;
using OMDb.Core.Services;
using OMDb.Maui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.Maui.ViewModels
{
    public partial class CollectionsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<EntryCollection> _entryCollections;

        [ObservableProperty]
        private IList<EntryCollection> _suggestions;

        [ObservableProperty]
        private string _suggestText;

        [ObservableProperty]
        private string _newCollectionTitle;

        [ObservableProperty]
        private string _newCollectionDesc;

        public CollectionsViewModel()
        {
            InitAsync();
        }

        private async void InitAsync()
        {
            EntryCollections = new ObservableCollection<EntryCollection>();
            var collections = await Core.Services.EntryCollectionService.GetAllCollectionsAsync();
            if (collections != null && collections.Any())
            {
                foreach (var c in collections)
                {
                    EntryCollections.Add(await EntryCollection.CreateBaseAsync(c));
                }
            }
        }

        private void UpdateSuggestions()
        {
            if (EntryCollections != null)
            {
                if (!string.IsNullOrEmpty(SuggestText))
                {
                    Suggestions = EntryCollections.Where(p => p.Title.Contains(SuggestText)).ToList();
                }
                else
                {
                    Suggestions = null;
                }
            }
        }

        partial void OnSuggestTextChanged(string oldValue, string newValue)
        {
            UpdateSuggestions();
        }

        [RelayCommand]
        private void AddNewCollection()
        {
            if (!string.IsNullOrWhiteSpace(NewCollectionTitle))
            {
                EntryCollectionDb entryCollectionDb = new EntryCollectionDb()
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
            }
        }

        [RelayCommand]
        private void CollectionDetail(EntryCollection selectedItem)
        {
            if (selectedItem != null)
            {
                // TODO: 导航到 EntryCollectionDetailPage
            }
        }

        [RelayCommand]
        private void Edit(EntryCollection selectedItem)
        {
        }

        [RelayCommand]
        private async Task Remove(EntryCollection selectedItem)
        {
            if (selectedItem != null)
            {
                // TODO: 显示确认对话框
                Core.Services.EntryCollectionService.RemoveCollection(selectedItem.Id);
                EntryCollections.Remove(selectedItem);
            }
        }

        [RelayCommand]
        private void Refresh()
        {
            InitAsync();
        }

        [RelayCommand]
        private void SuggestionChosen(EntryCollection item)
        {
            CollectionDetailCommand.Execute(item);
            SuggestText = null;
        }
    }
}
