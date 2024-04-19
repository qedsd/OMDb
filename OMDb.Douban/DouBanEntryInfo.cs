﻿using HtmlAgilityPack;
using OMDb.Core.Interfaces;
using OMDb.Core.Models;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OMDb.Core.Services;
using OMDb.Core.Utils;
using Newtonsoft.Json;
using NPOI.XWPF.UserModel;

namespace OMDb.Douban
{
    [Export(typeof(IEntryInfo))]
    public class DouBanEntryInfo : IEntryInfo
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        string h5Line = "<br>";
        Dictionary<string, object> IEntryInfo.GetEntryInfoNet(string keyword)
        {
            try
            {
                dic.Clear();
                HtmlWeb htmlWeb = new HtmlWeb();
                var name = keyword;
                var url_Sereach = ($"https://www.douban.com/search?cat=1002&q={name}");
                HtmlDocument htmlDoc_Sereach = htmlWeb.Load(url_Sereach);

                //搜索到的第一个
                var url = htmlDoc_Sereach.DocumentNode.SelectSingleNode(@"//*[@id=""content""]/div/div[1]/div[3]/div/div[1]/div[2]/div/h3/a").Attributes["href"].Value;
                HtmlDocument htmlDoc = htmlWeb.Load(url);
                GetActor(htmlDoc);//获取主演
                GetRate(htmlDoc);//获取评分
                GetCover(htmlDoc);//获取封面
                GetDirector(htmlDoc);//获取导演
                GetDate(htmlDoc);//获取日期
                GetClass(htmlDoc);//获取分类
                GetDescription(htmlDoc);//获取描述
            }
            catch (Exception ex)
            {
                Logger.Error("获取信息失败" + ex);
            }
            return dic;

        }


        /// <summary>
        /// 获取封面
        /// </summary>
        /// <param name="htmlDoc"></param>
        /// <param name="dic"></param>
        private void GetCover(HtmlDocument htmlDoc)
        {
            try
            {
                var cover = htmlDoc.DocumentNode.SelectSingleNode(@"//*[@id=""mainpic""]/a/img").Attributes["src"].Value;
                var bytes = GetUrlMemoryStream(cover);
                MemoryStream ms = new MemoryStream(bytes);
                dic.Add("封面", ms);
            }
            catch (Exception ex)
            {
                Logger.Error("封面获取失败" + ex);
            }
        }

        /// <summary>
        /// 获取评分
        /// </summary>
        /// <param name="htmlDoc"></param>
        /// <param name="dic"></param>
        private void GetRate(HtmlDocument htmlDoc)
        {
            try
            {
                var rate = Convert.ToDouble(htmlDoc.DocumentNode.SelectSingleNode(@"//*[@id=""interest_sectl""]/div/div[2]/strong").InnerText);
                dic.Add("评分", rate / 2.0);
            }
            catch (Exception ex)
            {
                Logger.Error("评分获取失败" + ex);
            }
        }

        /// <summary>
        /// 获取主演
        /// </summary>
        /// <param name="htmlDoc"></param>
        /// <param name="dic"></param>
        private void GetActor(HtmlDocument htmlDoc)
        {
            try
            {
                var actorStr = htmlDoc.DocumentNode.SelectSingleNode(@"//*[@id=""info""]/span[3]").InnerText;
                var actorStrSub = actorStr.Remove("主演:").Remove(" ");
                var actorArray = actorStrSub.Split("/");
                dic.Add("主演", actorArray);
            }
            catch (Exception ex)
            {
                Logger.Error("主演获取失败" + ex);
            }
        }


        private void GetDirector(HtmlDocument htmlDoc)
        {
            try
            {
                var directorStr = htmlDoc.DocumentNode.SelectSingleNode(@"//*[@id=""info""]/span[1]").InnerText;
                var directorStrSub = directorStr.Remove("导演:").Remove(" ");
                var directorArray = directorStrSub.Split("/");
                dic.Add("导演", directorArray);
            }
            catch (Exception ex)
            {
                Logger.Error("导演获取失败" + ex);
            }
        }

        private void GetDate(HtmlDocument htmlDoc)
        {
            try
            {
                var InfoStr = htmlDoc.DocumentNode.SelectSingleNode(@"//*[@id=""info""]").InnerText;
                if (InfoStr.Contains("上映日期:"))
                {
                    var ist = InfoStr.Replace(" ", "").Replace("\n", "");
                    var stratIndex = ist.IndexOf("上映日期:") + "上映日期:".Length;
                    var dateLength = 10;
                    var date = Convert.ToDateTime(ist.Substring(stratIndex, 10));
                    dic.Add("上映日期", date);
                }
                if (InfoStr.Contains("首播: "))
                {
                    var ist = InfoStr.Replace(" ", "").Replace("\n", "");
                    var stratIndex = ist.IndexOf("首播:") + "首播:".Length;
                    var lenth = 10;
                    var date = Convert.ToDateTime(ist.Substring(stratIndex, lenth));
                    dic.Add("上映日期", date);
                }
            }
            catch (Exception ex)
            {
                Logger.Info("上映日期获取失败" + ex);
            }
        }

        private void GetClass(HtmlDocument htmlDoc)
        {
            try
            {
                var InfoStr = htmlDoc.DocumentNode.SelectSingleNode(@"//*[@id=""info""]").InnerText;
                if (InfoStr.Contains("类型:"))
                {
                    var ist = InfoStr.Remove(" ").Remove("\n");
                    var stratIndex = ist.IndexOf("类型:") + "类型:".Length;
                    var endIndex = ist.IndexOf("制片国家");
                    var lenth = endIndex - stratIndex;
                    var classStr = ist.Substring(stratIndex, lenth);
                    var classArray = classStr.Split('/');
                    dic.Add("分类", classArray);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("分类获取失败" + ex);
            }
        }

        private void GetDescription(HtmlDocument htmlDoc)
        {
            try
            {
                var xPath = @"//*[@id=""link-report-intra""]/span[2]";
                var InfoStr = htmlDoc.DocumentNode.SelectSingleNode(xPath).InnerText;
                var result=InfoStr.Replace(h5Line, Environment.NewLine);
                dic.Add("描述", result);
            }
            catch (Exception ex)
            {
                Logger.Error("描述获取失败" + ex);
            }
        }

        public static byte[] GetUrlMemoryStream(string path)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();

            List<byte> btlst = new List<byte>();
            int b = responseStream.ReadByte();
            while (b > -1)
            {
                btlst.Add((byte)b);
                b = responseStream.ReadByte();
            }
            byte[] bts = btlst.ToArray();
            return bts;
        }
    }
}
