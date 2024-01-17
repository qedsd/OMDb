using HtmlAgilityPack;
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
    [Export(typeof(IEntryInfoDescription))]
    public class DouBanEntryInfoDescription : IEntryInfoDescription
    {
        string h5Line = "<br>";
        string IEntryInfoDescription.GetEntryInfoDescriptionNet(string keyword)
        {
            try
            {
                HtmlWeb htmlWeb = new HtmlWeb();
                var name = keyword;
                var url_Sereach = ($"https://www.douban.com/search?cat=1002&q={name}");
                HtmlDocument htmlDoc_Sereach = htmlWeb.Load(url_Sereach);

                //搜索到的第一个
                var url = htmlDoc_Sereach.DocumentNode.SelectSingleNode(@"//*[@id=""content""]/div/div[1]/div[3]/div/div[1]/div[2]/div/h3/a").Attributes["href"].Value;
                HtmlDocument htmlDoc = htmlWeb.Load(url);
                return GetDescription(htmlDoc);//获取描述
            }
            catch (Exception ex)
            {
                Logger.Error("获取信息失败" + ex);
                return string.Empty;
            }
        }


        private string GetDescription(HtmlDocument htmlDoc)
        {
            try
            {
                var xPath = @"//*[@id=""link-report-intra""]/span[1]";
                var InfoStr = htmlDoc.DocumentNode.SelectSingleNode(xPath).InnerText;
                var result=InfoStr.Replace(h5Line,"");
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error("描述获取失败" + ex);
                return string.Empty;
            }
        }


    }
}
