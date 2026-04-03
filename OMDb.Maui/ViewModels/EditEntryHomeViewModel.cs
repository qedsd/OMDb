using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OMDb.Core.Models;
using OMDb.Maui.Models;
using System.Collections.ObjectModel;

namespace OMDb.Maui.ViewModels;

/// <summary>
/// 编辑条目首页视图模型
/// 用于条目详情编辑页面
/// </summary>
public partial class EditEntryHomeViewModel : ObservableObject
{
    private Core.Models.Entry _entry;

    public Core.Models.Entry Entry
    {
        get => _entry;
        set => SetProperty(ref _entry, value);
    }

    private string _entryName = string.Empty;
    public string EntryName
    {
        get => _entryName;
        set => SetProperty(ref _entryName, value);
    }

    private string _entryPath = string.Empty;
    public string EntryPath
    {
        get => _entryPath;
        set => SetProperty(ref _entryPath, value);
    }

    private DateTime _releaseDate = DateTime.Now;
    public DateTime ReleaseDate
    {
        get => _releaseDate;
        set => SetProperty(ref _releaseDate, value);
    }

    private double _myRating;
    public double MyRating
    {
        get => _myRating;
        set => SetProperty(ref _myRating, value);
    }

    private ObservableCollection<LabelClass> _labels = new();
    public ObservableCollection<LabelClass> Labels
    {
        get => _labels;
        set => SetProperty(ref _labels, value);
    }

    public EditEntryHomeViewModel(Core.Models.Entry entry)
    {
        Entry = entry;
        if (entry != null)
        {
            EntryName = entry.Name;
        }
    }
}
