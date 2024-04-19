using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using OMDb.Core.DbModels;
using OMDb.Core.Models;
using OMDb.Core.Services;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.BulkAccess;

namespace OMDb.WinUI3.ViewModels
{
    public class StorageViewModel : ObservableObject
    {
        public ObservableCollection<EnrtyRepository> enrtyRepositories;
        public ObservableCollection<EnrtyRepository> EnrtyRepositories
        {
            get => enrtyRepositories;
            set => SetProperty(ref enrtyRepositories, value);
        }
        private EnrtyRepository enrtyRepository;
        public EnrtyRepository EnrtyRepository
        {
            get => enrtyRepository;
            set
            {
                enrtyRepository = value;
            }
        }
        public StorageViewModel()
        {
            Init();
            MyControls.StorageCard.AddStorageEvent += StorageCard_AddStorageEvent;
            MyControls.StorageCard.RemoveStorageEvent += StorageCard_RemoveStorageEvent;
            //MyControls.StorageCard.RefreshStorageEvent += StorageCard_RemoveStorageEvent;
        }
        public ICommand RefreshCommand => new RelayCommand(() =>
        {
            Init();
            Helpers.InfoHelper.ShowSuccess("刷新完成");
        });
        public ICommand AddCommand => new RelayCommand(async () =>
        {
            var ents = await Dialogs.EditStorageDialog.ShowDialog();
            StorageCard_AddStorageEvent(ents);
            Init();
        });

        private void StorageCard_RemoveStorageEvent(EnrtyRepository enrtyRepository)
        {
            EnrtyRepositories.Remove(enrtyRepository);
            Core.Config.RemoveDb(enrtyRepository.Name);           
            Core.Services.RepositoryService.RemoveRepository(enrtyRepository.Id);
            Services.ConfigService.LoadStorages();
            Init();
        }

        public async void Init()
        {
            var lstStorage = await RepositoryService.GetAllRepositoryAsync();
            if (EnrtyRepositories != null) { EnrtyRepositories.Clear(); } else { EnrtyRepositories = new ObservableCollection<EnrtyRepository>(); }
            if (lstStorage != null)
            {
                foreach (var item in lstStorage)
                {
                    EnrtyRepository enrtyStorage = new EnrtyRepository();
                    enrtyStorage.Name = item.Name;
                    enrtyStorage.Path = item.Path;
                    enrtyStorage.CoverImg = item.CoverImg;
                    enrtyStorage.EntryCount = (int)item.EntryCount;
                    EnrtyRepositories.Add(enrtyStorage);
                }
            }
        }

        private async void StorageCard_AddStorageEvent(EnrtyRepository enrtyStorage)
        {
            if (enrtyStorage != null)
            {
                bool isPathCorrect_Storage = Directory.Exists(enrtyStorage.Path);
                if (!isPathCorrect_Storage)
                {
                    await Dialogs.MsgDialog.ShowDialog("添加失败，仓库路径有误");
                    return;
                }

                bool isPathCorrect_Cover = File.Exists(enrtyStorage.CoverImg);
                if (!isPathCorrect_Cover)
                {
                    enrtyStorage.CoverImg= Services.CommonService.GetCover();
                }

                var path_omdb = System.IO.Path.Combine(enrtyStorage.Path, ConfigService.DefaultEntryFolder);
                Directory.CreateDirectory(path_omdb);
                var path_omdb_Cover = @$"{path_omdb}\Cover{Path.GetExtension(enrtyStorage.CoverImg)}";
                if(!File.Exists(path_omdb_Cover))File.Copy(enrtyStorage.CoverImg, path_omdb_Cover);
                var path_omdb_db = $@"{path_omdb}\{ConfigService.StorageDbName}";


                try
                {
                    //添加已有数据库
                    if (EnrtyRepositories.FirstOrDefault(p => p.Name == enrtyStorage.Name) != null) { await Dialogs.MsgDialog.ShowDialog("存在重名仓库"); }
                    else
                    {
                        bool needCodeFirst = !System.IO.File.Exists(path_omdb_db);
                        if (Core.Config.AddDbFile(path_omdb_db, enrtyStorage.Name, needCodeFirst))
                        {
                            Helpers.InfoHelper.ShowSuccess("创建成功");
                            RepositoryDb repositoryDb = new RepositoryDb()
                            {
                                Name = enrtyStorage.Name,
                                Path = enrtyStorage.Path,
                                EntryCount = enrtyStorage.EntryCount,
                                CoverImg = path_omdb_Cover
                            };
                            if (!needCodeFirst)
                            {
                                repositoryDb.EntryCount = await Core.Services.EntryService.QueryEntryCountAsync(enrtyStorage.Name);
                            }
                            RepositoryService.AddRepository(repositoryDb);
                        }
                        else
                        {
                            await Dialogs.MsgDialog.ShowDialog("添加失败");
                        }
                    }
                }
                catch (Exception e)
                {
                    await Dialogs.MsgDialog.ShowDialog($"添加失败!{e.Message}");
                }
                Init();
            }
        }
    }
}
