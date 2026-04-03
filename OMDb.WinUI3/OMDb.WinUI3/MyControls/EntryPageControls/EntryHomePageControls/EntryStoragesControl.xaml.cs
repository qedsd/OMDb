using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.MyControls
{
    public sealed partial class EntryStoragesControl : UserControl
    {
        public EntryStoragesControl()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty EntryStoragesProperty = DependencyProperty.Register
            (
            "EntryStorages",
            typeof(IEnumerable<Models.EnrtyStorage>),
            typeof(UserControl),
            new PropertyMetadata(null, new PropertyChangedCallback(SetEntryStorages))
            );
        private static void SetEntryStorages(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as EntryStoragesControl;
            if (card != null)
            {
                var items = e.NewValue as IEnumerable<Models.EnrtyStorage>;
                if (items != null)
                {
                    List<Models.EnrtyStorage> list = new List<Models.EnrtyStorage>();
                    list.Add(new Models.EnrtyStorage()
                    {
                        StorageName = "全部",
                        IsChecked = true
                    });

                    foreach (var item in items)
                    {
                        item.IsChecked = true;
                        list.Add(item);
                    }
                    card.GridView_Label.ItemsSource = list;
                    card.GridViewItemsSource = list;
                }
                else
                {
                    card.GridView_Label.ItemsSource = null;
                    card.GridViewItemsSource = null;
                }
            }
        }
        private IEnumerable<Models.EnrtyStorage> GridViewItemsSource;
        public IEnumerable<Models.EnrtyStorage> EntryStorages
        {
            get { return (IEnumerable<Models.EnrtyStorage>)GetValue(EntryStoragesProperty); }

            set { SetValue(EntryStoragesProperty, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as Models.EnrtyStorage;
            if (item != null)
            {
                if (item == GridViewItemsSource.FirstOrDefault())//全选、全不选
                {
                    item.IsChecked = !item.IsChecked;
                    foreach (var l in EntryStorages)
                    {
                        l.IsChecked = item.IsChecked;
                    }
                }
                else
                {
                    item.IsChecked = !item.IsChecked;
                    if (item.IsChecked)
                    {
                        if (EntryStorages.FirstOrDefault(p => !p.IsChecked) == null)
                        {
                            GridViewItemsSource.First().IsChecked = true;
                        }
                    }
                    else
                    {
                        GridViewItemsSource.First().IsChecked = false;
                    }
                }
                CallChanged();
            }
        }
        private void CallChanged()
        {
            var ls = EntryStorages.ToList();
            CheckChanged?.Invoke(ls);
            CheckChangedCommand?.Execute(ls);
        }
        /// <summary>
        /// 选择委托
        /// </summary>
        /// <param name="label">当前触发标签</param>
        public delegate void CheckChangedEventHandel(IEnumerable<Models.EnrtyStorage> allItems);
        private CheckChangedEventHandel CheckChanged;

        public static readonly DependencyProperty CheckChangedCommandProperty
           = DependencyProperty.Register(
               nameof(CheckChangedCommand),
               typeof(string),
               typeof(EntryStoragesControl),
               new PropertyMetadata(string.Empty));

        public ICommand CheckChangedCommand
        {
            get => (ICommand)GetValue(CheckChangedCommandProperty);
            set => SetValue(CheckChangedCommandProperty, value);
        }

        /// <summary>
        /// 选择标签后触发
        /// </summary>
        public event CheckChangedEventHandel OnCheckChanged
        {
            add
            {
                CheckChanged += value;
            }
            remove
            {
                CheckChanged -= value;
            }
        }
    }

}
