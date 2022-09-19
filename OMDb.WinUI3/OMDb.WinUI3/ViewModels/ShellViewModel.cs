using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using OMDb.WinUI3.Helpers;
using OMDb.WinUI3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.ViewModels
{
    public class ShellViewModel : ObservableObject
    {
        public static ShellViewModel Current
        {
            get;
            set;
        }
        public ShellViewModel()
        {
            Current = this;
        }
        private NavigationViewItem selected;
        public NavigationViewItem Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnSelectedChanged(selected);
                SetProperty(ref selected, value);
            }
        }
        private bool canGoBack;
        public bool CanGoBack
        {
            get => canGoBack;
            set => SetProperty(ref canGoBack, value);
        }
        private Dictionary<Type, NavigationViewItem> NavigationViewItemDic = new Dictionary<Type, NavigationViewItem>();
        public void Init(Frame frame)
        {
            NavigationService.Frame = frame;
        }
        private void OnSelectedChanged(NavigationViewItem selectedItem)
        {
            if (selectedItem != null)
            {
                if (selectedItem.Name == "SettingsItem")
                {
                    NavigationService.Navigate(typeof(Views.SettingPage), null);
                }
                else
                {
                    var pageType = selectedItem?.GetValue(NavHelper.NavigateToProperty) as Type;
                    if (pageType != null)
                    {
                        NavigationService.Navigate(pageType, null);
                    }
                }
                CanGoBack = NavigationService.CanGoBack;
            }
            
        }
        public void GoBack()
        {
            NavigationService.GoBack();
            CanGoBack = NavigationService.CanGoBack;
        }

        public void InitItems(NavigationView navigationView)
        {
            foreach (NavigationViewItem item in navigationView.MenuItems)
            {
                NavigationViewItemDic.TryAdd(item.GetValue(Helpers.NavHelper.NavigateToProperty) as Type, item);
            }
        }

        public void SetSelected(Type type)
        {
            if (type == null)
            {
                return;
            }
            if (Selected.GetValue(NavHelper.NavigateToProperty) as Type != type && NavigationViewItemDic.TryGetValue(type, out NavigationViewItem item))
            {
                Selected.IsSelected = false;
                Selected = item;
                Selected.IsSelected = true;
            }
        }
    }
}
