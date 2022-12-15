using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Services
{
    public static class RecentFileService
    {
        public static async Task<List<Core.Models.RecentFile>> GetRecentFilesAsync()
        {
            //TODO:接入potplayer观看历史
            List<Core.Models.RecentFile> recentFiles = new List<Core.Models.RecentFile>();
            recentFiles.Add(new Core.Models.RecentFile()
            {
                Path = "C:\\Users\\xxx\\Videos1.mp4",
                DbId = "仓库1",
                EntryId = "ee4cee31-56e9-45c0-9179-861a082c25bd",
                AccessTime = DateTime.Now,
                Duration = 1000000,
                MarkTime = 100000
            });
            recentFiles.Add(new Core.Models.RecentFile()
            {
                Path = "C:\\Users\\xxx\\Videos2.mp4",
                DbId = "仓库1",
                EntryId = "df41f3af-d920-4a83-a0b5-bea4bc835fd3",
                AccessTime = DateTime.Now,
                Duration = 1005330,
                MarkTime = 103626
            });
            recentFiles.Add(new Core.Models.RecentFile()
            {
                Path = "C:\\Users\\xxx\\Videos3.mp4",
                DbId = "仓库1",
                EntryId = "df41f3af-d920-4a83-a0b5-bea4bc835fd3",
                AccessTime = DateTime.Now,
                Duration = 16543430,
                MarkTime = 16000000
            });
            recentFiles.Add(new Core.Models.RecentFile()
            {
                Path = "C:\\Users\\xxx\\Videos4.mp4",
                DbId = "仓库1",
                EntryId = "df41f3af-d920-4a83-a0b5-bea4bc835fd3",
                AccessTime = DateTime.Now,
                Duration = 1866765750,
                MarkTime = 1076876655
            });
            await Task.Delay(1000);
            return recentFiles;
        }
    }
}
