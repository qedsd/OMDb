using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Services;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI;
using System.Xml.Linq;
using Microsoft.UI.Xaml;
using System.IO;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using OMDb.WinUI3.Services.Settings;
using OMDb.Core.Utils.Extensions;
using OMDb.Core.DbModels;

namespace OMDb.WinUI3.ViewModels
{
    public partial class LabelViewModel : ObservableObject
    {
        private void GetTag1stInfo()
        {
            IEnumerable<XElement> t1Color = from element in xe.Elements("Color1st") select element;
            var value_T1R = Convert.ToByte(t1Color.First().Element("ColorR").Value);
            var value_T1G = Convert.ToByte(t1Color.First().Element("ColorG").Value);
            var value_T1B = Convert.ToByte(t1Color.First().Element("ColorB").Value);
            Color1st = Windows.UI.Color.FromArgb(255, value_T1R, value_T1G, value_T1B);
            FontSize1st = Convert.ToDouble(xe.Element("FontSize1st").Value);
            Width1st = Convert.ToDouble(xe.Element("Width1st").Value);
            Height1st = Convert.ToDouble(xe.Element("Height1st").Value);
        }
        private void GetTag2ndInfo()
        {
            IEnumerable<XElement> t2Color = from element in xe.Elements("Color2nd") select element;
            var value_T2R = Convert.ToByte(t2Color.First().Element("ColorR").Value);
            var value_T2G = Convert.ToByte(t2Color.First().Element("ColorG").Value);
            var value_T2B = Convert.ToByte(t2Color.First().Element("ColorB").Value);
            Color2nd = Windows.UI.Color.FromArgb(255, value_T2R, value_T2G, value_T2B);
            FontSize2nd = Convert.ToDouble(xe.Element("FontSize2nd").Value);
            Width2nd = Convert.ToDouble(xe.Element("Width2nd").Value);
            Height2nd = Convert.ToDouble(xe.Element("Height2nd").Value);
        }
        private void SaveXml()
        {
            IEnumerable<XElement> t1Color = from element in xe.Elements("Color1st") select element;
            IEnumerable<XElement> t2Color = from element in xe.Elements("Color2nd") select element;
            xe.Element("FontSize1st").Value = FontSize1st.ToString();
            xe.Element("FontSize2nd").Value = FontSize2nd.ToString();
            xe.Element("Width2nd").Value = Width2nd.ToString();
            xe.Element("Height2nd").Value = Height2nd.ToString();
            xe.Element("Width1st").Value = Width1st.ToString();
            xe.Element("Height1st").Value = Height1st.ToString();
            xe.Element("FontFamily").Value = FontFamilyCurrent;
            t1Color.First().Element("ColorR").Value = Color1st.R.ToString();
            t1Color.First().Element("ColorG").Value = Color1st.G.ToString();
            t1Color.First().Element("ColorB").Value = Color1st.B.ToString();
            t2Color.First().Element("ColorR").Value = Color2nd.R.ToString();
            t2Color.First().Element("ColorG").Value = Color2nd.G.ToString();
            t2Color.First().Element("ColorB").Value = Color2nd.B.ToString();
            xe.Save(ConfigPath);
            xe = XElement.Load(ConfigPath);
        }

        private void RemoveLabelClass(LabelClassTree item)
        {
            Core.Services.LabelClassService.RemoveLabelClass(item.LabelClass.LabelClassDb.LCID);
            if (item.LabelClass.LabelClassDb.ParentId != null)//子类
            {
                var parent = LabelTrees.FirstOrDefault(p => p.LabelClass.LabelClassDb.LCID == item.LabelClass.LabelClassDb.ParentId);
                if (parent != null)
                {
                    var removeWhere = parent.Children.FirstOrDefault(t => t == item);
                    var index = parent.Children.IndexOf(removeWhere);
                    parent.Children.Remove(removeWhere);
                }
            }
            else//父类
            {
                InitAsync();
            }
        }

        private async void AddLabelClass(string path)
        {
            //var labelClassDbs = await Core.Services.LabelClassService.GetAllLabelClassAsync(DbSelectorService.dbCurrentId);
            if (System.IO.File.Exists(path))
            {
                string json = System.IO.File.ReadAllText(path);
                var collection = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
                var labelClassDbList = new List<LabelClassDb>();
                foreach (var item in collection)
                {
                    var property = item["Property"] as string;
                    var data = JsonConvert.DeserializeObject<List<string>>(item["Data"].ToString());
                    if (this.LabelTrees.Select(a => a.LabelClass.Name).Contains(property))
                    {
                        var labelClass = this.LabelTrees.Where(a => a.LabelClass.Name == property).FirstOrDefault();
                        var labelClassData = labelClass.Children.Select(a => a.LabelClass.Name).ToArray();
                        var parentId = labelClass.LabelClass.LabelClassDb.LCID;
                        foreach (var propertyData in data)
                        {
                            if (!labelClassData.Contains(propertyData))
                            {
                                var labelClassDataDb = new LabelClassDb()
                                {
                                    Name = propertyData,
                                    ParentId = parentId,
                                    DbCenterId = DbSelectorService.dbCurrentId,
                                    Level = 1
                                };
                                labelClassDbList.Add(labelClassDataDb);
                            }
                        }
                    }
                    else
                    {
                        var parentId = Guid.NewGuid().ToString();
                        var labelClassDb = new LabelClassDb()
                        {
                            LCID = parentId,
                            Name = property,
                            ParentId = null,
                            DbCenterId = DbSelectorService.dbCurrentId,
                            Level = 1
                        };
                        labelClassDbList.Add(labelClassDb);
                        foreach (var propertyData in data)
                        {
                            var labelClassDataDb = new LabelClassDb()
                            {
                                Name = propertyData,
                                ParentId = parentId,
                                DbCenterId = DbSelectorService.dbCurrentId,
                                Level = 1
                            };
                            labelClassDbList.Add(labelClassDataDb);
                        }
                    }
                }
                Core.Services.LabelClassService.AddLabelClass(labelClassDbList);
            }
        }

        private string GetJson()
        {
            var result = new List<Object>();
            foreach (var item in this.LabelTrees)
            {
                var labelPropertyData = new List<string>();
                foreach (var data in item.Children)
                {
                    labelPropertyData.Add(data.LabelClass.Name);
                }
                var labelProperty = new
                {
                    Class = item.LabelClass.Name,
                    Data = labelPropertyData,
                };
                result.Add(labelProperty);
            }
            JsonSerializerSettings settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
            string json = JsonConvert.SerializeObject(result, settings);
            return json;
        }
    }
}
