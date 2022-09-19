using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class Label : ObservableObject
    {
        public Core.DbModels.LabelDb LabelDb { get; set; }
        private bool isChecked;
        public bool IsChecked
        {
            get => isChecked;
            set=>SetProperty(ref isChecked, value);
        }
        public Label(Core.DbModels.LabelDb labelDb)
        {
            LabelDb = labelDb;
            isChecked = false;
        }
        public bool IsTemp { get; set; } = false;
        public Label() { }
    }
}
