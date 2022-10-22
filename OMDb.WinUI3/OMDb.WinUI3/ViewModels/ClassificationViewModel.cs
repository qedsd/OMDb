using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using OMDb.Core.Enums;
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
        private ObservableCollection<Label> labels;
        /// <summary>
        /// 只包括没有子类的
        /// </summary>
        public ObservableCollection<Label> Labels
        {
            get => labels;
            set => SetProperty(ref labels, value);
        }
        public ICommand CheckDetailBannerItemCommand => new RelayCommand<BannerItem>((item) =>
        {

        });
        public ClassificationViewModel()
        {
            //var items = new List<BannerItem>();
            //items.Add(new BannerItem()
            //{
            //    Title = "标题1",
            //    Description = "描述",
            //    Img = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanner.jpg"))),
            //    PreviewImg = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanneritem.jpg")))
            //});
            //items.Add(new BannerItem()
            //{
            //    Title = "标题2",
            //    Description = "描述",
            //    Img = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanner.jpg"))),
            //    PreviewImg = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanneritem.jpg")))
            //});
            //items.Add(new BannerItem()
            //{
            //    Title = "标题3",
            //    Description = "描述",
            //    Img = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanner.jpg"))),
            //    PreviewImg = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanneritem.jpg")))
            //});

            Init();
            //BannerItemSource = items;
        }

        private async void Init()
        {
            var labels = await Core.Services.LabelService.GetAllLabelAsync();
            if (labels != null)
            {
                Dictionary<string, LabelTree> labelsDb = new Dictionary<string, LabelTree>();
                Labels = new ObservableCollection<Label>();
                var root = labels.Where(p => p.ParentId == null).ToList();
                if (root != null)
                {
                    foreach (var label in root)
                    {
                        labelsDb.Add(label.Id, new LabelTree(label));
                    }
                }
                foreach (var label in labels)
                {
                    if (label.ParentId != null)
                    {
                        if (labelsDb.TryGetValue(label.ParentId, out var parent))
                        {
                            parent.Children.Add(label);
                        }
                        Labels.Add(new Label(label));
                    }
                }
                LabelTrees = new ObservableCollection<LabelTree>();
                foreach (var item in labelsDb)
                {
                    LabelTrees.Add(item.Value);
                }
            }
        }

        private async void InitBanner()
        {
            if(Labels == null)
            {
                return;
            }
            List<Label> target = Core.Helpers.RandomHelper.RandomList(Labels, 10);
            foreach(var item in target)
            {
                var queryResults = await Core.Services.EntryService.QueryEntryAsync(SortType.LastUpdateTime, SortWay.Positive, null, new List<string>() { item.LabelDb.Id });
                var result = Core.Helpers.RandomHelper.RandomList(queryResults,3);
                var entrys = await Core.Services.EntryService.QueryEntryAsync(result.Select(p => p.ToQueryItem()).ToList());
                if (entrys != null)
                {
                    List<string> covers = new List<string>(entrys.Count);//所有词条封面图
                    foreach(var entry in entrys)
                    {
                        covers.Add(Helpers.PathHelper.EntryCoverImgFullPath(entry));
                    }
                    string cover = FindBannerCover(entrys);//背景
                    //todo:合并成一个图

                }
            }
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
                return bestImages.OrderByDescending(p => p.Length).First().FullPath;
            }
            else
            {
                return null;
            }
        }
    }
}
