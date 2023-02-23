using NPOI.SS.Formula;
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
            var label_Classification = result_LabelInfo.Where(a => !a.IsProperty).Where(c=>(!label_Property.Select(b=>b.Id).Contains(c.ParentId))).ToList();
            label_Property.ForEach(s => dataTable.Columns.Add(s.Name, typeof(string)));
            //分類列
            dataTable.Columns.Add("Classification", typeof(string));

           
            var result_EntryInfo = Core.Services.EntryCommonService.SelectEntry(dbId);
            var result_EntryLabel = Core.Services.EntryLabelService.SelectAllEntryLabel(dbId);
            foreach (var item in result_EntryInfo)
            {
                var data = (IDictionary<String, Object>)item;
                object eid,name, releaseData, myRating, coverImg, path;
                data.TryGetValue("Eid", out eid);
                data.TryGetValue("NameStr", out name);
                data.TryGetValue("ReleaseDate", out releaseData);
                data.TryGetValue("MyRating", out myRating);
                data.TryGetValue("CoveriImg", out coverImg);
                data.TryGetValue("PathStr", out path);



                //创建数据行
                //基本
                DataRow row = dataTable.NewRow();
                row["Name"] = name;
                row["ReleaseDate"] = releaseData;
                row["MyRating"] = myRating;
                row["CoverImg"] = coverImg;
                //屬性
                var el=result_EntryLabel.Where(a => a.EntryId == Convert.ToString(eid));
                foreach (var lp in label_Property)
                {
                    var label_Property_child = result_LabelInfo.Where(a => a.ParentId == lp.Id);
                    foreach (var lpc in label_Property_child)
                    {
                        if (el.Select(a=>a.LabelId).Contains(lpc.Id))
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
                        row["Classification"] += string.Format("{0}/",lc.Name);
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
        public static void ImportExcel(string filePathAndName)
        {
            if (string.IsNullOrEmpty(filePathAndName)) return;

            //用于支持gb2312    
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //DataTable的列名和excel的列名对应字典
            Dictionary<string, string> dir = new Dictionary<string, string>();
            dir.Add("id", "编号");
            dir.Add("uname", "用户");
            dir.Add("sex", "性别");
            dir.Add("age", "年龄");
            dir.Add("pwd", "密码");
            dir.Add("email", "邮箱");
            dir.Add("address", "住址");

            //读取数据到DataTable，参数依此是（excel文件路径，列名对应字典，列名所在行，sheet索引）
            DataTable dt = ExcelHelper.ImportExceltoDt(filePathAndName, dir, 1, 0);
            //遍历DataTable---------------------------
            string tmpInfo = null;
            foreach (DataColumn item in dt.Columns)
            {
                //显示dataTable的列名
                tmpInfo += (item.ColumnName + new string('-', 10));
            }
            tmpInfo += "\r\n";

            //遍历DataTable中的数据
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    tmpInfo += (row[i].ToString() + new string(' ', 10));

                }
                tmpInfo += "\r\n";
            }

            /*//显示数据
            ShowInfo(tmpInfo);

            //导入成功提示
            ImportSuccessTips(filePathAndName);*/

        }
    }
}
