using OMDb.Core.DbModels;
using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class EntryCollectionService
    {
        private static bool IsLocalDbValid()
        {
            return DbService.ConfigDb != null;
        }

        /// <summary>
        /// 获取全部片单
        /// </summary>
        /// <returns></returns>
        public static async Task<List<EntryCollection>> GetAllCollectionsAsync()
        {
            if (IsLocalDbValid())
            {
                List<EntryCollection> entryCollections = new List<EntryCollection>();
                var collectionDbs = await DbService.ConfigDb.Queryable<EntryCollectionDb>().ToListAsync();
                var items = await DbService.ConfigDb.Queryable<EntryCollectionItemDb>().ToListAsync();
                await Task.Run(() =>
                {
                    var group = items.GroupBy(p => p.CollectionId).ToList();
                    if (group != null && group.Count > 0)
                    {
                        var dic = group.ToDictionary(p => p.Key);//key为EntryCollectionDb.Id
                        foreach (var item in group)
                        {
                            var collection = collectionDbs.FirstOrDefault(p => p.Id == item.Key);
                            if (collection != null)
                            {
                                EntryCollection entryCollection = EntryCollection.Create(collection);
                                entryCollection.Items = new List<EntryCollectionItemDb>(item);
                                entryCollections.Add(entryCollection);
                                collectionDbs.Remove(collection);
                            }
                        }
                        foreach (var emptyCollection in collectionDbs)
                        {
                            EntryCollection entryCollection = EntryCollection.Create(emptyCollection);
                            entryCollections.Add(entryCollection);
                        }
                    }
                    else
                    {
                        foreach (var collection in collectionDbs)
                        {
                            EntryCollection entryCollection = EntryCollection.Create(collection);
                            entryCollections.Add(entryCollection);
                        }
                    }
                });
                return entryCollections;
            }
            else
            {
                return null;
            }
        }

        public static void AddCollection(EntryCollectionDb entryCollectionDb)
        {
            if (IsLocalDbValid())
            {
                if (string.IsNullOrWhiteSpace(entryCollectionDb.Id))
                {
                    entryCollectionDb.Id = Guid.NewGuid().ToString();
                }
                DbService.ConfigDb.Insertable(entryCollectionDb).ExecuteCommand();
            }
        }
        public static void RemoveCollection(string collectionId)
        {
            if (IsLocalDbValid())
            {
                DbService.ConfigDb.Deleteable<EntryCollectionDb>(collectionId).ExecuteCommand();
            }
        }
        public static void AddCollectionItem(EntryCollectionItemDb item)
        {
            if (IsLocalDbValid())
            {
                DbService.ConfigDb.Insertable(item).ExecuteCommand();
            }
        }
        public static bool RemoveCollectionItem(List<string> entryCollectionItemIds)
        {
            if (IsLocalDbValid())
            {
                return DbService.ConfigDb.Deleteable<EntryCollectionItemDb>().In(entryCollectionItemIds).ExecuteCommand() > 0;
            }
            else
            {
                return false;
            }
        }
        public static bool RemoveCollectionItem(string entryCollectionItemId)
        {
            if (IsLocalDbValid())
            {
                return DbService.ConfigDb.Deleteable<EntryCollectionItemDb>().In(entryCollectionItemId).ExecuteCommand() > 0;
            }
            else
            {
                return false;
            }
        }


        public static void UpdateCollection(EntryCollectionDb entryCollectionDb)
        {
            if (IsLocalDbValid())
            {
                DbService.ConfigDb.Updateable(entryCollectionDb).ExecuteCommand();
            }
        }

        public static async Task<EntryCollectionItemDb> QueryFirstAsync(string collectionId, string entryId)
        {
            return await DbService.ConfigDb.Queryable<EntryCollectionItemDb>().FirstAsync(p => p.CollectionId == collectionId && p.EntryId == entryId);
        }

        public static EntryCollectionItemDb QueryFirst(string collectionId, string entryId)
        {
            return DbService.ConfigDb.Queryable<EntryCollectionItemDb>().First(p => p.CollectionId == collectionId && p.EntryId == entryId);
        }
    }
}
