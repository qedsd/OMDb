using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class LabelTree : ObservableObject
    {
        private Core.DbModels.LabelClassDb label;
        public Core.DbModels.LabelClassDb Label
        {
            get => label;
            set => SetProperty(ref label, value);
        }

        private ObservableCollection<LabelTree> children;
        public ObservableCollection<LabelTree> Children
        {
            get => children;
            set => SetProperty(ref children, value);
        }

        public LabelTree() { }
        public LabelTree(Core.DbModels.LabelClassDb labelDb) 
        { 
            label = labelDb;
            children = new ObservableCollection<LabelTree>();
        }
    }
}
