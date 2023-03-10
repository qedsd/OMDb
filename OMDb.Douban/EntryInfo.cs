using HtmlAgilityPack;
using OMDb.Core.Helpers;
using OMDb.Core.Interfaces;
using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Douban
{
    [Export(typeof(IEntryInfo))]
    public class EntryInfo : IEntryInfo
    {
        Dictionary<string, object> IEntryInfo.GetEntryInfo(string keyword)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                HtmlWeb htmlWeb = new HtmlWeb();
                var name = keyword;
                var url_Sereach = ($"https://www.douban.com/search?q={name}");
                HtmlDocument htmlDoc_Sereach = htmlWeb.Load(url_Sereach);
                var url = htmlDoc_Sereach.DocumentNode.SelectSingleNode(@"//*[@id=""content""]/div/div[1]/div[3]/div[2]/div[1]/div[2]/div/h3/a").Attributes["href"].Value;
                HtmlDocument htmlDoc = htmlWeb.Load(url);
                GetActor(htmlDoc, ref dic);//主演
                GetRate(htmlDoc, ref dic);//评分
                GetCover(htmlDoc, ref dic);//封面
            }
            catch (Exception ex)
            {
                LogHelper.Instance._logger.Error("获取信息失败" + ex);
            }
            return dic;

        }


        /// <summary>
        /// 获取封面
        /// </summary>
        /// <param name="htmlDoc"></param>
        /// <param name="dic"></param>
        private void GetCover(HtmlDocument htmlDoc, ref Dictionary<string, object> dic)
        {
            try
            {
                var cover = htmlDoc.DocumentNode.SelectSingleNode(@"//*[@id=""mainpic""]/a/img").Attributes["src"].Value;
                dic.Add("封面", cover);
            }
            catch (Exception ex)
            {
                LogHelper.Instance._logger.Error("封面获取失败" + ex);
            }
        }

        /// <summary>
        /// 获取评分
        /// </summary>
        /// <param name="htmlDoc"></param>
        /// <param name="dic"></param>
        private void GetRate(HtmlDocument htmlDoc, ref Dictionary<string, object> dic)
        {
            try
            {
                var rate = htmlDoc.DocumentNode.SelectSingleNode(@"//*[@id=""interest_sectl""]/div/div[2]/strong").InnerText;
                dic.Add("评分", rate);
            }
            catch (Exception ex)
            {
                LogHelper.Instance._logger.Error("评分获取失败" + ex);
            }
        }

        /// <summary>
        /// 获取主演
        /// </summary>
        /// <param name="htmlDoc"></param>
        /// <param name="dic"></param>
        private void GetActor(HtmlDocument htmlDoc, ref Dictionary<string, object> dic)
        {
            try
            {
                var actorStr = htmlDoc.DocumentNode.SelectSingleNode(@"//*[@id=""info""]/span[3]").InnerText;
                var actorStrSub = actorStr.Replace("主演: ", string.Empty);
                var actorArray = actorStrSub.Split("/");
                dic.Add("主演", actorArray);
            }
            catch (Exception ex)
            {
                LogHelper.Instance._logger.Error("主演获取失败" + ex);
            }
        }



    }
}
