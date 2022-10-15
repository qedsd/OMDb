using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels
{
    internal class ClassificationViewModel : ObservableObject
    {
        private List<BannerItem> bannerItemsSource;
        public List<BannerItem> BannerItemSource
        {
            get => bannerItemsSource;
            set => SetProperty(ref bannerItemsSource, value);
        }
        public ICommand CheckDetailBannerItemCommand => new RelayCommand<BannerItem>((item) =>
        {

        });
        public ClassificationViewModel()
        {
            var items = new List<BannerItem>();
            items.Add(new BannerItem()
            {
                Title = "标题1",
                Description = "描述",
                Img = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanner.jpg"))),
                PreviewImg = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanneritem.jpg")))
            });
            items.Add(new BannerItem()
            {
                Title = "标题2",
                Description = "描述",
                Img = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanner.jpg"))),
                PreviewImg = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanneritem.jpg")))
            });
            items.Add(new BannerItem()
            {
                Title = "标题3",
                Description = "描述",
                Img = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanner.jpg"))),
                PreviewImg = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanneritem.jpg")))
            });
            BannerItemSource = items;
        }
    }
}
