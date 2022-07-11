using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels
{
    public class EditEntryViewModel: ObservableObject
    {
        public Core.Models.Entry Entry { get; set; }
        public List<Models.EntryName> EntryNames { get; set; }
        public Models.EntryName EntryName { get; set; }
        private string entryPath;
        public string EntryPath
        {
            get => entryPath;
            set
            { 
                SetProperty(ref entryPath, value);
                Entry.Path = value;
            }
        }
        public List<Models.EnrtyStorage> EnrtyStorages { get; set; } = Services.ConfigService.EnrtyStorages.Where(p => p.StoragePath != null).ToList();
        private Models.EnrtyStorage selectedEnrtyStorage;
        public Models.EnrtyStorage SelectedEnrtyStorage
        {
            get => selectedEnrtyStorage;
            set
            {
                SetProperty(ref selectedEnrtyStorage, value);
                if (Entry!=null&&!string.IsNullOrEmpty(Entry.Name))
                {
                    Entry.Path = Path.Combine(Path.GetDirectoryName(selectedEnrtyStorage.StoragePath), Services.ConfigService.DefaultEntryFolder, Entry.Name);
                }
            }
        }

        public EditEntryViewModel()
        {
            EntryNames = new List<Models.EntryName>();
            foreach (Core.Enums.LangEnum p in Enum.GetValues(typeof(Core.Enums.LangEnum)))
            {
                EntryNames.Add(new Models.EntryName()
                {
                    Lang = p,
                    IsDefault = p == Core.Enums.LangEnum.zh_CN,
                });
            }
            SelectedEnrtyStorage = EnrtyStorages.FirstOrDefault();
        }

        public ICommand CheckDefaultCommand => new RelayCommand<Models.EntryName>((selected) =>
        {
            if (selected != null)
            {
                EntryNames.ForEach(p =>
                {
                    if (p.IsDefault && p != selected)
                    {
                        p.IsDefault = false;
                    }
                });
            }
        });
    }
}
