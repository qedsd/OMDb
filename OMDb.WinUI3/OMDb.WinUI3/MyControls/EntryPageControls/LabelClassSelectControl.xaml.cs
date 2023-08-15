using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
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
    public sealed partial class LabelClassSelectControl : UserControl
    {
        public LabelClassSelectControl()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty LabelClassTreesProperty = DependencyProperty.Register
            (
            "LabelClassTrees",
            typeof(List<Models.LabelClassTree>),
            typeof(UserControl),
            new PropertyMetadata(null, new PropertyChangedCallback(SetLabels))
            );
        private static void SetLabels(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as LabelClassSelectControl;
            if (card != null)
            {
                card.ItemsRepeater_LabelTree.ItemsSource = e.NewValue as List<Models.LabelClassTree>;
            }
        }
        public List<Models.LabelClassTree> LabelClassTrees
        {
            get { return (List<Models.LabelClassTree>)GetValue(LabelClassTreesProperty); }
            set { SetValue(LabelClassTreesProperty, value); }
        }

        private void Button_ConfirmAddLabels_Click(object sender, RoutedEventArgs e)
        {
            DoneEvent?.Invoke(true, LabelClassTrees);
        }

        private void Button_CancelAddLabels_Click(object sender, RoutedEventArgs e)
        {
            DoneEvent?.Invoke(false, LabelClassTrees);
        }
        public delegate void DoneDelegate(bool confirm, List<Models.LabelClassTree> LabelClassTrees);
        public event DoneDelegate DoneEvent;

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var labelClass = (sender as FrameworkElement)?.Tag as LabelClass;
            labelClass.IsChecked = !labelClass.IsChecked;
        }
    }
}
