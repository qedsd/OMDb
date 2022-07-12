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
        private Models.EntryName entryName;
        public Models.EntryName EntryName
        {
            get => entryName;
            set => SetProperty(ref entryName, value);
        }
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
                SelectedEntryDicPath = string.Empty;//更换仓库时重置已选的词条路径
                if (EntryName != null&&!string.IsNullOrEmpty(EntryName.Name))
                {
                    SelectedEntryDicPath = System.IO.Path.Combine(Path.GetDirectoryName(selectedEnrtyStorage.StoragePath),Services.ConfigService.DefaultEntryFolder);//重置为默认路径
                    SetEntryPath(EntryName.Name);
                }
            }
        }
        private string selectedEntryDicPath = string.Empty;
        /// <summary>
        /// 文件夹选取的词条存储路径
        /// 完整路径，必须在选中的仓库路径下
        /// </summary>
        public string SelectedEntryDicPath
        {
            get => selectedEntryDicPath;
            set
            {
                selectedEntryDicPath = value;
                SetEntryPath(EntryNames.FirstOrDefault(p=>p.IsDefault)?.Name);
            }
        }
        public void SetEntryPath(string name)
        {
            if (SelectedEntryDicPath != null && !string.IsNullOrEmpty(name))
            {
                EntryPath = System.IO.Path.Combine(SelectedEntryDicPath, name);
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
            EntryName = EntryNames.FirstOrDefault();
            SelectedEnrtyStorage = EnrtyStorages.FirstOrDefault();
        }

        public ICommand CheckDefaultCommand => new RelayCommand(() =>
        {
            if (EntryName != null)
            {
                EntryNames.ForEach(p =>
                {
                    if (p.IsDefault && p != EntryName)
                    {
                        p.IsDefault = false;
                    }
                });
                SetEntryPath(EntryName.Name);
            }
        });
    }
}
