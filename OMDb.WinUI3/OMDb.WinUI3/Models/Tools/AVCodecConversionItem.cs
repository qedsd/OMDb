using CommunityToolkit.Mvvm.ComponentModel;
using OMDb.Core.Models;
using OMDb.Core.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models.Tools
{
    internal class AVCodecConversionItem : ObservableObject
    {
        internal AVCodecConversionItem(MediaInfo info, List<string> aCodecs, List<string> vCodecs, List<string> filters)
        {
            MediaInfo = info;
            if(MediaInfo.VideoInfos.NotNullAndEmpty())
            {
                VCodecs = new List<VCodecConversionItem>();
                for(int i = 0; i < MediaInfo.VideoInfos.Count; i++)
                {
                    VCodecs.Add(new VCodecConversionItem(MediaInfo.VideoInfos[i], vCodecs, filters, i));
                }
            }
            if(MediaInfo.AudioInfos.NotNullAndEmpty())
            {
                ACodecs = new List<ACodecConversionItem>();
                for (int i = 0; i < MediaInfo.AudioInfos.Count; i++)
                {
                    ACodecs.Add(new ACodecConversionItem(MediaInfo.AudioInfos[i], aCodecs, filters,i));
                }
            }
            string outFileName = System.IO.Path.GetFileNameWithoutExtension(MediaInfo.Path) + "_avcodec" + System.IO.Path.GetExtension(MediaInfo.Path); ;
            string outFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(MediaInfo.Path), outFileName);
            OutputPath = outFile;
        }

        public MediaInfo MediaInfo { get; set; }

        private string outputPath;
        public string OutputPath
        {
            get => outputPath;
            set => SetProperty(ref outputPath, value);
        }

        public List<VCodecConversionItem> VCodecs { get; set; }
        public List<ACodecConversionItem> ACodecs { get; set; }
    }
}
