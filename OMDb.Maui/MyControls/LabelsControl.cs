using System.Collections;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;

namespace OMDb.Maui.MyControls;

/// <summary>
/// 标签控件
/// 用于显示和管理标签集合，支持点击选择
/// </summary>
public class LabelsControl : WrapLayout
{
    /// <summary>
    /// 标签树集合绑定属性
    /// </summary>
    public static readonly BindableProperty LabelClassTreesProperty =
        BindableProperty.Create(
            nameof(LabelClassTrees),
            typeof(System.Collections.IEnumerable),
            typeof(LabelsControl),
            null,
            propertyChanged: OnLabelClassTreesChanged);

    /// <summary>
    /// 标签属性树集合绑定属性
    /// </summary>
    public static readonly BindableProperty LabelPropertyTreesProperty =
        BindableProperty.Create(
            nameof(LabelPropertyTrees),
            typeof(System.Collections.IEnumerable),
            typeof(LabelsControl),
            null,
            propertyChanged: OnLabelPropertyTreesChanged);

    /// <summary>
    /// 选中变更命令绑定属性
    /// </summary>
    public static readonly BindableProperty CheckChangedCommandProperty =
        BindableProperty.Create(
            nameof(CheckChangedCommand),
            typeof(ICommand),
            typeof(LabelsControl),
            null);

    /// <summary>
    /// 标签类别树集合
    /// </summary>
    public System.Collections.IEnumerable LabelClassTrees
    {
        get => (System.Collections.IEnumerable)GetValue(LabelClassTreesProperty);
        set => SetValue(LabelClassTreesProperty, value);
    }

    /// <summary>
    /// 标签属性树集合
    /// </summary>
    public System.Collections.IEnumerable LabelPropertyTrees
    {
        get => (System.Collections.IEnumerable)GetValue(LabelPropertyTreesProperty);
        set => SetValue(LabelPropertyTreesProperty, value);
    }

    /// <summary>
    /// 选中变更命令
    /// </summary>
    public ICommand CheckChangedCommand
    {
        get => (ICommand)GetValue(CheckChangedCommandProperty);
        set => SetValue(CheckChangedCommandProperty, value);
    }

    /// <summary>
    /// 创建标签控件
    /// </summary>
    public LabelsControl()
    {
        HorizontalOptions = LayoutOptions.Start;
    }

    private static void OnLabelClassTreesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LabelsControl control && newValue != null)
        {
            control.UpdateContent();
        }
    }

    private static void OnLabelPropertyTreesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LabelsControl control && newValue != null)
        {
            control.UpdateContent();
        }
    }

    private void UpdateContent()
    {
        Children.Clear();

        var trees = LabelClassTrees ?? LabelPropertyTrees;
        if (trees == null)
            return;

        foreach (var tree in trees)
        {
            var type = tree.GetType();
            var labelProperty = type.GetProperty("LabelClass") ?? type.GetProperty("LabelProperty");
            var childrenProperty = type.GetProperty("Children");
            var colorProperty = type.GetProperty("Color1st") ?? type.GetProperty("Color2nd");

            if (labelProperty == null)
                continue;

            var labelObj = labelProperty.GetValue(tree);
            var name = labelObj?.GetType().GetProperty("Name")?.GetValue(labelObj)?.ToString() ?? "未知标签";

            // 创建标签按钮
            var labelBtn = new Border
            {
                BackgroundColor = Colors.LightGray,
                Padding = 10,
                Margin = new Thickness(0, 0, 5, 5)
            };

            var label = new Label
            {
                Text = name,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            labelBtn.Content = label;

            // 点击事件
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) =>
            {
                // 切换选中状态
                if (labelBtn.BackgroundColor == Colors.LightGray)
                {
                    labelBtn.BackgroundColor = Colors.Blue;
                    label.TextColor = Colors.White;
                }
                else
                {
                    labelBtn.BackgroundColor = Colors.LightGray;
                    label.TextColor = Colors.Black;
                }

                CheckChangedCommand?.Execute(null);
            };
            labelBtn.GestureRecognizers.Add(tapGesture);

            Children.Add(labelBtn);

            // 处理子标签
            if (childrenProperty != null)
            {
                var children = childrenProperty.GetValue(tree) as System.Collections.IEnumerable;
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        var childType = child.GetType();
                        var childLabelProperty = childType.GetProperty("LabelClass") ?? childType.GetProperty("LabelProperty");
                        var childName = childLabelProperty?.GetValue(child)?.GetType().GetProperty("Name")?.GetValue(childLabelProperty.GetValue(child))?.ToString() ?? "子标签";

                        var childBtn = new Border
                        {
                            BackgroundColor = Colors.LightGray,
                            Padding = 8,
                            Margin = new Thickness(0, 0, 5, 5)
                        };

                        var childLabel = new Label
                        {
                            Text = childName,
                            FontSize = 11,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center
                        };

                        childBtn.Content = childLabel;

                        var childTapGesture = new TapGestureRecognizer();
                        childTapGesture.Tapped += (s, e) =>
                        {
                            if (childBtn.BackgroundColor == Colors.LightGray)
                            {
                                childBtn.BackgroundColor = Colors.Green;
                                childLabel.TextColor = Colors.White;
                            }
                            else
                            {
                                childBtn.BackgroundColor = Colors.LightGray;
                                childLabel.TextColor = Colors.Black;
                            }

                            CheckChangedCommand?.Execute(null);
                        };
                        childBtn.GestureRecognizers.Add(childTapGesture);

                        Children.Add(childBtn);
                    }
                }
            }
        }
    }
}
