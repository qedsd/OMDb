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
    internal class ACodecConversionItem : ObservableObject
    {
        public AudioInfo AudioInfo { get; set; }

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

        private int channels;
        public int Channels
        {
            get => channels;
            set => SetProperty(ref channels, value);
        }

        private string codec;
        public string Codec
        {
            get => codec;
            set => SetProperty(ref codec, value);
        }

        private string inputFormat;
        public string InputFormat
        {
            get => inputFormat;
            set => SetProperty(ref inputFormat, value);
        }

        private int sampleRate;
        public int SampleRate
        {
            get => sampleRate;
            set => SetProperty(ref sampleRate, value);
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

        private int streamLoop;
        public int StreamLoop
        {
            get => streamLoop;
            set => SetProperty(ref streamLoop, value);
        }

        public List<string> Codecs { get; set; }
        public List<string> BitstreamFilters { get; set; }
        public int StreamIndex { get; set; }
        public ACodecConversionItem()
        {

        }

        public ACodecConversionItem(AudioInfo audioInfo, List<string> codecs, List<string> filters,int index)
        {
            AudioInfo = audioInfo;
            Codec = audioInfo.Codec;
            Bitrate = audioInfo.Bitrate;
            Channels = audioInfo.Channels;
            SampleRate = audioInfo.SampleRate;
            Codecs = codecs;
            BitstreamFilters = filters;
        }

        public ACodecConversionValue ToACodecConversionValue()
        {
            ACodecConversionValue conversionValue = new ACodecConversionValue();
            conversionValue.Values = new Dictionary<Core.Enums.ACodecConversion, dynamic>();
            conversionValue.StreamIndex = StreamIndex;
            conversionValue.Values.Add(Core.Enums.ACodecConversion.Bitrate, Bitrate);
            if (!string.IsNullOrEmpty(BitstreamFilter))
            {
                conversionValue.Values.Add(Core.Enums.ACodecConversion.BitstreamFilter, BitstreamFilter);
            }
            if(Channels != AudioInfo.Channels)
            {
                conversionValue.Values.Add(Core.Enums.ACodecConversion.Channels, Channels);
            }
            if(Codec != AudioInfo.Codec)
            {
                conversionValue.Values.Add(Core.Enums.ACodecConversion.Codec, Codec);
            }
            if (!string.IsNullOrEmpty(InputFormat))
            {
                conversionValue.Values.Add(Core.Enums.ACodecConversion.InputFormat, InputFormat);
            }
            if (SampleRate != AudioInfo.SampleRate)
            {
                conversionValue.Values.Add(Core.Enums.ACodecConversion.SampleRate, SampleRate);
            }
            if (Seek != 0)
            {
                conversionValue.Values.Add(Core.Enums.ACodecConversion.Seek, Seek);
            }
            if (StreamLoop != 0)
            {
                conversionValue.Values.Add(Core.Enums.ACodecConversion.StreamLoop, StreamLoop);
            }
            return conversionValue;
        }
    }
}
