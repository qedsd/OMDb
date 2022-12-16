using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConsoleDemo.Helper;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using OMDb.Core.DbModels;
using OMDb.WinUI3.Models;
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
    public class StorageViewModel : ObservableObject
    {
        public ObservableCollection<EnrtyStorage> enrtyStorages;
        public ObservableCollection<EnrtyStorage> EnrtyStorages
        {
            get => enrtyStorages;
            set => SetProperty(ref enrtyStorages, value);
        }
        private EnrtyStorage enrtyStorage;
        public EnrtyStorage EnrtyStorage
        {
            get => enrtyStorage;
            set
            {
                enrtyStorage = value;
            }
        }
        public StorageViewModel()
        {
            Init();
            MyControls.StorageCard.AddStorageEvent += StorageCard_AddStorageEvent;
            MyControls.StorageCard.RemoveStorageEvent += StorageCard_RemoveStorageEvent;
        }
        public ICommand RefreshCommand => new RelayCommand(() =>
        {
            Init();
            Helpers.InfoHelper.ShowMsg("刷新完成");
        });
        public ICommand AddCommand => new RelayCommand(async () =>
        {
            var ents = await Dialogs.EditStorageDialog.ShowDialog();
            StorageCard_AddStorageEvent(ents);
            Init();
        });

        private void StorageCard_RemoveStorageEvent(EnrtyStorage enrtyStorage)
        {
            EnrtyStorages.Remove(enrtyStorage);
            Core.Config.RemoveDb(enrtyStorage.StorageName);
            Services.ConfigService.LoadStorages();
            Core.Services.StorageService.RemoveStorage(enrtyStorage.StorageName);
            Init();
        }

        public async void Init()
        {
            var lstStorage = await Core.Services.StorageService.GetAllStorageAsync();
            if (EnrtyStorages != null) { EnrtyStorages.Clear(); } else { enrtyStorages = new ObservableCollection<EnrtyStorage>(); }
            if (lstStorage != null)
            {
                foreach (var item in lstStorage)
                {
                    EnrtyStorage enrtyStorage = new EnrtyStorage();
                    enrtyStorage.StorageName = item.StorageName;
                    enrtyStorage.StoragePath = item.StoragePath;
                    enrtyStorage.CoverImg = item.CoverImg;
                    enrtyStorage.EntryCount = (int)item.EntryCount;
                    enrtyStorages.Add(enrtyStorage);
                }
            }
        }

        private async void StorageCard_AddStorageEvent(EnrtyStorage enrtyStorage)
        {
            if (enrtyStorage != null)
            {
                try
                {
                    var storagePathFolder = enrtyStorage.StoragePath.SubString02B(@"\", 1, false);
                    bool isPathCorrect_Storage = Directory.Exists(storagePathFolder);
                    if (!isPathCorrect_Storage)
                    {
                        await Dialogs.MsgDialog.ShowDialog("添加失败，仓库路径有误");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    await Dialogs.MsgDialog.ShowDialog("添加失败，仓库路径有误");
                    return;
                }
                bool isPathCorrect_Cover = File.Exists(enrtyStorage.CoverImg);
                if (!isPathCorrect_Cover)
                {
                    await Dialogs.MsgDialog.ShowDialog("添加失败，封面路径有误");
                    return;
                }

                if (enrtyStorage.StoragePath.EndsWith(".db"))
                {
                    //添加已有数据库
                    if (EnrtyStorages.FirstOrDefault(p => p.StorageName == enrtyStorage.StorageName) != null)
                    {
                        await Dialogs.MsgDialog.ShowDialog("存在重名仓库");
                    }
                    else
                    {
                        bool needCodeFirst = !System.IO.File.Exists(enrtyStorage.StoragePath);
                        if (Core.Config.AddDbFile(enrtyStorage.StoragePath, enrtyStorage.StorageName, needCodeFirst))
                        {
                            Helpers.InfoHelper.ShowSuccess("创建成功");
                            //添加旧仓库
                            if (!needCodeFirst)
                            {
                                enrtyStorage.EntryCount = await Core.Services.EntryService.QueryEntryCountAsync(enrtyStorage.StorageName);
                                EnrtyStorages.Insert(EnrtyStorages.Count - 1, enrtyStorage);
                                StorageDb storageDb = new StorageDb()
                                {
                                    StorageName = enrtyStorage.StorageName,
                                    StoragePath = enrtyStorage.StoragePath,
                                    EntryCount = enrtyStorage.EntryCount,
                                    CoverImg = enrtyStorage.CoverImg
                                };
                                Core.Services.StorageService.AddStorage(storageDb);
                            }

                            //添加新仓库
                            else
                            {
                                EnrtyStorages.Insert(EnrtyStorages.Count - 1, enrtyStorage);
                                StorageDb storageDb = new StorageDb()
                                {
                                    StorageName = enrtyStorage.StorageName,
                                    StoragePath = enrtyStorage.StoragePath,
                                    EntryCount = 0,
                                    CoverImg = enrtyStorage.CoverImg
                                };
                                Core.Services.StorageService.AddStorage(storageDb);
                            }

                            //Services.ConfigService.Save();
                        }
                        else
                        {
                            await Dialogs.MsgDialog.ShowDialog("添加失败");
                        }
                    }
                }
                Init();
            }
        }

        public ICommand ItemClickCommand => new RelayCommand<EnrtyStorage>(async (item) =>
        {
            if (item != null && !string.IsNullOrEmpty(item.StoragePath))
            {
                Services.NavigationService.Navigate(typeof(Views.EntryPage), item.StorageName);
                await EntryViewModel.Current.UpdateEntryListAsync();
            }
        });
    }
}
