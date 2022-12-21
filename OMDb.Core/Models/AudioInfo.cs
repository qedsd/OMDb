using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace OMDb.Core.Models
{
    public class AudioInfo
    {
        public TimeSpan Duration { get; set; }

        public long Bitrate { get; set; }

        public int SampleRate { get; set; }

        public int Channels { get; set; }

        public string Language { get; set; }

        public string Title { get; set; }

        public int? Default { get; set; }

        public int? Forced { get; set; }

        public string Codec { get; set; }

        public AudioInfo(IAudioStream audioStream)
        {
            Duration = audioStream.Duration;
            Bitrate = audioStream.Bitrate;
            SampleRate = audioStream.SampleRate;
            Channels = audioStream.Channels;
            Language = audioStream.Language;
            Title = audioStream.Title;
            Default = audioStream.Default;
            Forced = audioStream.Forced;
            Codec = audioStream.Codec;
        }
    }
}
