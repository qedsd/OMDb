using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMDb.Core.Utils;
using Logger = OMDb.Core.Utils.Logger;

namespace OMDb.Core.Models
{
    /// <summary>
    /// 词条元数据
    /// 存储需要点击详情时才需要显示的信息
    /// 序列化保存于词条文件夹根目录下
    /// </summary>
    public class EntryMetadata
    {
        /// <summary>
        /// 保存的评分信息
        /// 评分不需每次都获取最新的，而是使用上一次保存的数据
        /// 第一次或者手动更新
        /// </summary>
        public List<Rating> Ratings { get; set; }

        public string Name { get; set; }
        public string Id { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }
        public List<ExtractsLineBase> ExtractsLines { get; set; }

        public bool Save(string file)
        {
            try
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                System.IO.File.WriteAllText(file, json);
                return true;
            }
            catch(Exception ex)
            {
                Logger.Error("MetaData保存失败！" + ex.Message);
                return false;
            }
        }

        public static EntryMetadata Read(string file)
        {
            var json = System.IO.File.ReadAllText(file);
            return JsonConvert.DeserializeObject<EntryMetadata>(json);
        }
    }
}
