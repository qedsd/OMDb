using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace OMDb.Maui.Models;

/// <summary>
/// 资源管理器项
/// 用于显示文件/文件夹树
/// </summary>
public partial class ExplorerItem : ObservableObject
{
    /// <summary>
    /// 文件/文件夹名
    /// </summary>
    [ObservableProperty]
    private string _name;

    /// <summary>
    /// 完整路径
    /// </summary>
    [ObservableProperty]
    private string _fullName;

    /// <summary>
    /// 是否为文件
    /// </summary>
    [ObservableProperty]
    private bool _isFile;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    [ObservableProperty]
    private long _length;

    /// <summary>
    /// 子项列表
    /// </summary>
    public ObservableCollection<ExplorerItem> Children { get; set; } = new();

    [ObservableProperty]
    private float _copyPercent;

    /// <summary>
    /// 复制文件源路径
    /// </summary>
    [ObservableProperty]
    private string _sourcePath;

    [ObservableProperty]
    private bool _isCopying;

    [ObservableProperty]
    private bool _isDeleting;

    [ObservableProperty]
    private bool _isVerifying;

    /// <summary>
    /// 取消复制命令
    /// </summary>
    [RelayCommand]
    private void CancelCopy()
    {
        if (IsCopying)
        {
            IsCopying = false;
        }
    }

    /// <summary>
    /// 删除命令
    /// </summary>
    [RelayCommand]
    private async Task DeleteAsync()
    {
        IsDeleting = true;
        await Task.Run(() =>
        {
            try
            {
                if (IsFile)
                {
                    System.IO.File.Delete(FullName);
                }
                else
                {
                    System.IO.Directory.Delete(FullName, true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"删除失败：{ex.Message}");
            }
        });
        IsDeleting = false;
    }
}
