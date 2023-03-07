using Google.Protobuf.WellKnownTypes;
using ICSharpCode.SharpZipLib.Core;
using NPOI.OpenXmlFormats.Vml;
using NPOI.POIFS.FileSystem;
using NPOI.SS.Formula;
using NPOI.SS.Formula.Functions;
using OMDb.Core.DbModels;
using OMDb.Core.Enums;
using OMDb.Core.Models;
using OMDb.WinUI3.Events;
using OMDb.WinUI3.Helpers;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Services.Settings;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Services
{
    public static class ExcelService
    {

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="enrtyStorage"></param>
        public static void ExportExcel(string filePath, EnrtyStorage enrtyStorage)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            //用于支持gb2312         
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //創建數據表            
            DataTable dataTable = new DataTable();
            //基本信息列
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("ReleaseDate", typeof(string));
            dataTable.Columns.Add("MyRating", typeof(double));

            //数据库查询 属性标签&分类标签
            var label_lc = Core.Services.LabelService.GetAllLabel(DbSelectorService.dbCurrentId);
            var label_lp = Core.Services.LabelPropertyService.GetAllLabel(DbSelectorService.dbCurrentId);

            //标签&词条 关联关系
            var result_EntryLabel = Core.Services.EntryLabelService.SelectAllEntryLabel(enrtyStorage.StorageName);
            var result_EntryLabelProperty = Core.Services.EntryLabelPropertyService.SelectAllEntryLabel(enrtyStorage.StorageName);

            label_lp.Where(a=>a.Level==1).ToList().ForEach(s => dataTable.Columns.Add(s.Name, typeof(string)));
            //分類列
            dataTable.Columns.Add("Classification", typeof(string));

            dataTable.Columns.Add("SaveType", typeof(string));
            dataTable.Columns.Add("path_source", typeof(string));
            dataTable.Columns.Add("path_entry", typeof(string));
            dataTable.Columns.Add("path_cover", typeof(string));




            var result_EntryInfo = Core.Services.EntryCommonService.SelectEntry(enrtyStorage.StorageName);
            foreach (var item in result_EntryInfo)
            {
                var data = (IDictionary<String, Object>)item;
                object eid, name, releaseData, myRating, path_cover, path_source, path_entry, saveType;
                data.TryGetValue("Eid", out eid);
                data.TryGetValue("NameStr", out name);
                data.TryGetValue("ReleaseDate", out releaseData);
                data.TryGetValue("MyRating", out myRating);


                data.TryGetValue("SaveType", out saveType);
                data.TryGetValue("path_entry", out path_entry);
                data.TryGetValue("path_cover", out path_cover);
                data.TryGetValue("path_source", out path_source);


                //创建数据行
                DataRow row = dataTable.NewRow();
                //基本
                row["Name"] = name;
                row["ReleaseDate"] = releaseData;
                row["MyRating"] = myRating;

                //屬性
                var elc = result_EntryLabel.Where(a => a.EntryId == Convert.ToString(eid));
                var elp = result_EntryLabelProperty.Where(a => a.EntryId == Convert.ToString(eid));
                foreach (var lp in label_lp)
                {
                    var label_Property_Child = label_lp.Where(a => a.ParentId == lp.LPId);
                    foreach (var lpc in label_Property_Child)
                    {
                        if (elp.Select(a => a.LPId).Contains(lpc.LPId))
                        {
                            if (row[lp.Name].ToString().Length > 0)
                                row[lp.Name] += "/";
                            row[lp.Name] += lpc.Name;
                        }
                    }
                }
                //分類
                foreach (var lc in label_lc)
                {
                    if (elc.Select(a => a.LCId).Contains(lc.LCId))
                    {
                        if (row["Classification"].ToString().Length > 0) row["Classification"] += "/";
                        row["Classification"] += lc.Name;
                        break;
                    }
                }


                row["path_entry"] = System.IO.Path.Combine(enrtyStorage.StoragePath + Convert.ToString(path_entry));
                row["path_cover"] = System.IO.Path.Combine(enrtyStorage.StoragePath + Convert.ToString(path_entry), Convert.ToString(path_cover));

                var saveMode = Convert.ToInt16(saveType) == 1 ? SaveType.Folder : Convert.ToInt16(saveType) == 2 ? SaveType.Files : SaveType.Local;
                row["SaveType"] = saveMode;
                switch (saveMode)
                {
                    case SaveType.Folder:
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(path_source)))
                            row["path_source"] = enrtyStorage.StoragePath + Convert.ToString(path_source);
                        break;
                    case SaveType.Files:
                        var lstPath = Convert.ToString(path_source).Split(">.<", StringSplitOptions.RemoveEmptyEntries).ToList();
                        var lstPath_Full = new List<string>();
                        foreach (var path in lstPath)
                        {
                            lstPath_Full.Add(enrtyStorage.StoragePath + path);
                        }
                        row["path_source"] = string.Format("<{0}>", string.Join(">,<", lstPath_Full));
                        break;
                    case SaveType.Local:
                        row["path_source"] = string.Empty;
                        break;
                    default:
                        break;
                }
                dataTable.Rows.Add(row);
            }



            //DataTable的列名和excel的列名对应字典，因为excel的列名一般是中文的，DataTable的列名是英文的，字典主要是存储excel和DataTable列明的对应关系，当然我们也可以把这个对应关系存在配置文件或者其他地方
            Dictionary<string, string> dir = new Dictionary<string, string>();
            dir.Add("Name", "詞條名稱");
            dir.Add("ReleaseDate", "發行日期");
            dir.Add("MyRating", "評分");

            label_lp.ForEach(s => dir.Add(s.Name, s.Name));
            dir.Add("Classification", "分類");

            dir.Add("SaveType", "存儲模式");
            dir.Add("path_source", "存儲地址");
            dir.Add("path_entry", "詞條路徑");
            dir.Add("path_cover", "封面路徑");
            //使用helper类导出DataTable数据到excel表格中,参数依次是 （DataTable数据源;  excel表名;  excel存放位置的绝对路径; 列名对应字典; 是否清空以前的数据，设置为false，表示内容追加; 每个sheet放的数据条数,如果超过该条数就会新建一个sheet存储）
            ExcelHelper.ExportDTtoExcel(dataTable, "Info表", filePath, dir, true);

        }


        /// <summary>
        /// 从Excel中导入词条数据
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="enrtyStorage"></param>
        public static async void ImportExcel(string filePath, EnrtyStorage enrtyStorage)
        {
            var msg = new StringBuilder();
            if (string.IsNullOrEmpty(filePath)) return;
            //用于支持gb2312    
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //DataTable的列名和excel的列名对应字典

            //读取数据到DataTable，参数依此是（excel文件路径，列名所在行，sheet索引）
            DataTable dt = ExcelHelper.ImportExceltoDt(filePath, 1, 0);
            //遍历DataTable---------------------------

            //基本信息
            List<string> list = new List<string>() { "詞條名稱", "發行日期", "評分", "分類", "存儲模式", "存儲地址", "詞條路徑", "封面路徑" };
            //已有屬性
            var lpdbs = Core.Services.LabelPropertyService.GetAllLabel(Settings.DbSelectorService.dbCurrentId);
            var lpdbs_Yeye = lpdbs.Where(a => a.Level==1);

            foreach (DataColumn item in dt.Columns)
            {
                if (!list.Contains(item.ColumnName) && !lpdbs_Yeye.Select(a => a.Name).Contains(item.ColumnName))
                {
                    var lpdb = new LabelPropertyDb()
                    {
                        Name = item.ColumnName,
                        DbSourceId = Settings.DbSelectorService.dbCurrentId,
                        Level = 1,
                    };
                    Core.Services.LabelPropertyService.AddLabel(lpdb);
                    lpdbs.Add(lpdb);
                }
            }

            int rowCount = 1;
            //遍历DataTable中的数据并插入数据库
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    var eid = Guid.NewGuid().ToString();
                    EntryDb edb = new EntryDb() { EntryId = eid };
                    List<EntryNameDb> endbs = new List<EntryNameDb>();
                    List<EntrySourceDb> esdbs = new List<EntrySourceDb>();

                    SaveType saveMode = SaveType.Local;
                    var strPath = string.Empty;
                    bool isAdd = true;
                    
                    
                    int n = -1;
                    foreach (DataColumn item in dt.Columns)
                    {
                        n++;
                        var baseInfo = list.Where(a => a.Equals(item.ColumnName));
                        if (baseInfo.Count() > 0)
                        {
                            switch (baseInfo.FirstOrDefault())
                            {
                                case "詞條名稱":
                                    var strName = Convert.ToString(row[n]);
                                    var lstName = strName.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();//根据分隔符构建list
                                    int c = 0;
                                    foreach (var name in lstName)
                                    {
                                        endbs.Add(new EntryNameDb()
                                        {
                                            Name = name,
                                            EntryId = eid,
                                            IsDefault = c == 0 ? true : false,
                                        });
                                    }
                                    break;
                                case "發行日期":
                                    edb.ReleaseDate = Convert.ToDateTime(row[n]);
                                    break;
                                case "評分":
                                    edb.MyRating = Convert.ToDouble(row[n]);
                                    break;
                                case "詞條路徑":
                                    var path_full = Convert.ToString(row[n]);
                                    var path_entry = path_full.Replace(enrtyStorage.StoragePath, string.Empty);
                                    var eid_update = Core.Services.EntryCommonService.GetEidBySameEntryPath(path_entry, enrtyStorage.StorageName);
                                    if (!string.IsNullOrWhiteSpace(eid_update))
                                    {
                                        isAdd = false;
                                        edb.EntryId = eid_update;
                                        edb.Path = path_full;
                                        eid = eid_update;
                                        endbs.ForEach(a => a.EntryId = eid);
                                        esdbs.ForEach(a => a.EntryId = eid);
                                    }
                                    edb.Path = Convert.ToString(row[n]);
                                    break;
                                case "封面路徑":
                                    edb.CoverImg = Convert.ToString(row[n]);
                                    break;
                                case "存儲模式":
                                    try { saveMode = (SaveType)System.Enum.Parse(typeof(SaveType), Convert.ToString(row[n])); } catch { }
                                    break;
                                case "存儲地址"://绝对地址
                                    strPath = Convert.ToString(row[n]);
                                    break;
                                case "分類":
                                    //暂不处理
                                    break;
                                default:
                                    break;
                            }
                        }
                        //标签->属性 插入
                        var lpdb_Yeye = lpdbs_Yeye.Where(a => a.Name.Equals(item.ColumnName));
                        if (lpdb_Yeye.Count() > 0)
                        {
                            var lpdbs_Baba = lpdbs.Where(a => a.ParentId == lpdb_Yeye.FirstOrDefault().LPId);
                            var lpdbName = Convert.ToString(row[n]);
                            if (string.IsNullOrWhiteSpace(lpdbName))
                                continue;
                            //不存在 属性_儿子
                            if (!lpdbs_Baba.Select(a => a.Name).Contains(lpdbName))
                            {
                                var lpdb = new LabelPropertyDb()
                                {
                                    LPId = Guid.NewGuid().ToString(),
                                    Name = lpdbName,
                                    DbSourceId = Settings.DbSelectorService.dbCurrentId,
                                    ParentId = lpdb_Yeye.FirstOrDefault().LPId,
                                };
                                lpdbs.Add(lpdb);
                                Core.Services.LabelPropertyService.AddLabel(lpdb);
                                var eldb = new EntryLabelPropertyLKDb()
                                {
                                    DbId = enrtyStorage.StorageName,
                                    LPId = lpdb.LPId,
                                    EntryId = eid,
                                };
                                Core.Services.EntryLabelPropertyService.AddEntryLabel(eldb);

                            }
                            //已存在 属性_儿子
                            else
                            {
                                var lpid = lpdbs_Baba.Where(a => a.Name.Equals(lpdbName)).FirstOrDefault().LPId;
                                var eldb = new EntryLabelPropertyLKDb()
                                {
                                    DbId = enrtyStorage.StorageName,
                                    LPId = lpid,
                                    EntryId = eid,
                                };
                                Core.Services.EntryLabelPropertyService.AddEntryLabel(eldb);
                            }
                        }
                    }


                    //校驗必錄字段
                    if (!(endbs.Count > 0))
                        throw new Exception($"詞條名稱為空！");
                    //選填字段自動錄入


                    //校驗存儲地址是否合法(仅插入合法地址)
                    switch (saveMode)
                    {
                        case SaveType.Folder:
                            edb.SaveType = '1';
                            if (!strPath.StartsWith(enrtyStorage.StorageName, StringComparison.OrdinalIgnoreCase))
                                strPath = string.Empty;
                            var esdb = new EntrySourceDb()
                            {
                                EntryId = eid,
                                FileType = '1',
                                Id = Guid.NewGuid().ToString(),
                                Path = strPath,
                            };
                            esdbs.Add(esdb);
                            break;
                        case SaveType.Files:
                            edb.SaveType = '2';
                            var lstPath = strPath.Substring(1, strPath.Length - 2).Split(">,<").ToList();
                            foreach (var path in lstPath)
                            {
                                if (!strPath.StartsWith(enrtyStorage.StorageName, StringComparison.OrdinalIgnoreCase)) continue;
                                var esdb_s = new EntrySourceDb()
                                {
                                    EntryId = eid,
                                    FileType = CommonService.GetFileType(path),
                                    Id = Guid.NewGuid().ToString(),
                                    Path = path,
                                };
                                esdbs.Add(esdb_s);
                            }
                            break;
                        case SaveType.Local:
                            edb.SaveType = '3';
                            break;
                        default:
                            break;
                    }

                    //詞條 插入or更新
                    if (isAdd)
                    {
                        //创建词条路径
                        if (edb.Path == null || !edb.Path.Contains(System.IO.Path.Combine(enrtyStorage.StoragePath, ConfigService.OMDbFolder, Settings.DbSelectorService.dbCurrentName)))//不在仓库路径内，强设置词条路径
                            edb.Path = System.IO.Path.Combine(enrtyStorage.StoragePath, ConfigService.OMDbFolder, Settings.DbSelectorService.dbCurrentName, endbs[0].Name);
                        if (Directory.Exists(edb.Path))//重名路径 -> 改名
                        {
                            int i = 1;
                            while (true)
                            {
                                string newPath = $"{edb.Path}({i++})";
                                if (!Directory.Exists(newPath))
                                {
                                    edb.Path = newPath;
                                    break;
                                }
                            }
                        }
                        //创建词条文件夹
                        Directory.CreateDirectory(edb.Path);
                        Directory.CreateDirectory(Path.Combine(edb.Path, Services.ConfigService.AudioFolder));
                        Directory.CreateDirectory(Path.Combine(edb.Path, Services.ConfigService.ImgFolder));
                        Directory.CreateDirectory(Path.Combine(edb.Path, Services.ConfigService.VideoFolder));
                        Directory.CreateDirectory(Path.Combine(edb.Path, Services.ConfigService.ResourceFolder));
                        Directory.CreateDirectory(Path.Combine(edb.Path, Services.ConfigService.SubFolder));
                        Directory.CreateDirectory(Path.Combine(edb.Path, Services.ConfigService.InfoFolder));
                        Directory.CreateDirectory(Path.Combine(edb.Path, Services.ConfigService.MoreFolder));

                        //创建元数据(MataData.Json)
                        string metaDateFile = Path.Combine(edb.Path, Services.ConfigService.MetadataFileNmae);
                        Core.Models.EntryMetadata metadata;
                        if (System.IO.File.Exists(metaDateFile))
                        {
                            metadata = Core.Models.EntryMetadata.Read(metaDateFile);
                            metadata.Name = endbs[0].Name;
                        }
                        else
                        {
                            metadata = new Core.Models.EntryMetadata()
                            {
                                Id = eid,
                                Name = endbs[0].Name,
                            };
                        }
                        metadata.Save(metaDateFile);
                    }

                    //封面路徑不正確
                    if (edb.CoverImg == null || !System.IO.File.Exists(edb.CoverImg) || CommonService.GetFileType(edb.CoverImg).Equals('1'))//不存在该路径或该文件不为图片
                    {
                        List<string> lstPath = new List<string>();
                        switch (saveMode)
                        {
                            case SaveType.Folder:
                                lstPath.Add(esdbs[0].Path);
                                edb.CoverImg = CommonService.GetCoverByPath(lstPath, FileType.Folder);
                                break;
                            case SaveType.Files:
                                if (esdbs.Count > 0)
                                    edb.CoverImg = CommonService.GetCoverByPath(lstPath, FileType.Folder);
                                else
                                    edb.CoverImg = CommonService.GetCoverByPath();
                                break;
                            case SaveType.Local:
                                edb.CoverImg = CommonService.GetCoverByPath();
                                break;
                            default:
                                break;
                        }
                    }
                    //复制封面图(Cover)、并同步修改封面路径
                    var coverType = Path.GetExtension(edb.CoverImg);
                    var newCoverFullPath = Path.Combine(edb.Path, Services.ConfigService.InfoFolder, $@"Cover{coverType}");
                    if (newCoverFullPath != edb.CoverImg)
                        File.Copy(edb.CoverImg, newCoverFullPath, true);





                    //數據庫 詞條路徑&圖片路徑 取相對地址
                    edb.Path = edb.Path.Replace(enrtyStorage.StoragePath, string.Empty);
                    edb.CoverImg = newCoverFullPath.Replace(enrtyStorage.StoragePath + edb.Path + "\\", string.Empty);

                    //保存至数据库
                    if (isAdd)
                    {
                        Core.Services.EntryService.AddEntry(edb, enrtyStorage.StorageName);
                        Core.Services.EntryNameSerivce.AddEntryName(endbs, enrtyStorage.StorageName);
                        Core.Services.EntrySourceSerivce.AddEntrySource(esdbs, enrtyStorage.StorageName);
                        msg.AppendLine($"第{rowCount}行導入：新增成功！");
                    }
                    else
                    {
                        Core.Services.EntryService.UpdateEntry(edb, enrtyStorage.StorageName);
                        Core.Services.EntrySourceSerivce.AddEntrySource(esdbs, enrtyStorage.StorageName);
                        msg.AppendLine($"第{rowCount}行導入：更新成功！");
                    }
                    rowCount++;

                }
                catch (Exception ex)
                {
                    msg.AppendLine($"第{rowCount}行導入：失敗！{ex.Message}");
                    rowCount++;
                }
            }
            InfoHelper.ShowMsgLong("Result", msg.ToString());
        }
    }
}
