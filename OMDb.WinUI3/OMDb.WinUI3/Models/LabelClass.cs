using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class LabelClass : ObservableObject
    {
        public Core.DbModels.LabelClassDb LabelClassDb { get; set; }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }

        public LabelClass(Core.DbModels.LabelClassDb labelDb)
        {
            LabelClassDb = labelDb;
            _isChecked = false;
            Name = labelDb.Name;
            Description= labelDb.Description;
        }

        public string Description;
        public string Name { get; set; }
        public bool IsTemp { get; set; } = false;
        public LabelClass() { }
    }
}
