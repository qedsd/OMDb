using Microsoft.Toolkit.Mvvm.ComponentModel;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.ViewModels
{
    public class EntryDetailViewModel : ObservableObject
    {
        private EntryDetail entry;
        public EntryDetail Entry
        {
            get => entry;
            set
            {
                SetProperty(ref entry, value);
            }
        }
        public EntryDetailViewModel(EntryDetail entry)
        {
            Entry = entry;
        }
    }
}
