using Microsoft.UI.Xaml.CustomAttributes;
using Newtonsoft.Json;
using OMDb.Core.Extensions;
using OMDb.Core.Models;
using OMDb.WinUI3.Services.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static ObservableCollection<Core.Models.RecentFile> RecentFiles { get; set; }
        public static void Init()
        {
            if(RecentFiles == null)
            {
                RecentFiles = new ObservableCollection<RecentFile>();
            }
            else
            {
                RecentFiles.Clear();
            }
            InitRecentFiles();//读取本地已记录的文件
            ResetRecentFiles();
            UpdateRecentFiles();

            MonitorPotPlayer();
        }
        private static void InitRecentFiles()
        {
            if (System.IO.File.Exists(GetConfigFilePath()))
            {
                string json = System.IO.File.ReadAllText(GetConfigFilePath());
                if (!string.IsNullOrEmpty(json))
                {
                    var files = JsonConvert.DeserializeObject<List<Core.Models.RecentFile>>(json);
                    if (files.NotNullAndEmpty())
                    {
                        foreach (var file in files.OrderByDescending(p => p.AccessTime))//最新的排前面
                        {
                            RecentFiles.Add(file);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 删除太久、过多文件记录
        /// </summary>
        private static void ResetRecentFiles()
        {
            if(RecentFiles.NotNullAndEmpty())
            {
                List<RecentFile> keepFiles = RecentFiles.Where(p => Math.Abs((DateTime.Now - p.AccessTime).TotalDays) < 10).ToList();
                keepFiles = keepFiles.Take(20).ToList();
                if(RecentFiles.Count != keepFiles.Count)
                {
                    RecentFiles.Clear();
                    keepFiles.ForEach(p => RecentFiles.Add(p));
                    SaveRecentFiles();
                }
            }
        }
        private static void UpdateRecentFiles()
        {
            var currentFiles = ReadPotPlayer();//最新的potplayer记录文件

            //合并potplayer最新到本地记录（如需要）
            if (currentFiles.NotNullAndEmpty())
            {
                bool needUpdateConfig = false;
                if (RecentFiles.Any())
                {
                    //本地记录不应该过多，所以无需复杂查找算法
                    foreach (var file in currentFiles)
                    {
                        var markedFile = RecentFiles.FirstOrDefault(p => p.Path == file.Path);
                        if (markedFile != null)
                        {
                            //本地有记录，判断是否要更新
                            if (markedFile.MarkTime != file.MarkTime)
                            {
                                markedFile.MarkTime = file.MarkTime;
                                markedFile.AccessTime = DateTime.Now;
                                RecentFiles.Remove(markedFile);
                                RecentFiles.Insert(0, markedFile);
                                needUpdateConfig = true;
                            }
                        }
                        else
                        {
                            //本地无记录，添加
                            file.AccessTime = DateTime.Now;
                            RecentFiles.Insert(0, file);
                            needUpdateConfig = true;
                        }
                    }
                }
                else
                {
                    foreach (var file in currentFiles)
                    {
                        file.AccessTime = DateTime.Now;
                        RecentFiles.Add(file);
                    }
                    needUpdateConfig = true;
                }
                if (needUpdateConfig)
                {
                    SaveRecentFiles();
                }
            }
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
                            //存在列表项，则一定包含了最后一次播放的文件
                            recentFiles.AddRange(files);
                        }
                        else if(!string.IsNullOrEmpty(file0.Path))
                        {
                            //可能不存在列表项，只有最后一次播放的文件的情况
                            if(FitRecentFile(file0))
                            {
                                recentFiles.Add(file0);
                            }
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
                    if(FitRecentFile(file))
                    {
                        recentFiles.Add(file);
                    }
                }
                return recentFiles;
            }
            return null;
        }
        private static bool FitRecentFile(Core.Models.RecentFile file)
        {
            var storage = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => file.Path.StartsWith(p.StorageDirectory));
            if (storage != null)
            {
                string relativePath = System.IO.Path.GetRelativePath(storage.StorageDirectory, file.Path);
                string commonPart = System.IO.Path.Combine(storage.StorageDirectory, "Entries");
                string remainPart = file.Path.Substring(commonPart.Length, file.Path.Length - commonPart.Length);
                string entryName = remainPart.Split('\\')[1];
                string entryPath = System.IO.Path.Combine(commonPart, entryName);
                //从视频文件反找词条路径，从词条路径读取元文件，从元文件读取id
                var meta = Core.Models.EntryMetadata.Read(System.IO.Path.Combine(entryPath, Services.ConfigService.MetadataFileNmae));
                if (meta != null)
                {
                    file.EntryId = meta.Id;
                    file.DbId = storage.StorageName;
                    return true;
                }
            }
            return false;
        }

        private static void SaveRecentFiles()
        {
            if (RecentFiles.NotNullAndEmpty())
            {
                string json = JsonConvert.SerializeObject(RecentFiles);
                System.IO.File.WriteAllText(GetConfigFilePath(), json);
            }
        }
        private static string GetConfigFilePath()
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "recentfiles.json");
        }


        private static System.IO.FileSystemWatcher FileSystemWatcher;
        /// <summary>
        /// 监控potplayer记录文件更新
        /// </summary>
        private static void MonitorPotPlayer()
        {
            string path = PotPlayerPlaylistSelectorService.PlaylistPath;
            if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
            {
                FileSystemWatcher = new System.IO.FileSystemWatcher(System.IO.Path.GetDirectoryName(path));
                FileSystemWatcher.EnableRaisingEvents = true;
                FileSystemWatcher.Changed += FileSystemWatcher_Changed;
            }
        }

        private static void FileSystemWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            if(e.FullPath == PotPlayerPlaylistSelectorService.PlaylistPath && e.ChangeType == System.IO.WatcherChangeTypes.Changed)
            {
                Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    UpdateRecentFiles();
                });
            }
        }
    }
}
