using ABI.Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Timers;
using System.Windows.Input;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.MyControls
{
    public sealed partial class DoubleThumbSlider : UserControl
    {
        private bool _ellMinPressed = false;
        private bool _ellMaxPressed = false;

        public DoubleThumbSlider()
        {
            this.InitializeComponent();
        }

        private void ellMin_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ellMin.CapturePointer(e.Pointer);
            _ellMinPressed = true;
            ellMin.Fill = (SolidColorBrush)Resources["DefaultBrush"];
        }

        private void ellMax_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ellMax.CapturePointer(e.Pointer);
            _ellMaxPressed = true;
            ellMax.Fill = (SolidColorBrush)Resources["DefaultBrush"];
        }

        private void ellMin_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ellMin.ReleasePointerCapture(e.Pointer);
            _ellMinPressed = false;
            ellMin.Fill = (SolidColorBrush)Resources["HighLightBrush"];
            CheckChangedCommand?.Execute(null);
        }

        private void ellMax_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ellMax.ReleasePointerCapture(e.Pointer);
            _ellMaxPressed = false;
            ellMax.Fill = (SolidColorBrush)Resources["HighLightBrush"];
            CheckChangedCommand?.Execute(null);
        }

        private void ell_MinPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.IsInContact)
            {
                double percentage = (e.GetCurrentPoint(rect).Position.X - 8) / (rect.ActualWidth - 8);
                double value = (Maximum - Minimum) * percentage + Minimum;
                MinValue = value >= MaxValue ? MaxValue : value <= 0 ? 0 : (int)value;
            }
        }

        private void ell_MaxPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.IsInContact)
            {
                double percentage = (e.GetCurrentPoint(rect).Position.X - 8) / (rect.ActualWidth - 8);
                double value = (Maximum - Minimum) * percentage + Minimum;
                MaxValue = value <= MinValue ? MinValue : (int)value >= 275 ? 275 : (int)value;
            }
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
                "Minimum",
                typeof(int),
                typeof(DoubleThumbSlider),
                new PropertyMetadata(0));

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum",
            typeof(int),
            typeof(DoubleThumbSlider),
            new PropertyMetadata(275));

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue",
            typeof(int),
            typeof(DoubleThumbSlider),
            new PropertyMetadata(0, MinValuePropertyChangedCallback));

        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        private static void MinValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DoubleThumbSlider slider)
            {
                slider.MinValuePropertyChanged();
            }
        }

        private void MinValuePropertyChanged()
        {
            // 处理 MinValue 属性变化的逻辑
            MinValue = MinValue <= 0 ? 0 : MinValue;
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue",
            typeof(int),
            typeof(DoubleThumbSlider),
            new PropertyMetadata(0, MaxValuePropertyChangedCallback));

        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        private static void MaxValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DoubleThumbSlider slider)
            {
                slider.MaxValuePropertyChanged();
            }
        }

        private void MaxValuePropertyChanged()
        {
            // 处理 MaxValue 属性变化的逻辑
            MaxValue = MaxValue >= 275 ? 275 : MaxValue;
            
        }

        public delegate void CheckChangedEventHandel(int value);
        private CheckChangedEventHandel CheckChanged;

        public static readonly DependencyProperty CheckChangedCommandProperty
           = DependencyProperty.Register(
               nameof(CheckChangedCommand),
               typeof(ICommand),
               typeof(DoubleThumbSlider),
               new PropertyMetadata(null));

        public ICommand CheckChangedCommand
        {
            get => (ICommand)GetValue(CheckChangedCommandProperty);
            set => SetValue(CheckChangedCommandProperty, value);
        }
    }
}

