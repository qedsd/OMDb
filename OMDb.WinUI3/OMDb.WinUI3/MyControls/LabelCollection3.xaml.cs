using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.MyControls
{
    public sealed partial class LabelCollection3 : UserControl
    {
        public LabelCollection3()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
           "Title", typeof(string), typeof(LabelCollection3), new PropertyMetadata(null, new PropertyChangedCallback(OnTitleChanged)));
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description", typeof(string), typeof(LabelCollection3), new PropertyMetadata(null, new PropertyChangedCallback(OnDescriptionhanged)));
        public static readonly DependencyProperty DetailCommandProperty = DependencyProperty.Register(
            nameof(DetailCommand), typeof(ICommand), typeof(LabelCollection3), new PropertyMetadata(default(ICommand)));
        public static readonly DependencyProperty BgImageSourceProperty = DependencyProperty.Register(
            "BgImageSource", typeof(ImageSource), typeof(LabelCollection3), new PropertyMetadata(null, new PropertyChangedCallback(OnBgImageSourceChanged)));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LabelCollection3).TitleTextBlock.Text = e.NewValue as string;
        }
        private static void OnDescriptionhanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LabelCollection3).DesTextBlock.Text = e.NewValue as string;
        }
        private static void OnBgImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LabelCollection3).BgImage.ImageSource = e.NewValue as ImageSource;
        }
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }

            set { SetValue(TitleProperty, value); }
        }
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }

            set { SetValue(DescriptionProperty, value); }
        }
        public ICommand DetailCommand
        {
            get => (ICommand)GetValue(DetailCommandProperty);
            set => SetValue(DetailCommandProperty, value);
        }

        public ImageSource BgImageSource
        {
            get => (ImageSource)GetValue(BgImageSourceProperty);
            set => SetValue(BgImageSourceProperty, value);
        }

        private void Item_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            TitleAnimation1();
            DesAnimation1();
        }
        private void Item_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            TitleAnimation2();
            DesAnimation2();
        }
        private void TitleAnimation1()
        {
            var elment = TitleTextBlock as FrameworkElement;
            var targetVisual = Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(TitleTextBlock as UIElement);
            var animation = targetVisual.Compositor.CreateVector3KeyFrameAnimation();
            animation.Duration = TimeSpan.FromSeconds(0.3);
            animation.Target = nameof(targetVisual.Offset);
            var linear = targetVisual.Compositor.CreateLinearEasingFunction();
            animation.InsertKeyFrame(1.0f, new Vector3((float)elment.ActualOffset.X, -14, 0), linear);
            targetVisual.StartAnimation(nameof(targetVisual.Offset), animation);
        }
        private void TitleAnimation2()
        {
            var elment = TitleTextBlock as FrameworkElement;
            var targetVisual = Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(TitleTextBlock as UIElement);
            var animation = targetVisual.Compositor.CreateVector3KeyFrameAnimation();
            animation.Duration = TimeSpan.FromSeconds(0.3);
            animation.Target = nameof(targetVisual.Offset);
            var linear = targetVisual.Compositor.CreateLinearEasingFunction();
            animation.InsertKeyFrame(1.0f, new Vector3((float)elment.ActualOffset.X, 0, 0), linear);
            targetVisual.StartAnimation(nameof(targetVisual.Offset), animation);
        }
        private void DesAnimation1()
        {
            Storyboard1.Begin();
        }
        private void DesAnimation2()
        {
            Storyboard2.Begin();
        }
    }
}
