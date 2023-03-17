using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Extensions;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Services.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;

namespace OMDb.WinUI3.Views
{
    public sealed partial class LabelPropertyPage : Page
    {
        public ViewModels.LabelPropertyViewModel VM { get; set; } = new ViewModels.LabelPropertyViewModel();
        public LabelPropertyPage()
        {
            this.InitializeComponent();
            DataContext = VM;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.Current_LPEZCollection.Clear();
            foreach (var item in e.AddedItems)
            {
                var lp_Baba = ((LabelPropertyTree)item);
                foreach (var lp in lp_Baba.Children)
                {
                    VM.Current_LPEZCollection.Add(lp);
                }
            }
        }

        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.TabView_LPEZ_Link.TabItems.Clear();
            VM.Current_LPEZ_Link.Clear();
            LabelPropertyTree current_LPEZ = (LabelPropertyTree)e.AddedItems.FirstOrDefault();
            if (current_LPEZ == null)
            {
                this.Grid_LPEZ_Link.Visibility = Visibility.Collapsed;
                return;
            } 
            var lst_LK = Core.Services.LabelPropertyService.GetLKId(DbSelectorService.dbCurrentId, current_LPEZ.LPDb.LPId);
            if (!(lst_LK.Count>0))
            {
                this.Grid_LPEZ_Link.Visibility = Visibility.Collapsed;
                return;
            }

            foreach (var lpbaba in VM.LabelPropertyTrees)
            {
                var lpRoot = new LabelPropertyTree(lpbaba.LPDb);
                foreach (var lpez in lpbaba.Children)
                {
                    if (lst_LK.Contains(lpez.LPDb.LPId))
                    {
                        var lpChild = new LabelPropertyTree(lpez.LPDb);
                        lpRoot.Children.Add(lpChild);
                    }
                }
                if (lpRoot.Children.Count > 0)
                { 
                    VM.Current_LPEZ_Link.Add(lpRoot);
                }
            }
            foreach (var lpbaba in VM.Current_LPEZ_Link)
            {
                var tbi = new TabViewItem();
                tbi.Header = lpbaba.LPDb.Name;
                var lv = new ListView();
                foreach (var lpez in lpbaba.Children)
                {
                    var lvi = new ListViewItem();
                    lvi.Content = lpez.LPDb.Name;
                    lv.Items.Add(lvi);
                }
                tbi.Content= lv;
                this.TabView_LPEZ_Link.TabItems.Add(tbi);
            }
            if (VM.Current_LPEZ_Link.Count()>0)
            {
                this.Grid_LPEZ_Link.Visibility=Visibility.Visible;
                this.TabView_LPEZ_Link.SelectedIndex=0;
            }
            else
            {
                this.Grid_LPEZ_Link.Visibility = Visibility.Collapsed;
            }
            
        }

        private async void ADD_Property_Click(object sender, RoutedEventArgs e)
        {
            var result=await Dialogs.EditLabelPropertyDialog.ShowDialog(); 
        }
        private async void ADD_PropertyData_Click(object sender, RoutedEventArgs e)
        {
            var result = await Dialogs.EditLabelPropertyDialog.ShowDialog();
        }

        private void ADD_PropertyDataLink_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_Property_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_PropertyData_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_PropertyDataLink_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Edit_Property_Click(object sender, RoutedEventArgs e)
        {
            var item = (LabelPropertyTree)this.ListView_LabelPropertyTrees.SelectedItem;
            if (item == null) return;
            var result = await Dialogs.EditLabelPropertyDialog.ShowDialog(item.LPDb);
        }

        private async void Edit_PropertyData_Click(object sender, RoutedEventArgs e)
        {
            var item=(LabelPropertyTree)this.GridView_Current_LPEZCollection.SelectedItem;
            if (item == null) return;
            var result = await Dialogs.EditLabelPropertyDialog.ShowDialog(item.LPDb);
        }
    }
}
