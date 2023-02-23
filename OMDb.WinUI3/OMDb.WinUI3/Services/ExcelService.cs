using NPOI.SS.Formula;
using OMDb.Core.DbModels;
using OMDb.Core.Models;
using OMDb.WinUI3.Helpers;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Services
{
    public static class ExcelService
    {
        public static void ExportExcel(string filePath, string dbId)
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
            dataTable.Columns.Add("CoverImg", typeof(string));
            //詞條屬性列
            var result_LabelInfo = Core.Services.LabelService.GetAllLabel(Settings.DbSelectorService.dbCurrentId);
            var label_Property = result_LabelInfo.Where(a => a.IsProperty).ToList();
            var label_Classification = result_LabelInfo.Where(a => !a.IsProperty).Where(c => (!label_Property.Select(b => b.Id).Contains(c.ParentId))).ToList();
            label_Property.ForEach(s => dataTable.Columns.Add(s.Name, typeof(string)));
            //分類列
            dataTable.Columns.Add("Classification", typeof(string));


            var result_EntryInfo = Core.Services.EntryCommonService.SelectEntry(dbId);
            var result_EntryLabel = Core.Services.EntryLabelService.SelectAllEntryLabel(dbId);
            foreach (var item in result_EntryInfo)
            {
                var data = (IDictionary<String, Object>)item;
                object eid, name, releaseData, myRating, coverImg, path;
                data.TryGetValue("Eid", out eid);
                data.TryGetValue("NameStr", out name);
                data.TryGetValue("ReleaseDate", out releaseData);
                data.TryGetValue("MyRating", out myRating);
                data.TryGetValue("CoverImg", out coverImg);
                data.TryGetValue("PathStr", out path);



                //创建数据行
                DataRow row = dataTable.NewRow();
                //基本
                row["Name"] = name;
                row["ReleaseDate"] = releaseData;
                row["MyRating"] = myRating;
                row["CoverImg"] = coverImg;
                //屬性
                var el = result_EntryLabel.Where(a => a.EntryId == Convert.ToString(eid));
                foreach (var lp in label_Property)
                {
                    var label_Property_Child = result_LabelInfo.Where(a => a.ParentId == lp.Id);
                    foreach (var lpc in label_Property_Child)
                    {
                        if (el.Select(a => a.LabelId).Contains(lpc.Id))
                        {
                            row[lp.Name] = lpc.Name;
                            break;
                        }
                    }
                }
                //分類
                foreach (var lc in label_Classification)
                {
                    if (el.Select(a => a.LabelId).Contains(lc.Id))
                    {
                        row["Classification"] += string.Format("{0}/", lc.Name);
                        break;
                    }
                }
                dataTable.Rows.Add(row);
            }



            //DataTable的列名和excel的列名对应字典，因为excel的列名一般是中文的，DataTable的列名是英文的，字典主要是存储excel和DataTable列明的对应关系，当然我们也可以把这个对应关系存在配置文件或者其他地方
            Dictionary<string, string> dir = new Dictionary<string, string>();
            dir.Add("Name", "詞條名稱");
            dir.Add("ReleaseDate", "發行日期");
            dir.Add("MyRating", "評分");
            dir.Add("CoverImg", "封面地址");
            label_Property.ForEach(s => dir.Add(s.Name, s.Name));
            dir.Add("Classification", "分類");
            //使用helper类导出DataTable数据到excel表格中,参数依次是 （DataTable数据源;  excel表名;  excel存放位置的绝对路径; 列名对应字典; 是否清空以前的数据，设置为false，表示内容追加; 每个sheet放的数据条数,如果超过该条数就会新建一个sheet存储）
            ExcelHelper.ExportDTtoExcel(dataTable, "Info表", filePath, dir, true);

        }


        //从Excel中导入数据到界面
        public static void ImportExcel(string filePath, string dbId)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            //用于支持gb2312    
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //DataTable的列名和excel的列名对应字典

            //读取数据到DataTable，参数依此是（excel文件路径，列名所在行，sheet索引）
            DataTable dt = ExcelHelper.ImportExceltoDt(filePath, 1, 0);
            //遍历DataTable---------------------------

            //基本信息
            List<string> list = new List<string>() { "詞條名稱", "發行日期", "評分", "封面地址", "分類" };
            //已有屬性
            var result_LabelInfo = Core.Services.LabelService.GetAllLabel(Settings.DbSelectorService.dbCurrentId);
            var label_Property_Parent = result_LabelInfo.Where(a => a.IsProperty);

            foreach (DataColumn item in dt.Columns)
            {
                if (!list.Contains(item.ColumnName) && !label_Property_Parent.Select(a => a.Name).Contains(item.ColumnName))
                {
                    var ldb = new LabelDb()
                    {
                        Name = item.ColumnName,
                        DbSourceId = Settings.DbSelectorService.dbCurrentId,
                        IsProperty = true,
                        IsShow = false,
                    };
                    Core.Services.LabelService.AddLabel(ldb);
                }
            }

            //重新加载已有属性
            result_LabelInfo = Core.Services.LabelService.GetAllLabel(Settings.DbSelectorService.dbCurrentId);
            label_Property_Parent = result_LabelInfo.Where(a => a.IsProperty);

            //遍历DataTable中的数据并插入数据库
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    foreach (DataColumn item in dt.Columns)
                    {
                        var eid = Guid.NewGuid().ToString();
                        EntryDb edb = new EntryDb() { EntryId = eid };
                        EntryNameDb endb = new EntryNameDb() { EntryId = eid };
                        EntrySourceDb esdb = new EntrySourceDb() { EntryId = eid };
                        int n = 0;

                        var baseInfo = list.Where(a => a.Equals(item.ColumnName));
                        if (baseInfo.Count() > 0)
                        {
                            switch (baseInfo.FirstOrDefault())
                            {
                                case "詞條名稱":
                                    endb.Name = Convert.ToString(row[n]);
                                    break;
                                case "發行日期":
                                    edb.ReleaseDate = Convert.ToDateTime(row[n]);
                                    break;
                                case "評分":
                                    edb.MyRating = Convert.ToDouble(row[n]);
                                    break;
                                case "封面地址":
                                    edb.CoverImg = Convert.ToString(row[n]);
                                    break;
                                case "分類":
                                    //不处理
                                    break;
                                default:
                                    break;
                            }
                        }
                        var propertyInfo = label_Property_Parent.Where(a => a.Name.Equals(item.ColumnName));
                        if (propertyInfo.Count() > 0)
                        {
                            var label_Property_Childs = result_LabelInfo.Where(a => a.ParentId == propertyInfo.FirstOrDefault().Id);
                            var property_Child = Convert.ToString(row[n]);
                            //不存在 属性_儿子
                            if (!label_Property_Childs.Select(a => a.Name).Contains(property_Child))
                            {
                                var ldb = new LabelDb()
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Name = property_Child,
                                    DbSourceId = Settings.DbSelectorService.dbCurrentId,
                                    IsProperty = false,
                                    IsShow = false,
                                    ParentId = propertyInfo.FirstOrDefault().Id,
                                };
                                result_LabelInfo.Add(ldb);
                                Core.Services.LabelService.AddLabel(ldb);
                                var eldb = new EntryLabelDb()
                                {
                                    DbId = dbId,
                                    LabelId = ldb.Id,
                                    EntryId = eid,
                                };
                                Core.Services.EntryLabelService.AddEntryLabel(eldb);

                            }
                            //已存在 属性_儿子
                            else
                            {
                                var labelId = label_Property_Childs.Where(a => a.Name.Equals(property_Child)).FirstOrDefault().Id;
                                var eldb = new EntryLabelDb()
                                {
                                    DbId = dbId,
                                    LabelId = labelId,
                                    EntryId = eid,
                                };
                                Core.Services.EntryLabelService.AddEntryLabel(eldb);
                            }
                        }

                        Core.Services.EntryService.AddEntry(edb,dbId);
                        Core.Services.EntryNameSerivce.AddEntryName(new List<EntryNameDb>() { endb },dbId);
                        Core.Services.EntrySourceSerivce.AddEntrySource(new List<EntrySourceDb>() { esdb }, dbId );
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            /*//显示数据
            ShowInfo(tmpInfo);

            //导入成功提示
            ImportSuccessTips(filePathAndName);*/

        }
    }
}
