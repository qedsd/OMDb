using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Services;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI;
using System.Xml.Linq;
using Microsoft.UI.Xaml;
using System.IO;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using OMDb.WinUI3.Services.Settings;
using OMDb.Core.Utils.Extensions;
using OMDb.WinUI3.Helpers;

namespace OMDb.WinUI3.ViewModels
{
    public partial class LabelViewModel : ObservableObject
    {
        public ICommand TestMeCommand => new RelayCommand(() =>
        {
            InitAsync();
        });

        /// <summary>
        /// 更变显示样式Exp
        /// </summary>
        public ICommand ChangeShowTypeToExpCommand => new RelayCommand(() =>
        {
            IsExpShow = true;
            IsTreeShow = false;
            IsRepeaterShow = false;
        });
        /// <summary>
        /// 更变显示样式Tree
        /// </summary>
        public ICommand ChangeShowTypeToTreeCommand => new RelayCommand(() =>
        {
            IsExpShow = false;
            IsTreeShow = true;
            IsRepeaterShow = false;
        });
        /// <summary>
        /// 更变显示样式Repeater
        /// </summary>
        public ICommand ChangeShowTypeToRepeaterCommand => new RelayCommand(() =>
        {
            IsExpShow = false;
            IsTreeShow = false;
            IsRepeaterShow = true;
        });
        /// <summary>
        /// 设置字体
        /// </summary>
        public ICommand SetFontFamilyCommand => new RelayCommand<string>((string font) =>
        {
            FontFamilyCurrent = font;
        });


        public ICommand RefreshCommand => new RelayCommand(async () =>
        {
            InitAsync();
            Helpers.InfoHelper.ShowSuccess("刷新完成");
        });
        public ICommand AddRootCommand => new RelayCommand(async () =>
        {
            var result = await Dialogs.EditLabelDialog.ShowDialog(true);
            if (result != null)
            {
                if (string.IsNullOrEmpty(result.Name))
                {
                    Helpers.InfoHelper.ShowError("标签名不可为空");
                }
                else
                {
                    Core.Services.LabelClassService.AddLabelClass(result);
                    LabelTrees.Add(new LabelClassTree(result));
                    Helpers.InfoHelper.ShowSuccess("已保存标签");
                }
            }
        });
        public ICommand AddSubCommand => new RelayCommand<LabelClassTree>(async (parent) =>
        {
            var result = await Dialogs.EditLabelDialog.ShowDialog(false);
            if (result != null)
            {
                if (string.IsNullOrEmpty(result.Name))
                {
                    Helpers.InfoHelper.ShowError("标签名不可为空");
                }
                else
                {
                    result.ParentId = parent.LabelClass.LabelClassDb.LCID;
                    Core.Services.LabelClassService.AddLabelClass(result);
                    parent.Children.Add(new LabelClassTree(result));
                    Helpers.InfoHelper.ShowSuccess("已保存标签");
                }
            }
        });

        /// <summary>
        /// 编辑二级标签 Edit 2nd Tag 
        /// </summary>
        public ICommand EditSubCommand => new RelayCommand<LabelClassTree>(async (item) =>
        {
            if (item != null)
            {
                var result = await Dialogs.EditLabelDialog.ShowDialog(false, item.LabelClass.LabelClassDb);
                if (result != null)
                {
                    if (string.IsNullOrEmpty(result.Name))
                    {
                        Helpers.InfoHelper.ShowError("标签名不可为空");
                    }
                    else
                    {
                        Core.Services.LabelClassService.UpdateLabelClass(result);
                        var parent = LabelTrees.FirstOrDefault(p => p.LabelClass.LabelClassDb.LCID == result.ParentId);
                        if (parent != null)
                        {
                            var removeWhere = parent.Children.FirstOrDefault(t => t.LabelClass.LabelClassDb == result);
                            var index = parent.Children.IndexOf(removeWhere);
                            parent.Children.Remove(removeWhere);
                            parent.Children.Insert(index, new LabelClassTree(result));
                        }
                        Helpers.InfoHelper.ShowSuccess("已保存标签");
                    }
                }
            }
        });

        /// <summary>
        /// 编辑一级标签 Edit 1st Tag
        /// </summary>
        public ICommand EditRootCommand => new RelayCommand<LabelClassTree>(async (item) =>
        {
            if (item != null)
            {
                var result = await Dialogs.EditLabelDialog.ShowDialog(true, item.LabelClass.LabelClassDb);
                if (result != null)
                {
                    if (string.IsNullOrEmpty(result.Name))
                    {
                        Helpers.InfoHelper.ShowError("标签名不可为空");
                    }
                    else
                    {
                        Core.Services.LabelClassService.UpdateLabelClass(result);
                        var index = LabelTrees.IndexOf(item);
                        LabelTrees.Remove(item);
                        LabelTrees.Insert(index, new LabelClassTree()
                        {
                            LabelClass = new LabelClass(result),
                            Children = item.Children
                        });
                        Helpers.InfoHelper.ShowSuccess("已保存标签");
                    }
                }
            }
        });

        public ICommand RemoveCommand => new RelayCommand<LabelClassTree>(async (item) =>
        {
            if (item == null) return;

            if (await Dialogs.QueryDialog.ShowDialog("是否确认", $"将删除{item.LabelClass.LabelClassDb.Name}标签"))
            {
                this.RemoveLabelClass(item);
                Helpers.InfoHelper.ShowSuccess("已删除标签");

            }

        });

        public ICommand StyleConfirmCommand => new RelayCommand(() =>
        {
            this.SaveXml();
            InitAsync();
        });
        public ICommand StyleCancelCommand => new RelayCommand(() =>
        {
            InitAsync();
        });


        public ICommand InitTag1stInfoCommand => new RelayCommand(() =>
        {
            GetTag1stInfo();
            ColorCurrent = Color1st;
            FontSizeCurrent = FontSize1st;
            WidthCurrent = Width1st;
            HeightCurrent = Height1st;
        });

        public ICommand InitTag2ndInfoCommand => new RelayCommand(() =>
        {
            GetTag2ndInfo();
            ColorCurrent = Color2nd;
            FontSizeCurrent = FontSize2nd;
            WidthCurrent = Width2nd;
            HeightCurrent = Height2nd;
        });

        public ICommand ExportCommand => new RelayCommand(async () =>
        {
            var outputPath = await Helpers.PickHelper.PickSaveFileAsync($"{DbSelectorService.dbCurrentName}.json");
            if (outputPath == null) return;
            string directoryPath = Path.GetDirectoryName(outputPath.Path);

            if (!Directory.Exists(directoryPath)) InfoHelper.ShowError("The export path is incorrect(導出路徑有誤)！");
            else
            {
                //var labelPropertyDbs = await Core.Services.LabelPropertyService.GetAllLabelPropertyAsync(DbSelectorService.dbCurrentId);
                var json = GetJson();
                System.IO.File.WriteAllText(outputPath.Path, json);
            }


            Helpers.InfoHelper.ShowSuccess("Export Success(导出成功)！");
        });

        public ICommand ImportCommand => new RelayCommand(async () =>
        {
            var inputPath = await Helpers.PickHelper.PickFileAsync();
            if (inputPath.Path != null && await Dialogs.QueryDialog.ShowDialog("Reminder(提示)", "Do you confirm import(确认导入)？"))
                AddLabelClass(inputPath.Path);
            await InitAsync();

            Helpers.InfoHelper.ShowSuccess("Import Success(导入成功)！");
        });



    }
}
