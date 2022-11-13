using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace OMDb.Core.Models
{
    public class MediaInfo
    {
        public string Path { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime? CreationTime { get; set; }

        public long Size { get; set; }

        public List<AudioInfo> AudioInfos { get; set; }
        public List<VideoInfo> VideoInfos { get; set; }
        public List<SubtitleInfo> SubtitleInfos { get; set; }

        public VideoInfo VideoInfo
        {
            get => VideoInfos?.FirstOrDefault();
        }

        public MediaInfo() { }

        public MediaInfo(IMediaInfo mediaInfo)
        {
            Path = mediaInfo.Path;
            Duration = mediaInfo.Duration;
            CreationTime = mediaInfo.CreationTime;
            Size = mediaInfo.Size;
            InitAudioInfos(mediaInfo);
            InitVideoInfos(mediaInfo);
            InitSubtitleInfos(mediaInfo);
        }
        private void InitAudioInfos(IMediaInfo mediaInfo)
        {
            AudioInfos = new List<AudioInfo>();
            foreach(var a in mediaInfo.AudioStreams)
            {
                AudioInfos.Add(new AudioInfo(a));
            }
        }
        private void InitVideoInfos(IMediaInfo mediaInfo)
        {
            VideoInfos = new List<VideoInfo>();
            foreach (var a in mediaInfo.VideoStreams)
            {
                VideoInfos.Add(new VideoInfo(a));
            }
        }
        private void InitSubtitleInfos(IMediaInfo mediaInfo)
        {
            SubtitleInfos = new List<SubtitleInfo>();
            foreach (var a in mediaInfo.SubtitleStreams)
            {
                SubtitleInfos.Add(new SubtitleInfo(a));
            }
        }
    }
}
