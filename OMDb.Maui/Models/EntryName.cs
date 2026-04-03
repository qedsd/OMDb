using CommunityToolkit.Mvvm.ComponentModel;
using OMDb.Core.DbModels;

namespace OMDb.Maui.Models;

/// <summary>
/// 条目名称
/// </summary>
public class EntryName : EntryNameDb
{
    /// <summary>
    /// 所属数据库唯一标识
    /// </summary>
    public string DbId { get; set; }

    public static EntryName Create(EntryNameDb dbItem, string dbId)
    {
        var newItem = new EntryName
        {
            Name = dbItem.Name,
            Mark = dbItem.Mark,
            IsDefault = dbItem.IsDefault,
            DbId = dbId
        };
        return newItem;
    }
}
