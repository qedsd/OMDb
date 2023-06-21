using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Services
{
    public static class ActivationService
    {
        public static void Init()
        {
            Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "zh-CN";
            ConfigService.Load();
            //SettingService.Load();
            RecentFileService.Init();
        }
    }
}
