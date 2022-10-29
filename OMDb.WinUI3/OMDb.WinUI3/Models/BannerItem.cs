using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class BannerItem : ObservableObject
    {
        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ImageSource Img { get; set; }
        public ImageSource PreviewImg { get; set; }

        public BannerItem()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
