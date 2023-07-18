using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class EntrySortInfoResult : ObservableObject
    {
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _sortType;
        public string SortType
        {
            get => _sortType;
            set => SetProperty(ref _sortType, value);
        }

        public List<string> SortTypes= new List<string>() { "升序","降序"};

        public EntrySortInfoResult(string title) 
        {
            _title = title;
            _sortType = "升序";
        }
    }
}
