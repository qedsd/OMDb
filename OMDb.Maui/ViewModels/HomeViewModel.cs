using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.Maui.ViewModels
{
    /// <summary>
    /// 主页视图模型
    /// 负责主页数据的加载和刷新
    /// 主要功能：
    /// 1. 显示最近更新的词条列表
    /// 2. 显示最近观看的文件记录
    /// 3. 显示随机推荐的词条
    /// 4. 显示统计数据（词条总数、观看次数等）
    /// </summary>
    public class HomeViewModel : ObservableObject
    {
        /// <summary>
        /// 构造函数
        /// 初始化主页视图模型
        /// </summary>
        public HomeViewModel()
        {
        }

        /// <summary>
        /// 初始化主页显示项目
        /// 根据用户配置加载主页部件（最近更新、最近观看、随机推荐等）
        /// TODO: 需要实现从配置服务读取用户自定义的主页布局
        /// </summary>
        private void InitShowItem()
        {
            // TODO: 初始化主页部件
            // Views.HomePage.Current.ClearItem();
            // foreach(var item in HomeItemConfigsService.ActiveItems)
            // {
            //     Views.HomePage.Current.AddItem(Activator.CreateInstance(item.Type) as HomeItemBasePage);
            // }
        }

        /// <summary>
        /// 异步初始化每个主页项目
        /// 加载每个主页部件的具体数据
        /// TODO: 需要实现每个主页部件的数据加载逻辑
        /// </summary>
        /// <returns>Task</returns>
        private async Task ItemInitAsync()
        {
            // TODO: 初始化每个主页部件
            // foreach (var item in Views.HomePage.Current.HomeItems)
            // {
            //     await item.InitAsync();
            // }
        }

        /// <summary>
        /// 初始化主页数据
        /// 在页面加载时调用，加载所有主页部件及其数据
        /// </summary>
        public async void Init()
        {
            // InfoHelper.ShowWaiting();
            InitShowItem();
            await ItemInitAsync();
            // InfoHelper.HideWaiting();
        }

        /// <summary>
        /// 刷新命令
        /// 重新加载主页所有数据
        /// 用户点击刷新按钮时触发
        /// </summary>
        public ICommand RefreshCommand => new RelayCommand(async () =>
        {
            // InfoHelper.ShowWaiting();
            InitShowItem();
            await ItemInitAsync();
            // InfoHelper.HideWaiting();

            // 可测试的方式显示警报
            try
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("提示", "刷新完成", "确定");
                }
            }
            catch
            {
                // 在测试环境中，Application.Current 可能为 null，忽略错误
            }
        });
    }
}
