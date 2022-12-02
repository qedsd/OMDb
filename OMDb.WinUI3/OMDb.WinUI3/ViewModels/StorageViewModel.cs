using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels
{
    public class StorageViewModel
    {
        public ObservableCollection<EnrtyStorage> EnrtyStorages { get; set; } = Services.ConfigService.EnrtyStorages;
        private EnrtyStorage enrtyStorage;
        public EnrtyStorage EnrtyStorage
        {
            get=> enrtyStorage;
            set
            {
                enrtyStorage = value;
            }
        }
        public StorageViewModel()
        {
            MyControls.StorageCard.AddStorageEvent += StorageCard_AddStorageEvent;
            MyControls.StorageCard.RemoveStorageEvent += StorageCard_RemoveStorageEvent;
        }

        private void StorageCard_RemoveStorageEvent(EnrtyStorage enrtyStorage)
        {
            EnrtyStorages.Remove(enrtyStorage);
            Core.Config.RemoveDb(enrtyStorage.StorageName);
            Services.ConfigService.Save();
            Services.ConfigService.LoadStorages();
        }

        private async void StorageCard_AddStorageEvent(EnrtyStorage enrtyStorage)
        {
            if(enrtyStorage != null)
            {
                if(enrtyStorage.StoragePath.EndsWith(".db"))
                {
                    //添加已有数据库
                    if (EnrtyStorages.FirstOrDefault(p => p.StorageName == enrtyStorage.StorageName) != null)
                    {
                        await Dialogs.MsgDialog.ShowDialog("存在重名仓库");
                    }
                    else
                    {
                        bool needCodeFirst = !System.IO.File.Exists(enrtyStorage.StoragePath);
                        if(Core.Config.AddDbFile(enrtyStorage.StoragePath, enrtyStorage.StorageName, needCodeFirst))
                        {
                            Helpers.InfoHelper.ShowSuccess("创建成功");
                            if(!needCodeFirst)
                            {
                                enrtyStorage.EntryCount = await Core.Services.EntryService.QueryEntryCountAsync(enrtyStorage.StorageName);
                            }
                            EnrtyStorages.Insert(EnrtyStorages.Count - 1, enrtyStorage);
                            Services.ConfigService.Save();
                            Services.ConfigService.LoadStorages();
                        }
                        else
                        {
                            await Dialogs.MsgDialog.ShowDialog("添加失败");
                        }
                    }
                }
            }
        }
        public ICommand ItemClickCommand => new RelayCommand<EnrtyStorage>(async(item) =>
        {
            if (item != null && !string.IsNullOrEmpty(item.StoragePath))
            {
                Services.NavigationService.Navigate(typeof(Views.EntryPage), item.StorageName);
                await EntryViewModel.Current.UpdateEntryListAsync();
            }
        });
    }
}
