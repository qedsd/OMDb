using HtmlAgilityPack;
using OMDb.Core.Interfaces;
using OMDb.Core.Models;
using OMDb.Core.Utils;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Douban
{
    [Export(typeof(IRate))]
    public class Rate : IRate
    {

        Rating IRate.Rate(string id)
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            var name = id;
            var url_Sereach = ($"https://www.douban.com/search?q={name}");
            HtmlDocument htmlDoc_Sereach = htmlWeb.Load(url_Sereach);
            var url = htmlDoc_Sereach.DocumentNode.SelectSingleNode(@"//*[@id=""content""]/div/div[1]/div[3]/div[2]/div[1]/div[2]/div/h3/a").Attributes["href"].Value;
            HtmlDocument htmlDoc = htmlWeb.Load(url);
            var rate =GetRate(htmlDoc);//评分
            Rating rt = new Rating(rate,10);
            return rt;
        }

        private double GetRate(HtmlDocument htmlDoc)
        {
            try
            {
                return Convert.ToDouble(htmlDoc.DocumentNode.SelectSingleNode(@"//*[@id=""interest_sectl""]/div/div[2]/strong").InnerText);

            }
            catch (Exception ex)
            {
                Logger.Error("评分获取失败" + ex);
                return 0;
            }
        }
    }
}
