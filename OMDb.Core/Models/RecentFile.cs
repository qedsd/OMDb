using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Models
{
    public class RecentFile
    {
        public string Path { get; set; }
        public string Name
        {
            get => System.IO.Path.GetFileName(Path);
        }
        public string EntryId { get; set; }
        public string DbId { get; set; }
        public DateTime AccessTime { get; set; }
        /// <summary>
        /// 时长
        /// 毫秒
        /// </summary>
        public long Duration { get; set; }
        /// <summary>
        /// 记录位置
        /// 毫秒
        /// </summary>
        public long MarkTime { get; set; }

        public double WatchedPrecent
        {
            get => Duration == 0 ? 0 : Math.Round((double)MarkTime / Duration * 100,2);
        }
    }
}
