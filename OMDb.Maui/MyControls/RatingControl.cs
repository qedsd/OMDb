using Microsoft.Maui.Controls;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 评分控件
/// 支持 0-5 星评分
/// </summary>
public class RatingControl : HorizontalStackLayout
{
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value), typeof(double), typeof(RatingControl), 0.0, BindingMode.TwoWay,
        propertyChanged: OnValueChanged);

    private static void OnValueChanged(BindableObject d, object oldValue, object newValue)
    {
        if (d is RatingControl control)
        {
            control.UpdateStars();
        }
    }

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly BindableProperty MaxRatingProperty = BindableProperty.Create(
        nameof(MaxRating), typeof(int), typeof(RatingControl), 5);

    public int MaxRating
    {
        get => (int)GetValue(MaxRatingProperty);
        set => SetValue(MaxRatingProperty, value);
    }

    private readonly List<Label> _starLabels = new();

    public RatingControl()
    {
        Spacing = 4;
        HorizontalOptions = LayoutOptions.Start;

        for (int i = 0; i < 5; i++)
        {
            var star = new Label
            {
                Text = "\uE735", // Segoe MDL2 Assets star icon
                FontSize = 24,
                TextColor = Colors.Gray,
                VerticalOptions = LayoutOptions.Center
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) =>
            {
                Value = i + 1;
            };

            star.GestureRecognizers.Add(tapGesture);
            _starLabels.Add(star);
            Children.Add(star);
        }

        UpdateStars();
    }

    private void UpdateStars()
    {
        for (int i = 0; i < _starLabels.Count; i++)
        {
            if (i < Value)
            {
                _starLabels[i].TextColor = Colors.Gold;
            }
            else
            {
                _starLabels[i].TextColor = Colors.Gray;
            }
        }
    }
}
