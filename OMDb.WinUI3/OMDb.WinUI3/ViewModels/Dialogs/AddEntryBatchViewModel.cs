using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OMDb.WinUI3.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels
{
    public class AddEntryBatchViewModel : ObservableObject
    {

        public  AddEntryBatchViewModel()
        {
            EnrtyStorages = Services.ConfigService.EnrtyStorages.Where(p => p.StoragePath != null).ToList();
            SelectedEnrtyStorage = EnrtyStorages?.FirstOrDefault();
        }

        public ObservableCollection<Models.EntryDetail> EntryDetailCollection { get; set; }=new ObservableCollection<Models.EntryDetail>();

        public List<Models.EnrtyStorage> EnrtyStorages { get; set; }
        private Models.EnrtyStorage selectedEnrtyStorage;
        public Models.EnrtyStorage SelectedEnrtyStorage
        {
            get => selectedEnrtyStorage;
            set{ SetProperty(ref selectedEnrtyStorage, value);}
        }
    }
}
