using CommunityToolkit.Mvvm.ComponentModel;
using OMDb.Core.Extensions;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.ViewModels.Homes
{
    public class StatisticsViewModel : ObservableObject
    {
        private int storageCount;
        public int StorageCount
        {
            get => storageCount;
            set => SetProperty(ref storageCount, value);
        }

        private int entryCount;
        public int EntryCount
        {
            get => entryCount;
            set => SetProperty(ref entryCount, value);
        }

        private int labelCount;
        public int LabelCount
        {
            get => labelCount;
            set => SetProperty(ref labelCount, value);
        }

        private long storageUsedByte;
        public long StorageUsedByte
        {
            get => storageUsedByte;
            set => SetProperty(ref storageUsedByte, value);
        }

        private long storageUsableByte;
        public long StorageUsableByte
        {
            get => storageUsableByte;
            set => SetProperty(ref storageUsableByte, value);
        }

        private List<Models.StorageSize> storageSizes;
        public List<Models.StorageSize> StorageSizes
        {
            get => storageSizes;
            set => SetProperty(ref storageSizes, value);
        }

        public async Task InitAsync()
        {
            StorageCount = Services.ConfigService.EnrtyStorages.Count;
            LabelCount = await Core.Services.LabelService.GetLabelCountAsync();
            EntryCount = 0;
            foreach (var s in Services.ConfigService.EnrtyStorages)
            {
                EntryCount += await Core.Services.EntryService.QueryEntryCountAsync(s.StorageName);
            }
            StorageSizes = new List<StorageSize>();
            if (Services.ConfigService.EnrtyStorages.NotNullAndEmpty())
            {
                await Task.Run(() =>
                {
                    foreach (var s in Services.ConfigService.EnrtyStorages)
                    {
                        Helpers.PathHelper.CalFolderSize(s.StorageDirectory, out long used, out long usbale);
                        StorageSizes.Add(new StorageSize()
                        {
                            Path = s.StoragePath,
                            Name = s.StorageName,
                            UsedByte = used,
                            UsableByte = usbale
                        });
                    }
                });
                StorageUsedByte = StorageSizes.Sum(p => p.UsedByte);
                StorageUsableByte = StorageSizes.GroupBy(p => p.Path[0]).Sum(p=>p.First().UsableByte);//按盘符分组
            }
        }

        
    }
}
