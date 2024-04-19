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
        public LabelPropertyViewModel(LabelProperty labelPropertyReomove)
        {
            InitRemoveSelf(labelPropertyReomove);
        }

        private ObservableCollection<LabelProperty> _labelPropertyList= new ObservableCollection<LabelProperty>();
        public ObservableCollection<LabelProperty> LabelPropertyList
        {
            get => _labelPropertyList;
            set => SetProperty(ref _labelPropertyList, value);
        }

        public void InitRemoveSelf(LabelProperty labelPropertyReomove)
        {
            var labelPropertyList = Core.Services.LabelPropertyService.GetLabelPropertyHeader();
            var labelPropertyCheckList = Core.Services.LabelPropertyService.GetLinkId(labelPropertyReomove.LPDb.LPID);
            if (labelPropertyList == null) return;
            labelPropertyList.Remove(labelPropertyList.FirstOrDefault(a => a.LPID == labelPropertyReomove.LPDb.LPID));
            labelPropertyList.ForEach(a => {
                var labelProperty = new LabelProperty(a);
                labelProperty.IsChecked= labelPropertyCheckList.Contains(labelProperty.LPDb.LPID)?true:false;
                LabelPropertyList.Add(labelProperty); 
            });
        }

    }
}
