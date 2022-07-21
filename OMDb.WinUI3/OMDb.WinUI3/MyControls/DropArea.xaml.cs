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
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OMDb.WinUI3.MyControls
{
    public sealed partial class DropArea : UserControl
    {
        public DropArea()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty CaptionProperty
            = DependencyProperty.Register(
                nameof(Caption),
                typeof(string),
                typeof(ExpandableSettingControl),
                new PropertyMetadata(string.Empty));

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public static readonly DependencyProperty DropCommandProperty
            = DependencyProperty.Register(
                nameof(DropCommand),
                typeof(string),
                typeof(ExpandableSettingControl),
                new PropertyMetadata(string.Empty));

        public ICommand DropCommand
        {
            get => (ICommand)GetValue(DropCommandProperty);
            set => SetValue(DropCommandProperty, value);
        }

        private async void Grid_DragLeave(object sender, DragEventArgs e)
        {
            var ui = sender as FrameworkElement;
            var pos = e.GetPosition(sender as UIElement);
            if (pos.X <= 0 || pos.X >= ui.ActualWidth || pos.Y <= 0 || pos.Y >= ui.ActualHeight)
            {
                
            }
            else
            {
                var dataPackage = e.DataView;
                if(dataPackage!=null)
                {
                    var items = await dataPackage.GetStorageItemsAsync();
                    if(items?.Count!=0)
                    {
                        Helpers.InfoHelper.ShowMsg(items.FirstOrDefault()?.Path);
                        DropCommand?.Execute(items);
                    }
                }
            }
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            e.DragUIOverride.Caption = Caption;
            e.DragUIOverride.IsGlyphVisible = false;
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.DragUIOverride.Caption = Caption;
            e.DragUIOverride.IsGlyphVisible = false;
        }
    }
}
