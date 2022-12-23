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
        /// <summary>
        /// 列表显示
        /// </summary>
        public bool IsShowByTree
        {
            get => isShowByTree;
            set => SetProperty(ref isShowByTree, value);
        }
        private bool isShowByTree;

        public bool IsShowByExpander
        {
            get => isShowByExpander;
            set => SetProperty(ref isShowByExpander, value);
        }
        private bool isShowByExpander;

        private ObservableCollection<LabelTree> labelTrees;
        public ObservableCollection<LabelTree> LabelTrees
        {
            get => labelTrees;
            set => SetProperty(ref labelTrees, value);
        }

        
        public LabelViewModel()
        {
            Init();
            IsShowByTree = false;
            IsShowByExpander = true;
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
                            parent.Children.Add(new LabelTree(label));
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

        public ICommand ShowTreeCommand => new RelayCommand(() =>
        {
            IsShowByTree = true;
            IsShowByExpander = false;
        });

        public ICommand ShowExpanderCommand => new RelayCommand(() =>
        {
            IsShowByTree = false;
            IsShowByExpander = true;
        });


        //刷新
        public ICommand RefreshCommand => new RelayCommand( () =>
        {
            Init();
        });

        //新增一级标签
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
                    result.Img = (result.Img != null && result.Img.Length != 0) ? result.Img: (System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Icon-LabelDefault.png"));
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
                    result.Img = (result.Img!=null&&result.Img.Length != 0 )? result.Img : (System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Icon-LabelDefault.png"));
                    result.ParentId = parent.Label.Id;
                    Core.Services.LabelService.AddLabel(result);
                    parent.Children.Add(new LabelTree(result));
                    Helpers.InfoHelper.ShowSuccess("已保存标签");
                }
            }
        });
        public ICommand EditSubCommand => new RelayCommand<LabelTree>(async(item) =>
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
                        var parent = LabelTrees.FirstOrDefault(p => p.Label.Id == result.ParentId);
                        if(parent != null)
                        {
                            var index = parent.Children.IndexOf(new LabelTree(result));

                            foreach (var childrenLabel in parent.Children)
                            {
                                if (childrenLabel.Label.Name== item.Name)
                                {
                                    parent.Children.Remove(childrenLabel);
                                    break;
                                }
                            }
                            parent.Children.Insert(index, new LabelTree(result));
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
        public ICommand RemoveCommand => new RelayCommand<LabelTree>(async (item) =>
        {
            if (item != null)
            {
                if (await Dialogs.QueryDialog.ShowDialog("是否确认", $"将删除{item.Name}标签"))
                {
                    Core.Services.LabelService.RemoveLabel(item.Label.Id);
                    if(item.Label.ParentId != null)//子类
                    {
                        var parent = LabelTrees.FirstOrDefault(p => p.Label.Id == item.Label.ParentId);
                        if(parent != null)
                        {
                            foreach (var childrenLabel in parent.Children)
                            {
                                if (childrenLabel.Label.Name == item.Name)
                                {
                                    parent.Children.Remove(childrenLabel);
                                    break;
                                }
                            }
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
