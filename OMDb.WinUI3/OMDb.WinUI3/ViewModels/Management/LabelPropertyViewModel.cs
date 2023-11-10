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
        private ObservableCollection<LabelPropertyTree> _labelPropertyTrees;
        public ObservableCollection<LabelPropertyTree> LabelPropertyTrees
        {
            get => _labelPropertyTrees;
            set => SetProperty(ref _labelPropertyTrees, value);
        }

        private ObservableCollection<LabelPropertyTree> _currentLabelPropertyDataCollection = new ObservableCollection<LabelPropertyTree>();
        public ObservableCollection<LabelPropertyTree> CurrentLabelPropertyDataCollection
        {
            get => _currentLabelPropertyDataCollection;
            set => SetProperty(ref _currentLabelPropertyDataCollection, value);
        }


        private LabelPropertyTree _currentLabelPropertyTree = null;
        public LabelPropertyTree CurrentLabelPropertyTree
        {
            get => _currentLabelPropertyTree;
            set => SetProperty(ref _currentLabelPropertyTree, value);
        }
        
        public ICommand LabelPropertySelectionChangedCommand => new RelayCommand<LabelPropertyTree>((labelPropertyTree) =>
        {
            if (labelPropertyTree != null)
                CurrentLabelPropertyDataCollection = labelPropertyTree.Children;
            else if(LabelPropertyTrees.Count>0)
                CurrentLabelPropertyDataCollection = LabelPropertyTrees.FirstOrDefault().Children;
        });
    }
}
