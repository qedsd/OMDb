using CommunityToolkit.Mvvm.ComponentModel;
using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models.Tools
{
    internal class AVCodecConversionItem: ObservableObject
    {
        internal AVCodecConversionItem(MediaInfo info, List<string> aCodecs, List<string> vCodecs)
        {
            mediaInfo = info;
            ACodecs = aCodecs;
            VCodecs = vCodecs;
            videoCodec = mediaInfo.VideoInfo.Codec;
            audioCodec = mediaInfo.AudioInfo.Codec;
            string outFileName = System.IO.Path.GetFileNameWithoutExtension(MediaInfo.Path) + "_avcodec" + System.IO.Path.GetExtension(MediaInfo.Path); ;
            string outFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(MediaInfo.Path), outFileName);
            OutputPath = outFile;
        }

        private MediaInfo mediaInfo;
        public MediaInfo MediaInfo
        {
            get => mediaInfo;
            set => SetProperty(ref mediaInfo, value);
        }

        private string videoCodec;
        public string VideoCodec
        {
            get => videoCodec;
            set => SetProperty(ref videoCodec, value);
        }

        private string audioCodec;
        public string AudioCodec
        {
            get => audioCodec;
            set => SetProperty(ref audioCodec, value);
        }

        public List<string> ACodecs { get; set; }

        public List<string> VCodecs { get; set; }

        public string OutputPath { get; set; }
    }
}
