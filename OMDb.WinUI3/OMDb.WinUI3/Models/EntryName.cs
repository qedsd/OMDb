using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class EntryName : ObservableObject
    {
        private string _name;
        public string Name
        {
            get => _name;
            set { SetProperty(ref _name, value); }
        }

        //private Core.Enums.LangEnum _lang;
        //public Core.Enums.LangEnum Lang
        //{
        //    get => _lang;
        //    set { SetProperty(ref _lang, value); }
        //}

        private string _mark;
        public string Mark
        {
            get => _mark;
            set { SetProperty(ref _mark, value); }
        }

        private bool _isDefault;
        public bool IsDefault
        {
            get => _isDefault;
            set { SetProperty(ref _isDefault, value); }
        }
        public EntryName() { }
        public EntryName(Core.Models.EntryName entryName)
        {
            Name = entryName.Name;
            Mark = entryName.Mark;
            IsDefault = entryName.IsDefault;
        }
        public Core.DbModels.EntryNameDb ToCoreEntryNameDb(string entryId)
        {
            return new Core.DbModels.EntryNameDb()
            {
                Name = Name,
                Mark = Mark,
                IsDefault = IsDefault,
                EntryId = entryId
            };
        }
    }
}
