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
        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set=>SetProperty(ref _isChecked, value);
        }

        private bool _isHiden;
        public bool IsHiden
        {
            get => _isHiden;
            set => SetProperty(ref _isHiden, value);
        }
        public LabelProperty(Core.DbModels.LabelPropertyDb lpdb)
        {
            LPDb = lpdb;
            _isChecked = false;
            _isHiden = false;
            _name=lpdb.Name;
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool IsTemp { get; set; } = false;
        public LabelProperty() { }
    }
}
