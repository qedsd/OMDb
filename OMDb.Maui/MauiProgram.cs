using Microsoft.Extensions.Logging;
using OMDb.Maui.ViewModels;
using OMDb.Maui.Views;

namespace OMDb.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// 注册 ViewModel
		builder.Services.AddSingleton<ShellViewModel>();
		builder.Services.AddSingleton<HomeViewModel>();
		builder.Services.AddSingleton<ClassificationViewModel>();
		builder.Services.AddSingleton<CollectionsViewModel>();

		// 注册页面
		builder.Services.AddTransient<HomePage>();
		builder.Services.AddTransient<ClassificationPage>();
		builder.Services.AddTransient<CollectionsPage>();
		builder.Services.AddTransient<EntryHomePage>();
		builder.Services.AddTransient<ManagementPage>();
		builder.Services.AddTransient<ToolsPage>();
		builder.Services.AddTransient<SettingPage>();

		return builder.Build();
	}
}
