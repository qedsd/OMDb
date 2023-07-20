using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using OMDb.WinUI3.Helpers;
using OMDb.WinUI3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        private bool isInTabView;
        public bool IsInTabView
        {
            get => isInTabView;
            set => SetProperty(ref isInTabView, value);
        }

        private string selectedPage;
        public string SelectedPage
        {
            get => selectedPage;
            set
            {
                SetProperty(ref selectedPage, value);
            }
        }

        public void Init(Frame frame)
        {
            NavigationService.Frame = frame;
            NavigationService.Navigate(typeof(Views.HomePage), null);
        }

        public void SetSelected(Type type)
        {
            SelectedPage = type.Name;
        }

        public ICommand NavClickCommand => new RelayCommand<ListViewItem>((item) =>
        {
            var pageType = item?.GetValue(NavHelper.NavigateToProperty) as Type;
            if (pageType != null)
            {
                NavigationService.Navigate(pageType, null);
            }
        });
    }
}
