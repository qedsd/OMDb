using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
namespace OMDb.WinUI3.ViewModels
{
    public class LabelViewModel : ObservableObject
    {
        private ObservableCollection<Core.DbModels.LabelDb> labels;
        public ObservableCollection<Core.DbModels.LabelDb> Labels
        {
            get => labels;
            set => SetProperty(ref labels, value);
        }
        public LabelViewModel()
        {
            Init();
        }
        private async void Init()
        {
            var labels = await Core.Services.LabelService.GetAllLabelAsync();
            if(labels != null)
            {
                Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    Labels = new ObservableCollection<Core.DbModels.LabelDb>(labels);
                });
            }
        }
        public ICommand AddCommand => new RelayCommand(async() =>
        {
            var result = await Dialogs.EditLabelDialog.ShowDialog();
            if(result != null)
            {
                if (string.IsNullOrEmpty(result.Name))
                {
                    Helpers.InfoHelper.ShowError("标签名不可为空");
                }
                else
                {
                    if (Labels == null)
                    {
                        Core.Services.LabelService.AddLabel(result);
                        Labels = new ObservableCollection<Core.DbModels.LabelDb>();
                        Labels.Add(result);
                        Helpers.InfoHelper.ShowSuccess("已保存标签");
                    }
                    else if (Labels.FirstOrDefault(p => p.Name == result.Name) != null)
                    {
                        Helpers.InfoHelper.ShowError("标签名不可重复");
                    }
                    else
                    {
                        Core.Services.LabelService.AddLabel(result);
                        Labels.Add(result);
                        Helpers.InfoHelper.ShowSuccess("已保存标签");
                    }
                }
            }
        });
        public ICommand ItemClickCommand => new RelayCommand<Core.DbModels.LabelDb>(async(item) =>
        {
            if (item != null)
            {
                var result = await Dialogs.EditLabelDialog.ShowDialog(item);
                if (result != null)
                {
                    if (string.IsNullOrEmpty(result.Name))
                    {
                        Helpers.InfoHelper.ShowError("标签名不可为空");
                    }
                    else
                    {
                        if (Labels.FirstOrDefault(p => p!=result && p.Name == result.Name) != null)
                        {
                            Helpers.InfoHelper.ShowError("标签名不可重复");
                        }
                        else
                        {
                            Core.Services.LabelService.UpdateLabel(result);
                            Labels.Remove(result);
                            Labels.Add(result);
                            Helpers.InfoHelper.ShowSuccess("已保存标签");
                        }
                    }
                }
            }
        });
        public ICommand RemoveCommand => new RelayCommand<Core.DbModels.LabelDb>(async(item) =>
        {
            if(item != null)
            {
                if(await Dialogs.QueryDialog.ShowDialog("是否确认",$"将删除{item.Name}标签"))
                {
                    Core.Services.LabelService.RemoveLabel(item.Id);
                    Labels.Remove(item);
                    Helpers.InfoHelper.ShowSuccess("已删除标签");
                }
            }
        });
    }
}
