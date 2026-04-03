using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    internal class NewSubtitle
    {
        public string Path { get; set; }
        public string Language { get; set; }
        public string Title { get; set; }

        public string Codec { get; set; }

        public Core.Models.SubtitleInfo ToSubtitleInfo()
        {
            return new Core.Models.SubtitleInfo()
            {
                Codec = Codec,
                Title = Title,
                Language = Language,
                Path = Path
            };
        }

        public List<string> Codecs { get; set; }

        public List<string> Langs { get; set; }
    }
}
