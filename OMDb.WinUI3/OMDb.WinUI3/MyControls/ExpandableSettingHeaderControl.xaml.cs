using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
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
using Windows.UI.Xaml.Markup;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OMDb.WinUI3.MyControls
{
    [ContentProperty(Name = nameof(SettingActionableElement))]
    public sealed partial class ExpandableSettingHeaderControl : UserControl
    {
        public FrameworkElement SettingActionableElement { get; set; }

        public static readonly DependencyProperty TitleProperty
           = DependencyProperty.Register(
               nameof(Title),
               typeof(string),
               typeof(ExpandableSettingHeaderControl),
               new PropertyMetadata(
                   string.Empty,
                   (d, e) =>
                   {
                       AutomationProperties.SetName(d, (string)e.NewValue);
                   }));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty
            = DependencyProperty.Register(
                nameof(Description),
                typeof(string),
                typeof(ExpandableSettingHeaderControl),
                new PropertyMetadata(
                    string.Empty,
                    (d, e) =>
                    {
                        AutomationProperties.SetHelpText(d, (string)e.NewValue);
                    }));

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(
                nameof(Icon),
                typeof(IconElement),
                typeof(ExpandableSettingHeaderControl),
                new PropertyMetadata(null));

        public IconElement Icon
        {
            get => (IconElement)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public ExpandableSettingHeaderControl()
        {
            InitializeComponent();
            VisualStateManager.GoToState(this, "NormalState", false);
        }

        private void MainPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width == e.PreviousSize.Width || ActionableElement == null)
            {
                return;
            }

            if (ActionableElement.ActualWidth > e.NewSize.Width / 3)
            {
                VisualStateManager.GoToState(this, "CompactState", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "NormalState", false);
            }
        }
    }
}
