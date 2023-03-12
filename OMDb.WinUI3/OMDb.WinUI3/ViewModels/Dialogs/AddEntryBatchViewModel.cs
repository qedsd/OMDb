using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OMDb.Core.Extensions;
using OMDb.WinUI3.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels
{
    public class AddEntryBatchViewModel : ObservableObject
    {
        public ObservableCollection<Models.EntryDetail> EntryDetailCollection { get; set; }=new ObservableCollection<Models.EntryDetail>();


    }
}
