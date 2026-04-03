using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.Interfaces;
using OMDb.WinUI3.MyControls;
using OMDb.WinUI3.Views.Homes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Views
{
    public sealed partial class HomePage : Page
    {
        public static HomePage Current { get; private set; }
        public ViewModels.HomeViewModel VM { get; set; } = new ViewModels.HomeViewModel();
        public HomePage()
        {
            this.InitializeComponent();
            Current = this;
            VM.Init();
            SizeChanged += ClassificationPage_SizeChanged;
            ClassificationPage_SizeChanged(null, null);
        }
        private void ClassificationPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //CoverLineGrid.Height = this.ActualHeight * 0.8;
        }

        public List<IHomeItem> HomeItems
        {
            get => ItemContentPanel.Children.Select(p => p as IHomeItem).ToList();
        }

        /// <summary>
        /// 添加主页部件
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index">不设置则添加到最后</param>
        public void AddItem(HomeItemBasePage item,int index = -1)
        {
            if(index < 0)
            {
                ItemContentPanel.Children.Add(item);
            }
            else
            {
                ItemContentPanel.Children.Insert(index,item);
            }
        }
        /// <summary>
        /// 移除主页部件
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(HomeItemBasePage item)
        {
            ItemContentPanel.Children.Remove(item);
        }
        public void ClearItem()
        {
            ItemContentPanel.Children.Clear();
        }
    }
}
