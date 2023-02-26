using OMDb.Core;
using OMDb.WinUI3.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Helpers
{
    public static class PathHelper
    {
        public static void OpenBySystem(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(path));
        }

        /// <summary>
        /// 调用cmd获取文件夹已使用、可用大小
        /// </summary>
        /// <param name="path"></param>
        /// <param name="usedByte"></param>
        /// <param name="usableByte"></param>
        /// <exception cref="Exception"></exception>
        public static void CalFolderSize(string path,out long usedByte,out long usableByte)
        {
            usedByte = 0;
            usableByte = 0;
            Process p = new Process();
            //设置要启动的应用程序
            p.StartInfo.FileName = "cmd.exe";
            //是否使用操作系统shell启动
            p.StartInfo.UseShellExecute = false;
            // 接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardInput = true;
            //输出信息
            p.StartInfo.RedirectStandardOutput = true;
            // 输出错误
            p.StartInfo.RedirectStandardError = true;
            //不显示程序窗口
            p.StartInfo.CreateNoWindow = true;

            //启动程序
            p.Start();

            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine($"cd /d {path}");

            p.StandardInput.WriteLine("dir /a/s &exit");
            p.StandardInput.AutoFlush = true;
            //获取输出信息
            string strOuput = p.StandardOutput.ReadToEnd();
            //等待程序执行完退出进程
            p.WaitForExit();
            p.Close();

            try
            {
                var lines = strOuput.Split("\r\n");
                if (lines.Length > 0)
                {
                    var lines2 = lines.Where(p => !string.IsNullOrEmpty(p)).ToArray();
                    string fileLine = lines2[lines2.Length - 2];
                    string folderLine = lines2[lines2.Length - 1];
                    var fileLineChars = fileLine.Split(' ');
                    var folderLineChars = folderLine.Split(' ');
                    usedByte = long.Parse(fileLineChars[fileLineChars.Length - 2].Replace(",", string.Empty));
                    usableByte = long.Parse(folderLineChars[folderLineChars.Length - 2].Replace(",", string.Empty));
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"处理cmd结果出错：{ex.Message}");
            }
        }
    }
}
