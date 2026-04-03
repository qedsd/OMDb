using CommunityToolkit.Mvvm.ComponentModel;

namespace OMDb.Maui.Models
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
        public object Img { get; set; }
        public object PreviewImg { get; set; }
        public string Tag { get; set; }

        public BannerItem()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
