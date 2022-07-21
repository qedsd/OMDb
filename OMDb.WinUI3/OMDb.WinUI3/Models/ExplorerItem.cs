using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class ExplorerItem : ObservableObject
    {
        /// <summary>
        /// 文件/文件夹名
        /// 带文件后缀名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullName { get; set; }
        public bool IsFile { get; set; }
        /// <summary>
        /// 文件大小
        /// 单位为字节
        /// </summary>
        public long Length { get; set; }
        public List<ExplorerItem> Children { get; set; }

        private float copyPercent;
        public float CopyPercent
        {
            get => copyPercent;
            set=>SetProperty(ref copyPercent, value);
        }
        public string SourcePath { get; set; }
        private void CopyFileCallBack(float p)
        {
            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                CopyPercent = p;
                if(CopyPercent == 1)
                {
                    IsCopying = false;
                    Helpers.InfoHelper.ShowSuccess($"{Name}已复制完成");
                }
            });
        }
        public void Copy()
        {
            IsCopying = true;
            Task.Run(() =>
            {
                Helpers.FileHelper.CopyFiles(SourcePath, FullName, CopyFileCallBack);
            });
        }
        public async Task CopyAsync()
        {
            IsCopying = true;
            await Task.Run(() =>
            {
                Helpers.FileHelper.CopyFiles(SourcePath, FullName, CopyFileCallBack);
            });
        }
        private bool isCopying = false;
        public bool IsCopying
        {
            get=> isCopying;
            set=>SetProperty(ref isCopying, value);
        }
    }
}
