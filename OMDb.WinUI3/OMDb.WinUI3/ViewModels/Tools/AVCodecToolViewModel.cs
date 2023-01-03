using CommunityToolkit.Mvvm.Input;
using OMDb.Core.Extensions;
using OMDb.WinUI3.Models.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels.Tools
{
    internal class AVCodecToolViewModel : ToolBaseViewModel
    {
        private ObservableCollection<AVCodecConversionItem> conversionItems;
        public ObservableCollection<AVCodecConversionItem> ConversionItems
        {
            get => conversionItems;
            set
            {
                SetProperty(ref conversionItems, value);
            }
        }
        private List<string> aCodecs;
        /// <summary>
        /// 音频编码
        /// </summary>
        public List<string> ACodecs
        {
            get => aCodecs;
            set => SetProperty(ref aCodecs, value);
        }
        private List<string> vCodecs;
        /// <summary>
        /// 视频编码
        /// </summary>
        public List<string> VCodecs
        {
            get => vCodecs;
            set => SetProperty(ref vCodecs, value);
        }

        public List<string> BitstreamFilteres { get; set; }

        public AVCodecToolViewModel()
        {
            ConversionItems = new ObservableCollection<AVCodecConversionItem>();
            var items1 = new List<string>();
            foreach (var p in Enum.GetValues(typeof(Xabe.FFmpeg.VideoCodec)))
            {
                items1.Add(p.ToString());
            }
            VCodecs = items1;

            var items2 = new List<string>();
            foreach (var p in Enum.GetValues(typeof(Xabe.FFmpeg.AudioCodec)))
            {
                items2.Add(p.ToString());
            }
            ACodecs = items2;

            var items3 = new List<string>();
            foreach (var p in Enum.GetValues(typeof(Xabe.FFmpeg.BitstreamFilter)))
            {
                items3.Add(p.ToString());
            }
            BitstreamFilteres = items3;
        }

        public ICommand AddCommand => new RelayCommand(async () =>
        {
            var file = await Helpers.PickHelper.PickFileAsync(".mkv", ToolPageBase.Window);
            if (file != null)
            {
                try
                {
                    var mediaInfo = await Core.Helpers.FFmpegHelper.GetMediaInfoAsync(file.Path);
                    if(mediaInfo != null)
                    {
                        ConversionItems.Add(new AVCodecConversionItem(mediaInfo,aCodecs, vCodecs, BitstreamFilteres));
                    }
                }
                catch (Exception ex)
                {
                    Helpers.InfoHelper.ShowError(ex.Message);
                }
            }
        });
        public ICommand PickSaveFileCommand => new RelayCommand<AVCodecConversionItem>(async (item) =>
        {
            var file = await Helpers.PickHelper.PickSaveFileAsync(System.IO.Path.GetFileName(item.OutputPath), ToolPageBase.Window);
            if (file != null)
            {
                item.OutputPath = file.Path;
            }
        });

        public ICommand StartCommand => new RelayCommand(async () =>
        {
            if(ConversionItems.NotNullAndEmpty())
            {
                ShowWaiting();
                foreach(var item in ConversionItems)
                {
                    var vcodecs = item.VCodecs.Select(p => p.ToVCodecConversionValue()).ToList();
                    var acodecs = item.ACodecs.Select(p => p.ToACodecConversionValue()).ToList();
                    await Core.Helpers.FFmpegHelper.ConverAVCodecAsync(item.MediaInfo.Path, item.OutputPath, acodecs, vcodecs, Conversion_OnProgress);
                }
                HideWaiting();
            }
        });
        private void Conversion_OnProgress(object sender, Xabe.FFmpeg.Events.ConversionProgressEventArgs args)
        {
            var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
            string msg;
            if(percent == 100)
            {
                msg = $"{percent}% {DateTime.Now}";
            }
            else
            {
                msg = $"{percent}% ({args.Duration} / {args.TotalLength})";
            }
            ShowMsg(msg, false);
        }
    }
}
