using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class EnrtyRepository : ObservableObject
    {
        public string Id{ get;  set;}

        private string name;
        public string Name
        {
            get => name;
            set { SetProperty(ref name, value); }
        }
        private string path;
        public string Path
        {
            get => path;
            set { SetProperty(ref path, value); }
        }
        public string StorageDirectory
        {
            get => Path;
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
        private bool isChecked;
        public bool IsChecked
        {
            get => isChecked;
            set => SetProperty(ref isChecked, value);
        }
        public void Update(EnrtyRepository copy)
        {
            if (copy != null)
            {
                Name = copy.Name;
                Path = copy.Path;
                CoverImg = copy.CoverImg;
                EntryCount = copy.EntryCount;
            }
        }
    }
}
