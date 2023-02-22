using OMDb.WinUI3.Helpers;
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
        public static void ExportExcel(string filePathAndName)
        {
            if (string.IsNullOrEmpty(filePathAndName)) return;

            //用于支持gb2312         
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //测试dataTable数据源 
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("id", typeof(int));
            dataTable.Columns.Add("uname", typeof(string));
            dataTable.Columns.Add("sex", typeof(string));
            dataTable.Columns.Add("age", typeof(int));
            dataTable.Columns.Add("pwd", typeof(string));
            dataTable.Columns.Add("email", typeof(string));
            dataTable.Columns.Add("address", typeof(string));
            Random r = new Random();
            for (int i = 0; i < 1000; i++)
            {
                DataRow row = dataTable.NewRow();
                row["id"] = i;
                row["uname"] = "hellox" + i.ToString();
                row["sex"] = r.Next(2) % 2 == 0 ? "男" : "女";
                row["age"] = r.Next(40) + 18;
                row["pwd"] = "pwd" + r.Next(5000).ToString();
                row["email"] = r.Next(100000) + "@qq.com";
                row["address"] = $"北京市，西{r.Next(4) + 1}环，xx路{r.Next(100)}号";
                dataTable.Rows.Add(row);
            }
            //DataTable的列名和excel的列名对应字典，因为excel的列名一般是中文的，DataTable的列名是英文的，字典主要是存储excel和DataTable列明的对应关系，当然我们也可以把这个对应关系存在配置文件或者其他地方
            Dictionary<string, string> dir = new Dictionary<string, string>();
            dir.Add("id", "编号");
            dir.Add("uname", "用户");
            dir.Add("sex", "性别");
            dir.Add("age", "年龄");
            dir.Add("pwd", "密码");
            dir.Add("email", "邮箱");
            dir.Add("address", "住址");

            //使用helper类导出DataTable数据到excel表格中,参数依次是 （DataTable数据源;  excel表名;  excel存放位置的绝对路径; 列名对应字典; 是否清空以前的数据，设置为false，表示内容追加; 每个sheet放的数据条数,如果超过该条数就会新建一个sheet存储）
            ExcelHelper.ExportDTtoExcel(dataTable, "考勤信息表", filePathAndName, dir, true);

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

            //显示数据
            ShowInfo(tmpInfo);

            //导入成功提示
            ImportSuccessTips(filePathAndName);

        }
    }
}
