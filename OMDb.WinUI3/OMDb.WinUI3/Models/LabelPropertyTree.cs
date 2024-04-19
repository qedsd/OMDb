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
        private LabelProperty _labelProperty;
        public LabelProperty LabelProperty
        {
            get => _labelProperty;
            set => SetProperty(ref _labelProperty, value);
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
            _labelProperty = new LabelProperty(lpdb);
            children = new ObservableCollection<LabelPropertyTree>();
        }
    }
}
