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

namespace OMDb.WinUI3.ViewModels
{
    public class LabelViewModel : ObservableObject
    {
        //Propriety

        XElement xe = null;
        string ConfigPath = null;
        # region 字段
        private ObservableCollection<LabelClassTree> labelTrees;
        public ObservableCollection<LabelClassTree> LabelTrees
        {
            get => labelTrees;
            set => SetProperty(ref labelTrees, value);
        }

        private bool isTreeShow;
        public bool IsTreeShow
        {
            get => isTreeShow;
            set => SetProperty(ref isTreeShow, value);
        }

        private bool isRepeaterShow;
        public bool IsRepeaterShow
        {
            get => isRepeaterShow;
            set => SetProperty(ref isRepeaterShow, value);
        }

        private bool isExpShow;
        public bool IsExpShow
        {
            get => isExpShow;
            set => SetProperty(ref isExpShow, value);
        }

        private double _fontSizeCurrent = 18;
        public double FontSizeCurrent
        {
            get => _fontSizeCurrent;
            set
            {
                SetProperty(ref _fontSizeCurrent, value);
                if (TagSelector)
                {
                    FontSize1st = _fontSizeCurrent;
                }
                else
                {
                    FontSize2nd = _fontSizeCurrent;
                }
            }
        }

        private double _fontSize1st;
        public double FontSize1st
        {
            get => _fontSize1st;
            set => SetProperty(ref _fontSize1st, value);
        }

        private double _fontSize2nd;
        public double FontSize2nd
        {
            get => _fontSize2nd;
            set => SetProperty(ref _fontSize2nd, value);
        }

        private string _fontFamilyCurrent;
        public string FontFamilyCurrent
        {
            get => _fontFamilyCurrent;
            set => SetProperty(ref _fontFamilyCurrent, value);
        }

        private double _widthCurrent = 18;
        public double WidthCurrent
        {
            get => _widthCurrent;
            set
            {
                SetProperty(ref _widthCurrent, value);
                if (TagSelector)
                {
                    Width1st = _widthCurrent;
                }
                else
                {
                    Width2nd = _widthCurrent;
                }
            }
        }

        private double _width1st;
        public double Width1st
        {
            get => _width1st;
            set => SetProperty(ref _width1st, value);
        }

        private double _width2nd;
        public double Width2nd
        {
            get => _width2nd;
            set => SetProperty(ref _width2nd, value);
        }

        private double _heightCurrent = 18;
        public double HeightCurrent
        {
            get => _heightCurrent;
            set
            {
                SetProperty(ref _heightCurrent, value);
                if (TagSelector)
                {
                    Height1st = _heightCurrent;
                }
                else
                {
                    Height2nd = _heightCurrent;
                }
            }
        }

        private double _height1st;
        public double Height1st
        {
            get => _height1st;
            set => SetProperty(ref _height1st, value);
        }

        private double _height2nd;
        public double Height2nd
        {
            get => _height2nd;
            set => SetProperty(ref _height2nd, value);
        }

        private Windows.UI.Color _colorCurrent;
        public Windows.UI.Color ColorCurrent
        {
            get => _colorCurrent;
            set
            {
                SetProperty(ref _colorCurrent, value);
                if (TagSelector)
                {
                    Color1st = _colorCurrent;
                }
                else
                {
                    Color2nd = _colorCurrent;
                }
            }
        }

        private Windows.UI.Color _color1st;
        public Windows.UI.Color Color1st
        {
            get => _color1st;
            set => SetProperty(ref _color1st, value);
        }

        private Windows.UI.Color _color2nd;
        public Windows.UI.Color Color2nd
        {
            get => _color2nd;
            set => SetProperty(ref _color2nd, value);
        }


        private System.Drawing.Brush _brush;
        public System.Drawing.Brush Brush
        {
            get => _brush;
            set => SetProperty(ref _brush, value);
        }



        private bool _tagSelector = true;
        public bool TagSelector
        {
            get => _tagSelector;
            set => SetProperty(ref _tagSelector, value);
        }

        #endregion



        //初始化
        private async void Init()
        {
            ConfigPath = System.AppDomain.CurrentDomain.BaseDirectory + @"Assets/Config/LabelConfig.xml";
            xe = XElement.Load(ConfigPath);

            var labelTrees = await CommonService.GetLabelClassTrees();

            Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
               LabelTrees = labelTrees.ToObservableCollection();
            });


