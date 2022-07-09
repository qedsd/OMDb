using Newtonsoft.Json;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class EntryViewModel
    {
        public ObservableCollection<EnrtyStorage> EnrtyStorages { get; set; } = Services.ConfigService.EnrtyStorages;
        public List<Core.Models.Entry> Entries { get; set; }
        public EntryViewModel()
        {
            Init();
        }
        private async void Init()
        {
            var queryResults = await Core.Services.EntryService.QueryEntryAsync(Core.Enums.SortType.LastUpdateTime, Core.Enums.SortWay.Positive);
            if(queryResults?.Count > 0)
            {
                Entries = await Core.Services.EntryService.QueryEntryAsync(queryResults.Select(p=>p.ToQueryItem()).ToList());
            }
        }
    }
}
