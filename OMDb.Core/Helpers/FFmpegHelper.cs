using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace OMDb.Core.Helpers
{
    public static class FFmpegHelper
    {
        //TODO:目前的音频、视频流都假设为只有一个

        internal static void SetExecutablesPath(string path)
        {
            FFmpeg.SetExecutablesPath(path);
        }
        public static async Task<Models.MediaInfo> GetMediaInfoAsync(string path)
        {
            var info = await FFmpeg.GetMediaInfo(path);
            return new Models.MediaInfo(info);
        }

        public static async Task AddSubtitleAsync(string inputVideo, string outputVideo, SubtitleInfo newsubtitle)
        {
            await AddSubtitleAsync(inputVideo,outputVideo, new List<SubtitleInfo>() { newsubtitle });
        }

        public static async Task AddSubtitleAsync(string inputVideo, string outputVideo, List<SubtitleInfo> newsubtitles)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputVideo);
            IVideoStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                ?.SetCodec(VideoCodec.copy);

            IAudioStream audioStream = mediaInfo.AudioStreams.FirstOrDefault()
                ?.SetCodec(AudioCodec.copy);

            var conversion = FFmpeg.Conversions.New()
                .AddStream(audioStream)
                .AddStream(videoStream);

            if (mediaInfo.SubtitleStreams.Count() != 0)
            {
                foreach (var subStream in mediaInfo.SubtitleStreams)
                {
                    conversion.AddStream(subStream);
                }
            }

            foreach (var newSubtitle in newsubtitles)
            {
                IMediaInfo subtitleInfo = await FFmpeg.GetMediaInfo(newSubtitle.Path);
                var newSubStream = subtitleInfo.SubtitleStreams.First();
                newSubStream.SetCodec(newSubtitle.Codec);
                newSubStream.SetLanguage(newSubtitle.Language);
                //TODO:title?
                conversion.AddStream(newSubStream);
            }

            conversion.SetOutput(outputVideo);
            conversion.OnProgress += Conversion_OnProgress;
            await conversion.Start();
        }

        private static void Conversion_OnProgress(object sender, Xabe.FFmpeg.Events.ConversionProgressEventArgs args)
        {
            var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
            Console.WriteLine($"[{args.Duration} / {args.TotalLength}] {percent}%");
        }


        /// <summary>
        /// 视频、音频编码转换
        /// </summary>
        /// <param name="inputVideo"></param>
        /// <param name="outputVideo"></param>
        /// <param name="vcodec"></param>
        /// <param name="acodec"></param>
        /// <param name="progressCallback"></param>
        /// <returns></returns>
        public static async Task ConverAVCodecAsync(string inputVideo, string outputVideo, string vcodec, string acodec, Xabe.FFmpeg.Events.ConversionProgressEventHandler progressCallback = null)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputVideo);
            IVideoStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                ?.SetCodec(vcodec);

            IAudioStream audioStream = mediaInfo.AudioStreams.FirstOrDefault()
                ?.SetCodec(acodec);

            var conversion = FFmpeg.Conversions.New()
                .AddStream(audioStream)
                .AddStream(videoStream);

            if (mediaInfo.SubtitleStreams.Count() != 0)
            {
                foreach (var subStream in mediaInfo.SubtitleStreams)
                {
                    conversion.AddStream(subStream);
                }
            }

            conversion.SetOutput(outputVideo);
            if(progressCallback != null)
            {
                conversion.OnProgress += progressCallback;
            }
            await conversion.Start();
        }

        /// <summary>
        /// 视频、音频转码
        /// </summary>
        /// <param name="inputVideo"></param>
        /// <param name="outputVideo"></param>
        /// <param name="aConversionValues"></param>
        /// <param name="vConversionValues"></param>
        /// <param name="progressCallback"></param>
        /// <returns></returns>
        public static async Task ConverAVCodecAsync(string inputVideo, string outputVideo, List<ACodecConversionValue> aConversionValues, List<VCodecConversionValue> vConversionValues, Xabe.FFmpeg.Events.ConversionProgressEventHandler progressCallback = null)
        {
            var conversion = FFmpeg.Conversions.New();
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputVideo);

            for(int i = 0; i < mediaInfo.VideoStreams.Count(); i++)
            {
                IVideoStream videoStream = mediaInfo.VideoStreams.ElementAt(i); 
                var values = vConversionValues.FirstOrDefault(p => p.StreamIndex == i);
                if(values != null)
                {
                    foreach(var value in values.Values)
                    {
                        switch (value.Key)
                        {
                            case Enums.VCodecConversion.Bitrate: videoStream.SetBitrate(value.Value); break;
                            case Enums.VCodecConversion.BitstreamFilter: videoStream.SetBitstreamFilter(Enum.Parse(typeof(BitstreamFilter), value.Value)); break;
                            case Enums.VCodecConversion.Codec: videoStream.SetCodec(value.Value); break;
                            case Enums.VCodecConversion.Flags: videoStream.SetFlags(value.Value); break;
                            case Enums.VCodecConversion.Framerate: videoStream.SetFramerate(value.Value); break;
                            case Enums.VCodecConversion.InputFormat: videoStream.SetInputFormat(value.Value); break;
                            case Enums.VCodecConversion.LoopCount:
                                {
                                    if(values.Values.TryGetValue(Enums.VCodecConversion.LoopDelay,out var loopDelay))
                                    {
                                        videoStream.SetLoop(value.Value, loopDelay);
                                    }
                                    else
                                    {
                                        videoStream.SetLoop(value.Value);
                                    }
                                }; break;
                            case Enums.VCodecConversion.LoopDelay:
                                {
                                    if (values.Values.TryGetValue(Enums.VCodecConversion.LoopCount, out var loopCount))
                                    {
                                        videoStream.SetLoop(loopCount, value.Value);
                                    }
                                    else
                                    {
                                        videoStream.SetLoop(1,value.Value);
                                    }
                                }; break;
                            case Enums.VCodecConversion.OutputFramesCount: videoStream.SetOutputFramesCount(value.Value); break;
                            case Enums.VCodecConversion.Seek:
                                {
                                    var timeSpan = TimeSpan.FromMilliseconds((double)value.Value);
                                    videoStream.SetSeek(timeSpan);
                                }; break;
                            case Enums.VCodecConversion.Size:
                                {
                                    var array = ((string)value.Value).Split(',');//规定用w,h形式传入
                                    videoStream.SetSize(int.Parse(array[0]), int.Parse(array[1]));
                                }; break;
                            case Enums.VCodecConversion.StreamLoop:
                                {
                                    videoStream.SetStreamLoop(value.Value);
                                }; break;
                        }
                    }
                }
                conversion.AddStream(videoStream);
            }

            for (int i = 0; i < mediaInfo.AudioStreams.Count(); i++)
            {
                IAudioStream audioStream = mediaInfo.AudioStreams.ElementAt(i);
                var values = aConversionValues.FirstOrDefault(p => p.StreamIndex == i);
                if (values != null)
                {
                    foreach (var value in values.Values)
                    {
                        switch (value.Key)
                        {
                            case Enums.ACodecConversion.Bitrate: audioStream.SetBitrate(value.Value); break;
                            case Enums.ACodecConversion.BitstreamFilter: audioStream.SetBitstreamFilter(Enum.Parse(typeof(BitstreamFilter), value.Value)); break;
                            case Enums.ACodecConversion.Channels: audioStream.SetChannels(value.Value); break;
                            case Enums.ACodecConversion.Codec: audioStream.SetCodec(value.Value); break;
                            case Enums.ACodecConversion.InputFormat: audioStream.SetInputFormat(value.Value); break;
                            case Enums.ACodecConversion.SampleRate: audioStream.SetSampleRate(value.Value); break;
                            case Enums.ACodecConversion.Seek: audioStream.SetSeek(TimeSpan.FromMilliseconds(value.Value)); break;
                            case Enums.ACodecConversion.StreamLoop: audioStream.SetStreamLoop(value.Value); break;
                        }
                    }
                }
                conversion.AddStream(audioStream);
            }

            foreach (var subStream in mediaInfo.SubtitleStreams)
            {
                conversion.AddStream(subStream);
            }

            conversion.SetOutput(outputVideo);
            if (progressCallback != null)
            {
                conversion.OnProgress += progressCallback;
            }
            await conversion.Start();
        }
    }
}
