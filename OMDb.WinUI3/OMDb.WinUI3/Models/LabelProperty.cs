using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class LabelProperty : ObservableObject
    {
        public Core.DbModels.LabelPropertyDb LPDb { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        private bool isChecked;
        public bool IsChecked
        {
            get => isChecked;
            set=>SetProperty(ref isChecked, value);
        }

        private bool isHiden;
        public bool IsHiden
        {
            get => isHiden;
            set => SetProperty(ref isHiden, value);
        }
        public LabelProperty(Core.DbModels.LabelPropertyDb lpdb)
        {
            LPDb = lpdb;
            isChecked = false;
            isHiden = false;
        }
        public bool IsTemp { get; set; } = false;
        public LabelProperty() { }
    }
}
