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
using OMDb.Core.Utils;
using Newtonsoft.Json;
using NPOI.XWPF.UserModel;

namespace OMDb.JavDb
{
    [Export(typeof(IEntryInfo))]
    public class EntryInfo : IEntryInfo
    {
        Dictionary<string, object> IEntryInfo.GetEntryInfoNet(string keyword)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                HtmlWeb htmlWeb = new HtmlWeb();
                var lstIgno = new List<string>() { "TokyoHot-", "Carib-", "Heyzo-" };

                foreach (var item in lstIgno)
                {
                    if (keyword.StartsWith(item))
                        keyword = keyword.Replace(item, string.Empty);
                }

                var name = keyword;
                var url_Sereach = ($"https://javdb.com/search?q={name}&sb=0");
                HtmlDocument htmlDoc_Sereach = htmlWeb.Load(url_Sereach);
                var url = htmlDoc_Sereach.DocumentNode.SelectSingleNode(@"/html/body/section/div/div[6]/div[1]/a").Attributes["href"].Value;
                HtmlDocument htmlDoc = htmlWeb.Load($"https://javdb.com/{url}");
                GetCover(htmlDoc, ref dic);//封面


                var infoStr = htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/section/div/div[4]/div[1]/div/div[2]/nav").InnerText;
                infoStr = infoStr.Replace(" ", string.Empty).Replace("\n", string.Empty).Replace("&nbsp;", string.Empty).Replace(":", string.Empty);
                GetDic(infoStr, ref dic);

            }
            catch (Exception ex)
            {
                Logger.Error("JavDb获取信息失败！" + ex);
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
                var cover = htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/section/div/div[4]/div[1]/div/div[1]/a/img").Attributes["src"].Value;
                var stream = CoverCrop(cover);
                dic.Add("封面", stream);
            }
            catch (Exception ex)
            {
                Logger.Error("封面获取失败！" + ex);
            }
        }

        private void GetDic(string infoStr, ref Dictionary<string, object> dic)
        {
            List<string> lst = new List<string> { "日期", "片商", "發行", "系列", "導演", "評分", "類別", "演員" };
            List<Tuple<string, int>> lstTup = new List<Tuple<string, int>>();
            foreach (string str in lst)
            {
                if (infoStr.Contains(str))
                {
                    var startIndex = infoStr.IndexOf(str, StringComparison.OrdinalIgnoreCase);
                    var tup = new Tuple<string, int>(str, startIndex);
                    lstTup.Add(tup);
                }
            }
            lstTup.Sort((Tuple<string, int> t1, Tuple<string, int> t2) =>
            {
                return t1.Item2 - t2.Item2;
            });
            var arrTup = lstTup.ToArray();
            for (int i = 0; i < lstTup.Count; i++)
            {
                if (arrTup[i].Item1.Equals("日期"))
                {
                    var startIndex = arrTup[i].Item2 + arrTup[i].Item1.Length;
                    var lenth = 10;
                    var result = Convert.ToDateTime(infoStr.Substring(startIndex, lenth));
                    //dic.Add(arrTup[i].Item1, result);
                    dic.Add("上映日期", result);
                }
                else if (arrTup[i].Item1.Equals("評分"))
                {
                    var startIndex = arrTup[i].Item2 + arrTup[i].Item1.Length;
                    var lenth = 4;
                    var result = Convert.ToDouble(infoStr.Substring(startIndex, lenth).Replace("分", string.Empty).Replace(",", string.Empty));
                    //dic.Add(arrTup[i].Item1, result);
                    dic.Add("评分", result);
                }
                else if (i < lstTup.Count - 1)
                {
                    var startIndex = arrTup[i].Item2 + arrTup[i].Item1.Length;
                    var lenth = arrTup[i + 1].Item2 - arrTup[i].Item2 - arrTup[i].Item1.Length;
                    var resultStr = infoStr.Substring(startIndex, lenth);
                    var result = resultStr.Split(',');
                    dic.Add(arrTup[i].Item1, result);
                }
                else
                {
                    var startIndex = arrTup[i].Item2 + arrTup[i].Item1.Length;
                    var endIndex = (infoStr.Contains("No.")) ? infoStr.IndexOf("No.") : infoStr.IndexOf("想看");
                    var lenth = endIndex - arrTup[i].Item2 - arrTup[i].Item1.Length;
                    var resultStr = infoStr.Substring(startIndex, lenth);
                    string[] spreator = new string[1] { "♀" };
                    var result = resultStr.Split(spreator, StringSplitOptions.RemoveEmptyEntries).ToList();
                    result.RemoveAt(result.Count - 1);
                    var arrResult = result.ToArray();
                    dic.Add(arrTup[i].Item1, arrResult);
                }
            }

        }

        public MemoryStream CoverCrop(string path)
        {
            var bytes = GetUrlMemoryStream(path);
            using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(bytes))
            {
                var size = image.Size;
                var l = 19 * (size.Width / 40);
                var t = 0;
                var r = size.Width;
                var b = size.Height;

                image.Mutate(x => x.Crop(Rectangle.FromLTRB(l, t, r, b)));

                MemoryStream stream = new MemoryStream();
                image.Save(stream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }

        /*private MemoryStream GetUrlMemoryStream(string path)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            return (MemoryStream)responseStream;
        }*/



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
