using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Utils.Extensions;
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
        public static readonly DependencyProperty LabelsProperty = DependencyProperty.Register
            (
            "Labels",
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
                    if(card.Mode == LabelControlMode.Add)
                    {
                        list.Add(new Models.LabelClass(new Core.DbModels.LabelClassDb() { Name = "+"}) { IsTemp = true,IsChecked = true});
                    }
                    else if(card.Mode == LabelControlMode.Selecte)
                    {
                        list.Insert(0,new Models.LabelClass(new Core.DbModels.LabelClassDb() { Name = "全选" }) { IsTemp = true, IsChecked = true });
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
        public IEnumerable<Models.LabelClass> Labels
        {
            get { return (IEnumerable<Models.LabelClass>)GetValue(LabelsProperty); }

            set { SetValue(LabelsProperty, value); }
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
                if(labelDbs != null)
                {
                    card.Labels = new List<Models.LabelClass>(labelDbs.Select(p=>new Models.LabelClass(p)));
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
            get { return (LabelControlMode) GetValue(ModeProperty); }

            set { SetValue(ModeProperty, value); }
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var label = (sender as Button).DataContext as Models.LabelClass;
            if(label != null)
            {
                if (Mode == LabelControlMode.Add && label.IsTemp)//新增标签
                {
                    ShowAddLabelsFlyout(sender as Button);
                }
                else if(Mode == LabelControlMode.Selecte && label.IsTemp)//全选、全不选
                {
                    label.IsChecked = !label.IsChecked;
                    foreach(var l in Labels)
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
                                if(label.IsChecked)
                                {
                                    if(GridViewItemsSource.FirstOrDefault(p=>!p.IsTemp && !p.IsChecked) == null)
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
            var ls = Labels.Where(p => !p.IsTemp).ToList();
            CheckChanged?.Invoke(ls);
            CheckChangedCommand?.Execute(ls);
        }
        private List<Models.LabelClass> AllLabels;
        private Flyout AddLabelFlyout;
        private async void ShowAddLabelsFlyout(FrameworkElement element)
        {
            if(AllLabels == null)
            {
                var labels = await Core.Services.LabelClassService.GetAllLabelAsync(Services.Settings.DbSelectorService.dbCurrentId);
                if (labels != null)
                {
                    AllLabels = labels.Select(p => new Models.LabelClass(p)).ToList();
                    if(labels?.Count!=0)
                    {
                        var dic = Labels.ToDictionary(p => p.LabelClassDb.LCId);
                        foreach(var label in AllLabels)
                        {
                            if(dic.ContainsKey(label.LabelClassDb.LCId))
                            {
                                label.IsChecked = true;
                            }
                        }
                    }
                }
            }
            if(AddLabelFlyout == null)
            {
                /*AddLabelFlyout = new Flyout();
                AddLabelsControl addLabelsControl = new AddLabelsControl();
                addLabelsControl.Labels = AllLabels.DepthClone<List<Models.Label>>();
                AddLabelFlyout.Content = addLabelsControl;
                addLabelsControl.DoneEvent += AddLabelsControl_DoneEvent;*/

                AddLabelFlyout = new Flyout();
                LabelClassSelectControl labelClassSelectControl = new LabelClassSelectControl();
                labelClassSelectControl.LabelClassTrees = await Services.CommonService.GetLabelTrees();

                Grid container = new Grid();
                container.Width = 300;
                container.Height = 200;
                container.Children.Add(labelClassSelectControl);


                AddLabelFlyout.Content = container;

            }
            AddLabelFlyout.ShowAt(element);
        }

        private void AddLabelsControl_DoneEvent(bool confirm, IEnumerable<Models.LabelClass> labels)
        {
            AddLabelFlyout.Hide();
            if(confirm)
            {
                var dic = labels.ToDictionary(p => p.LabelClassDb.LCId);
                foreach (var label in AllLabels)
                {
                    if(dic.TryGetValue(label.LabelClassDb.LCId, out var value))
                    {
                        label.IsChecked = value.IsChecked;
                    }
                }
                Labels = labels.Where(p=>p.IsChecked).ToList();
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
