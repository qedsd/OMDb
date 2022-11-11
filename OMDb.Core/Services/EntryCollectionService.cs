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
            return DbService.LocalDb != null;
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
                var collectionDbs = await DbService.LocalDb.Queryable<EntryCollectionDb>().ToListAsync();
                var items = await DbService.LocalDb.Queryable<EntryCollectionItemDb>().ToListAsync();
                await Task.Run(() =>
                {
                    var group = items.GroupBy(p => p.CollectionId).ToList();
                    if (group != null && group.Count > 0)
                    {
                        var dic = group.ToDictionary(p => p.Key);
                        foreach (var collection in collectionDbs)
                        {
                            if (dic.TryGetValue(collection.Id, out var g))
                            {
                                EntryCollection entryCollection = EntryCollection.Create(collection);
                                entryCollection.Items = new List<EntryCollectionItemDb>(g);
                                entryCollections.Add(entryCollection);
                            }
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
                DbService.LocalDb.Insertable(entryCollectionDb);
            }
        }

        public static void AddCollectionItem(EntryCollectionItemDb item)
        {
            if (IsLocalDbValid())
            {
                DbService.LocalDb.Insertable(item);
            }
        }
    }
}
