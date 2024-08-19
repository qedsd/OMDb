using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Shapes;
using OMDb.Core.DbModels;
using OMDb.Core.Utils.Extensions;
using OMDb.WinUI3.Dialogs;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels
{
    public partial class EntryDetailViewModel : ObservableObject
    {
        public EntryDetailViewModel(EntryDetail entry)
        {
            Entry = entry;
        }


        private EntryDetail entry;
        public EntryDetail Entry
        {
            get => entry;
            set
            {
                SetProperty(ref entry, value);
                Desc = Entry?.Metadata?.Desc;
                Rating = Entry?.Entry.MyRating?? 0;
                Names = Entry?.Names?.DepthClone<ObservableCollection<EntryName>>();
            }
        }
    }
}
