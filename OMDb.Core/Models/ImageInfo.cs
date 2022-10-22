using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Models
{
    public class ImageInfo
    {
        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        /// <summary>
        /// 文件大小
        /// 单位为字节
        /// </summary>
        public long Length { get; set; }
        /// <summary>
        /// 文件大小
        /// 单位kb
        /// </summary>
        public double Size_kb
        {
            get => Length / 1024;
        }

        /// <summary>
        /// 长宽比
        /// </summary>
        public double Scale
        {
            get => Height / Width;
        }
    }
}
