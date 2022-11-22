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
                        var dic = group.ToDictionary(p => p.Key);//key为EntryCollectionDb.Id
                        foreach (var item in group)
                        {
                            var collection = collectionDbs.FirstOrDefault(p => p.Id == item.Key);
                            if(collection != null)
                            {
                                EntryCollection entryCollection = EntryCollection.Create(collection);
                                entryCollection.Items = new List<EntryCollectionItemDb>(item);
                                entryCollections.Add(entryCollection);
                                collectionDbs.Remove(collection);
                            }
                        }
                        foreach(var emptyCollection in collectionDbs)
                        {
                            EntryCollection entryCollection = EntryCollection.Create(emptyCollection);
                            entryCollections.Add(entryCollection);
                        }
                        //foreach (var collection in collectionDbs)
                        //{
                        //    if (dic.TryGetValue(collection.Id, out var g))
                        //    {
                        //        EntryCollection entryCollection = EntryCollection.Create(collection);
                        //        entryCollection.Items = new List<EntryCollectionItemDb>(g);
                        //        entryCollections.Add(entryCollection);
                        //    }
                        //}
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
                if(string.IsNullOrWhiteSpace(entryCollectionDb.Id))
                {
                    entryCollectionDb.Id = Guid.NewGuid().ToString();
                }
                DbService.LocalDb.Insertable(entryCollectionDb).ExecuteCommand();
            }
        }
        public static void RemoveCollection(string key)
        {
            if (IsLocalDbValid())
            {
                DbService.LocalDb.Deleteable<EntryCollectionDb>(key).ExecuteCommand();
            }
        }
        public static void AddCollectionItem(EntryCollectionItemDb item)
        {
            if (IsLocalDbValid())
            {
                DbService.LocalDb.Insertable(item).ExecuteCommand();
            }
        }
        public static void RemoveCollectionItem(string key)
        {
            if (IsLocalDbValid())
            {
                DbService.LocalDb.Deleteable<EntryCollectionDb>(key).ExecuteCommand();
            }
        }

        public static void UpdateCollection(EntryCollectionDb entryCollectionDb)
        {
            if (IsLocalDbValid())
            {
                DbService.LocalDb.Updateable(entryCollectionDb).ExecuteCommand();
            }
        }

        public static async Task<EntryCollectionItemDb> QueryFirstAsync(string collectionId,string entryId)
        {
            return await DbService.LocalDb.Queryable<EntryCollectionItemDb>().FirstAsync(p=>p.CollectionId == collectionId && p.EntryId == entryId);
        }

        public static EntryCollectionItemDb QueryFirst(string collectionId, string entryId)
        {
            return DbService.LocalDb.Queryable<EntryCollectionItemDb>().First(p => p.CollectionId == collectionId && p.EntryId == entryId);
        }
    }
}
