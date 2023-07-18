using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class EntrySortInfoTree : ObservableObject
    {
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private ObservableCollection<EntrySortInfoTree> _children;
        public ObservableCollection<EntrySortInfoTree> Children
        {
            get => _children;
            set => SetProperty(ref _children, value);
        }

        public EntrySortInfoTree(string title) 
        {
            _title = title;
        }
        public EntrySortInfoTree()
        {

        }
    }
}
