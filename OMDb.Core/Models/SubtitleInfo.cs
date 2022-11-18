using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace OMDb.Core.Models
{
    public class SubtitleInfo
    {
        public string Language { get; set; }

        public int? Default { get; set; }

        public int? Forced { get; set; }

        public string Title { get; set; }

        public string Codec { get; set; }

        public string Path { get; set; }

        public SubtitleInfo() { }

        public SubtitleInfo(ISubtitleStream subtitleStream)
        {
            Language = subtitleStream.Language;
            Default = subtitleStream.Default;
            Forced = subtitleStream.Forced;
            Title = subtitleStream.Title;
            Codec = subtitleStream.Codec;
        }
    }
}
