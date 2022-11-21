using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.Models;
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
    public sealed partial class EntryCollectionCard2 : UserControl
    {
        public EntryCollectionCard2()
        {
            this.InitializeComponent();
        }
        public static readonly DependencyProperty DetailCommandProperty = DependencyProperty.Register(
            nameof(DetailCommand), typeof(ICommand), typeof(EntryCollectionCard2), new PropertyMetadata(default(ICommand)));
        public ICommand DetailCommand
        {
            get => (ICommand)GetValue(DetailCommandProperty);
            set => SetValue(DetailCommandProperty, value);
        }
        public static readonly DependencyProperty EntryCollectionProperty = DependencyProperty.Register(
           "EntryCollection", typeof(EntryCollection), typeof(EntryCollectionCard2), new PropertyMetadata(null, new PropertyChangedCallback(OnEntryCollectionChanged)));
        private static void OnEntryCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as EntryCollectionCard2;
            var collection = e.NewValue as EntryCollection;
            if(collection != null)
            {
                card.TitleTextBlock.Text = collection.Title;
                card.DescTextBlock.Text = collection.Description;
                card.LastUpdateTextBlock.Text = collection.LastUpdateTime.ToString();
                card.WatchedCountTextBlock.Text = collection.WatchedCount.ToString();
                card.TotalCountTextBlock.Text = collection.Items == null ? "0" : collection.Items.Count.ToString();
                card.WatchingCountTextBlock.Text = collection.WatchingCount.ToString();
                if (collection.CoverImage != null)
                {
                    card.BgImage.ImageSource = collection.CoverImage;

                }
                else
                {
                    card.BgImage.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets/Img/defaultbanneritem.jpg")));
                }
            }
        }

        public EntryCollection EntryCollection
        {
            get { return (EntryCollection)GetValue(EntryCollectionProperty); }

            set { SetValue(EntryCollectionProperty, value); }
        }

        private void Item_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            UpAnimation(AnimationArea1);
            DesAnimation1();
        }
        private void Item_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            DownAnimation(AnimationArea1);
            DesAnimation2();
        }
        private void UpAnimation(FrameworkElement element)
        {
            var targetVisual = Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(element);
            var animation = targetVisual.Compositor.CreateVector3KeyFrameAnimation();
            animation.Duration = TimeSpan.FromSeconds(0.3);
            animation.Target = nameof(targetVisual.Offset);
            var linear = targetVisual.Compositor.CreateLinearEasingFunction();
            animation.InsertKeyFrame(1.0f, new Vector3(element.ActualOffset.X, -10, 0), linear);
            targetVisual.StartAnimation(nameof(targetVisual.Offset), animation);
        }
        private void DownAnimation(FrameworkElement element)
        {
            var targetVisual = Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(element);
            var animation = targetVisual.Compositor.CreateVector3KeyFrameAnimation();
            animation.Duration = TimeSpan.FromSeconds(0.3);
            animation.Target = nameof(targetVisual.Offset);
            var linear = targetVisual.Compositor.CreateLinearEasingFunction();
            animation.InsertKeyFrame(1.0f, new Vector3((float)element.ActualOffset.X, 0, 0), linear);
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

        private void Grid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            DetailCommand?.Execute(EntryCollection);
        }
    }
}
