using OMDb.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class EntryDetail:Core.Models.Entry
    {
        public static EntryDetail Create(Core.Models.Entry entry)
        {
            var entryDetail = entry.DepthClone<EntryDetail>();
            if(entryDetail != null)
            {
                entryDetail.FullEntryPath = Helpers.PathHelper.EntryFullPath(entry);
                entryDetail.FullCoverImgPath = Helpers.PathHelper.EntryCoverImgFullPath(entry);
                entryDetail.FullMetaDataPath = System.IO.Path.Combine(entryDetail.FullEntryPath, Services.ConfigService.MetadataFileNmae);
            }
            return entryDetail;
        }
        public List<EntryName> Names { get; set; }
        public string FullEntryPath { get; set; }
        public string FullCoverImgPath { get; set; }
        public string FullMetaDataPath { get; set; }
        public Core.Models.EntryMetadata Metadata { get; set; }
        public List<Core.Models.WatchHistory> WatchHistory { get; set; }
        public List<Core.DbModels.LabelDb> Labels { get; set; }
        /// <summary>
        /// 修改词条存储路径
        /// </summary>
        /// <param name="newPath"></param>
        public void ChangeEntryPath(string newPath)
        {

        }
        /// <summary>
        /// 修改词条封面
        /// </summary>
        /// <param name="newImgName">新封面图带后缀名名字，必须为词条Img文件夹下的图片文件</param>
        public void ChangeCoverImg(string newImgName)
        {

        }
    }
}
