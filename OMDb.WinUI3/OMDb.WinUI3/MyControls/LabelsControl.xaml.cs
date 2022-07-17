using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.MyControls
{
    public sealed partial class LabelsControl : UserControl
    {
        public LabelsControl()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty LabelsProperty = DependencyProperty.Register
            (
            "Labels",
            typeof(IEnumerable<Core.DbModels.LabelDb>),
            typeof(UserControl),
            new PropertyMetadata(null, new PropertyChangedCallback(SetLabels))
            );
        private static void SetLabels(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as LabelsControl;
            if (card != null)
            {
                card.Labels = e.NewValue as IEnumerable<Core.DbModels.LabelDb>;
                if (card.Labels != null)
                {
                    List<Models.Label> list = new List<Models.Label>();
                    foreach (var item in card.Labels)
                    {
                        list.Add(new Models.Label(item));
                    }
                    if(card.Mode == LabelControlMode.Add)
                    {
                        list.Add(new Models.Label(new Core.DbModels.LabelDb() { Name = "+"}) { IsTemp = true});
                    }
                    card.GridView_Label.ItemsSource = list;
                }
                else
                {
                    card.GridView_Label.ItemsSource = null;
                }
            }
        }
        public IEnumerable<Core.DbModels.LabelDb> Labels
        {
            get { return (IEnumerable<Core.DbModels.LabelDb>)GetValue(LabelsProperty); }

            set { SetValue(LabelsProperty, value); }
        }

        //public static readonly DependencyProperty SelecteModeProperty = DependencyProperty.Register
        //    (
        //    "SelecteMode",
        //    typeof(bool),
        //    typeof(UserControl),
        //    new PropertyMetadata(false, new PropertyChangedCallback(SetSelecteMode))
        //    );
        //private static void SetSelecteMode(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var card = d as LabelsControl;
        //    if (card != null)
        //    {
                
        //    }
        //}
        ///// <summary>
        ///// 是否为选择模式
        ///// </summary>
        //public bool SelecteMode
        //{
        //    get { return (bool)GetValue(SelecteModeProperty); }

        //    set { SetValue(SelecteModeProperty, value); }
        //}

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register
            (
            "Mode",
            typeof(LabelControlMode),
            typeof(UserControl),
            new PropertyMetadata(LabelControlMode.None, new PropertyChangedCallback(SetMode))
            );
        private static void SetMode(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        public LabelControlMode Mode
        {
            get { return (LabelControlMode) GetValue(ModeProperty); }

            set { SetValue(ModeProperty, value); }
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var label = (sender as Button).DataContext as Models.Label;
            if(label != null)
            {
                label.IsChecked = !label.IsChecked;
                var labelsOut = GridView_Label.ItemsSource as List<Models.Label>;
                if(Mode == LabelControlMode.Add && label.IsTemp)
                {
                    ShowAddLabelsFlyout(sender as Button);
                }
                SelecteItemEvent?.Invoke(Mode == LabelControlMode.Add? labelsOut.Take(labelsOut.Count -1): labelsOut, label);
                switch(Mode)
                {
                    case LabelControlMode.None:break;
                    case LabelControlMode.Selecte:
                        {
                            if (label.IsChecked)
                            {
                                ((sender as Button).Content as TextBlock).Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(100, 0, 200, 0));
                            }
                            else
                            {
                                if (Services.ThemeSelectorService.IsDark)
                                {
                                    ((sender as Button).Content as TextBlock).Foreground = new SolidColorBrush(Microsoft.UI.Colors.White);
                                }
                                else
                                {
                                    ((sender as Button).Content as TextBlock).Foreground = new SolidColorBrush(Microsoft.UI.Colors.Black);
                                }
                                //(sender as Button).BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 235, 234, 231));
                            }
                        }break;
                    case LabelControlMode.Add:break;
                }
            }
        }
        private List<Models.Label> AllLabels;
        private Flyout AddLabelFlyout;
        private async void ShowAddLabelsFlyout(FrameworkElement element)
        {
            if(AllLabels == null)
            {
                var labels = await Core.Services.LabelService.GetAllLabelAsync();
                if (labels != null)
                {
                    AllLabels = labels.Select(p => new Models.Label(p)).ToList();
                }
            }
            if(AddLabelFlyout == null)
            {
                AddLabelFlyout = new Flyout();
                AddLabelsControl addLabelsControl = new AddLabelsControl();
                addLabelsControl.Labels = AllLabels.DepthClone<List<Models.Label>>();
                AddLabelFlyout.Content = addLabelsControl;
                addLabelsControl.DoneEvent += AddLabelsControl_DoneEvent;
            }
            AddLabelFlyout.ShowAt(element);
        }

        private void AddLabelsControl_DoneEvent(bool confirm, IEnumerable<Models.Label> labels)
        {
            AddLabelFlyout.Hide();
            if(confirm)
            {
                var dic = labels.ToDictionary(p => p.LabelDb.Id);
                foreach (var label in AllLabels)
                {
                    if(dic.TryGetValue(label.LabelDb.Id, out var value))
                    {
                        label.IsChecked = value.IsChecked;
                    }
                }
                Labels = labels.Where(p=>p.IsChecked).Select(p => p.LabelDb).ToList();
            }
            else
            {
                (AddLabelFlyout.Content as AddLabelsControl).Labels = AllLabels.DepthClone<List<Models.Label>>();
            }
        }

        /// <summary>
        /// 选择标签委托
        /// </summary>
        /// <param name="labels">所有标签</param>
        /// <param name="label">当前触发标签</param>
        public delegate void SelecteItemDelegate(IEnumerable<Models.Label> labels, Models.Label label);
        private SelecteItemDelegate SelecteItemEvent;
        /// <summary>
        /// 选择标签后触发
        /// </summary>
        public event SelecteItemDelegate OnSelectedItem
        {
            add
            {
                SelecteItemEvent+=value;
            }
            remove
            {
                SelecteItemEvent-=value;
            }
        }

        public enum LabelControlMode
        {
            [Description("无")]
            None,
            [Description("多个选择模式")]
            Selecte,
            [Description("添加模式")]
            Add
        }
    }
}
