using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace OMDb.Maui.Models
{
    /// <summary>
    /// 词条存储模型 - 表示一个词条仓库
    ///
    /// 主要属性：
    /// - StorageId: 存储 ID
    /// - StorageName: 存储名称
    /// - StoragePath: 存储路径
    /// - EntryCount: 词条数量
    /// - CoverImg: 封面图片
    /// - IsChecked: 是否选中
    ///
    /// 用于：
    /// - 配置服务中显示所有可用的词条仓库
    /// - 设置页面中切换当前使用的数据库
    /// </summary>
    public class EnrtyStorage : ObservableObject
    {
        /// <summary>
        /// 存储 ID
        /// </summary>
        public string StorageId { get; set; }

        private string storageName;
        /// <summary>
        /// 存储名称
        /// </summary>
        public string StorageName
        {
            get => storageName;
            set => SetProperty(ref storageName, value);
        }

        private string storagePath;
        /// <summary>
        /// 存储路径
        /// </summary>
        public string StoragePath
        {
            get => storagePath;
            set => SetProperty(ref storagePath, value);
        }

        /// <summary>
        /// 存储目录（与 StoragePath 相同）
        /// </summary>
        public string StorageDirectory => StoragePath;

        private int entryCount;
        /// <summary>
        /// 词条数量
        /// </summary>
        public int EntryCount
        {
            get => entryCount;
            set => SetProperty(ref entryCount, value);
        }

        private string coverImg;
        /// <summary>
        /// 封面图片路径
        /// </summary>
        public string CoverImg
        {
            get => coverImg;
            set => SetProperty(ref coverImg, value);
        }

        private bool isChecked;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked
        {
            get => isChecked;
            set => SetProperty(ref isChecked, value);
        }

        /// <summary>
        /// 从另一个 EnrtyStorage 对象更新当前对象
        /// </summary>
        /// <param name="copy">源对象</param>
        public void Update(EnrtyStorage copy)
        {
            if (copy != null)
            {
                StorageName = copy.StorageName;
                StoragePath = copy.StoragePath;
                CoverImg = copy.CoverImg;
                EntryCount = copy.EntryCount;
            }
        }
    }
}
