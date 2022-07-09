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
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class ShellViewModel
    {
        private NavigationViewItem selected;
        public NavigationViewItem Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnSelectedChanged(selected);
            }
        }
        public void Init(Frame frame)
        {
            NavigationService.Frame = frame;
        }
        private void OnSelectedChanged(NavigationViewItem selectedItem)
        {
            var pageType = selectedItem?.GetValue(NavHelper.NavigateToProperty) as Type;
            if (pageType != null)
            {
                NavigationService.Navigate(pageType, null);
            }
        }
    }
}
