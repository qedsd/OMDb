using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace OMDb.Core.Helpers
{
    public sealed class LogHelper
    {
        public Logger _logger = LogManager.GetLogger("OMDB");

        private static readonly LogHelper instance = new LogHelper();

        static LogHelper() { }
        private LogHelper() { }
        public static LogHelper Instance
        {
            get
            {
                return instance;
            }
        }
    }
}