            GetTag1stInfo();
            GetTag2ndInfo();
            ColorCurrent = TagSelector ? Color1st : Color2nd;
            FontSizeCurrent = TagSelector ? FontSize1st : FontSize2nd;
            WidthCurrent = TagSelector ? Width1st : Width2nd;
            HeightCurrent = TagSelector ? Height1st : Height2nd;
            FontFamilyCurrent = Convert.ToString(xe.Element("FontFamily").Value);
        }

        public LabelViewModel()
        {
            IsExpShow = false;
            IsTreeShow = false;
            IsRepeaterShow = true;
            Init();
        }

        public ICommand TestMeCommand => new RelayCommand(() =>
        {
            Init();
        });
        public ICommand ChangeShowTypeToExpCommand => new RelayCommand(() =>
        {
            IsExpShow = true;
            IsTreeShow = false;
            IsRepeaterShow = false;
        });

        public ICommand SetFontFamilyCommand => new RelayCommand<string>((string font) =>
        {
            FontFamilyCurrent = font;
        });

        public ICommand ChangeShowTypeToTreeCommand => new RelayCommand(() =>
        {
            IsExpShow = false;
            IsTreeShow = true;
            IsRepeaterShow = false;
        });

        public ICommand ChangeShowTypeToRepeaterCommand => new RelayCommand(() =>
        {
            IsExpShow = false;
            IsTreeShow = false;
            IsRepeaterShow = true;
        });




        public ICommand RefreshCommand => new RelayCommand(async () =>
        {
            Init();
            Helpers.InfoHelper.ShowSuccess("刷新完成");
        });
        public ICommand AddRootCommand => new RelayCommand(async () =>
        {
            var result = await Dialogs.EditLabelDialog.ShowDialog(true);
            if (result != null)
            {
                if (string.IsNullOrEmpty(result.Name))
                {
                    Helpers.InfoHelper.ShowError("标签名不可为空");
                }
                else
                {
                    Core.Services.LabelClassService.AddLabel(result);
                    LabelTrees.Add(new LabelClassTree(result));
                    Helpers.InfoHelper.ShowSuccess("已保存标签");
                }
            }
        });
        public ICommand AddSubCommand => new RelayCommand<LabelClassTree>(async (parent) =>
        {
            var result = await Dialogs.EditLabelDialog.ShowDialog(false);
            if (result != null)
            {
                if (string.IsNullOrEmpty(result.Name))
                {
                    Helpers.InfoHelper.ShowError("标签名不可为空");
                }
                else
                {
                    result.ParentId = parent.LabelClass.LabelClassDb.LCId;
                    Core.Services.LabelClassService.AddLabel(result);
                    parent.Children.Add(new LabelClassTree(result));
                    Helpers.InfoHelper.ShowSuccess("已保存标签");
                }
            }
        });

        /// <summary>
        /// 编辑二级标签 Edit 2nd Tag 
        /// </summary>
        public ICommand EditSubCommand => new RelayCommand<LabelClassTree>(async (item) =>
        {
            if (item != null)
            {
                var result = await Dialogs.EditLabelDialog.ShowDialog(false, item.LabelClass.LabelClassDb);
                if (result != null)
                {
                    if (string.IsNullOrEmpty(result.Name))
                    {
                        Helpers.InfoHelper.ShowError("标签名不可为空");
                    }
                    else
                    {
                        Core.Services.LabelClassService.UpdateLabel(result);
                        var parent = LabelTrees.FirstOrDefault(p => p.LabelClass.LabelClassDb.LCId == result.ParentId);
                        if (parent != null)
                        {
                            var removeWhere = parent.Children.FirstOrDefault(t => t.LabelClass.LabelClassDb == result);
                            var index = parent.Children.IndexOf(removeWhere);
                            parent.Children.Remove(removeWhere);
                            parent.Children.Insert(index, new LabelClassTree(result));
                        }
                        Helpers.InfoHelper.ShowSuccess("已保存标签");
                    }
                }
            }
        });

        /// <summary>
        /// 编辑一级标签 Edit 1st Tag
        /// </summary>
        public ICommand EditRootCommand => new RelayCommand<LabelClassTree>(async (item) =>
        {
            if (item != null)
            {
                var result = await Dialogs.EditLabelDialog.ShowDialog(true, item.LabelClass.LabelClassDb);
                if (result != null)
                {
                    if (string.IsNullOrEmpty(result.Name))
                    {
                        Helpers.InfoHelper.ShowError("标签名不可为空");
                    }
                    else
                    {
                        Core.Services.LabelClassService.UpdateLabel(result);
                        var index = LabelTrees.IndexOf(item);
                        LabelTrees.Remove(item);
                        LabelTrees.Insert(index, new LabelClassTree()
                        {
                            LabelClass = new LabelClass(result),
                            Children = item.Children
                        });
                        Helpers.InfoHelper.ShowSuccess("已保存标签");
                    }
                }
            }
        });

        public ICommand RemoveCommand => new RelayCommand<LabelClassTree>(async (item) =>
        {
            if (item != null)
            {
                if (await Dialogs.QueryDialog.ShowDialog("是否确认", $"将删除{item.LabelClass.LabelClassDb.Name}标签"))
                {
                    Core.Services.LabelClassService.RemoveLabel(item.LabelClass.LabelClassDb.LCId);
                    if (item.LabelClass.LabelClassDb.ParentId != null)//子类
                    {
                        var parent = LabelTrees.FirstOrDefault(p => p.LabelClass.LabelClassDb.LCId == item.LabelClass.LabelClassDb.ParentId);
                        if (parent != null)
                        {
                            var removeWhere = parent.Children.FirstOrDefault(t => t == item);
                            var index = parent.Children.IndexOf(removeWhere);
                            parent.Children.Remove(removeWhere);
                        }
                    }
                    else//父类
                    {
                        Init();
                    }
                    Helpers.InfoHelper.ShowSuccess("已删除标签");

                }
            }
        });

        public ICommand StyleConfirmCommand => new RelayCommand(() =>
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
            Init();
        });
        public ICommand StyleCancelCommand => new RelayCommand(() =>
        {
            Init();
        });


        public ICommand InitTag1stInfoCommand => new RelayCommand(() =>
        {
            GetTag1stInfo();
            ColorCurrent = Color1st;
            FontSizeCurrent = FontSize1st;
            WidthCurrent = Width1st;
            HeightCurrent = Height1st;
        });

        public ICommand InitTag2ndInfoCommand => new RelayCommand(() =>
        {
            GetTag2ndInfo();
            ColorCurrent = Color2nd;
            FontSizeCurrent = FontSize2nd;
            WidthCurrent = Width2nd;
            HeightCurrent = Height2nd;
        });

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


    }
}
