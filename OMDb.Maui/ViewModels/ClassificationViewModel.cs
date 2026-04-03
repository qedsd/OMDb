using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OMDb.Core.DbModels;
using OMDb.Core.Enums;
using OMDb.Core.Helpers;
using OMDb.Core.Models;
using OMDb.Core.Services;
using OMDb.Maui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.Maui.ViewModels
{
    /// <summary>
    /// 分类页视图模型
    /// 负责分类页面的数据加载和展示
    /// 主要功能：
    /// 1. 显示标签分类树形结构
    /// 2. 显示推荐合集
    /// 3. 支持按分类筛选词条
    /// 4. 支持列表视图和网格视图切换
    /// 5. 显示轮播图（Banner）
    /// </summary>
    public partial class ClassificationViewModel : ObservableObject
    {
        /// <summary>
        /// 轮播图数据源
        /// 显示在分类页顶部的推荐轮播图
        /// 包含"全部"选项和随机选择的标签分类
        /// </summary>
        [ObservableProperty]
        private List<BannerItem> _bannerItemsSource;

        /// <summary>
        /// 标签分类树
        /// 树形结构展示标签的层级关系（父子标签）
        /// 用于显示完整的分类结构
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<LabelClassTree> _labelTrees;

        /// <summary>
        /// 标签合集树
        /// 按分类组织的推荐合集列表
        /// 每个合集包含多个词条推荐
        /// </summary>
        [ObservableProperty]
        private List<LabelCollectionTree> _labelCollectionTrees;

        /// <summary>
        /// 扁平化的标签列表
        /// 包含所有标签（不含根标签）
        /// 用于快速访问所有标签
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<LabelClass> _labelClasses;

        /// <summary>
        /// 标签字典
        /// 键为标签 ID（LCID），值为标签对象
        /// 用于快速查找和访问标签
        /// </summary>
        private Dictionary<string, LabelClass> labelsDic;

        /// <summary>
        /// 是否为列表视图模式
        /// true = 列表视图，false = 网格视图
        /// </summary>
        private bool isList;

        /// <summary>
        /// 是否为列表视图模式
        /// true = 列表视图，false = 网格视图
        /// </summary>
        public bool IsList
        {
            get => isList;
            set => SetProperty(ref isList, value);
        }

        /// <summary>
        /// 构造函数
        /// 初始化分类视图模型
        /// 默认使用列表视图模式
        /// 自动启动数据初始化
        /// </summary>
        public ClassificationViewModel()
        {
            IsList = true; // 默认列表显示
            Init();
        }

        /// <summary>
        /// 初始化方法
        /// 异步加载分类页所需的所有数据
        /// 调用顺序：标签 → 轮播图 → 合集
        /// </summary>
        private async void Init()
        {
            await InitLabels();
            await InitBannerAsync();
            await InitLabelCollectionAsync();
        }

        /// <summary>
        /// 初始化标签数据
        /// 从数据库加载所有标签，构建标签字典和树形结构
        /// 1. 加载所有标签到字典
        /// 2. 构建根标签和子标签的树形结构
        /// 3. 初始化 LabelTrees 和 LabelClasses 集合
        /// 即使数据库中没有标签，也会初始化为空集合以避免 null 引用
        /// </summary>
        private async Task InitLabels()
        {
            if (labelsDic == null)
            {
                labelsDic = new Dictionary<string, LabelClass>();
            }
            else
            {
                labelsDic.Clear();
            }

            // 初始化集合，确保即使没有数据也不会是 null
            LabelClasses = new ObservableCollection<LabelClass>();
            LabelTrees = new ObservableCollection<LabelClassTree>();

            var labels = await Core.Services.LabelClassService.GetAllLabelAsync(".omdb");
            if (labels != null)
            {
                foreach (var label in labels)
                {
                    labelsDic.Add(label.LCID, new LabelClass(label));
                }

                Dictionary<string, LabelClassTree> labelsDb = new Dictionary<string, LabelClassTree>();
                var root = labels.Where(p => p.ParentId == null).ToList();
                if (root != null)
                {
                    foreach (var label in root)
                    {
                        labelsDb.Add(label.LCID, new LabelClassTree(label));
                    }
                }

                foreach (var label in labels)
                {
                    if (label.ParentId != null)
                    {
                        if (labelsDb.TryGetValue(label.ParentId, out var parent))
                        {
                            parent.Children.Add(new LabelClassTree(label));
                        }
                        LabelClasses.Add(new LabelClass(label));
                    }
                }

                foreach (var item in labelsDb)
                {
                    if (item.Value.Children.Count == 0)
                    {
                        LabelClasses.Add(new LabelClass(item.Value.LabelClass.LabelClassDb));
                    }
                }

                LabelTrees = new ObservableCollection<LabelClassTree>();
                foreach (var item in labelsDb)
                {
                    LabelTrees.Add(item.Value);
                }
            }
        }

        private async Task InitBannerAsync()
        {
            if (LabelClasses == null)
            {
                return;
            }

            var items = new List<BannerItem>();
            items.Add(await GetAllBannerItem());
            List<LabelClass> target = RandomHelper.RandomList(LabelClasses, 10);
            foreach (var item in target)
            {
                var sortModel = new SortModel(SortType.LastUpdateTime, SortWay.Positive);
                var filterModel = new FilterModel();
                filterModel.IsFilterLabelClass = true;
                filterModel.LabelClassIds.Add(item.LabelClassDb.LCID);

                var queryResults = await Core.Services.EntryService.QueryEntryAsync(sortModel, filterModel);
                var result = RandomHelper.RandomList(queryResults, 3);
                if (result?.Any() == true)
                {
                    var entrys = await Core.Services.EntryService.QueryEntryAsync(result.Select(p => p.ToQueryItem()).ToList());
                    if (entrys?.Any() == true)
                    {
                        // TODO: 实现封面图查找逻辑
                        items.Add(new BannerItem()
                        {
                            Id = item.LabelClassDb.LCID,
                            Title = item.LabelClassDb.Name,
                            Description = item.LabelClassDb.Description
                        });
                    }
                }
            }
            BannerItemsSource = items;
        }

        private async Task<BannerItem> GetAllBannerItem()
        {
            var sortModel = new SortModel(SortType.LastUpdateTime, SortWay.Positive);
            var filterModel = new FilterModel();
            filterModel.IsFilterLabelClass = true;
            filterModel.LabelClassIds = LabelClasses.Select(p => p.LabelClassDb.LCID).ToList();
            var queryResults = await Core.Services.EntryService.QueryEntryAsync(sortModel, filterModel);
            if (queryResults?.Count > 2)
            {
                var result = RandomHelper.RandomList(queryResults, 40);
                if (result?.Any() == true)
                {
                    var entrys = await Core.Services.EntryService.QueryEntryAsync(result.Select(p => p.ToQueryItem()).ToList());
                    if (entrys?.Any() == true)
                    {
                        // TODO: 实现封面图逻辑
                        return new BannerItem()
                        {
                            Title = "全部",
                            Tag = "All",
                            Description = null
                        };
                    }
                }
            }
            return new BannerItem()
            {
                Title = "全部",
                Tag = "All",
                Description = null
            };
        }

        private async Task InitLabelCollectionAsync()
        {
            if (IsList)
            {
                await InitLabelCollection3Async();
            }
            else
            {
                await InitLabelCollection4Async();
            }
        }

        private async Task InitLabelCollection3Async()
        {
            var items = new List<LabelCollectionTree>();
            LabelCollectionTree otherCollectionTree = new LabelCollectionTree()
            {
                LabelCollection = new LabelCollection()
                {
                    Title = "所有",
                    Description = string.Empty,
                },
                Children = new List<LabelCollection>()
            };

            // 检查 LabelTrees 是否为 null
            if (LabelTrees == null)
            {
                LabelCollectionTrees = new List<LabelCollectionTree>();
                return;
            }

            foreach (var labelTree in LabelTrees)
            {
                if (labelTree.Children.Any())
                {
                    LabelCollectionTree labelCollectionTree = new LabelCollectionTree()
                    {
                        LabelCollection = new LabelCollection()
                        {
                            Title = labelTree.LabelClass.LabelClassDb.Name,
                            Description = labelTree.LabelClass.LabelClassDb.Description,
                        },
                        Children = new List<LabelCollection>()
                    };

                    foreach (var label in labelTree.Children)
                    {
                        var sortModel = new SortModel(SortType.LastUpdateTime, SortWay.Positive);
                        var filterModel = new FilterModel();
                        filterModel.IsFilterLabelClass = true;
                        filterModel.LabelClassIds.Add(label.LabelClass.LabelClassDb.LCID);
                        var queryResults = await Core.Services.EntryService.QueryEntryAsync(sortModel, filterModel);
                        int entryCount = RandomHelper.RandomOne(new int[] { 6, 8 });
                        var result = RandomHelper.RandomList(queryResults, entryCount);
                        if (result?.Any() == true)
                        {
                            var entrys = await Core.Services.EntryService.QueryEntryAsync(result.Select(p => p.ToQueryItem()).ToList());
                            if (entrys?.Any() == true)
                            {
                                labelCollectionTree.Children.Add(new LabelCollection()
                                {
                                    Title = label.LabelClass.LabelClassDb.Name,
                                    Description = label.LabelClass.LabelClassDb.Description,
                                    Entries = entrys,
                                    Template = entryCount == 6 ? 1 : 2,
                                    Id = label.LabelClass.LabelClassDb.LCID
                                });
                            }
                        }
                    }
                    items.Add(labelCollectionTree);
                }
                else
                {
                    var sortModel = new SortModel(SortType.LastUpdateTime, SortWay.Positive);
                    var filterModel = new FilterModel();
                    filterModel.IsFilterLabelClass = true;
                    filterModel.LabelClassIds.Add(labelTree.LabelClass.LabelClassDb.LCID);
                    var queryResults = await Core.Services.EntryService.QueryEntryAsync(sortModel, filterModel);
                    int entryCount = RandomHelper.RandomOne(new int[] { 6, 8 });
                    var result = RandomHelper.RandomList(queryResults, entryCount);
                    if (result?.Any() == true)
                    {
                        var entrys = await Core.Services.EntryService.QueryEntryAsync(result.Select(p => p.ToQueryItem()).ToList());
                        if (entrys?.Any() == true)
                        {
                            otherCollectionTree.Children.Add(new LabelCollection()
                            {
                                Title = labelTree.LabelClass.LabelClassDb.Name,
                                Description = labelTree.LabelClass.LabelClassDb.Description,
                                Entries = entrys,
                                Template = entryCount == 6 ? 1 : 2,
                                Id = labelTree.LabelClass.LabelClassDb.LCID
                            });
                        }
                    }
                }
            }

            if (otherCollectionTree.Children.Any())
            {
                if (items.Count != 0)
                {
                    otherCollectionTree.LabelCollection.Title = "其他";
                }
                items.Add(otherCollectionTree);
            }
            LabelCollectionTrees = items;
        }

        private async Task InitLabelCollection4Async()
        {
            var items = new List<LabelCollectionTree>();
            LabelCollectionTree otherCollectionTree = new LabelCollectionTree()
            {
                LabelCollection = new LabelCollection()
                {
                    Title = "所有",
                    Description = string.Empty,
                },
                Children = new List<LabelCollection>()
            };

            // 检查 LabelTrees 是否为 null
            if (LabelTrees == null)
            {
                LabelCollectionTrees = new List<LabelCollectionTree>();
                return;
            }

            foreach (var labelTree in LabelTrees)
            {
                if (labelTree.Children.Any())
                {
                    LabelCollectionTree labelCollectionTree = new LabelCollectionTree()
                    {
                        LabelCollection = new LabelCollection()
                        {
                            Title = labelTree.LabelClass.LabelClassDb.Name,
                            Description = labelTree.LabelClass.LabelClassDb.Description,
                        },
                        Children = new List<LabelCollection>()
                    };

                    foreach (var label in labelTree.Children)
                    {
                        var sortModel = new SortModel(SortType.LastUpdateTime, SortWay.Positive);
                        var filterModel = new FilterModel();
                        filterModel.IsFilterLabelClass = true;
                        filterModel.LabelClassIds.Add(label.LabelClass.LabelClassDb.LCID);
                        var queryResults = await Core.Services.EntryService.QueryEntryAsync(sortModel, filterModel);
                        var showItem = RandomHelper.RandomOne(queryResults);
                        if (showItem != null)
                        {
                            var entry = await Core.Services.EntryService.QueryEntryAsync(showItem.ToQueryItem());
                            if (entry != null)
                            {
                                labelCollectionTree.Children.Add(new LabelCollection()
                                {
                                    Title = label.LabelClass.LabelClassDb.Name,
                                    Description = label.LabelClass.LabelClassDb.Description,
                                    Id = label.LabelClass.LabelClassDb.LCID,
                                });
                            }
                        }
                    }
                    items.Add(labelCollectionTree);
                }
                else
                {
                    var sortModel = new SortModel(SortType.LastUpdateTime, SortWay.Positive);
                    var filterModel = new FilterModel();
                    filterModel.IsFilterLabelClass = true;
                    filterModel.LabelClassIds.Add(labelTree.LabelClass.LabelClassDb.LCID);
                    var queryResults = await Core.Services.EntryService.QueryEntryAsync(sortModel, filterModel);
                    var showItem = RandomHelper.RandomOne(queryResults);
                    if (showItem != null)
                    {
                        var entry = await Core.Services.EntryService.QueryEntryAsync(showItem.ToQueryItem());
                        if (entry != null)
                        {
                            otherCollectionTree.Children.Add(new LabelCollection()
                            {
                                Title = labelTree.LabelClass.LabelClassDb.Name,
                                Description = labelTree.LabelClass.LabelClassDb.Description,
                                Id = labelTree.LabelClass.LabelClassDb.LCID
                            });
                        }
                    }
                }
            }

            if (otherCollectionTree.Children.Any())
            {
                if (items.Count != 0)
                {
                    otherCollectionTree.LabelCollection.Title = "其他";
                }
                items.Add(otherCollectionTree);
            }
            LabelCollectionTrees = items;
        }

        [RelayCommand]
        private async Task BannerDetail(BannerItem item)
        {
            if (item?.Tag == "All")
            {
                var sortModel = new SortModel(SortType.LastUpdateTime, SortWay.Positive);
                var filterModel = new FilterModel();
                var queryResults = await Core.Services.EntryService.QueryEntryAsync(sortModel, filterModel);
                var entrys = await Core.Services.EntryService.QueryEntryAsync(queryResults.Select(p => p.ToQueryItem()).ToList());
                if (entrys?.Any() == true)
                {
                    LabelCollection labelCollection = new LabelCollection()
                    {
                        Title = "所有",
                        Description = String.Empty,
                        Entries = entrys,
                        Id = null
                    };
                    // TODO: 导航到 LabelCollectionPage
                }
            }
            else
            {
                LabelDetailCommand.Execute(item?.Id);
            }
        }

        [RelayCommand]
        private async Task LabelDetail(string id)
        {
            if (string.IsNullOrEmpty(id) || labelsDic == null || !labelsDic.TryGetValue(id, out var label))
            {
                return;
            }

            var sortModel = new SortModel(SortType.LastUpdateTime, SortWay.Positive);
            var filterModel = new FilterModel();
            filterModel.IsFilterLabelClass = true;
            filterModel.LabelClassIds.Add(label.LabelClassDb.LCID);
            var queryResults = await Core.Services.EntryService.QueryEntryAsync(sortModel, filterModel);
            var entrys = await Core.Services.EntryService.QueryEntryAsync(queryResults.Select(p => p.ToQueryItem()).ToList());
            if (entrys?.Any() == true)
            {
                LabelCollection labelCollection = new LabelCollection()
                {
                    Title = label.LabelClassDb.Name,
                    Description = label.LabelClassDb.Description,
                    Entries = entrys,
                    Id = label.LabelClassDb.LCID
                };
                // TODO: 导航到 LabelCollectionPage
            }
        }

        [RelayCommand]
        private void Refresh()
        {
            Init();
        }

        [RelayCommand]
        private async Task ChangeShowType(string isListStr)
        {
            LabelCollectionTrees?.Clear();
            LabelCollectionTrees = null;
            IsList = bool.Parse(isListStr);
            await InitLabelCollectionAsync();
        }

        [RelayCommand]
        private void ItemClick(Core.Models.Entry entry)
        {
            // TODO: 导航到 EntryDetailPage
        }
    }
}
