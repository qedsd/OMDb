using CommunityToolkit.Mvvm.ComponentModel;
using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace OMDb.WinUI3.Models.Tools
{
    internal class VCodecConversionItem : ObservableObject
    {
        public VideoInfo VideoInfo { get; set; }

        private long bitrate;
        public long Bitrate
        {
            get => bitrate;
            set => SetProperty(ref bitrate, value);
        }

        private string bitstreamFilter;
        public string BitstreamFilter
        {
            get => bitstreamFilter;
            set => SetProperty(ref bitstreamFilter, value);
        }

        private string codec;
        public string Codec
        {
            get => codec;
            set => SetProperty(ref codec, value);
        }

        private string flags;
        public string Flags
        {
            get => flags;
            set => SetProperty(ref flags, value);
        }

        private double framerate;
        public double Framerate
        {
            get => framerate;
            set => SetProperty(ref framerate, value);
        }

        private string inputFormat;
        public string InputFormat
        {
            get => inputFormat;
            set => SetProperty(ref inputFormat, value);
        }

        private int loopCount;
        public int LoopCount
        {
            get => loopCount;
            set => SetProperty(ref loopCount, value);
        }

        private int loopDelay;
        /// <summary>
        /// 单位：s
        /// </summary>
        public int LoopDelay
        {
            get => loopDelay;
            set => SetProperty(ref loopDelay, value);
        }

        private int outputFramesCount;
        public int OutputFramesCount
        {
            get => outputFramesCount;
            set => SetProperty(ref outputFramesCount, value);
        }

        private long seek;
        /// <summary>
        /// ms
        /// </summary>
        public long Seek
        {
            get => seek;
            set => SetProperty(ref seek, value);
        }

        private int sizeW;
        public int SizeW
        {
            get => sizeW;
            set => SetProperty(ref sizeW, value);
        }

        private int sizeH;
        public int SizeH
        {
            get => sizeH;
            set => SetProperty(ref sizeH, value);
        }

        private int streamLoop;
        public int StreamLoop
        {
            get => streamLoop;
            set => SetProperty(ref streamLoop, value);
        }

        public List<string> Codecs { get; set; }
        public List<string> BitstreamFilters { get; set; }
        public int StreamIndex { get; set; }

        public VCodecConversionItem() { }
        public VCodecConversionItem(VideoInfo videoInfo, List<string> codecs, List<string> filters,int index)
        {
            StreamIndex = index;
            VideoInfo = videoInfo;
            Bitrate = videoInfo.Bitrate;
            Codec = videoInfo.Codec;
            Framerate = videoInfo.Framerate;
            SizeW = videoInfo.Width;
            SizeH = videoInfo.Height;
            Codecs = codecs;
            BitstreamFilters = filters;
        }

        public VCodecConversionValue ToVCodecConversionValue()
        {
            VCodecConversionValue conversionValue = new VCodecConversionValue();
            conversionValue.StreamIndex = StreamIndex;
            conversionValue.Values = new Dictionary<Core.Enums.VCodecConversion, dynamic>();
            conversionValue.Values.Add(Core.Enums.VCodecConversion.Bitrate, Bitrate);
            if (!string.IsNullOrEmpty(BitstreamFilter))
            {
                conversionValue.Values.Add(Core.Enums.VCodecConversion.BitstreamFilter, BitstreamFilter);
            }
            if (Codec != VideoInfo.Codec)
            {
                conversionValue.Values.Add(Core.Enums.VCodecConversion.Codec, Codec);
            }
            if (!string.IsNullOrEmpty(Flags))
            {
                conversionValue.Values.Add(Core.Enums.VCodecConversion.Flags, Flags);
            }
            if (Framerate != VideoInfo.Framerate)
            {
                conversionValue.Values.Add(Core.Enums.VCodecConversion.Framerate, Framerate);
            }
            if (!string.IsNullOrEmpty(InputFormat))
            {
                conversionValue.Values.Add(Core.Enums.VCodecConversion.InputFormat, InputFormat);
            }
            if(LoopCount != 0)
            {
                conversionValue.Values.Add(Core.Enums.VCodecConversion.LoopCount, LoopCount);
            }
            if (LoopDelay != 0)
            {
                conversionValue.Values.Add(Core.Enums.VCodecConversion.LoopDelay, LoopDelay);
            }
            if (OutputFramesCount != 0)
            {
                conversionValue.Values.Add(Core.Enums.VCodecConversion.OutputFramesCount, OutputFramesCount);
            }
            if (Seek != 0)
            {
                conversionValue.Values.Add(Core.Enums.VCodecConversion.Seek, Seek);
            }
            if(SizeW != VideoInfo.Width || SizeH != VideoInfo.Height)
            {
                conversionValue.Values.Add(Core.Enums.VCodecConversion.Size, $"{SizeW},{SizeH}");
            }
            if (StreamLoop != 0)
            {
                conversionValue.Values.Add(Core.Enums.VCodecConversion.StreamLoop, StreamLoop);
            }
            return conversionValue;
        }
    }
}
