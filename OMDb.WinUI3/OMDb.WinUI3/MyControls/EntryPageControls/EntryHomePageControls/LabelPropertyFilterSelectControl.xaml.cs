using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Utils.Extensions;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OMDb.WinUI3.MyControls
{
    /// <summary>
    /// 分类标签选择控件
    /// </summary>
    public sealed partial class LabelPropertyFilterSelectControl : UserControl
    {
        public LabelPropertyFilterSelectControl()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty LabelPropertyTreesProperty = DependencyProperty.Register
            (
            "LabelPropertyTrees",
            typeof(ObservableCollection<Models.LabelPropertyTree>),
            typeof(UserControl),
            new PropertyMetadata(null, new PropertyChangedCallback(SetLabels))
            );
        private static void SetLabels(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as LabelPropertyFilterSelectControl;
            if (card != null)
            {
                card.ListView_LabelPropertyTrees.ItemsSource = e.NewValue as ObservableCollection<Models.LabelPropertyTree>;
            }
        }
        public ObservableCollection<Models.LabelPropertyTree> LabelPropertyTrees
        {
            get { return (ObservableCollection<Models.LabelPropertyTree>)GetValue(LabelPropertyTreesProperty); }
            set { SetValue(LabelPropertyTreesProperty, value); }
        }
        private void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GridView_Current_LPEZCollection.ItemsSource = null;
        }
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridView_Current_LPEZCollection.ItemsSource = ((LabelPropertyTree)e.AddedItems[0]).Children;
        }
    }
}
