﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using OMDb.Core.Enums;
using OMDb.Core.Helpers;
using OMDb.Core.Models;
using OMDb.Core.Services;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels
{
    internal class ClassificationViewModel : ObservableObject
    {
        private List<BannerItem> _bannerItemsSource;
        public List<BannerItem> BannerItemSource
        {
            get => _bannerItemsSource;
            set => SetProperty(ref _bannerItemsSource, value);
        }

        private ObservableCollection<LabelClassTree> _labelTrees;
        public ObservableCollection<LabelClassTree> LabelTrees
        {
            get => _labelTrees;
            set => SetProperty(ref _labelTrees, value);
        }

        private List<LabelCollectionTree> _labelCollectionTrees;
        public List<LabelCollectionTree> LabelCollectionTrees
        {
            get => _labelCollectionTrees;
            set => SetProperty(ref _labelCollectionTrees, value);
        }

        private ObservableCollection<LabelClass> _labelClasses;
        /// <summary>
        /// 只包括没有子类的
        /// </summary>
        public ObservableCollection<LabelClass> LabelClasses
        {
            get => _labelClasses;
            set => SetProperty(ref _labelClasses, value);
        }
        /// <summary>
        /// 所有标签
        /// </summary>
        private Dictionary<string, LabelClass> LabelsDic;
        private bool isList;
        /// <summary>
        /// 列表显示
        /// </summary>
        public bool IsList
        {
            get => isList;
            set => SetProperty(ref isList, value);
        }

        public ClassificationViewModel()
        {
            IsList = Services.LabelCollectionStyleSelectorService.IsList;
            Init();
        }

        private async void Init()
        {
            Helpers.InfoHelper.ShowWaiting();
            await InitLabels();
            await InitBannerAsync();
            await InitLabelCollectionAsync();
            Helpers.InfoHelper.HideWaiting();
        }

        private async Task InitLabels()
        {
            if (LabelsDic == null)
            {
                LabelsDic = new Dictionary<string, LabelClass>();
            }
            else
            {
                LabelsDic.Clear();
            }
            var labels = await Core.Services.LabelClassService.GetAllLabelAsync(Services.Settings.DbSelectorService.dbCurrentId);
            if (labels != null)
            {
                foreach (var label in labels)
                {
                    LabelsDic.Add(label.LCID, new LabelClass(label));
                }

                Dictionary<string, LabelClassTree> labelsDb = new Dictionary<string, LabelClassTree>();//string为父节点id
                LabelClasses = new ObservableCollection<LabelClass>();
                var root = labels.Where(p => p.ParentId == null).ToList();
                if (root != null)//建立所有父节点
                {
                    foreach (var label in root)
                    {
                        labelsDb.Add(label.LCID, new LabelClassTree(label));
                    }
                }
                foreach (var label in labels)
                {
                    if (label.ParentId != null)//归类每个子节点
                    {
                        if (labelsDb.TryGetValue(label.ParentId, out var parent))
                        {
                            parent.Children.Add(new LabelClassTree(label));
                        }
                        LabelClasses.Add(new LabelClass(label));//保存所有子节点
                    }
                }
                foreach (var item in labelsDb)
                {
                    if (item.Value.Children.Count == 0)//没有实际子节点的父节点也保存到Labels
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

        #region Banner
        private async Task InitBannerAsync()
        {
            if (LabelClasses == null)
            {
                return;
            }
            var items = new List<BannerItem>();
            items.Add(await GetAllBannerItem());
            List<LabelClass> target = Core.Helpers.RandomHelper.RandomList(LabelClasses, 10);
            foreach (var item in target)
            {
                var sortModel = new SortModel(SortType.LastUpdateTime, SortWay.Positive);
                var filterModel = new FilterModel();
                filterModel.IsFilterLabelClass = true;
                filterModel.LabelClassIds.Add(item.LabelClassDb.LCID);

                var queryResults = await Core.Services.EntryService.QueryEntryAsync(sortModel, filterModel);
                var result = Core.Helpers.RandomHelper.RandomList(queryResults, 3);
                if (result?.Any() == true)
                {
                    var entrys = await Core.Services.EntryService.QueryEntryAsync(result.Select(p => p.ToQueryItem()).ToList());
                    if (entrys?.Any() == true)
                    {
                        string bg = FindBannerCover(entrys);//背景
                        if (!string.IsNullOrEmpty(bg))
                        {
                            var samllStream = await Core.Helpers.ImageHelper.ResetSizeAsync(bg, 400, 0);
                            items.Add(new BannerItem()
                            {
                                Id = item.LabelClassDb.LCID,
                                Title = item.LabelClassDb.Name,
                                Description = item.LabelClassDb.Description,
                                Img = new BitmapImage(new Uri(bg)),
                                PreviewImg = await Helpers.ImgHelper.CreateBitmapImageAsync(samllStream)
                            });
                            samllStream.Dispose();
                        }
                        else
                        {
                            items.Add(new BannerItem()
                            {
                                Id = item.LabelClassDb.LCID,
                                Title = item.LabelClassDb.Name,
                                Description = item.LabelClassDb.Description,
                                Img = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanner.jpg"))),
                                PreviewImg = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanneritem.jpg")))
                            });
                        }
                    }
                }
            }
            BannerItemSource = items;
        }
        private async Task<BannerItem> GetAllBannerItem()
        {
            var sortModel = new SortModel(SortType.LastUpdateTime, SortWay.Positive);//排序规则
            var filterModel = new FilterModel();//过滤规则
            filterModel.IsFilterLabelClass = true;
            filterModel.LabelClassIds = LabelClasses.Select(p => p.LabelClassDb.LCID).ToList();//过滤分类
            var queryResults = await Core.Services.EntryService.QueryEntryAsync(sortModel, filterModel);
            if (queryResults?.Count > 2)
            {
                var result = Core.Helpers.RandomHelper.RandomList(queryResults, 40);
                if (result?.Any() == true)
                {
                    var entrys = await Core.Services.EntryService.QueryEntryAsync(result.Select(p => p.ToQueryItem()).ToList());
                    if (entrys?.Any() == true)
                    {
                        List<string> covers = new List<string>(entrys.Count);//所有词条封面图
                        foreach (var entry in entrys)
                        {
                            covers.Add(PathService.EntryCoverImgFullPath(entry));
                        }
                        string bg = covers.FirstOrDefault();
                        if (!string.IsNullOrEmpty(bg))
                        {
                            //合并成一个图
                            var bg1920Stream = await Core.Helpers.ImageHelper.ResetSizeAsync(bg, 1920, 1080);
                            //手动绘制实现封面图
                            var savedStream = await Core.Helpers.ImageHelper.DrawWaterfallAsync(covers, bg1920Stream);
                            var smallStream = await Core.Helpers.ImageHelper.ResetSizeAsync(savedStream, 400, 0);
                            var bannerItem = new BannerItem()
                            {
                                Title = "全部",
                                Tag = "All",
                                Description = null,
                                Img = await Helpers.ImgHelper.CreateBitmapImageAsync(savedStream),
                                PreviewImg = await Helpers.ImgHelper.CreateBitmapImageAsync(smallStream)
                            };
                            bg1920Stream.Dispose();
                            savedStream.Dispose();
                            smallStream.Dispose();
                            return bannerItem;
                        }
                    }
                }
            }
            return new BannerItem()
            {
                Title = "全部",
                Tag = "All",
                Description = null,
                Img = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanner.jpg"))),
                PreviewImg = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanneritem.jpg")))
            };
        }

        /// <summary>
        /// 按长宽比、文件大小选出最优封面
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        private string FindBannerCover(List<Core.Models.Entry> entries)
        {
            List<Core.Models.ImageInfo> bestImages = new List<Core.Models.ImageInfo>();
            foreach (var entry in entries)
            {
                var fullPath = PathService.EntryFullPath(entry);
                string imgFolder = Path.Combine(fullPath, Services.ConfigService.InfoFolder);
                if (Directory.Exists(imgFolder))
                {
                    var items = Helpers.FileHelper.GetAllFiles(imgFolder);
                    List<Core.Models.ImageInfo> infos = new List<Core.Models.ImageInfo>();
                    if (items != null && items.Any())
                    {
                        foreach (var file in Core.Helpers.RandomHelper.RandomList(items, 100))//仅对100张照片计算
                        {
                            if (ImageHelper.IsSupportImg(file.FullName))
                            {
                                infos.Add(Core.Helpers.ImageHelper.GetImageInfo(file.FullName));
                            }
                        }
                    }
                    //优先匹配长大于宽、文件更大的照片
                    var sortedInfos = infos.Where(p => p.Scale > 1.2).OrderBy(p => p.Length).ToList();
                    int[] weights = new int[sortedInfos.Count];
                    for (int i = 0; i < sortedInfos.Count; i++)
                    {
                        weights[i] = i + 1;//权重从1开始递增
                    }
                    var coverItems = Core.Helpers.RandomHelper.RandomList(sortedInfos, weights, 1);//获取最优的
                    if (coverItems != null && coverItems.Any())
                    {
                        bestImages.Add(coverItems.First());
                    }
                }
            }
            if (bestImages.Count > 0)
            {
                return Core.Helpers.RandomHelper.RandomOne(bestImages).FullPath;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region LabelCollection
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
                        var sortModel = new SortModel(SortType.LastUpdateTime, SortWay.Positive);//排序规则
                        var filterModel = new FilterModel();//过滤规则
                        filterModel.IsFilterLabelClass = true;
                        filterModel.LabelClassIds.Add(label.LabelClass.LabelClassDb.LCID);//过滤分类
                        var queryResults = await Core.Services.EntryService.QueryEntryAsync(sortModel, filterModel);
                        int entryCount = Core.Helpers.RandomHelper.RandomOne(new int[] { 6, 8 });
                        var result = Core.Helpers.RandomHelper.RandomList(queryResults, entryCount);
                        if (result?.Any() == true)
                        {
                            var entrys = await Core.Services.EntryService.QueryEntryAsync(result.Select(p => p.ToQueryItem()).ToList());
                            if (entrys?.Any() == true)
                            {
                                var bgStream = await Core.Helpers.ImageHelper.BlurAsync(PathService.EntryCoverImgFullPath(entrys.FirstOrDefault()));
                                labelCollectionTree.Children.Add(new LabelCollection()
                                {
                                    Title = label.LabelClass.LabelClassDb.Name,
                                    Description = label.LabelClass.LabelClassDb.Description,
                                    Entries = entrys,
                                    ImageSource = await Helpers.ImgHelper.CreateBitmapImageAsync(bgStream),
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
                    var sortModel = new SortModel(SortType.LastUpdateTime, SortWay.Positive);//排序规则
                    var filterModel = new FilterModel();//过滤规则
                    filterModel.IsFilterLabelClass = true;
                    filterModel.LabelClassIds.Add(labelTree.LabelClass.LabelClassDb.LCID);//过滤分类
                    var queryResults = await Core.Services.EntryService.QueryEntryAsync(sortModel, filterModel);
                    int entryCount = Core.Helpers.RandomHelper.RandomOne(new int[] { 6, 8 });
                    var result = Core.Helpers.RandomHelper.RandomList(queryResults, entryCount);
                    if (result?.Any() == true)
                    {
                        var entrys = await Core.Services.EntryService.QueryEntryAsync(result.Select(p => p.ToQueryItem()).ToList());
                        if (entrys?.Any() == true)
                        {
                            var bgStream = await Core.Helpers.ImageHelper.BlurAsync(PathService.EntryCoverImgFullPath(entrys.FirstOrDefault()));
                            otherCollectionTree.Children.Add(new LabelCollection()
                            {
                                Title = labelTree.LabelClass.LabelClassDb.Name,
                                Description = labelTree.LabelClass.LabelClassDb.Description,
                                Entries = entrys,
                                ImageSource = await Helpers.ImgHelper.CreateBitmapImageAsync(bgStream),
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
                        var sortModel = new SortModel(SortType.LastUpdateTime, SortWay.Positive);//排序规则
                        var filterModel = new FilterModel();//过滤规则
                        filterModel.IsFilterLabelClass = true;
                        filterModel.LabelClassIds.Add(label.LabelClass.LabelClassDb.LCID);//过滤分类
                        var queryResults = await Core.Services.EntryService.QueryEntryAsync(sortModel, filterModel);
                        var showItem = Core.Helpers.RandomHelper.RandomOne(queryResults);
                        if (showItem != null)
                        {
                            var entry = await Core.Services.EntryService.QueryEntryAsync(showItem.ToQueryItem());
                            if (entry != null)
                            {
                                var bgStream = await Core.Helpers.ImageHelper.ResetSizeAsync(PathService.EntryCoverImgFullPath(entry), 600, 0);
                                labelCollectionTree.Children.Add(new LabelCollection()
                                {
                                    Title = label.LabelClass.LabelClassDb.Name,
                                    Description = label.LabelClass.LabelClassDb.Description,
                                    ImageSource = await Helpers.ImgHelper.CreateBitmapImageAsync(bgStream),
                                    Id = label.LabelClass.LabelClassDb.LCID,
                                });
                            }
                        }
                    }
                    items.Add(labelCollectionTree);
                }
                else
                {
                    var sortModel = new SortModel(SortType.LastUpdateTime, SortWay.Positive);//排序规则
                    var filterModel = new FilterModel();//过滤规则
                    filterModel.IsFilterLabelClass = true;
                    filterModel.LabelClassIds.Add(labelTree.LabelClass.LabelClassDb.LCID);//过滤分类
                    var queryResults = await Core.Services.EntryService.QueryEntryAsync(sortModel, filterModel);
                    var showItem = Core.Helpers.RandomHelper.RandomOne(queryResults);
                    if (showItem != null)
                    {
                        var entry = await Core.Services.EntryService.QueryEntryAsync(showItem.ToQueryItem());
                        if (entry != null)
                        {
                            var bgStream = await Core.Helpers.ImageHelper.ResetSizeAsync(PathService.EntryCoverImgFullPath(entry), 600, 0);
                            otherCollectionTree.Children.Add(new LabelCollection()
                            {
                                Title = labelTree.LabelClass.LabelClassDb.Name,
                                Description = labelTree.LabelClass.LabelClassDb.Description,
                                ImageSource = await Helpers.ImgHelper.CreateBitmapImageAsync(bgStream),
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
        #endregion

        public ICommand BannerDetailCommand => new RelayCommand<BannerItem>(async (item) =>
        {
            if (item.Tag == "All")
            {
                Helpers.InfoHelper.ShowWaiting();
                var sortModel = new SortModel(SortType.LastUpdateTime, SortWay.Positive);//排序规则
                var filterModel = new FilterModel();//过滤规则
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
                    Services.TabViewService.AddItem(new Views.LabelCollectionPage(labelCollection));
                }
                Helpers.InfoHelper.HideWaiting();
            }
            else
            {
                LabelDetailCommand.Execute(item.Id);
            }
        });
        public ICommand LabelDetailCommand => new RelayCommand<string>(async (id) =>
        {
            if (LabelsDic.TryGetValue(id, out var label))
            {
                Helpers.InfoHelper.ShowWaiting();
                var sortModel = new SortModel(SortType.LastUpdateTime, SortWay.Positive);//排序规则
                var filterModel = new FilterModel();//过滤规则
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
                    Services.TabViewService.AddItem(new Views.LabelCollectionPage(labelCollection));
                }
                Helpers.InfoHelper.HideWaiting();
            }
        });

        public ICommand RefreshCommand => new RelayCommand(() =>
        {
            Init();
        });

        public ICommand ChangeShowTypeCommand => new RelayCommand<string>(async (islistStr) =>
        {
            LabelCollectionTrees?.Clear();
            LabelCollectionTrees = null;
            IsList = bool.Parse(islistStr);
            Helpers.InfoHelper.ShowWaiting();
            await InitLabelCollectionAsync();
            await Services.LabelCollectionStyleSelectorService.SetAsync(IsList ? 0 : 1);
            Helpers.InfoHelper.HideWaiting();
        });
        public ICommand ItemClickCommand => new RelayCommand<Entry>((entry) =>
        {
            TabViewService.AddItem(new Views.EntryDetailPage(entry));
        });
    }
}
