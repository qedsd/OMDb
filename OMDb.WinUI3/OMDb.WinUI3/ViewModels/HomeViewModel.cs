using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
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
            if(Services.ConfigService.EnrtyStorages.Count == 0)
            {
                await Dialogs.MsgDialog.ShowDialog("请先创建仓库");
            }
            else
            {
                var entry = await Dialogs.EditEntryDialog.ShowDialog();
                if (entry != null)
                {
                    if (System.IO.Directory.Exists(entry.Path))
                    {
                        int i = 1;
                        while (true)
                        {
                            entry.Path = $"{entry.Path}({i}";
                            if (!System.IO.Directory.Exists(entry.Path))
                            {
                                break;
                            }
                        }
                    }
                    //创建文件夹、元文件、复制封面
                }
            }
        });
    }
}
