using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace OMDb.Core.Utils
{
    public class Logger
    {
        public NLog.Logger _logger = LogManager.GetLogger("OMDB");

        private static readonly Logger instance = new Logger();

        static Logger() { }
        private Logger() { }
        private static Logger Instance
        {
            get
            {
                return instance;
            }
        }


        public static void Info(string msg)
        {
            Instance._logger.Info(msg);
        }

        public static void Error(Exception ex)
        {
            Instance._logger.Error(ex);
        }

        public static void Error(string msg)
        {
            Instance._logger.Error(msg);
        }
    }
}

