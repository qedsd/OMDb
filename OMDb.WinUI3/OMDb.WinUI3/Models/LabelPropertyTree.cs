using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class LabelPropertyTree : ObservableObject
    {
        private Core.DbModels.LabelPropertyDb _lpdb;
        public Core.DbModels.LabelPropertyDb LPDb
        {
            get => _lpdb;
            set => SetProperty(ref _lpdb, value);
        }

        private ObservableCollection<LabelPropertyTree> children;
        public ObservableCollection<LabelPropertyTree> Children
        {
            get => children;
            set => SetProperty(ref children, value);
        }

        public LabelPropertyTree() { }
        public LabelPropertyTree(Core.DbModels.LabelPropertyDb lpdb) 
        {
            LPDb = lpdb;
            children = new ObservableCollection<LabelPropertyTree>();
        }
    }
}
