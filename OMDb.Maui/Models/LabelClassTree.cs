using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace OMDb.Maui.Models
{
    public class LabelClassTree : ObservableObject
    {
        private LabelClass _labelClass;
        public LabelClass LabelClass
        {
            get => _labelClass;
            set => SetProperty(ref _labelClass, value);
        }

        private ObservableCollection<LabelClassTree> children;
        public ObservableCollection<LabelClassTree> Children
        {
            get => children;
            set => SetProperty(ref children, value);
        }

        public LabelClassTree() { }
        public LabelClassTree(Core.DbModels.LabelClassDb labelDb)
        {
            _labelClass = new LabelClass(labelDb);
            children = new ObservableCollection<LabelClassTree>();
        }
    }
}
