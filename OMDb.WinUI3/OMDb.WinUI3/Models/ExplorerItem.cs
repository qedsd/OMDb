using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class ExplorerItem
    {
        public string Name { get; set; }
        public bool IsFile { get; set; }
        /// <summary>
        /// 文件大小
        /// 单位为字节
        /// </summary>
        public long Length { get; set; }
        public List<ExplorerItem> Children { get; set; }
    }
}
