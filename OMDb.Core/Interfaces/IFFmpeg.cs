using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Interfaces
{
    public interface IFFmpeg
    {
        /// <summary>
        /// 获取媒体文件信息
        /// </summary>
        /// <param name="path">媒体文件地址</param>
        /// <returns></returns>
        MediaInfo GetMediaInfo(string path);
    }
}
