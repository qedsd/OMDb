// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.ViewModels;
using OMDb.WinUI3.ViewModels.Controls;
using System.Runtime.Serialization.Formatters;
using OMDb.Core.Enums;
using OMDb.WinUI3.Models;
using Microsoft.UI.Xaml.Shapes;
using OMDb.Core.Utils;
using System.Collections.ObjectModel;
using OMDb.Core.Utils.Extensions;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OMDb.WinUI3.MyControls
{
    public sealed partial class ExplorerControl : UserControl
    {
        public ExplorerViewModel VM = new ExplorerViewModel();
        public ExplorerControl()
        {
            this.InitializeComponent();
        }

        private async void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

            var targetFile = (e.OriginalSource as FrameworkElement).DataContext as ExplorerItem;
            if (targetFile == null)
            {
                return;
            }
            var result = targetFile.Children;
            if (targetFile.Children!=null)
                VM.CurrentFileInfos = new ObservableCollectionEx<ExplorerItem>(result);
            else
                VM.CurrentFileInfos = new ObservableCollectionEx<ExplorerItem>();
            VM.PathStack.Add(targetFile.Name);


            /*var targetFolder = (this.MainFrame.DataContext as MainPageViewModel).CurrentExplorerItem.Children.FirstOrDefault(c => c.Name == targetFile.FileName);
            if (targetFolder != null)
            {
                (this.MainFrame.DataContext as MainPageViewModel).NavigationTo(targetFolder);

            }*/

        }

        private void BreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs e)
        {
            var oldPathStack = VM.PathStack.DepthClone<ObservableCollection<string>>();
            VM.PathStack.Clear();
            var startIndex = 0;
            foreach (var item in oldPathStack)
            {
                VM.PathStack.Add(item);
                startIndex++;
                if (startIndex > e.Index)
                    break;
            }
            var currentFolders = GetCurrentExplorerItems(VM.PathStack,VM.Root);
            VM.CurrentFileInfos = new ObservableCollectionEx<ExplorerItem>(currentFolders);
        }

        private List<ExplorerItem> GetCurrentExplorerItems(ObservableCollection<string> pathStack,ExplorerItem root)
        {
            if (pathStack.Count == 1)
                return root.Children;

            var currentFolders=new List<ExplorerItem>();
            var index = 0;
            foreach (var item in pathStack)
            {
                index++;
                if (index==1)
                {
                    currentFolders=root.Children;
                    index++;
                    continue;
                }
                currentFolders = currentFolders.Where(a => a.Name == item).FirstOrDefault().Children;
            }

            return currentFolders;
        }

        private void btn_up_click(object sender, RoutedEventArgs e)
        {
            if (VM.PathStack.Count <= 1) return;
            VM.PathStack.RemoveAt(VM.PathStack.Count-1);
            var currentFolders = GetCurrentExplorerItems(VM.PathStack, VM.Root);
            VM.CurrentFileInfos = new ObservableCollectionEx<ExplorerItem>(currentFolders);
        }
    }
}
