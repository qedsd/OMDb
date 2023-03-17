﻿using OMDb.Core.DbModels;
using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Services
{
    public static class EntryLabelService
    {
        /// <summary>
        /// 查詢所有Entry&Label對應關係
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static List<EntryLabelLKDb> SelectAllEntryLabel()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"select * from EntryLabel ");
            return DbService.LocalDb.Ado.SqlQuery<EntryLabelLKDb>(sb.ToString());
        }


        /// <summary>
        /// 查詢所有Entry&Label對應關係
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static List<EntryLabelLKDb> SelectAllEntryLabel(string dbId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($@"select * from EntryLabel where DbId='{dbId}'");
            return DbService.LocalDb.Ado.SqlQuery<EntryLabelLKDb>(sb.ToString());
        }



        /// <summary>
        /// 添加Entry&Label 关联关系
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        public static void AddEntryLabel(EntryLabelLKDb eldb)
        {
            DbService.LocalDb.Insertable<EntryLabelLKDb>(eldb).ExecuteCommand();
        }
    }
}