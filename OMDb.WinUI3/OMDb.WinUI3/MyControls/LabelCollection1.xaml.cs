using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
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
    public sealed partial class LabelCollection1 : UserControl
    {
        public LabelCollection1()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource", typeof(IEnumerable<Entry>), typeof(LabelCollection1), new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
           "Title", typeof(string), typeof(LabelCollection1), new PropertyMetadata(null, new PropertyChangedCallback(OnTitleChanged)));
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description",typeof(string),typeof(LabelCollection1),new PropertyMetadata(null, new PropertyChangedCallback(OnDescriptionhanged)));
        public static readonly DependencyProperty DetailCommandProperty= DependencyProperty.Register(
            nameof(DetailCommand),typeof(ICommand),typeof(Banner), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty BgImageSourceProperty = DependencyProperty.Register(
            "BgImageSource", typeof(ImageSource), typeof(LabelCollection1), new PropertyMetadata(null, new PropertyChangedCallback(OnBgImageSourceChanged)));
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LabelCollection1).ItemsList.ItemsSource = e.NewValue as IEnumerable<Entry>;
        }
        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LabelCollection1).TitleTextBlock.Text = e.NewValue as string;
        }
        private static void OnDescriptionhanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LabelCollection1).DesTextBlock.Text = e.NewValue as string;
        }
        private static void OnBgImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LabelCollection1).BgImage.Source = e.NewValue as ImageSource;
        }
        public IEnumerable<Entry> ItemsSource
        {
            get { return (IEnumerable<Entry>)GetValue(ItemsSourceProperty); }

            set { SetValue(ItemsSourceProperty, value); }
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
            var targetVisual = Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(sender as UIElement);
            var scaleAnimation = targetVisual.Compositor.CreateVector3KeyFrameAnimation();
            scaleAnimation.Duration = TimeSpan.FromSeconds(0.3);
            scaleAnimation.Target = nameof(targetVisual.Scale);
            var linear = targetVisual.Compositor.CreateLinearEasingFunction();
            scaleAnimation.InsertKeyFrame(1.0f, new Vector3((float)1.1), linear);
            targetVisual.StartAnimation(nameof(targetVisual.Scale), scaleAnimation);
        }

        private void Item_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var targetVisual = Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(sender as UIElement);
            var scaleAnimation = targetVisual.Compositor.CreateVector3KeyFrameAnimation();
            scaleAnimation.Duration = TimeSpan.FromSeconds(0.3);
            scaleAnimation.Target = nameof(targetVisual.Scale);
            var linear = targetVisual.Compositor.CreateLinearEasingFunction();
            scaleAnimation.InsertKeyFrame(1.0f, new Vector3((float)1.0), linear);
            targetVisual.StartAnimation(nameof(targetVisual.Scale), scaleAnimation);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DetailCommand?.Execute(null);
        }

        private void ItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
