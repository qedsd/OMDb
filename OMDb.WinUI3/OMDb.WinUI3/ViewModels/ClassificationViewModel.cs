using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using OMDb.Core.Enums;
using OMDb.Core.Models;
using OMDb.WinUI3.Models;
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
        private List<BannerItem> bannerItemsSource;
        public List<BannerItem> BannerItemSource
        {
            get => bannerItemsSource;
            set => SetProperty(ref bannerItemsSource, value);
        }

        private ObservableCollection<LabelTree> labelTrees;
        public ObservableCollection<LabelTree> LabelTrees
        {
            get => labelTrees;
            set => SetProperty(ref labelTrees, value);
        }

        private List<LabelCollectionTree> labelCollectionTrees;
        public List<LabelCollectionTree> LabelCollectionTrees
        {
            get => labelCollectionTrees;
            set => SetProperty(ref labelCollectionTrees, value);
        }

        private ObservableCollection<Label> labels;
        /// <summary>
        /// 只包括没有子类的
        /// </summary>
        public ObservableCollection<Label> Labels
        {
            get => labels;
            set => SetProperty(ref labels, value);
        }

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
            var labels = await Core.Services.LabelService.GetAllLabelAsync();
            if (labels != null)
            {
                Dictionary<string, LabelTree> labelsDb = new Dictionary<string, LabelTree>();//string为父节点id
                Labels = new ObservableCollection<Label>();
                var root = labels.Where(p => p.ParentId == null).ToList();
                if (root != null)//建立所有父节点
                {
                    foreach (var label in root)
                    {
                        labelsDb.Add(label.Id, new LabelTree(label));
                    }
                }
                foreach (var label in labels)
                {
                    if (label.ParentId != null)//归类每个子节点
                    {
                        if (labelsDb.TryGetValue(label.ParentId, out var parent))
                        {
                            parent.Children.Add(label);
                        }
                        Labels.Add(new Label(label));//保存所有子节点
                    }
                }
                foreach (var item in labelsDb)
                {
                    if(item.Value.Children.Count == 0)//没有实际子节点的父节点也保存到Labels
                    {
                        Labels.Add(new Label(item.Value.Label));
                    }
                }
                LabelTrees = new ObservableCollection<LabelTree>();
                foreach (var item in labelsDb)
                {
                    LabelTrees.Add(item.Value);
                }
            }
            await InitBannerAsync();
            await InitLabelCollectionAsync();
            Helpers.InfoHelper.HideWaiting();
        }

        #region Banner
        private async Task InitBannerAsync()
        {
            if(Labels == null)
            {
                return;
            }
            var items = new List<BannerItem>();
            items.Add(await GetAllBannerItem());
            List<Label> target = Core.Helpers.RandomHelper.RandomList(Labels, 10);
            foreach(var item in target)
            {
                var queryResults = await Core.Services.EntryService.QueryEntryAsync(SortType.LastUpdateTime, SortWay.Positive, null, new List<string>() { item.LabelDb.Id });
                var result = Core.Helpers.RandomHelper.RandomList(queryResults,3);
                if(result?.Any() == true)
                {
                    var entrys = await Core.Services.EntryService.QueryEntryAsync(result.Select(p => p.ToQueryItem()).ToList());
                    if (entrys?.Any() == true)
                    {
                        string bg = FindBannerCover(entrys);//背景
                        if(!string.IsNullOrEmpty(bg))
                        {
                            var samllStream = await Core.Helpers.ImageHelper.ResetSizeAsync(bg, 400, 0);
                            items.Add(new BannerItem()
                            {
                                Title = item.LabelDb.Name,
                                Description = item.LabelDb.Description,
                                Img = new BitmapImage(new Uri(bg)),
                                PreviewImg = await Helpers.ImgHelper.CreateBitmapImageAsync(samllStream)
                            });
                            samllStream.Dispose();
                        }
                        else
                        {
                            items.Add(new BannerItem()
                            {
                                Title = item.LabelDb.Name,
                                Description = item.LabelDb.Description,
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
            var queryResults = await Core.Services.EntryService.QueryEntryAsync(SortType.LastUpdateTime, SortWay.Positive, null, Labels.Select(p=>p.LabelDb.Id).ToList());
            if(queryResults?.Count > 2)
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
                            covers.Add(Helpers.PathHelper.EntryCoverImgFullPath(entry));
                        }
                        string bg = covers.FirstOrDefault();
                        if (!string.IsNullOrEmpty(bg))
                        {
                            //合并成一个图
                            var bg1920Stream = await Core.Helpers.ImageHelper.ResetSizeAsync(bg, 1920, 1080);
                            //手动绘制实现封面图
                            var savedStream = await Core.Helpers.ImageHelper.DrawWaterfallAsync(covers, bg1920Stream);
                            var smallStream = await Core.Helpers.ImageHelper.ResetSizeAsync(savedStream, 400, 0);
                            var bannerItem =  new BannerItem()
                            {
                                Title = "全部",
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
            foreach(var entry in entries)
            {
                var fullPath = Helpers.PathHelper.EntryFullPath(entry);
                string imgFolder = Path.Combine(fullPath, Services.ConfigService.ImgFolder);
                if (Directory.Exists(imgFolder))
                {
                    var items = Helpers.FileHelper.GetAllFiles(imgFolder);
                    List<Core.Models.ImageInfo> infos = new List<Core.Models.ImageInfo>();
                    if (items != null && items.Any())
                    {
                        foreach (var file in Core.Helpers.RandomHelper.RandomList(items,100))//仅对100张照片计算
                        {
                            if (Helpers.ImgHelper.IsSupportImg(file.FullName))
                            {
                                infos.Add(Core.Helpers.ImageHelper.GetImageInfo(file.FullName));
                            }
                        }
                    }
                    //优先匹配长大于宽、文件更大的照片
                    var sortedInfos = infos.Where(p=>p.Scale > 1.2).OrderBy(p=>p.Length).ToList();
                    int[] weights = new int[sortedInfos.Count];
                    for (int i = 0; i < sortedInfos.Count; i++)
                    {
                        weights[i] = i + 1;//权重从1开始递增
                    }
                    var coverItems = Core.Helpers.RandomHelper.RandomList(sortedInfos, weights, 1);//获取最优的
                    if(coverItems != null && coverItems.Any())
                    {
                        bestImages.Add(coverItems.First());
                    }
                }
            }
            if(bestImages.Count > 0)
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
            if(IsList)
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
                    Title = "未分类",
                    Description = string.Empty,
                },
                Children = new List<LabelCollection>()
            };
            foreach (var labelTree in LabelTrees)
            {
                if(labelTree.Children.Any())
                {
                    LabelCollectionTree labelCollectionTree = new LabelCollectionTree()
                    {
                        LabelCollection = new LabelCollection()
                        {
                            Title = labelTree.Label.Name,
                            Description = labelTree.Label.Description,
                        },
                        Children = new List<LabelCollection>()
                    };
                    foreach (var label in labelTree.Children)
                    {
                        var queryResults = await Core.Services.EntryService.QueryEntryAsync(SortType.LastUpdateTime, SortWay.Positive, null, new List<string>() { label.Id });
                        int entryCount = Core.Helpers.RandomHelper.RandomOne(new int[] { 6, 8 });
                        var result = Core.Helpers.RandomHelper.RandomList(queryResults, entryCount);
                        if (result?.Any() == true)
                        {
                            var entrys = await Core.Services.EntryService.QueryEntryAsync(result.Select(p => p.ToQueryItem()).ToList());
                            if (entrys?.Any() == true)
                            {
                                var bgStream = await Core.Helpers.ImageHelper.BlurAsync(Helpers.PathHelper.EntryCoverImgFullPath(entrys.FirstOrDefault()));
                                labelCollectionTree.Children.Add(new LabelCollection()
                                {
                                    Title = label.Name,
                                    Description = label.Description,
                                    Entries = entrys,
                                    ImageSource = await Helpers.ImgHelper.CreateBitmapImageAsync(bgStream),
                                    Template = entryCount == 6 ? 1 : 2
                                });
                            }
                        }
                    }
                    items.Add(labelCollectionTree);
                }
                else
                {
                    var queryResults = await Core.Services.EntryService.QueryEntryAsync(SortType.LastUpdateTime, SortWay.Positive, null, new List<string>() { labelTree.Label.Id });
                    int entryCount = Core.Helpers.RandomHelper.RandomOne(new int[] { 6, 8 });
                    var result = Core.Helpers.RandomHelper.RandomList(queryResults, entryCount);
                    if (result?.Any() == true)
                    {
                        var entrys = await Core.Services.EntryService.QueryEntryAsync(result.Select(p => p.ToQueryItem()).ToList());
                        if (entrys?.Any() == true)
                        {
                            var bgStream = await Core.Helpers.ImageHelper.BlurAsync(Helpers.PathHelper.EntryCoverImgFullPath(entrys.FirstOrDefault()));
                            otherCollectionTree.Children.Add(new LabelCollection()
                            {
                                Title = labelTree.Label.Name,
                                Description = labelTree.Label.Description,
                                Entries = entrys,
                                ImageSource = await Helpers.ImgHelper.CreateBitmapImageAsync(bgStream),
                                Template = entryCount == 6 ? 1 : 2
                            });
                        }
                    }
                }
            }
            if(otherCollectionTree.Children.Any())
            {
                items.Insert(0,otherCollectionTree);
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
                    Title = "未分类",
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
                            Title = labelTree.Label.Name,
                            Description = labelTree.Label.Description,
                        },
                        Children = new List<LabelCollection>()
                    };
                    foreach (var label in labelTree.Children)
                    {
                        var queryResults = await Core.Services.EntryService.QueryEntryAsync(SortType.LastUpdateTime, SortWay.Positive, null, new List<string>() { label.Id });
                        var showItem = Core.Helpers.RandomHelper.RandomOne(queryResults);
                        if (showItem != null)
                        {
                            var entry = await Core.Services.EntryService.QueryEntryAsync(showItem.ToQueryItem());
                            if (entry != null)
                            {
                                var bgStream = await Core.Helpers.ImageHelper.ResetSizeAsync(Helpers.PathHelper.EntryCoverImgFullPath(entry), 600, 0);
                                labelCollectionTree.Children.Add(new LabelCollection()
                                {
                                    Title = label.Name,
                                    Description = label.Description,
                                    ImageSource = await Helpers.ImgHelper.CreateBitmapImageAsync(bgStream),
                                });
                            }
                        }
                    }
                    items.Add(labelCollectionTree);
                }
                else
                {
                    var queryResults = await Core.Services.EntryService.QueryEntryAsync(SortType.LastUpdateTime, SortWay.Positive, null, new List<string>() { labelTree.Label.Id });
                    var showItem = Core.Helpers.RandomHelper.RandomOne(queryResults);
                    if (showItem != null)
                    {
                        var entry = await Core.Services.EntryService.QueryEntryAsync(showItem.ToQueryItem());
                        if (entry != null)
                        {
                            var bgStream = await Core.Helpers.ImageHelper.ResetSizeAsync(Helpers.PathHelper.EntryCoverImgFullPath(entry), 600, 0);
                            otherCollectionTree.Children.Add(new LabelCollection()
                            {
                                Title = labelTree.Label.Name,
                                Description = labelTree.Label.Description,
                                ImageSource = await Helpers.ImgHelper.CreateBitmapImageAsync(bgStream),
                            });
                        }
                    }
                }
                    
            }
            if (otherCollectionTree.Children.Any())
            {
                items.Insert(0, otherCollectionTree);
            }
            LabelCollectionTrees = items;
        }
        #endregion

        public ICommand CheckDetailBannerItemCommand => new RelayCommand<BannerItem>((item) =>
        {

        });

        public ICommand RefreshCommand => new RelayCommand(() =>
        {
            Init();
        });

        public ICommand ChangeShowTypeCommand => new RelayCommand<string>(async(islistStr) =>
        {
            LabelCollectionTrees?.Clear();
            LabelCollectionTrees = null;
            IsList = bool.Parse(islistStr);
            Helpers.InfoHelper.ShowWaiting();
            await InitLabelCollectionAsync();
            await Services.LabelCollectionStyleSelectorService.SetAsync(IsList ? 0 : 1);
            Helpers.InfoHelper.HideWaiting();
        });
    }
}
