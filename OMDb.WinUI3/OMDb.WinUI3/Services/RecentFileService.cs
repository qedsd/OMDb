using OMDb.Core.Extensions;
using OMDb.WinUI3.Services.Settings;
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
            List<Core.Models.RecentFile> recentFiles = await Task.Run(()=>ReadPotPlayer());
            //TODO:本地记录、监控文件修改
            return recentFiles;
        }
        private static List<Core.Models.RecentFile> ReadPotPlayer()
        {
            List<Core.Models.RecentFile> recentFiles = new List<Core.Models.RecentFile>();
            string path = PotPlayerPlaylistSelectorService.PlaylistPath;
            if(!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
            {
                string[] lines = System.IO.File.ReadAllLines(path);
                Core.Models.RecentFile file0 = new Core.Models.RecentFile();//最后一次播放的文件
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if(int.TryParse(line[0].ToString(),out var num))
                    {
                        //开始列表项
                        var files = ReadPotPlayerAllFiles(lines.Skip(i).ToArray());
                        if(files.NotNullAndEmpty())
                        {
                            recentFiles.AddRange(files);
                        }
                        else if(!string.IsNullOrEmpty(file0.Path))
                        {
                            recentFiles.Add(file0);
                        }
                        break;
                    }
                    else
                    {
                        var array = line.Split('=');
                        if(array.Length == 2)
                        {
                            switch(array[0])
                            {
                                case "playname":file0.Path = array[1];break;
                                case "playtime": file0.MarkTime = long.Parse(array[1]); break;
                                //TODO:其他topindex、saveplaypos信息是否需要
                            }
                        }
                    }
                }
            }
            return recentFiles;
        }
        private static List<Core.Models.RecentFile> ReadPotPlayerAllFiles(string[] lines)
        {
            Dictionary<int, Core.Models.RecentFile> filesDic = new Dictionary<int, Core.Models.RecentFile>();
            for(int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if(int.TryParse(line[0].ToString(),out var num))
                {
                    var array = line.Split('*');
                    if (array.Length == 3)
                    {
                        Core.Models.RecentFile file;
                        if (filesDic.TryGetValue(num, out var value))
                        {
                            file = value;
                        }
                        else
                        {
                            file = new Core.Models.RecentFile();
                            filesDic.Add(num, file);
                        }
                        switch(array[1])
                        {
                            case "file":file.Path = array[2];break;
                            case "duration2": file.Duration = long.Parse(array[2]); break;
                            case "start": file.MarkTime = long.Parse(array[2]); break;
                        }
                    }
                }
            }
            var files = filesDic.Where(p => p.Value.MarkTime != 0).Select(p => p.Value).ToList();
            if(files.Count > 0)
            {
                List<Core.Models.RecentFile> recentFiles = new List<Core.Models.RecentFile>();
                //从文件路径匹配词条
                foreach (var file in files)
                {
                    var storage = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => file.Path.StartsWith(p.StorageDirectory));
                    if(storage != null)
                    {
                        string relativePath = System.IO.Path.GetRelativePath(storage.StorageDirectory, file.Path);
                        string commonPart = System.IO.Path.Combine(storage.StorageDirectory, "Entries");
                        string remainPart = file.Path.Substring(commonPart.Length, file.Path.Length - commonPart.Length);
                        string entryName = remainPart.Split('\\')[1];
                        string entryPath = System.IO.Path.Combine(commonPart, entryName);
                        //从视频文件反找词条路径，从词条路径读取元文件，从元文件读取id
                        var meta = Core.Models.EntryMetadata.Read(System.IO.Path.Combine(entryPath, Services.ConfigService.MetadataFileNmae));
                        if(meta != null)
                        {
                            file.EntryId = meta.Id;
                            file.DbId = storage.StorageName;
                            recentFiles.Add(file);
                        }
                    }
                }
                return recentFiles;
            }
            return null;
        }
    }
}
