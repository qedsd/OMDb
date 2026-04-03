using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.Maui.Testing;

/// <summary>
/// ViewModel 命令测试器 - 用于测试 ViewModel 的命令是否抛出异常
/// </summary>
public static class ViewModelCommandTester
{
    /// <summary>
    /// 测试命令执行结果
    /// </summary>
    public class TestResult
    {
        public string CommandName { get; set; }
        public bool Success { get; set; }
        public Exception Exception { get; set; }
        public TimeSpan Duration { get; set; }
    }

    /// <summary>
    /// 测试所有命令
    /// </summary>
    public static async Task<List<TestResult>> TestAllCommands(object viewModel, List<(string Name, ICommand Command)> commands)
    {
        var results = new List<TestResult>();

        foreach (var (name, command) in commands)
        {
            var result = await TestCommand(name, command);
            results.Add(result);
        }

        return results;
    }

    /// <summary>
    /// 测试单个命令
    /// </summary>
    public static async Task<TestResult> TestCommand(string commandName, ICommand command)
    {
        var result = new TestResult { CommandName = commandName };
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            if (command is IAsyncCommand asyncCommand)
            {
                await asyncCommand.ExecuteAsync(null);
            }
            else
            {
                command.Execute(null);
            }
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Exception = ex;
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
        }

        return result;
    }

    /// <summary>
    /// 测试命令在不同参数下的行为
    /// </summary>
    public static async Task<List<TestResult>> TestCommandWithParameters(string commandName, ICommand command, List<object> parameters)
    {
        var results = new List<TestResult>();

        foreach (var param in parameters)
        {
            var result = new TestResult { CommandName = $"{commandName}({param ?? "null"})" };
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                if (command is IAsyncCommand asyncCommand)
                {
                    await asyncCommand.ExecuteAsync(param);
                }
                else
                {
                    command.Execute(param);
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
            }
            finally
            {
                stopwatch.Stop();
                result.Duration = stopwatch.Elapsed;
            }

            results.Add(result);
        }

        return results;
    }
}

/// <summary>
/// 异步命令接口
/// </summary>
public interface IAsyncCommand : ICommand
{
    Task ExecuteAsync(object parameter);
}
