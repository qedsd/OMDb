using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class EnrtyStorage : ObservableObject
    {
        private string storageName;
        public string StorageName
        {
            get => storageName;
            set { SetProperty(ref storageName, value); }
        }
        private string storagePath;
        public string StoragePath
        {
            get => storagePath;
            set { SetProperty(ref storagePath, value); }
        }
        private int entryCount;
        public int EntryCount
        {
            get => entryCount;
            set { SetProperty(ref entryCount, value); }
        }
        private string coverImg;
        public string CoverImg
        {
            get => coverImg;
            set { SetProperty(ref coverImg, value); }
        }

        public void Update(EnrtyStorage copy)
        {
            if (copy != null)
            {
                StorageName = copy.StorageName;
                StoragePath = copy.StoragePath;
                CoverImg = copy.CoverImg;
                EntryCount = copy.EntryCount;
            }
        }
    }
}
