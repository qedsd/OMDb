using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Interfaces
{
    public interface ITabViewItemPage
    {
        string Title { get; }
        /// <summary>
        /// 关闭页面
        /// </summary>
        void Close();
    }
}
