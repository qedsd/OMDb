﻿using Microsoft.UI.Xaml;
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
            typeof(IEnumerable<Models.Label>),
            typeof(UserControl),
            new PropertyMetadata(null, new PropertyChangedCallback(SetLabels))
            );
        private static void SetLabels(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as LabelsControl;
            if (card != null)
            {
                var labels = e.NewValue as IEnumerable<Models.Label>;
                if (labels != null)
                {
                    List<Models.Label> list = new List<Models.Label>();
                    foreach (var item in labels)
                    {
                        item.IsChecked = true;
                        list.Add(item);
                    }
                    if (card.Mode == LabelControlMode.Add)
                    {
                        list.Add(new Models.Label(new Core.DbModels.LabelDb() { Name = "+" }) { IsTemp = true, IsChecked = true });
                    }
                    else if (card.Mode == LabelControlMode.Selecte)
                    {
                        list.Insert(0, new Models.Label(new Core.DbModels.LabelDb() { Name = "全选" }) { IsTemp = true, IsChecked = true });
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
        private IEnumerable<Models.Label> GridViewItemsSource;
        public IEnumerable<Models.Label> LabelClasses
        {
            get { return (IEnumerable<Models.Label>)GetValue(LabelClassesProperty); }

            set { SetValue(LabelClassesProperty, value); }
        }

        public static readonly DependencyProperty LabelDbsProperty = DependencyProperty.Register
           (
           "LabelDbs",
           typeof(IEnumerable<Core.DbModels.LabelDb>),
           typeof(UserControl),
           new PropertyMetadata(null, new PropertyChangedCallback(SetLabelDbs))
           );
        private static void SetLabelDbs(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as LabelsControl;
            if (card != null)
            {
                var labelDbs = e.NewValue as IEnumerable<Core.DbModels.LabelDb>;
                if (labelDbs != null)
                {
                    card.LabelClasses = new List<Models.Label>(labelDbs.Select(p => new Models.Label(p)));
                }
            }
        }
        public IEnumerable<Core.DbModels.LabelDb> LabelDbs
        {
            get { return (IEnumerable<Core.DbModels.LabelDb>)GetValue(LabelDbsProperty); }

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
            var label = (sender as Button).DataContext as Models.Label;
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
        private List<Models.Label> AllLabels;
        private Flyout AddLabelFlyout;
        private async void ShowAddLabelsFlyout(FrameworkElement element)
        {
            if (AllLabels == null)
            {
                var labels = await Core.Services.LabelService.GetAllLabelAsync();
                if (labels != null)
                {
                    AllLabels = labels.Select(p => new Models.Label(p)).ToList();
                    if (labels?.Count != 0)
                    {
                        var dic = LabelClasses.ToDictionary(p => p.LabelClassDb.ID);
                        foreach (var label in AllLabels)
                        {
                            if (dic.ContainsKey(label.LabelClassDb.ID))
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

        private List<Models.Label> DisassemblyLabelClassTrees(List<Models.LabelClassTree> labelClassTrees)
        {
            var reuslt = new List<Models.Label>();
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

        private void AddLabelsControl_DoneEvent(bool confirm, IEnumerable<Models.Label> labels)
        {
            AddLabelFlyout.Hide();
            if (confirm)
            {
                var dic = labels.ToDictionary(p => p.LabelClassDb.ID);
                foreach (var label in AllLabels)
                {
                    if (dic.TryGetValue(label.LabelClassDb.ID, out var value))
                    {
                        label.IsChecked = value.IsChecked;
                    }
                }
                LabelClasses = labels.Where(p => p.IsChecked).ToList();
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
        public delegate void CheckChangedEventHandel(IEnumerable<Models.Label> allLabels);
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
