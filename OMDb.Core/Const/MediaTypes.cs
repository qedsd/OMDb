using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Const
{
    public class MediaTypes
    {
        public static readonly ImmutableList<string> Image = ImmutableList.Create("BMP", "JPG", "PNG", "TIF", "GIF", "PCX", "TGA", "EXIF", "FPX", "SVG", "PSD", "CDR", "PCD", "DXF", "UFO", "EPS", "AI", "RAW", "WMF", "WEBP", "AVIF", "APNG");

        public static readonly ImmutableList<string> Video = ImmutableList.Create("AVI","WMV","MPEG","MP4","M4V","MOV","ASF","FLV","F4V","RMVB","RM","3GP","VOB");

        public static readonly ImmutableList<string> VideoSub = ImmutableList.Create("SRT","WEBVTT","STL","SBV","ASS","DFXP","TTML");

        public static readonly ImmutableList<string> Audio = ImmutableList.Create("MP3","WAV","WMA","MP2","Flac","MIDI","RA","APE","AAC","CDA","MOV");

    }
}
