using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Extensions;
using OMDb.WinUI3.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.MyControls
{
    public sealed partial class LabelsProPertyControl : UserControl
    {
        public LabelsProPertyControl()
        {
            this.InitializeComponent();
        }


        public static readonly DependencyProperty LabelPropertyCollectionProperty = DependencyProperty.Register
            (
                "LabelPropertyCollection",
                typeof(IEnumerable<Models.LabelProperty>),
                typeof(UserControl),
                new PropertyMetadata(null, new PropertyChangedCallback(SetLabelPropertyCollection))
            );
        public ObservableCollection<Models.LabelProperty> LabelPropertyCollection
        {
            get { return (ObservableCollection<Models.LabelProperty>)GetValue(LabelPropertyCollectionProperty); }

            set { SetValue(LabelPropertyCollectionProperty, value); }
        }


        private static void SetLabelPropertyCollection(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as LabelsProPertyControl;
            if (card != null)
            {
                var labels = e.NewValue as IEnumerable<Models.LabelProperty>;
                if (labels != null)
                {
                    var str= labels.Where(a=>a.IsChecked==true).Select(a=>a.LPDb.Name).ToList();
                    card.StrSelectItem.Text = string.Join("/",str);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GlobalEvent.NotifyLPSChanged(null, new LPSEventArgs(LabelPropertyCollection.Where(a => a.IsChecked == true).ToList()));
            if (LabelPropertyCollection != null)
            {
                var str = LabelPropertyCollection.Where(a => a.IsChecked == true).Select(a => a.LPDb.Name).ToList();
                this.StrSelectItem.Text = string.Join("/", str);
            }
            this.btn.Flyout.Hide();
            
        }




        public static readonly DependencyProperty LabelDbsProperty = DependencyProperty.Register
           (
           "LabelDbs",
           typeof(IEnumerable<Core.DbModels.LabelDb>),
           typeof(UserControl),
           new PropertyMetadata(null, new PropertyChangedCallback(SetLabelDbs))
           );
        private static void SetLabelDbs(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as LabelsProPertyControl;
            if (card != null)
            {
                var labelDbs = e.NewValue as IEnumerable<Core.DbModels.LabelPropertyDb>;
                if (labelDbs != null)
                {
                    card.LabelPropertyCollection = new ObservableCollection<Models.LabelProperty>(labelDbs.Select(p => new Models.LabelProperty(p)));
                }
            }
        }
        public IEnumerable<Core.DbModels.LabelDb> LabelDbs
        {
            get { return (IEnumerable<Core.DbModels.LabelDb>)GetValue(LabelDbsProperty); }

            set { SetValue(LabelDbsProperty, value); }
        }

        

        private void Flyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            foreach (var item in LabelPropertyCollection)
            {
                if (this.StrSelectItem.Text.Contains(item.LPDb.Name)) item.IsChecked = true;
                else item.IsChecked = false;
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var lpc=LabelPropertyCollection.DepthClone<ObservableCollection<Models.LabelProperty>>();
            lpc.Clear();
            foreach (var item in LabelPropertyCollection)
            {
                if (!item.IsHiden)
                {
                    lpc.Add(item);
                }
            }
            LabelPropertyCollection = lpc;
        }
    }
}
