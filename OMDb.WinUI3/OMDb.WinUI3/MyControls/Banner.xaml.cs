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
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.MyControls
{
    public sealed partial class Banner : UserControl
    {
        public Banner()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register
            (
            "ItemsSource",
            typeof(IEnumerable<BannerItem>),
            typeof(Banner),
            new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged))
            );
        //public static readonly DependencyProperty TitleProperty = DependencyProperty.Register
        //   (
        //   "Title",
        //   typeof(string),
        //   typeof(Banner),
        //   new PropertyMetadata(null, new PropertyChangedCallback(OnTitleChanged))
        //   );
        //public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register
        //   (
        //   "Description",
        //   typeof(string),
        //   typeof(Banner),
        //   new PropertyMetadata(null, new PropertyChangedCallback(OnDescriptionhanged))
        //   );
        public static readonly DependencyProperty DetailCommandProperty
            = DependencyProperty.Register(
                nameof(DetailCommand),
                typeof(ICommand),
                typeof(Banner),
                new PropertyMetadata(string.Empty));
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Banner).UpdateItemsSource(e.NewValue as IEnumerable<BannerItem>);
        }
        //private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    (d as Banner).TitleTextBlock.Text = e.NewValue as string;
        //}
        //private static void OnDescriptionhanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    (d as Banner).DesTextBlock.Text = e.NewValue as string;
        //}

        public IEnumerable<BannerItem> ItemsSource
        {
            get { return (IEnumerable<BannerItem>)GetValue(ItemsSourceProperty); }

            set { SetValue(ItemsSourceProperty, value); }
        }
        //public string Title
        //{
        //    get { return (string)GetValue(TitleProperty); }

        //    set { SetValue(TitleProperty, value); }
        //}
        //public string Description
        //{
        //    get { return (string)GetValue(DescriptionProperty); }

        //    set { SetValue(DescriptionProperty, value); }
        //}
        public ICommand DetailCommand
        {
            get => (ICommand)GetValue(DetailCommandProperty);
            set => SetValue(DetailCommandProperty, value);
        }
        private DispatcherTimer _timer;
        private void UpdateItemsSource(IEnumerable<BannerItem> items)
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(8);
                _timer.Tick += _timer_Tick;
            }
            _timer.Stop();
            ItemsListBox.ItemsSource = items;
            if(items != null && items.Count() != 0)
            {
                ItemsListBox.SelectedIndex = 0;
            }
            _timer.Start();
        }

        private void _timer_Tick(object sender, object e)
        {
            int newIndex = ItemsListBox.SelectedIndex + 1;
            ItemsListBox.SelectedIndex = newIndex ==ItemsListBox.Items.Count ? 0 : newIndex;
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
            DetailCommand?.Execute(ItemsListBox.SelectedItem);
        }

        private void ItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ItemsListBox.SelectedItem != null)
            {
                var item = ItemsListBox.SelectedItem as BannerItem;
                if (item != null)
                {
                    TitleTextBlock.Text = item.Title;
                    DesTextBlock.Text = item.Description;
                    GetTargetImage(out Image showImage, out Image hideImage);
                    showImage.Source = item.Img;
                    HideAnimation(hideImage);
                    ShowAnimation(showImage);
                }
                if(_timer != null)
                {
                    _timer.Start();
                }
            }
        }
        private void GetTargetImage(out Image showImage,out Image hideImage)
        {
            if(MainImage1.Opacity == 0)
            {
                showImage = MainImage1;
                hideImage = MainImage2;
            }
            else
            {
                showImage = MainImage2;
                hideImage = MainImage1;
            }
        }
        private void ShowAnimation(UIElement ui)
        {
            var targetVisual = Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(ui);
            Storyboard sbClock = new Storyboard();
            DoubleAnimation animColon = new DoubleAnimation();
            animColon.From = 0;
            animColon.To = 1;
            animColon.Duration = TimeSpan.FromSeconds(1);
            Storyboard.SetTarget(animColon, ui);
            Storyboard.SetTargetProperty(animColon, new PropertyPath(nameof(targetVisual.Opacity)).Path);
            sbClock.Children.Add(animColon);
            sbClock.Begin();
        }
        private void HideAnimation(UIElement ui)
        {
            var targetVisual = Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(ui);
            Storyboard sbClock = new Storyboard();
            DoubleAnimation animColon = new DoubleAnimation();
            animColon.From = 1;
            animColon.To = 0;
            animColon.Duration = TimeSpan.FromSeconds(1);
            Storyboard.SetTarget(animColon, ui);
            Storyboard.SetTargetProperty(animColon, new PropertyPath(nameof(targetVisual.Opacity)).Path);
            sbClock.Children.Add(animColon);
            sbClock.Begin();
        }
    }
}
