using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

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
        /// <summary>
        /// 复制百分比
        /// 0-1
        /// </summary>
        public float CopyPercent
        {
            get => copyPercent;
            set=>SetProperty(ref copyPercent, value);
        }
        /// <summary>
        /// 复制文件源路径
        /// </summary>
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
        private CancellationTokenSource CancellationTokenSource;
        /// <summary>
        /// 复制完成事件
        /// 完成取消复制也会触发
        /// </summary>
        public ExplorerItemEventHandler FinishCopyEventHandler;
        /// <summary>
        /// 复制完成事件
        /// 完成取消复制也会触发
        /// </summary>
        public event ExplorerItemEventHandler FinishCopyEvent
        {
            add
            {
                FinishCopyEventHandler += value;
            }
            remove
            {
                FinishCopyEventHandler -= value;
            }
        }
        public void Copy()
        {
            if (!IsCanceled&&!string.IsNullOrEmpty(SourcePath))
            {
                CancellationTokenSource = new CancellationTokenSource();
                IsCopying = true;
                string newFullName = Helpers.FileHelper.CopyFile(SourcePath, FullName, CopyFileCallBack, CancellationTokenSource.Token);
                //如果目标文件重名，会自动改名
                if(newFullName != FullName)
                {
                    FullName = newFullName;
                    Name = System.IO.Path.GetFileName(newFullName);
                }
                if(CancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (System.IO.File.Exists(FullName))
                    {
                        IsDeleting = true;
                        try
                        {
                            System.IO.File.Delete(FullName);
                        }
                        catch (Exception)
                        {

                        }
                        IsDeleting = false;
                    }
                    CanceledCopyEventHandler?.Invoke(this);
                }
                else
                {
                    Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                    {
                        IsVerifying = true;
                    });
                    //校验MD5
                    byte[] sourceMd5 = null;
                    byte[] newMd5 = null;
                    using (System.IO.FileStream fs = System.IO.File.OpenRead(SourcePath))
                    {
                        using (var crypto = System.Security.Cryptography.MD5.Create())
                        {
                            sourceMd5 = crypto.ComputeHash(fs);
                        }
                    }
                    using (System.IO.FileStream fs = System.IO.File.OpenRead(FullName))
                    {
                        using (var crypto = System.Security.Cryptography.MD5.Create())
                        {
                            newMd5 = crypto.ComputeHash(fs);
                        }
                    }
                    Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                    {
                        IsVerifying = false;
                    });
                    bool isVerifySuccess = true;
                    for (int i=0;i< sourceMd5.Length;i++)
                    {
                        if (sourceMd5[i] != newMd5[i])
                        {
                            isVerifySuccess = false;
                            break;
                        }
                    }
                    if (isVerifySuccess)
                    {
                        FinishCopyEventHandler?.Invoke(this);
                    }
                    else
                    {
                        VerifyFaiedEventHandler?.Invoke(this);
                    }
                }
            }
        }
        public async Task CopyAsync()
        {
            if (!IsCanceled && !string.IsNullOrEmpty(SourcePath))
            {
                IsCopying = true;
                await Task.Run(() =>
                {
                    Copy();
                });
            }
        }
        private bool isCopying = false;
        public bool IsCopying
        {
            get=> isCopying;
            set=>SetProperty(ref isCopying, value);
        }
        private ExplorerItemEventHandler CanceledCopyEventHandler;
        /// <summary>
        /// 完成取消复制后触发事件
        /// </summary>
        public event ExplorerItemEventHandler CanceledCopyEvent
        {
            add
            {
                CanceledCopyEventHandler += value;
            }
            remove
            {
                CanceledCopyEventHandler -= value;
            }
        }
        private bool IsCanceled = false;
        /// <summary>
        /// 取消复制
        /// 不会等待取消完成再返回，需要可以监听取消完成事件CanceledCopyEvent
        /// </summary>
        public void CancelCopy()
        {
            if (IsCopying)
            {
                IsCanceled = true;
                if (CancellationTokenSource != null)
                {
                    CancellationTokenSource.Cancel();
                }
                IsCopying = false;
            }
        }

        private bool isDeleting = false;
        public bool IsDeleting
        {
            get => isDeleting;
            set => SetProperty(ref isDeleting, value);
        }

        public ICommand CancelCopyCommand => new RelayCommand(() =>
        {
            CancelCopy();
        });
        public ICommand DeleteCommand => new RelayCommand(async() =>
        {
            IsDeleting = true;
            await Task.Run(() =>
            {
                if (IsFile)
                {
                    try
                    {
                        System.IO.File.Delete(FullName);
                    }
                    catch(Exception ex)
                    {
                        Helpers.InfoHelper.ShowError(ex.Message);
                    }
                }
                else
                {
                    try
                    {
                        System.IO.Directory.Delete(FullName);
                    }
                    catch (Exception ex)
                    {
                        Helpers.InfoHelper.ShowError(ex.Message);
                    }
                }
            });
            IsDeleting = false;
            DeletedEventHandler?.Invoke(this);
        });
        private static ExplorerItemEventHandler DeletedEventHandler;
        /// <summary>
        /// 删除文件后触发事件
        /// </summary>
        public static event ExplorerItemEventHandler DeletedEvent
        {
            add
            {
                DeletedEventHandler += value;
            }
            remove
            {
                DeletedEventHandler -= value;
            }
        }

        private bool isVerifying = false;
        /// <summary>
        /// 复制完文件后的校验中
        /// </summary>
        public bool IsVerifying
        {
            get => isVerifying;
            set => SetProperty(ref isVerifying, value);
        }
        public delegate void ExplorerItemEventHandler(ExplorerItem explorerItem);
        private static event ExplorerItemEventHandler VerifyFaiedEventHandler;
        /// <summary>
        /// 复制完文件后校验失败触发事件
        /// </summary>
        public static event ExplorerItemEventHandler VerifyFaiedEvent
        {
            add
            {
                VerifyFaiedEventHandler += value;
            }
            remove
            {
                VerifyFaiedEventHandler -= value;
            }
        }
    }
}
