using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class DbSource : ObservableObject
    {
        public Core.DbModels.DbSourceDb DbSourceDb { get; set; }
        private bool isChecked;
        public bool IsChecked
        {
            get => isChecked;
            set=>SetProperty(ref isChecked, value);
        }
        public DbSource(Core.DbModels.DbSourceDb dbSourceDb)
        {
            this.DbSourceDb = dbSourceDb;
            IsChecked = false;
        }

    }
}
