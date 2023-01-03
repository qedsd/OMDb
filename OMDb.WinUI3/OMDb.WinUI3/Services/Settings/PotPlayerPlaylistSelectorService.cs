using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Services.Settings
{
    internal static class PotPlayerPlaylistSelectorService
    {
        private const string Key = "PotPlayerPlaylistPath";

        public static string PlaylistPath { get; set; }

        public static void Initialize()
        {
            PlaylistPath = LoadFromSettings();
        }

        public static async Task SetAsync(string value)
        {
            PlaylistPath = value;
            await SaveInSettingsAsync(value);
        }

        private static string LoadFromSettings()
        {
            return SettingService.GetValue(Key);
        }

        private static async Task SaveInSettingsAsync(string value)
        {
            await SettingService.SetValueAsync(Key, value);
        }
    }
}
