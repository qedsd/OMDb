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

namespace OMDb.WinUI3.ViewModels
{
    public partial class LabelPropertyViewModel : ObservableObject
    {

        public LabelPropertyViewModel(string parentId)
        {
            Init(parentId);
        }



        private ObservableCollection<LabelPropertyTree> _current_LPEZ_Link = new ObservableCollection<LabelPropertyTree>();
        public ObservableCollection<LabelPropertyTree> Current_LPEZ_Link
        {
            get => _current_LPEZ_Link;
            set => SetProperty(ref _current_LPEZ_Link, value);
        }


        private ObservableCollection<LabelPropertyDb> _dtData = new ObservableCollection<LabelPropertyDb>();
        public ObservableCollection<LabelPropertyDb> DtData
        {
            get => _dtData;
            set => SetProperty(ref _dtData, value);
        }

        public async void Init(string parentId)
        {
            var labelPropertyList = await Core.Services.LabelPropertyService.GetAllLabelPropertyAsync(DbSelectorService.dbCurrentId);
            var linkIdList = Core.Services.LabelPropertyService.GetLinkId(parentId);
            if (labelPropertyList == null) return;

            Dictionary<string, LabelPropertyTree> labelPropertyTreeDic = new Dictionary<string, LabelPropertyTree>();
            var root = labelPropertyList.Where(p => p.ParentId == null).Where(a => linkIdList.Contains(a.LPID)).ToList();
            if (root == null) return;

            foreach (var labelPropertyHeader in root)//新增树头
                labelPropertyTreeDic.Add(labelPropertyHeader.LPID, new LabelPropertyTree(labelPropertyHeader));

            foreach (var labelProperty in labelPropertyList)
            {
                if (labelProperty.ParentId == null)//属性标题->跳过
                    continue;

                if (labelPropertyTreeDic.TryGetValue(labelProperty.ParentId, out var parent)) //属性数据->添加至父树
                    parent.Children.Add(new LabelPropertyTree(labelProperty));

            }
            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                LabelPropertyTrees = new ObservableCollection<LabelPropertyTree>();
                foreach (var item in labelPropertyTreeDic)
                    LabelPropertyTrees.Add(item.Value);
            });

        }

    }
}
