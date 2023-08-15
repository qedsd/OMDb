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
        private EntrySortInfoTree _esit;
        public EntrySortInfoTree ESIT
        {
            get => _esit;
            set => SetProperty(ref _esit, value);
        }
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        
        private bool _isDescending;
        public bool IsDescending
        {
            get => _isDescending;
            set => SetProperty(ref _isDescending, value);
        }

        public EntrySortInfoResult(EntrySortInfoTree esit) 
        {
            _esit = esit;
            _title = esit.Title;
            _isDescending = true;
        }
    }
}
