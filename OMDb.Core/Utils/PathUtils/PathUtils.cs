using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Utils.PathUtils
{
    public static class PathUtils
    {
        private static readonly string[] IllegalPathChar = new string[10] { "\\", "/", ":", "*", "?", "\"", "<", ">", "|", ";" };
        private static readonly string IllegalPathCharStr = "\\ / : * ? \" < > | ;";
    }
}
