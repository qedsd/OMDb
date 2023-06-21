using CommunityToolkit.Mvvm.ComponentModel;
using OMDb.Core.DbModels.ManagerCenterDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class DbCenter : ObservableObject
    {
        public DbCenterDb DbCenterDb { get; set; }
        private bool isChecked;
        public bool IsChecked
        {
            get => isChecked;
            set=>SetProperty(ref isChecked, value);
        }
        public DbCenter(DbCenterDb DbCenterDb)
        {
            this.DbCenterDb = DbCenterDb;
            IsChecked = false;
        }

    }
}
