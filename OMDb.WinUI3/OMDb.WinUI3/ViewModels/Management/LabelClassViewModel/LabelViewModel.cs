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

namespace OMDb.WinUI3.ViewModels
{
    public partial class LabelViewModel : ObservableObject
    {

        


        //初始化
        private async Task InitAsync()
        {
            ConfigPath = System.AppDomain.CurrentDomain.BaseDirectory + @"Assets/Config/LabelConfig.xml";
            xe = XElement.Load(ConfigPath);

            var labelTrees = await CommonService.GetLabelClassTrees();

            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
               LabelTrees = labelTrees.ToObservableCollection();
            });


            GetTag1stInfo();
            GetTag2ndInfo();
            ColorCurrent = TagSelector ? Color1st : Color2nd;
            FontSizeCurrent = TagSelector ? FontSize1st : FontSize2nd;
            WidthCurrent = TagSelector ? Width1st : Width2nd;
            HeightCurrent = TagSelector ? Height1st : Height2nd;
            FontFamilyCurrent = Convert.ToString(xe.Element("FontFamily").Value);
        }

        public LabelViewModel()
        {
            IsExpShow = false;
            IsTreeShow = false;
            IsRepeaterShow = true;
            InitAsync();
        }

 




    }
}
