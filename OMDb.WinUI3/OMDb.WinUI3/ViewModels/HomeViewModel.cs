using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels
{
    public class HomeViewModel
    {
        public ICommand AddEntryCommand => new RelayCommand(async() =>
        {
            if (Services.ConfigService.EnrtyStorages.Count == 0)
            {
                await Dialogs.MsgDialog.ShowDialog("请先创建仓库");
            }
            else
            {
                await Services.EntryService.AddEntryAsync();
            }
        });
        
    }
}
