using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Events
{
    public class RecentFileChangedEventArgs : EventArgs
    {
        public RecentFileChangedEventArgs() { }
        public RecentFileChangedEventArgs(List<RecentFile> recentFiles)
        {
            RecentFiles = recentFiles;
        }

        /// <summary>
        /// 更新的文件
        /// </summary>
        public List<RecentFile> RecentFiles { get; set; }
    }
}
