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

namespace OMDb.WinUI3.ViewModels
{
    public partial class LabelPropertyViewModel : ObservableObject
    {
        public LabelPropertyViewModel()
        {
            InitAsync();
            if (!this.LabelPropertyTreeCollection.IsNullOrEmptyOrWhiteSpazeOrCountZero())
            {
                this.CurrentLabelPropertyTree = this.LabelPropertyTreeCollection.FirstOrDefault();
                this.LabelPropertySelectionChangedCommand.Execute(CurrentLabelPropertyTree);

            }
        }



        //初始化
        public async Task InitAsync()
        {
            var labelPropertyList = await Core.Services.LabelPropertyService.GetAllLabelPropertyAsync(DbSelectorService.dbCurrentId);
            if (labelPropertyList == null) return;

            Dictionary<string, LabelPropertyTree> dicLpdbs = new Dictionary<string, LabelPropertyTree>();
            var root = labelPropertyList.Where(p => p.ParentId == null).ToList();
            if (root == null) return;

            foreach (var labelPropertyHeader in root)
            {
                dicLpdbs.Add(labelPropertyHeader.LPID, new LabelPropertyTree(labelPropertyHeader));
            }//标头
            foreach (var labelProperty in labelPropertyList)
            {
                if (labelProperty.ParentId != null)
                {
                    if (dicLpdbs.TryGetValue(labelProperty.ParentId, out var parent))
                    {
                        parent.Children.Add(new LabelPropertyTree(labelProperty));
                    }
                }
            }//造树

            //Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>{ });

            LabelPropertyTreeCollection = new ObservableCollection<LabelPropertyTree>();
            foreach (var item in dicLpdbs)
                LabelPropertyTreeCollection.Add(item.Value);
        }

        public void LoadLabel(string labelPropertyId)
        {
            CurrentLabelPropertyDataCollection = LabelPropertyTreeCollection.FirstOrDefault(a => a.LabelProperty.LPDb.LPID == labelPropertyId)?.Children;
        }





    }
}
