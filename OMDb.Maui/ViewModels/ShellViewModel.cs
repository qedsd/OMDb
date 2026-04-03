using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Input;

namespace OMDb.Maui.ViewModels
{
    public class ShellViewModel : ObservableObject
    {
        public static ShellViewModel Current { get; set; }

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
            set => SetProperty(ref selectedPage, value);
        }

        public void Init()
        {
            // MAUI 使用 Shell 导航，不需要 Frame
        }

        public void SetSelected(Type type)
        {
            SelectedPage = type?.Name ?? string.Empty;
        }

        public ICommand NavClickCommand => new RelayCommand<object>((item) =>
        {
            // MAUI Shell 导航实现
            if (item is Microsoft.Maui.Controls.MenuFlyoutItem menuItem)
            {
                var route = menuItem.CommandParameter as string;
                if (!string.IsNullOrEmpty(route))
                {
                    try
                    {
                        Shell.Current?.GoToAsync(route);
                    }
                    catch
                    {
                        // 在测试环境中，Shell.Current 可能为 null，忽略错误
                    }
                }
            }
        });
    }
}
