using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Utils.Extensions;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
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
        public static readonly DependencyProperty LabelClassesProperty = DependencyProperty.Register
            (
            "LabelClasses",
            typeof(IEnumerable<Models.LabelClass>),
            typeof(UserControl),
            new PropertyMetadata(null, new PropertyChangedCallback(SetLabels))
            );
        private static void SetLabels(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as LabelsControl;
            if (card != null)
            {
                var labels = e.NewValue as IEnumerable<Models.LabelClass>;
                if (labels != null)
                {
                    List<Models.LabelClass> list = new List<Models.LabelClass>();
                    foreach (var item in labels)
                    {
                        item.IsChecked = true;
                        list.Add(item);
                    }
                    if (card.Mode == LabelControlMode.Add)
                    {
                        list.Add(new Models.LabelClass(new Core.DbModels.LabelClassDb() { Name = "+" }) { IsTemp = true, IsChecked = true });
                    }
                    else if (card.Mode == LabelControlMode.Selecte)
                    {
                        list.Insert(0, new Models.LabelClass(new Core.DbModels.LabelClassDb() { Name = "全选" }) { IsTemp = true, IsChecked = true });
                    }
                    card.GridView_Label.ItemsSource = list;
                    card.GridViewItemsSource = list;
                }
                else
                {
                    card.GridView_Label.ItemsSource = null;
                    card.GridViewItemsSource = null;
                }
            }
        }
        private IEnumerable<Models.LabelClass> GridViewItemsSource;
        public IEnumerable<Models.LabelClass> LabelClasses
        {
            get { return (IEnumerable<Models.LabelClass>)GetValue(LabelClassesProperty); }

            set { SetValue(LabelClassesProperty, value); }
        }

        public static readonly DependencyProperty LabelDbsProperty = DependencyProperty.Register
           (
           "LabelDbs",
           typeof(IEnumerable<Core.DbModels.LabelClassDb>),
           typeof(UserControl),
           new PropertyMetadata(null, new PropertyChangedCallback(SetLabelDbs))
           );
        private static void SetLabelDbs(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as LabelsControl;
            if (card != null)
            {
                var labelDbs = e.NewValue as IEnumerable<Core.DbModels.LabelClassDb>;
                if (labelDbs != null)
                {
                    card.LabelClasses = new List<Models.LabelClass>(labelDbs.Select(p => new Models.LabelClass(p)));
                }
            }
        }
        public IEnumerable<Core.DbModels.LabelClassDb> LabelDbs
        {
            get { return (IEnumerable<Core.DbModels.LabelClassDb>)GetValue(LabelDbsProperty); }

            set { SetValue(LabelDbsProperty, value); }
        }

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
            get { return (LabelControlMode)GetValue(ModeProperty); }

            set { SetValue(ModeProperty, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var label = (sender as Button).DataContext as Models.LabelClass;
            if (label != null)
            {
                if (Mode == LabelControlMode.Add && label.IsTemp)//新增标签
                {
                    ShowAddLabelsFlyout(sender as Button);
                }
                else if (Mode == LabelControlMode.Selecte && label.IsTemp)//全选、全不选
                {
                    label.IsChecked = !label.IsChecked;
                    foreach (var l in LabelClasses)
                    {
                        l.IsChecked = label.IsChecked;
                    }
                    CallChanged();
                }
                else
                {
                    switch (Mode)
                    {
                        case LabelControlMode.None: break;
                        case LabelControlMode.Selecte:
                            {
                                label.IsChecked = !label.IsChecked;
                                if (label.IsChecked)
                                {
                                    if (GridViewItemsSource.FirstOrDefault(p => !p.IsTemp && !p.IsChecked) == null)
                                    {
                                        GridViewItemsSource.First().IsChecked = true;
                                    }
                                }
                                else
                                {
                                    GridViewItemsSource.First().IsChecked = false;
                                }
                                CallChanged();
                            }
                            break;
                        case LabelControlMode.Add: break;
                    }
                }
            }
        }
        private void CallChanged()
        {
            var ls = LabelClasses.Where(p => !p.IsTemp).ToList();
            CheckChanged?.Invoke(ls);
            CheckChangedCommand?.Execute(ls);
        }
        private List<Models.LabelClass> AllLabels;
        private Flyout AddLabelFlyout;
        private async void ShowAddLabelsFlyout(FrameworkElement element)
        {
            if (AllLabels == null)
            {
                var labels = await Core.Services.LabelClassService.GetAllLabelAsync(Services.Settings.DbSelectorService.dbCurrentId);
                if (labels != null)
                {
                    AllLabels = labels.Select(p => new Models.LabelClass(p)).ToList();
                    if (labels?.Count != 0)
                    {
                        var dic = LabelClasses.ToDictionary(p => p.LabelClassDb.LCID);
                        foreach (var label in AllLabels)
                        {
                            if (dic.ContainsKey(label.LabelClassDb.LCID))
                            {
                                label.IsChecked = true;
                            }
                        }
                    }
                }
            }
            if (AddLabelFlyout == null)
            {
                Grid container = new Grid();
                container.Width = 400;
                container.Height = 300;

                // 添加两个行定义，第一行用于放置分类选择控件，第二行用于放置确认按钮
                container.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                container.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                //分类选择弹出框
                AddLabelFlyout = new Flyout();

                labelClassSelectControl = new LabelClassSelectControl();
                labelClassSelectControl.LabelClassTrees = await Services.CommonService.GetLabelClassTrees();
                foreach (var LabelClass in LabelClasses)
                {
                    if (LabelClass.LabelClassDb.ParentId == null)
                    {
                        var lc = labelClassSelectControl.LabelClassTrees.FirstOrDefault(a => a.LabelClass.LabelClassDb.LCID == LabelClass.LabelClassDb.LCID).LabelClass.IsChecked = true;
                    }
                    else
                    {
                        var lc = labelClassSelectControl.LabelClassTrees.FirstOrDefault(a => a.LabelClass.LabelClassDb.LCID == LabelClass.LabelClassDb.ParentId);
                        lc.Children.FirstOrDefault(a => a.LabelClass.LabelClassDb.LCID == LabelClass.LabelClassDb.LCID).LabelClass.IsChecked = true;
                    }
                }
                // 将分类选择控件放置在第一行
                Grid.SetRow(labelClassSelectControl, 0);
                container.Children.Add(labelClassSelectControl);

                Button confirmButton = new Button();
                confirmButton.Content = "确认";
                confirmButton.Margin = new Thickness(10, 0, 0, 0);
                confirmButton.HorizontalAlignment = HorizontalAlignment.Left;
                confirmButton.VerticalAlignment = VerticalAlignment.Bottom;
                confirmButton.Click += ConfirmButton_Click;
                // 将确认按钮放置在第二行
                Grid.SetRow(confirmButton, 1);
                container.Children.Add(confirmButton);

                AddLabelFlyout.Content = container;

            }
            AddLabelFlyout.ShowAt(element);
        }
        private LabelClassSelectControl labelClassSelectControl;
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var labelClasses = DisassemblyLabelClassTrees(labelClassSelectControl.LabelClassTrees);
            LabelClasses = labelClasses.Where(a => a.IsChecked == true).ToList();
        }

        private List<Models.LabelClass> DisassemblyLabelClassTrees(List<Models.LabelClassTree> labelClassTrees)
        {
            var reuslt = new List<Models.LabelClass>();
            foreach (var lct in labelClassTrees)
            {
                reuslt.Add(lct.LabelClass);
                foreach (var lctc in lct.Children)
                {
                    reuslt.Add(lctc.LabelClass);
                }
            }
            return reuslt;
        }

        private void AddLabelsControl_DoneEvent(bool confirm, IEnumerable<Models.LabelClass> labels)
        {
            AddLabelFlyout.Hide();
            if (confirm)
            {
                var dic = labels.ToDictionary(p => p.LabelClassDb.LCID);
                foreach (var label in AllLabels)
                {
                    if (dic.TryGetValue(label.LabelClassDb.LCID, out var value))
                    {
                        label.IsChecked = value.IsChecked;
                    }
                }
                LabelClasses = labels.Where(p => p.IsChecked).ToList();
            }
            else
            {
                (AddLabelFlyout.Content as AddLabelsControl).Labels = AllLabels.DepthClone<List<Models.LabelClass>>();
            }
        }

        /// <summary>
        /// 选择标签委托
        /// </summary>
        /// <param name="labels">所有标签</param>
        /// <param name="label">当前触发标签</param>
        public delegate void CheckChangedEventHandel(IEnumerable<Models.LabelClass> allLabels);
        private CheckChangedEventHandel CheckChanged;

        public static readonly DependencyProperty CheckChangedCommandProperty
           = DependencyProperty.Register(
               nameof(CheckChangedCommand),
               typeof(string),
               typeof(LabelsControl),
               new PropertyMetadata(string.Empty));

        public ICommand CheckChangedCommand
        {
            get => (ICommand)GetValue(CheckChangedCommandProperty);
            set => SetValue(CheckChangedCommandProperty, value);
        }

        /// <summary>
        /// 选择标签后触发
        /// </summary>
        public event CheckChangedEventHandel OnCheckChanged
        {
            add
            {
                CheckChanged += value;
            }
            remove
            {
                CheckChanged -= value;
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
