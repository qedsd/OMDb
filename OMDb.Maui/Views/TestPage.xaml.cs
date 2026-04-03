using System;
using System.Text;
using Microsoft.Maui.Controls;

namespace OMDb.Maui.Views;

public partial class TestPage : ContentPage
{
    public TestPage()
    {
        InitializeComponent();
    }

    private async void OnRunTestsClicked(object sender, EventArgs e)
    {
        RunTestsButton.IsEnabled = false;
        StatusLabel.Text = "正在运行测试...";
        ResultsLabel.Text = "";

        try
        {
            var results = await Testing.InAppViewModelTester.RunAllTests();

            var sb = new StringBuilder();
            int successCount = 0;
            int failCount = 0;

            foreach (var result in results)
            {
                if (result.Success)
                {
                    successCount++;
                    sb.AppendLine($"✓ {result.ViewModelName}.{result.CommandName} - 成功 ({result.Duration.TotalMilliseconds:F2}ms)");
                }
                else
                {
                    failCount++;
                    sb.AppendLine($"✗ {result.ViewModelName}.{result.CommandName} - 失败：{result.ErrorMessage}");
                }
            }

            StatusLabel.Text = $"测试完成：{successCount} 成功，{failCount} 失败";
            StatusLabel.TextColor = failCount > 0 ? Colors.Red : Colors.Green;
            ResultsLabel.Text = sb.ToString();
        }
        catch (Exception ex)
        {
            StatusLabel.Text = "测试运行出错";
            StatusLabel.TextColor = Colors.Red;
            ResultsLabel.Text = ex.ToString();
        }
        finally
        {
            RunTestsButton.IsEnabled = true;
        }
    }
}
