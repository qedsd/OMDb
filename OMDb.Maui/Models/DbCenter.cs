using CommunityToolkit.Mvvm.ComponentModel;
using OMDb.Core.DbModels.ManagerCenterDb;
using System;

namespace OMDb.Maui.Models
{
    /// <summary>
    /// 数据库中心模型 - 表示一个数据库配置
    ///
    /// 主要属性：
    /// - DbCenterDb: 数据库中心数据
    /// - IsChecked: 是否选中
    ///
    /// 用于：
    /// - 数据库选择器服务中管理多个数据库配置
    /// - 设置页面中切换当前使用的数据库
    /// </summary>
    public class DbCenter : ObservableObject
    {
        /// <summary>
        /// 数据库中心数据
        /// </summary>
        public DbCenterDb DbCenterDb { get; set; }

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
        /// 构造函数
        /// </summary>
        /// <param name="dbCenterDb">数据库中心数据</param>
        public DbCenter(DbCenterDb dbCenterDb)
        {
            DbCenterDb = dbCenterDb;
            IsChecked = false;
        }
    }
}
