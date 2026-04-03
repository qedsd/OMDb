using Microsoft.UI.Xaml;
using Microsoft.UI;
using OMDb.WinUI3.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Services
{
    internal static class LabelCollectionStyleSelectorService
    {
        private const string Key = "LabelCollectionStyle";

        /// <summary>
        /// 0 List
        /// 1 GridView
        /// </summary>
        public static int Style { get; set; }

        public static bool IsList
        {
            get => Style == 0;
        }

        public static void Initialize()
        {
            Style = LoadFromSettings();
        }

        public static async Task SetAsync(int style)
        {
            Style = style;
            await SaveInSettingsAsync(Style);
        }

        private static int LoadFromSettings()
        {
            string style = SettingService.GetValue(Key);

            if (!string.IsNullOrEmpty(style))
            {
                return int.Parse(style);
            }
            else
            {
                return default;
            }
        }

        private static async Task SaveInSettingsAsync(int style)
        {
            await SettingService.SetValueAsync(Key, style.ToString());
        }
    }
}
