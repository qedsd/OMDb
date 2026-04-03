using ABI.System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Shapes;
using OMDb.WinUI3.Views.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xabe.FFmpeg.Streams.SubtitleStream;

namespace OMDb.WinUI3.ViewModels.Tools
{
    internal class SubToolViewModel : ToolBaseViewModel
    {
        private bool infoPanelVisible;
        public bool InfoPanelVisible
        {
            get => infoPanelVisible;
            set => SetProperty(ref infoPanelVisible, value);
        }

        private Core.Models.MediaInfo mediaInfo;
        public Core.Models.MediaInfo MediaInfo
        {
            get => mediaInfo;
            set 
            {
                SetProperty(ref mediaInfo, value);
                InfoPanelVisible = value == null? false : true;
            }
        }

        private ObservableCollection<Models.NewSubtitle> newSubtitleInfos = new ObservableCollection<Models.NewSubtitle>();
        public ObservableCollection<Models.NewSubtitle> NewSubtitleInfos
        {
            get => newSubtitleInfos;
            set => SetProperty(ref newSubtitleInfos, value);
        }

        private List<string> subCodecs;
        /// <summary>
        /// 字幕格式
        /// </summary>
        public List<string> SubCodecs
        {
            get => subCodecs;
            set => SetProperty(ref subCodecs, value);
        }

        private List<string> langs;
        /// <summary>
        /// 语言
        /// </summary>
        public List<string> Langs
        {
            get => langs;
            set => SetProperty(ref langs, value);
        }

        public SubToolViewModel()
        {
            var subs = new List<string>();
            foreach(var c in Enum.GetValues(typeof(SubtitleCodec)))
            {
                subs.Add(c.ToString());
            }
            SubCodecs = subs;
            Langs = new List<string>()
            {
                "English",
                "French",
                "Chinese",
                "Japanese",
            };
        }

        public ICommand SelecteFileCommand => new RelayCommand(async() =>
        {
            var file = await Helpers.PickHelper.PickFileAsync(".mkv",ToolPageBase.Window);
            if (file != null)
            {
                try
                {
                    MediaInfo = await Core.Helpers.FFmpegHelper.GetMediaInfoAsync(file.Path);
                }
                catch(Exception ex)
                {
                    Helpers.InfoHelper.ShowError(ex.Message);
                    MediaInfo = null;
                }
                NewSubtitleInfos.Clear();
            }
        });
        public ICommand AddSubCommand => new RelayCommand(async () =>
        {
            var file = await Helpers.PickHelper.PickFileAsync(string.Empty, ToolPageBase.Window);
            if (file != null)
            {
                var info = await Core.Helpers.FFmpegHelper.GetMediaInfoAsync(file.Path);
                var sub = info.SubtitleInfos?.FirstOrDefault();
                if(sub != null)
                {
                    NewSubtitleInfos.Add(new Models.NewSubtitle()
                    {
                        Path = file.Path,
                        Title = sub.Title,
                        Language = sub.Language,
                        Codec = sub.Codec,
                        Langs = Langs,
                        Codecs = SubCodecs
                    });
                }
            }
        });
        public ICommand RemoveSubCommand => new RelayCommand<IList<object>>((items) =>
        {
            if(items != null)
            {
                foreach (var item in items.ToList())
                {
                    NewSubtitleInfos.Remove(item as Models.NewSubtitle);
                }
            }
        });

        public ICommand StartAddSubComman => new RelayCommand(async() =>
        {
            if(NewSubtitleInfos.Count > 0)
            {
                string outFileName = System.IO.Path.GetFileNameWithoutExtension(MediaInfo.Path) + "_AddSub" + System.IO.Path.GetExtension(MediaInfo.Path);
                string outFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(MediaInfo.Path), outFileName);
                Helpers.InfoHelper.ShowMsg("写入中，请稍等");
                Helpers.InfoHelper.ShowWaiting();
                try
                {
                    await Core.Helpers.FFmpegHelper.AddSubtitleAsync(MediaInfo.Path, outFile, NewSubtitleInfos.Select(p => p.ToSubtitleInfo()).ToList());
                    Helpers.InfoHelper.ShowSuccess("已完成");
                }
                catch(Exception ex)
                {
                    Helpers.InfoHelper.ShowError(ex.Message);
                }
                Helpers.InfoHelper.HideWaiting();
            }
        });
    }
}
