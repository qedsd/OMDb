using Newtonsoft.Json;
using OMDb.Core.DbModels.ManagerCenterDb;
using OMDb.Maui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMDb.Maui.Services.Settings
{
    /// <summary>
    /// 数据库选择器服务 - 管理多数据库配置
    ///
    /// 主要功能：
    /// 1. 管理多个数据库中心（DbCenter）
    /// 2. 保存和加载当前选中的数据库
    /// 3. 添加、删除、修改数据库配置
    /// 4. 初始化时自动加载选中的数据库
    ///
    /// 配置存储：
    /// - 使用 SettingService 保存当前选中的数据库 ID
    /// - 键名："DbSelector"
    ///
    /// 使用示例：
    /// <code>
    /// // 初始化（应用启动时调用）
    /// DbSelectorService.Initialize();
    ///
    /// // 切换数据库
    /// await DbSelectorService.SetAsync("new-db-id");
    ///
    /// // 获取当前数据库 ID
    /// string currentId = DbSelectorService.DbCurrentId;
    ///
    /// // 获取当前数据库名称
    /// string currentName = DbSelectorService.DbCurrentName;
    /// </code>
    /// </summary>
    public static class DbSelectorService
    {
        /// <summary>
        /// 设置键名
        /// 用于在 SettingService 中存储当前数据库 ID
        /// </summary>
        private const string Key = "DbSelector";

        /// <summary>
        /// 当前选中的数据库 ID
        /// 只读属性，通过 SetAsync 方法修改
        /// </summary>
        public static string DbCurrentId { get; private set; } = string.Empty;

        /// <summary>
        /// 当前选中的数据库名称
        /// 只读属性，随 DbCurrentId 自动更新
        /// </summary>
        public static string DbCurrentName { get; private set; } = string.Empty;

        /// <summary>
        /// 所有数据库中心列表
        /// 包含所有已配置的数据库及其选中状态
        /// </summary>
        public static List<DbCenter> DbsCollection { get; set; } = new List<DbCenter>();

        /// <summary>
        /// 初始化数据库选择器
        /// 从配置文件加载上次选中的数据库，并初始化相关配置
        ///
        /// 执行步骤：
        /// 1. 从数据库读取所有 DbCenter 配置
        /// 2. 从 SettingService 加载上次选中的数据库 ID
        /// 3. 如果未找到或为空，选择第一个数据库
        /// 4. 初始化 DCDb 数据库
        /// 5. 设置选中状态
        /// 6. 加载存储配置
        ///
        /// 注意：此方法会同步执行，建议在应用启动时使用
        /// </summary>
        public static void Initialize()
        {
            LoadFromSettings();
        }

        /// <summary>
        /// 设置当前选中的数据库
        /// 异步方法，保存到配置文件并重新加载存储
        ///
        /// 使用示例：
        /// <code>
        /// // 切换到指定数据库
        /// await DbSelectorService.SetAsync("target-db-id");
        ///
        /// // 切换后，DbCurrentName 和 ConfigService.DefaultEntryFolder 会自动更新
        /// </code>
        /// </summary>
        /// <param name="dbSwitch">目标数据库 ID</param>
        /// <returns>Task</returns>
        public static async Task SetAsync(string dbSwitch)
        {
            DbCurrentId = dbSwitch;

            // 保存到配置文件
            await SaveInSettingsAsync();

            // 初始化 DCDb 数据库
            string dcdbPath = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Configs",
                $"DCDb_{DbCurrentId}.db"
            );
            Core.Config.InitDCDb(dcdbPath);

            // 更新选中状态
            foreach (var db in DbsCollection)
            {
                db.IsChecked = db.DbCenterDb.Id == DbCurrentId;
            }

            // 重新加载存储
            ConfigService.LoadStoragesAsync();
        }

        /// <summary>
        /// 从配置文件加载数据库选择
        /// 私有方法，被 Initialize 调用
        ///
        /// 执行逻辑：
        /// 1. 调用 LoadAllDbs 加载所有数据库
        /// 2. 从 SettingService 获取保存的数据库 ID
        /// 3. 如果 ID 无效或为空，选择第一个数据库
        /// 4. 调用 SetAsync 初始化
        /// </summary>
        private static async void LoadFromSettings()
        {
            // 从数据库读取所有 DbCenter
            LoadAllDbs();

            // 从配置文件获取当前数据库 ID
            string savedId = SettingService.GetValue(Key);
            DbCurrentId = savedId ?? string.Empty;

            // 如果 ID 不存在或为空，选择第一个数据库
            if (string.IsNullOrEmpty(DbCurrentId) ||
                !DbsCollection.Select(d => d.DbCenterDb.Id).Contains(DbCurrentId))
            {
                DbCurrentId = DbsCollection.FirstOrDefault()?.DbCenterDb.Id ?? string.Empty;
            }

            // 初始化 DCDb 并设置选中状态
            string dcdbPath = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Configs",
                $"DCDb_{DbCurrentId}.db"
            );
            Core.Config.InitDCDb(dcdbPath);

            // 设置选中状态
            foreach (var db in DbsCollection)
            {
                db.IsChecked = db.DbCenterDb.Id == DbCurrentId;
            }

            // 保存到配置文件（同步当前选择）
            await SaveInSettingsAsync();
        }

        /// <summary>
        /// 保存到配置文件
        /// 将当前数据库 ID 保存到 SettingService
        /// 同时更新 DbCurrentName 和 DefaultEntryFolder
        ///
        /// 注意：这是私有方法，仅由 SetAsync 和 LoadFromSettings 调用
        /// </summary>
        /// <returns>Task</returns>
        private static async Task SaveInSettingsAsync()
        {
            // 保存数据库 ID
            await SettingService.SetValueAsync(Key, DbCurrentId);

            // 更新数据库名称
            var currentDb = DbsCollection.FirstOrDefault(d => d.IsChecked);
            DbCurrentName = currentDb?.DbCenterDb.DbName ?? string.Empty;

            // 更新默认词条文件夹路径
            ConfigService.DefaultEntryFolder = System.IO.Path.Combine(
                ConfigService.OMDbFolder,
                DbCurrentName
            );

            // 重新加载存储配置
            await ConfigService.LoadStoragesAsync();
        }

        /// <summary>
        /// 添加新的数据库
        /// 调用 Core.Services.DbCenterService 添加到数据库
        ///
        /// 使用示例：
        /// <code>
        /// DbSelectorService.AddDbAsync("新数据库");
        /// // 添加后需要重新调用 Initialize 或手动刷新
        /// </code>
        /// </summary>
        /// <param name="dbName">新数据库名称</param>
        public static void AddDbAsync(string dbName)
        {
            Core.Services.DbCenterService.AddDbCenter(dbName);
            // 重新加载所有数据库
            LoadAllDbs();
        }

        /// <summary>
        /// 删除数据库
        /// 从数据库中删除指定 DbCenter
        ///
        /// 注意：删除当前选中的数据库会导致异常，请先切换到其他数据库
        ///
        /// 使用示例：
        /// <code>
        /// // 确保不删除当前选中的数据库
        /// if (dbId != DbSelectorService.DbCurrentId)
        /// {
        ///     DbSelectorService.RemoveDbAsync(dbId);
        /// }
        /// </code>
        /// </summary>
        /// <param name="dbId">要删除的数据库 ID</param>
        public static void RemoveDbAsync(string dbId)
        {
            // 不允许删除当前选中的数据库
            if (dbId == DbCurrentId)
            {
                throw new InvalidOperationException("不能删除当前正在使用的数据库");
            }

            Core.Services.DbCenterService.RemoveDbCenter(dbId);
            // 重新加载所有数据库
            LoadAllDbs();
        }

        /// <summary>
        /// 修改数据库配置
        /// 更新 DbCenter 的信息（如名称）
        ///
        /// 使用示例：
        /// <code>
        /// var db = DbSelectorService.DbsCollection[0].DbCenterDb;
        /// db.DbName = "新名称";
        /// DbSelectorService.EditDbAsync(db);
        /// </code>
        /// </summary>
        /// <param name="db">要修改的 DbCenterDb 对象</param>
        public static void EditDbAsync(DbCenterDb db)
        {
            Core.Services.DbCenterService.EditDbCenter(db);
            // 重新加载所有数据库
            LoadAllDbs();
        }

        /// <summary>
        /// 获取所有数据库
        /// 从 Core.Services.DbCenterService 读取所有 DbCenter
        /// 并包装为 DbCenter 模型（包含 IsChecked 属性）
        ///
        /// 这是私有方法，被 LoadFromSettings 调用
        /// </summary>
        private static void LoadAllDbs()
        {
            var result = Core.Services.DbCenterService.GetAllDbCenter();
            DbsCollection.Clear();

            foreach (var item in result)
            {
                DbsCollection.Add(new DbCenter(item));
            }
        }

        /// <summary>
        /// 获取所有可用的数据库列表
        /// 供 UI 绑定时使用
        ///
        /// 使用示例：
        /// <code>
        /// // 绑定到下拉列表
        /// myComboBox.ItemsSource = DbSelectorService.GetAllDatabases();
        /// </code>
        /// </summary>
        /// <returns>所有数据库列表</returns>
        public static List<DbCenter> GetAllDatabases()
        {
            return new List<DbCenter>(DbsCollection);
        }

        /// <summary>
        /// 检查指定数据库是否存在
        ///
        /// 使用示例：
        /// <code>
        /// if (DbSelectorService.Exists(dbId))
        /// {
        ///     // 数据库存在，可以切换
        /// }
        /// </code>
        /// </summary>
        /// <param name="dbId">数据库 ID</param>
        /// <returns>true=存在，false=不存在</returns>
        public static bool Exists(string dbId)
        {
            return DbsCollection.Any(d => d.DbCenterDb.Id == dbId);
        }
    }
}
