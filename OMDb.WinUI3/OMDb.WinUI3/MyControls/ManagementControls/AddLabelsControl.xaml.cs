using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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
    public sealed partial class AddLabelsControl : UserControl
    {
        public AddLabelsControl()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty LabelsProperty = DependencyProperty.Register
            (
            "Labels",
            typeof(IEnumerable<Models.LabelClass>),
            typeof(UserControl),
            new PropertyMetadata(null, new PropertyChangedCallback(SetLabels))
            );
        private static void SetLabels(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as AddLabelsControl;
            if (card != null)
            {
                card.ListBox_Labels.ItemsSource = e.NewValue as IEnumerable<Models.LabelClass>;
            }
        }
        public IEnumerable<Models.LabelClass> Labels
        {
            get { return (IEnumerable<Models.LabelClass>)GetValue(LabelsProperty); }

            set { SetValue(LabelsProperty, value); }
        }

        private void Button_ConfirmAddLabels_Click(object sender, RoutedEventArgs e)
        {

            DoneEvent?.Invoke(true, Labels);
        }

        private void Button_CancelAddLabels_Click(object sender, RoutedEventArgs e)
        {
            DoneEvent?.Invoke(false, Labels);
        }
        public delegate void DoneDelegate(bool confirm,IEnumerable<Models.LabelClass> labels);
        public event DoneDelegate DoneEvent;
    }
}
