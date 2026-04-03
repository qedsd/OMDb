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
using System.ComponentModel.Design;
using OMDb.Core.DbModels;
using OMDb.Core.Utils.Extensions;
using OMDb.WinUI3.Helpers;
using OMDb.Core.DbModels.ManagerCenterDb;
using NPOI.POIFS.Properties;

namespace OMDb.WinUI3.ViewModels
{
    public partial class LabelPropertyViewModel : ObservableObject
    {
        public ICommand RefreshCommand => new RelayCommand(async () =>
        {
            await InitAsync();
            this.LabelPropertySelectionChangedCommand.Execute(null);
            Helpers.InfoHelper.ShowSuccess("刷新完成");
        });


        public ICommand RefreshCommandWithParam => new RelayCommand<string>(async (labelPropertyId) =>
        {
            await InitAsync();
            LoadLabel(labelPropertyId);
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

            this.LabelPropertySelectionChangedCommand.Execute(null);
            Helpers.InfoHelper.ShowSuccess("Export Success(导出成功)！");
        });

        public ICommand ImportCommand => new RelayCommand(async () =>
        {
            var inputPath = await Helpers.PickHelper.PickFileAsync();
            if (inputPath.Path != null && await Dialogs.QueryDialog.ShowDialog("Reminder(提示)", "Do you confirm import(确认导入)？"))
                AddLabelProperty(inputPath.Path);
            await InitAsync();
            this.LabelPropertySelectionChangedCommand.Execute(null);
            Helpers.InfoHelper.ShowSuccess("Import Success(导入成功)！");
        });



        private async void AddLabelProperty(string path)
        {
            var labelPropertyDbs = await Core.Services.LabelPropertyService.GetAllLabelPropertyAsync(DbSelectorService.dbCurrentId);
            if (System.IO.File.Exists(path))
            {
                string json = System.IO.File.ReadAllText(path);
                var collection = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
                var labelPropertyDbList = new List<LabelPropertyDb>();
                foreach (var item in collection)
                {
                    var property = item["Property"] as string;
                    var data = JsonConvert.DeserializeObject<List<string>>(item["Data"].ToString());
                    if (this.LabelPropertyTreeCollection.Select(a => a.LabelProperty.Name).Contains(property))
                    {
                        var labelPropertyTree = this.LabelPropertyTreeCollection.Where(a => a.LabelProperty.Name == property).FirstOrDefault();
                        var labelPropertyData = labelPropertyTree.Children.Select(a => a.LabelProperty.Name).ToArray();
                        var parentId = labelPropertyTree.LabelProperty.LPDb.LPID;
                        foreach (var propertyData in data)
                        {
                            if (!labelPropertyData.Contains(propertyData))
                            {
                                var labelPropertyDataDb = new LabelPropertyDb()
                                {
                                    Name = propertyData,
                                    ParentId = parentId,
                                    DbCenterId = DbSelectorService.dbCurrentId,
                                    Level = 1
                                };
                                labelPropertyDbList.Add(labelPropertyDataDb);
                            }
                        }
                    }
                    else
                    {
                        var parentId = Guid.NewGuid().ToString();
                        var labelPropertyDb = new LabelPropertyDb()
                        {
                            LPID = parentId,
                            Name = property,
                            ParentId = null,
                            DbCenterId = DbSelectorService.dbCurrentId,
                            Level = 1
                        };
                        labelPropertyDbList.Add(labelPropertyDb);
                        foreach (var propertyData in data)
                        {
                            var labelPropertyDataDb = new LabelPropertyDb()
                            {
                                Name = propertyData,
                                ParentId = parentId,
                                DbCenterId = DbSelectorService.dbCurrentId,
                                Level = 1
                            };
                            labelPropertyDbList.Add(labelPropertyDataDb);
                        }
                    }
                }
                Core.Services.LabelPropertyService.AddLabelProperty(labelPropertyDbList);
            }
        }

        private string GetJson()
        {
            var result = new List<Object>();
            foreach (var item in this.LabelPropertyTreeCollection)
            {
                var labelPropertyData = new List<string>();
                foreach (var data in item.Children)
                {
                    labelPropertyData.Add(data.LabelProperty.Name);
                }
                var labelProperty = new
                {
                    Property = item.LabelProperty.Name,
                    Data = labelPropertyData,
                };
                result.Add(labelProperty);
            }
            JsonSerializerSettings settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
            string json = JsonConvert.SerializeObject(result, settings);
            return json;
        }
    }
}
