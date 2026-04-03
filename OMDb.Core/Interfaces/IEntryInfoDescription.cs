using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Interfaces
{
    public interface IEntryInfoDescription
    {
        /// <summary>
        /// 获取描述
        /// </summary>
        /// <param name="id">影片唯一标识，规范由各自确定</param>
        /// <returns></returns>
        string GetEntryInfoDescriptionNet(string keyword);
    }
}
