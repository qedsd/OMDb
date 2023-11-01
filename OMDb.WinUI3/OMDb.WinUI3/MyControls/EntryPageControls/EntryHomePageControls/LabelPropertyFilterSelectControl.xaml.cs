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
using System.Windows.Input;
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
            GridView_Current_LPEZCollection.ItemsSource = ((LabelPropertyTree)e.AddedItems[0]).Children as ObservableCollection<Models.LabelPropertyTree>;
            //CallChanged();
        }


        private void CallChanged()
        {
            var ls = LabelPropertyTrees.ToList();
            CheckChangedCommand?.Execute(ls);
        }

        public delegate void CheckChangedEventHandel(IEnumerable<Models.LabelPropertyTree> allItems);
        private CheckChangedEventHandel CheckChanged;

        public static readonly DependencyProperty CheckChangedCommandProperty
           = DependencyProperty.Register(
               nameof(CheckChangedCommand),
               typeof(string),
               typeof(LabelPropertyFilterSelectControl),
               new PropertyMetadata(string.Empty));

        public ICommand CheckChangedCommand
        {
            get => (ICommand)GetValue(CheckChangedCommandProperty);
            set => SetValue(CheckChangedCommandProperty, value);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

            ((OMDb.WinUI3.Models.LabelPropertyTree)((Microsoft.UI.Xaml.FrameworkElement)sender).DataContext).LabelProperty.IsChecked = Convert.ToBoolean(((Microsoft.UI.Xaml.Controls.Primitives.ToggleButton)sender).IsChecked);
            foreach (var lpt1st in LabelPropertyTrees)
            {
                foreach (var lpt2ndt in lpt1st.Children)
                {
                    if (lpt2ndt.LabelProperty.LPDb.LPID == ((OMDb.WinUI3.Models.LabelPropertyTree)((Microsoft.UI.Xaml.FrameworkElement)sender).DataContext).LabelProperty.LPDb.LPID)
                        lpt2ndt.LabelProperty.IsChecked = Convert.ToBoolean(((Microsoft.UI.Xaml.Controls.Primitives.ToggleButton)sender).IsChecked);
                }
            }
            CallChanged();
        }
    }
}
