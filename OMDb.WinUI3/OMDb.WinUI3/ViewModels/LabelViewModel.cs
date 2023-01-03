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

        private ObservableCollection<LabelTree> labelTrees;
        public ObservableCollection<LabelTree> LabelTrees
        {
            get => labelTrees;
            set => SetProperty(ref labelTrees, value);
        }
        public LabelViewModel()
        {
            Init();
        }
        private async void Init()
        {
            var labels = await Core.Services.LabelService.GetAllLabelAsync();
            if (labels != null)
            {
                Dictionary<string, LabelTree> labelsDb = new Dictionary<string, LabelTree>();
                var root = labels.Where(p => p.ParentId == null).ToList();
                if(root != null)
                {
                    foreach(var label in root)
                    {
                        labelsDb.Add(label.Id, new LabelTree(label));
                    }
                }
                foreach (var label in labels)
                {
                    if(label.ParentId != null)
                    {
                        if(labelsDb.TryGetValue(label.ParentId,out var parent))
                        {
                            parent.Children.Add(label);
                        }
                    }
                }
                Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    LabelTrees = new ObservableCollection<LabelTree>();
                    foreach(var item in labelsDb)
                    {
                        LabelTrees.Add(item.Value);
                    }
                });
            }
        }
        public ICommand RefreshCommand => new RelayCommand( () =>
        {
            Init();
        });
        public ICommand AddRootCommand => new RelayCommand(async() =>
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
                    Core.Services.LabelService.AddLabel(result);
                    LabelTrees.Add(new LabelTree(result));
                    Helpers.InfoHelper.ShowSuccess("已保存标签");
                }
            }
        });
        public ICommand AddSubCommand => new RelayCommand<LabelTree>(async (parent) =>
        {
            var result = await Dialogs.EditLabelDialog.ShowDialog();
            if (result != null)
            {
                if (string.IsNullOrEmpty(result.Name))
                {
                    Helpers.InfoHelper.ShowError("标签名不可为空");
                }
                else
                {
                    result.ParentId = parent.Label.Id;
                    Core.Services.LabelService.AddLabel(result);
                    parent.Children.Add(result);
                    Helpers.InfoHelper.ShowSuccess("已保存标签");
                }
            }
        });
        public ICommand EditSubCommand => new RelayCommand<Core.DbModels.LabelDb>(async(item) =>
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
                        Core.Services.LabelService.UpdateLabel(result);
                        var parent = LabelTrees.FirstOrDefault(p => p.Label.Id == result.ParentId);
                        if(parent != null)
                        {
                            var index = parent.Children.IndexOf(result);
                            parent.Children.Remove(item);
                            parent.Children.Insert(index, result);
                        }
                        Helpers.InfoHelper.ShowSuccess("已保存标签");
                    }
                }
            }
        });
        public ICommand EditRootCommand => new RelayCommand<LabelTree>(async (item) =>
        {
            if (item != null)
            {
                var result = await Dialogs.EditLabelDialog.ShowDialog(item.Label);
                if (result != null)
                {
                    if (string.IsNullOrEmpty(result.Name))
                    {
                        Helpers.InfoHelper.ShowError("标签名不可为空");
                    }
                    else
                    {
                        Core.Services.LabelService.UpdateLabel(result);
                        var index = LabelTrees.IndexOf(item);
                        LabelTrees.Remove(item);
                        LabelTrees.Insert(index,new LabelTree()
                        {
                            Label = result,
                            Children = item.Children
                        });
                        Helpers.InfoHelper.ShowSuccess("已保存标签");
                    }
                }
            }
        });
        public ICommand RemoveCommand => new RelayCommand<Core.DbModels.LabelDb>(async (item) =>
        {
            if (item != null)
            {
                if (await Dialogs.QueryDialog.ShowDialog("是否确认", $"将删除{item.Name}标签"))
                {
                    Core.Services.LabelService.RemoveLabel(item.Id);
                    if(item.ParentId != null)//子类
                    {
                        var parent = LabelTrees.FirstOrDefault(p => p.Label.Id == item.ParentId);
                        if(parent != null)
                        {
                            parent.Children.Remove(item);
                        }
                    }
                    else//父类
                    {
                        Init();
                    }
                    Helpers.InfoHelper.ShowSuccess("已删除标签");
                }
            }
        });
    }
}
