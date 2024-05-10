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
    public partial class LabelViewModel : ObservableObject
    {
        XElement xe = null;
        string ConfigPath = null;


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

    }
}
