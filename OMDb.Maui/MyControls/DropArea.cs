using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 拖放区域控件
/// MAUI 不支持原生拖放，使用点击事件替代
/// </summary>
public class DropArea : Border
{
    public static readonly BindableProperty CaptionProperty = BindableProperty.Create(
        nameof(Caption), typeof(string), typeof(DropArea), string.Empty);

    public string Caption
    {
        get => (string)GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    public static readonly BindableProperty DropCommandProperty = BindableProperty.Create(
        nameof(DropCommand), typeof(ICommand), typeof(DropArea), null);

    public ICommand DropCommand
    {
        get => (ICommand)GetValue(DropCommandProperty);
        set => SetValue(DropCommandProperty, value);
    }

    public DropArea()
    {
        // MAUI 不支持 Windows 风格的拖放，使用简单的点击触发
        // 实际拖放功能需要平台特定实现
        var tapGestureRecognizer = new TapGestureRecognizer();
        tapGestureRecognizer.Tapped += async (sender, e) =>
        {
            // MAUI 没有内置拖放 API，这里仅作为占位实现
            // 实际使用需要自定义平台特定代码
            if (DropCommand?.CanExecute(null) == true)
            {
                DropCommand.Execute(null);
            }
        };
        GestureRecognizers.Add(tapGestureRecognizer);

        Stroke = new SolidColorBrush(Color.FromArgb("#40FFFFFF"));
        StrokeThickness = 1;
        BackgroundColor = Colors.Transparent;
        Padding = 0;
    }
}
