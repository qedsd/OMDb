# OMDb WinUI3 迁移到 .NET 10 MAUI 计划文档

## 项目概述

**目标**: 将 OMDb 电影数据库应用从 WinUI 3 (.NET 6) 迁移到 .NET 10 MAUI (跨平台)

**支持平台**: Windows, macOS, Android, iOS, MacCatalyst

**目标框架**: .NET 10 (LTS)

**当前状态**: WinUI3 项目可成功编译

---

## 一、项目结构分析

### 1.1 当前架构

```
OMDb.sln
├── OMDb.WinUI3/OMDb.WinUI3.csproj    # WinUI3 主项目 (.NET 6, Windows 独占)
├── OMDb.Core/OMDb.Core.csproj        # 核心业务层 (.NET 6)
├── OMDb.Douban/OMDb.Douban.csproj    # 豆瓣数据插件 (.NET 6)
├── OMDb.JavDb/OMDb.JavDb.csproj      # JavDb 数据插件 (.NET 6)
└── OMDb.IMDb/OMDb.IMDb.csproj        # IMDb 数据插件 (.NET 6)
```

### 1.2 目标架构 (.NET 10)

```
OMDb.sln
├── OMDb.Maui/                        # 新建 - .NET 10 MAUI 主项目
├── OMDb.Core/                        # 迁移为 .NET 10 类库
├── OMDb.Douban/                      # 迁移为 .NET 10 类库
├── OMDb.JavDb/                       # 迁移为 .NET 10 类库
└── OMDb.IMDb/                        # 迁移为 .NET 10 类库
```

---

## 二、技术栈对比

### 2.1 当前依赖 (WinUI3 / .NET 6)

```xml
<TargetFramework>net6.0-windows10.0.19041.0</TargetFramework>

<PackageReference Include="Microsoft.WindowsAppSDK" Version="1.3.230602002" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.1" />
<PackageReference Include="CommunityToolkit.WinUI.UI.Controls" Version="7.1.2" />
<PackageReference Include="DotNetCore.NPOI" Version="1.2.3" />
<PackageReference Include="Xabe.FFmpeg" Version="5.2.6" />
```

### 2.2 目标依赖 (.NET 10 MAUI)

```xml
<TargetFrameworks>net10.0-android;net10.0-ios;net10.0-maccatalyst;net10.0-windows10.0.19041.0</TargetFrameworks>
<RuntimeIdentifiers>win-x64;osx-x64;osx-arm64;android-arm64;ios-arm64</RuntimeIdentifiers>

<PackageReference Include="Microsoft.Maui.Controls" Version="10.0.x" />
<PackageReference Include="Microsoft.Maui.Controls.Compatibility" Version="10.0.x" />
<PackageReference Include="CommunityToolkit.Maui" Version="10.x" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="9.x" />
<PackageReference Include="DotNetCore.NPOI" Version="1.2.3" />
<PackageReference Include="Xabe.FFmpeg" Version="5.2.6" />
<PackageReference Include="SQLite-net-pcl" Version="1.9.x" />
```

### 2.3 .NET 10 MAUI 新特性利用

| 特性 | 说明 |
|------|------|
| **AOT 编译** | iOS/Android 原生 AOT，Windows/Mac 可选 |
| **Hot Reload 2.0** | 改进的热重载，支持更多场景 |
| **改进的 Blazor Hybrid** | 可选 Blazor 视图用于复杂 UI |
| **原生 AOT 支持** | 更小的包体积，更快的启动 |
| **统一的 API** | .NET 10 统一所有平台的 API |

---

## 三、控件映射表

### 3.1 核心控件替换

| WinUI 3 控件 | MAUI 替代方案 | 复杂度 | 备注 |
|-------------|--------------|--------|------|
| `TabView` | `TabBar` + `Shell` 或 `TabView` (CommunityToolkit) | 中 | .NET 10 有原生 TabView |
| `NavigationView` | `Flyout` + `Shell` | 低 | |
| `Frame` (导航) | `Shell.Navigation` | 中 | |
| `InfoBar` | `CommunityToolkit.Maui Snackbar` | 低 | |
| `ProgressRing` | `ActivityIndicator` | 低 | |
| `Expander` | `Expander` (MAUI 原生) | 低 | .NET 9+ 原生支持 |
| `ContentDialog` | `CommunityToolkit.Maui Popup` | 中 | |
| `TextBlock` | `Label` | 低 | |
| `TextBox` | `Entry` / `Editor` | 低 | |
| `ListView` | `CollectionView` | 低 | |
| `GridView` | `CollectionView` (ItemsLayout) | 中 | |
| `ComboBox` | `Picker` | 低 | |
| `ToggleSwitch` | `Switch` | 低 | |
| `Slider` | `Slider` | 低 | |
| `CalendarView` | `CalendarPicker` (.NET 8+) | 低 | |
| `ColorPicker` | `RgbColorPicker` (.NET 9+) | 低 | |

### 3.2 WinUI3 特有功能迁移

| 功能 | WinUI3 实现 | MAUI 实现 |
|------|------------|----------|
| 自定义标题栏 | `AppWindowTitleBar` | `Window.TitleBar` + 平台特定 |
| 标签页拖拽 | `TabView.CanDragTabs` | CommunityToolkit.TabView |
| 系统托盘 | `SystemTray` | 平台特定 API |
| 文件选择器 | `FileOpenPicker` | `FileSystem.PickAsync()` |
| 启动外部程序 | `Launcher.LaunchUriAsync` | `Launcher.OpenAsync()` |
| 剪贴板 | `Clipboard` | `Clipboard.Default` |
| 主题切换 | `ElementTheme` | `Application.UserAppTheme` |
| 分屏/多窗口 | `AppWindow` | `IWindow` + `OpenWindow()` |

---

## 四、XAML 语法差异

### 4.1 命名空间

```xml
<!-- WinUI3 -->
<Page
    x:Class="OMDb.WinUI3.Views.HomePage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:OMDb.WinUI3.MyControls">
</Page>

<!-- MAUI (.NET 10) -->
<ContentPage
    x:Class="OMDb.Maui.Views.HomePage"
    xmlns="http://schemas.dot.net/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:local="clr-namespace:OMDb.Maui.Controls"
    xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit">
</ContentPage>
```

### 4.2 属性绑定

```xml
<!-- WinUI3 - x:Bind (编译时绑定) -->
<TextBlock Text="{x:Bind VM.Title, Mode=OneWay}" />
<Grid Visibility="{x:Bind VM.IsVisible, Converter={StaticResource BoolConverter}}" />

<!-- MAUI - Binding (运行时绑定) -->
<Label Text="{Binding Title, Mode=OneWay}" />
<Grid IsVisible="{Binding IsVisible}" />

<!-- MAUI - Compiled Binding (.NET 8+, 性能接近 x:Bind) -->
<Label Text="{CompiledBinding Title}" />
```

### 4.3 资源引用

```xml
<!-- WinUI3 -->
<StackPanel Margin="{StaticResource MediumMargin}">
    <Brush x:Key="MyBrush">{ThemeResource SystemAccentColor}</Brush>
</StackPanel>

<!-- MAUI -->
<VerticalStackLayout Margin="{StaticResource MediumMargin}">
    <Brush x:Key="MyBrush">{DynamicResource SystemAccentColor}</Brush>
</VerticalStackLayout>
```

### 4.4 样式和主题

```xml
<!-- MAUI .NET 10 隐式样式 -->
<ResourceDictionary>
    <Style TargetType="Label">
        <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource DarkText}, Dark={StaticResource LightText}}" />
    </Style>
</ResourceDictionary>
```

---

## 五、必须重写的组件清单

### 5.1 高优先级 (核心框架)

| 文件/目录 | 说明 | 预估工作量 | 优先级 |
|-----------|------|-----------|--------|
| `App.xaml/App.xaml.cs` | 应用入口和资源配置 | 0.5 天 | P0 |
| `Wins/MainWindow.xaml` | 窗口管理 | 1 天 | P0 |
| `Views/ShellPage.xaml` | 主导航框架 (TabView) | 2 天 | P0 |
| `Services/NavigationService.cs` | 导航服务 | 0.5 天 | P0 |
| `Services/ThemeSelectorService.cs` | 主题服务 | 0.5 天 | P1 |

### 5.2 中优先级 (自定义控件)

| 目录 | 文件数 | 说明 | 预估工作量 |
|------|--------|------|-----------|
| `MyControls/` | ~20 个 | 自定义控件 (卡片、弹窗、选择器等) | 5 天 |
| `Converters/` | ~20 个 | 值转换器 (需重写为 IValueConverter) | 1 天 |
| `Dialogs/` | ~15 个 | 对话框页面 (改为 Popup) | 3 天 |

### 5.3 业务页面

| 目录 | 文件数 | 说明 | 预估工作量 |
|------|--------|------|-----------|
| `Views/Home*` | 5 个 | 首页、统计、随机等 | 2 天 |
| `Views/Management/` | 4 个 | 标签、存储、属性管理 | 2 天 |
| `Views/Tools/` | 3 个 | 工具页面 (字幕、音视频) | 1 天 |
| `Views/Entry*` | 3 个 | 词条相关页面 | 1.5 天 |

### 5.4 需移除的 Windows 专属代码

```csharp
// ❌ 移除这些命名空间
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml.Markup;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

// ✅ 替换为 MAUI 命名空间
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Converters;

// ✅ 平台特定代码使用条件编译
#if WINDOWS
    // Windows 特定逻辑
#elif ANDROID
    // Android 特定逻辑
#elif IOS
    // iOS 特定逻辑
#endif
```

---

## 六、迁移步骤详解

### 阶段 1: 环境搭建 (0.5 天)

#### 1.1 安装 .NET 10 SDK

```bash
# 检查已安装的 SDK
dotnet --list-sdks

# 需要安装 .NET 10 SDK
# 下载地址：https://dotnet.microsoft.com/download/dotnet/10
```

#### 1.2 创建 MAUI 项目

```bash
# 创建新的 MAUI 项目
dotnet new maui -n OMDb.Maui -f net10.0 -o OMDb.Maui

# 进入项目目录
cd OMDb.Maui

# 添加 CommunityToolkit.MaUI (需要 10.x 版本支持 .NET 10)
dotnet add package CommunityToolkit.Maui --version 10.*

# 添加 CommunityToolkit.Mvvm
dotnet add package CommunityToolkit.Mvvm --version 9.*

# 添加 SQLite (如果需要本地数据库)
dotnet add package SQLite-net-pcl
```

#### 1.3 配置 .csproj 文件

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFrameworks>net10.0-android;net10.0-ios;net10.0-maccatalyst;net10.0-windows10.0.19041.0</TargetFrameworks>
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net10.0-windows10.0.19041.0</TargetFrameworks>
    
    <!-- 输出类型 -->
    <OutputType>Exe</OutputType>
    
    <!-- MAUI 特定 -->
    <UseMaui>true</UseMaui>
    <SingleProject>true</SingleProject>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    
    <!-- 应用信息 -->
    <ApplicationId>com.omdb.movies</ApplicationId>
    <ApplicationVersion>1</ApplicationVersion>
    <ApplicationDisplayVersion>1.0.0</ApplicationDisplayVersion>
    
    <!-- Windows 特定 -->
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.17763.0</SupportedOSPlatformVersion>
    <TargetPlatformMinVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'windows'">10.0.17763.0</TargetPlatformMinVersion>
    
    <!-- Android 特定 -->
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">24.0</SupportedOSPlatformVersion>
    
    <!-- iOS 特定 -->
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'ios'">15.0</SupportedOSPlatformVersion>
    <SupportedOSPlatformVersion Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst'">15.0</SupportedOSPlatformVersion>
    
    <!-- AOT 编译 (可选，减小包体积) -->
    <PublishAot Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'">true</PublishAot>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Maui.Controls" Version="10.*" />
    <PackageReference Include="Microsoft.Maui.Controls.Compatibility" Version="10.*" />
    <PackageReference Include="CommunityToolkit.Maui" Version="10.*" />
    <PackageReference Include="CommunityToolkit.Mvvm" Version="9.*" />
  </ItemGroup>

</Project>
```

### 阶段 2: 核心层迁移到 .NET 10 (1-2 天)

#### 2.1 修改 OMDb.Core.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="SQLite-net-pcl" Version="1.9.*" />
    <PackageReference Include="System.Text.Json" Version="10.*" />
  </ItemGroup>

</Project>
```

#### 2.2 修复兼容性警告

```bash
# 构建并查看警告
dotnet build OMDb.Core/OMDb.Core.csproj -v detailed

# 重点检查:
# - SYSLIB 警告 (过时 API)
# - CA 警告 (代码分析)
```

#### 2.3 替换过时 API

```csharp
// ❌ 过时 (WebRequest)
var request = WebRequest.Create(url);

// ✅ 替换为 HttpClient
using var httpClient = new HttpClient();
var response = await httpClient.GetAsync(url);
```

### 阶段 3: MAUI 框架搭建 (2-3 天)

#### 3.1 创建 Shell 导航

```csharp
// AppShell.cs - 替代 ShellPage.xaml
namespace OMDb.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        Routing.RegisterRoute(nameof(Views.HomePage), typeof(Views.HomePage));
        Routing.RegisterRoute(nameof(Views.EntryDetailPage), typeof(Views.EntryDetailPage));
        Routing.RegisterRoute(nameof(Views.ManagementPage), typeof(Views.ManagementPage));
        
        Items.Add(new FlyoutItem
        {
            Title = "OMDb",
            Icon = "icon_home.png",
            Items = new ShellContent
            {
                Title = "首页",
                Icon = "icon_home.png",
                ContentTemplate = new DataTemplate(typeof(Views.HomePage)),
                Route = nameof(Views.HomePage)
            }
        });
    }
}
```

```xml
<!-- AppShell.xaml (可选，用于 Flyout 自定义) -->
<Shell xmlns="http://schemas.dot.net/2021/maui"
       xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
       x:Class="OMDb.Maui.AppShell"
       FlyoutHeaderBehavior="CollapseOnScroll">
    
    <FlyoutItem>
        <ShellContent Title="首页" Icon="icon_home.png" ContentTemplate="{DataTemplate local:HomePage}" />
        <ShellContent Title="分类" Icon="icon_category.png" ContentTemplate="{DataTemplate local:ClassificationPage}" />
        <ShellContent Title="片单" Icon="icon_collection.png" ContentTemplate="{DataTemplate local:CollectionsPage}" />
        <ShellContent Title="词条" Icon="icon_entry.png" ContentTemplate="{DataTemplate local:EntryHomePage}" />
        <ShellContent Title="管理" Icon="icon_manage.png" ContentTemplate="{DataTemplate local:ManagementPage}" />
        <ShellContent Title="工具" Icon="icon_tool.png" ContentTemplate="{DataTemplate local:ToolsPage}" />
        <ShellContent Title="设置" Icon="icon_setting.png" ContentTemplate="{DataTemplate local:SettingPage}" />
    </FlyoutItem>
</Shell>
```

#### 3.2 配置 MauiProgram.cs

```csharp
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using OMDb.Core.Services;
using OMDb.Maui.Services;
using OMDb.Maui.ViewModels;

namespace OMDb.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        // 应用配置
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit(options =>
            {
                options.SetShouldSuppressExceptionsInConverters(true);
                options.SetShouldSuppressExceptionsInBehaviors(true);
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Segoe-MDL2.ttf", "SegoeMDL2"); // WinUI Segoe 图标
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // 注册核心服务
        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
        builder.Services.AddSingleton<IShare>(Share.Default);
        builder.Services.AddSingleton<IFileSystem>(FileSystem.Current);
        
        // 注册应用服务
        builder.Services.AddSingleton<ConfigService>();
        builder.Services.AddSingleton<NavigationService>();
        builder.Services.AddSingleton<ThemeService>();
        
        // 注册数据服务
        builder.Services.AddSingleton<EntryService>();
        builder.Services.AddSingleton<LabelService>();
        builder.Services.AddSingleton<StorageService>();
        
        // 注册 ViewModel
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<EntryDetailViewModel>();
        builder.Services.AddTransient<ManagementViewModel>();
        
        // 注册页面
        builder.Services.AddTransient<Views.HomePage>();
        builder.Services.AddTransient<Views.EntryDetailPage>();
        builder.Services.AddTransient<Views.ManagementPage>();

        return builder.Build();
    }
}
```

#### 3.3 实现基础服务

```csharp
// Services/ThemeService.cs
namespace OMDb.Maui.Services;

public class ThemeService
{
    public void SetTheme(AppTheme theme)
    {
        Application.Current.UserAppTheme = theme;
    }
    
    public AppTheme GetCurrentTheme()
    {
        return Application.Current.UserAppTheme;
    }
    
    public void ToggleTheme()
    {
        Application.Current.UserAppTheme = 
            Application.Current.UserAppTheme == AppTheme.Light 
                ? AppTheme.Dark 
                : AppTheme.Light;
    }
}

// Services/DialogService.cs (使用 CommunityToolkit Popup)
using CommunityToolkit.Maui.Core;

namespace OMDb.Maui.Services;

public class DialogService
{
    private readonly IPopupService _popupService;
    
    public DialogService(IPopupService popupService)
    {
        _popupService = popupService;
    }
    
    public async Task ShowAlertAsync(string title, string message)
    {
        await _popupService.ShowPopupAsync(new Views.Dialogs.AlertPopup
        {
            Title = title,
            Message = message
        });
    }
    
    public async Task<bool> ShowConfirmAsync(string title, string message)
    {
        var result = await _popupService.ShowPopupAsync(new Views.Dialogs.ConfirmPopup
        {
            Title = title,
            Message = message
        });
        
        return result is bool b && b;
    }
}
```

### 阶段 4: 页面迁移 (5-7 天)

#### 4.1 页面迁移模板

```csharp
// ===== BEFORE (WinUI3) =====
namespace OMDb.WinUI3.Views;

public sealed partial class HomePage : Page
{
    public HomeViewModel VM { get; set; }
    
    public HomePage()
    {
        InitializeComponent();
        VM = new HomeViewModel();
        DataContext = VM;
    }
}

// ===== AFTER (MAUI .NET 10) =====
namespace OMDb.Maui.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;
    
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearingCommand.Execute(null);
    }
}
```

#### 4.2 XAML 迁移示例

```xml
<!-- ===== BEFORE (WinUI3) ===== -->
<Page
    x:Class="OMDb.WinUI3.Views.HomePage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:OMDb.WinUI3.Views"
    Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
    
    <Grid Padding="20">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>
        
        <TextBlock Text="{x:Bind VM.Title, Mode=OneWay}" 
                   Style="{StaticResource TitleTextBlockStyle}" />
        
        <ListView Grid.Row="1" 
                  ItemsSource="{x:Bind VM.Entries, Mode=OneWay}"
                  SelectionMode="Single">
            <ListView.ItemTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Horizontal">
                        <Image Source="{Binding CoverUrl}" Width="100" Height="150" />
                        <StackPanel Margin="10,0,0,0">
                            <TextBlock Text="{Binding Title}" />
                            <TextBlock Text="{Binding Year}" />
                        </StackPanel>
                    </StackPanel>
                </DataTemplate>
            </ListView.ItemTemplate>
        </ListView>
    </Grid>
</Page>

<!-- ===== AFTER (MAUI .NET 10) ===== -->
<ContentPage
    x:Class="OMDb.Maui.Views.HomePage"
    xmlns="http://schemas.dot.net/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:local="clr-namespace:OMDb.Maui.Views"
    xmlns:vm="clr-namespace:OMDb.Maui.ViewModels"
    x:DataType="vm:HomeViewModel"
    BackgroundColor="{AppThemeBinding Light={StaticResource LightBackground}, Dark={StaticResource DarkBackground}}">
    
    <VerticalStackLayout Padding="20" Spacing="10">
        <Label Text="{Binding Title}" 
               Style="{StaticResource HeadlineLabelStyle}" />
        
        <CollectionView ItemsSource="{Binding Entries}"
                        SelectionMode="Single"
                        SelectionChangedCommand="{Binding EntrySelectedCommand}">
            <CollectionView.ItemsLayout>
                <LinearItemsLayout Orientation="Vertical" ItemSpacing="10" />
            </CollectionView.ItemsLayout>
            <CollectionView.ItemTemplate>
                <DataTemplate x:DataType="local:EntryCard">
                    <HorizontalStackLayout>
                        <Image Source="{Binding CoverUrl}" 
                               WidthRequest="100" 
                               HeightRequest="150" 
                               Aspect="AspectFill" />
                        <VerticalStackLayout Margin="10,0,0,0" Spacing="5">
                            <Label Text="{Binding Title}" Style="{StaticResource BodyLabelStyle}" />
                            <Label Text="{Binding Year}" Style="{StaticResource CaptionLabelStyle}" />
                        </VerticalStackLayout>
                    </HorizontalStackLayout>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>
    </VerticalStackLayout>
</ContentPage>
```

### 阶段 5: 自定义控件重写 (3-4 天)

#### 5.1 控件基类变更

```csharp
// ===== BEFORE (WinUI3) =====
public sealed partial class EntryCard : UserControl
{
    public static readonly DependencyProperty EntryProperty =
        DependencyProperty.Register(nameof(Entry), typeof(EntryInfo), typeof(EntryCard), ...);
}

// ===== AFTER (MAUI .NET 10) =====
public partial class EntryCard : Border
{
    public static readonly BindableProperty EntryProperty =
        BindableProperty.Create(nameof(Entry), typeof(EntryInfo), typeof(EntryCard), default(EntryInfo));
    
    public EntryInfo Entry
    {
        get => (EntryInfo)GetValue(EntryProperty);
        set => SetValue(EntryProperty, value);
    }
    
    public EntryCard()
    {
        InitializeComponent();
        BindingContext = this;
    }
}
```

#### 5.2 使用 .NET 10 新控件

```xml
<!-- .NET 9+ 原生 TabView (CommunityToolkit) -->
<toolkit:TabView>
    <toolkit:TabViewItem Header="首页" Content="{Binding HomePageContent}" />
    <toolkit:TabViewItem Header="分类" Content="{Binding CategoryPageContent}" />
</toolkit:TabView>

<!-- .NET 9+ Expander -->
<Expander Header="高级选项">
    <VerticalStackLayout Padding="10">
        <!-- 展开内容 -->
    </VerticalStackLayout>
</Expander>

<!-- .NET 8+ CalendarPicker -->
<CalendarPicker Date="{Binding SelectedDate}" />
```

### 阶段 6: 平台特定功能实现 (2-3 天)

#### 6.1 文件访问

```csharp
// Services/FileService.cs
namespace OMDb.Maui.Services;

public class FileService
{
    public async Task<string> PickFileAsync(string[] fileTypes)
    {
        var customFileType = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.movie", "public.video" } },
                { DevicePlatform.Android, new[] { "video/*", "application/octet-stream" } },
                { DevicePlatform.WinUI, new[] { ".mp4", ".mkv", ".avi" } },
                { DevicePlatform.macOS, new[] { "mp4", "mkv", "avi" } }
            }
        );
        
        var options = new PickOptions
        {
            PickerTitle = "选择视频文件",
            FileTypes = customFileType
        };
        
        var result = await FilePicker.PickAsync(options);
        return result?.FullPath;
    }
    
    public async Task OpenFileAsync(string filePath)
    {
        await Launcher.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(filePath)
        });
    }
}
```

#### 6.2 窗口管理

```csharp
// Services/WindowService.cs
namespace OMDb.Maui.Services;

public class WindowService
{
    public async Task OpenNewWindowAsync(Page page)
    {
        var window = new Window(page);
        await Application.Current.OpenWindowAsync(window);
    }
    
    // Windows 平台特定的多窗口
    public void ActivateExistingWindow(string windowId)
    {
#if WINDOWS
        // 使用 WinUI 3 API 管理 Windows 窗口
#endif
    }
}
```

#### 6.3 FFmpeg 集成 (Xabe.FFmpeg)

```csharp
// Services/FFmpegService.cs
using Xabe.FFmpeg;

namespace OMDb.Maui.Services;

public class FFmpegService
{
    public async Task ConvertVideoAsync(string inputFile, string outputFile, string format)
    {
        var conversion = await FFmpeg.Conversions.New()
            .AddStream(new VideoStream(inputFile))
            .SetOutputFormat(format)
            .SetOutput(outputFile)
            .Start();
    }
    
    // 平台特定的 FFmpeg 二进制路径
    public void SetupFFmpegPath()
    {
        string ffmpegPath = Path.Combine(FileSystem.AppDataDirectory, "ffmpeg");
        
#if ANDROID
        // 从 Resources/raw 复制
#elif IOS
        // 从 Bundle 复制
#elif WINDOWS
        ffmpegPath = Path.Combine(AppContext.BaseDirectory, "ffmpeg");
#endif
        
        FFmpeg.SetExecutablesPath(ffmpegPath);
    }
}
```

### 阶段 7: 测试和优化 (2-3 天)

#### 7.1 各平台编译测试

```bash
# Windows
dotnet build -f net10.0-windows10.0.19041.0 -c Release

# Android
dotnet build -f net10.0-android -c Release

# macOS
dotnet build -f net10.0-maccatalyst -c Release

# iOS (需要 Mac)
dotnet build -f net10.0-ios -c Release
```

#### 7.2 性能优化

```xml
<!-- .csproj - 启用 AOT 编译 -->
<PropertyGroup Condition="'$(TargetFramework)' == 'net10.0-android'">
    <PublishAot>true</PublishAot>
    <TrimMode>link</TrimMode>
    <AndroidEnableProfiledAot>true</AndroidEnableProfiledAot>
</PropertyGroup>

<!-- 减小包体积 -->
<PropertyGroup>
    <AndroidPackageFormat>apk</AndroidPackageFormat> <!-- 或 aab -->
    <AndroidCreatePackagePerAbi>true</AndroidCreatePackagePerAbi>
</PropertyGroup>
```

---

## 七、已知技术难点和解决方案

### 7.1 TabView 标签页系统

**问题**: MAUI 早期版本没有原生 TabView

**解决方案 (.NET 10)**:
```xml
<!-- 方案 A: 使用 Shell TabBar (推荐) -->
<Shell>
    <TabBar>
        <Tab Title="首页" Icon="home.png">
            <ShellContent ContentTemplate="{DataTemplate local:HomePage}" />
        </Tab>
        <Tab Title="分类" Icon="category.png">
            <ShellContent ContentTemplate="{DataTemplate local:CategoryPage}" />
        </Tab>
    </TabBar>
</Shell>

<!-- 方案 B: CommunityToolkit TabView (更灵活) -->
<toolkit:TabView TabPlacement="Bottom">
    <toolkit:TabViewItem>
        <toolkit:TabViewItem.Header>
            <StackLayout Orientation="Horizontal">
                <Image Source="home.png" />
                <Label Text="首页" />
            </StackLayout>
        </toolkit:TabViewItem.Header>
        <ContentView Content="{Binding HomePageContent}" />
    </toolkit:TabViewItem>
</toolkit:TabView>
```

### 7.2 对话框系统

**问题**: MAUI 没有 ContentDialog

**解决方案**:
```csharp
// 使用 CommunityToolkit.Maui Popup
public class ConfirmPopup : Popup
{
    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(ConfirmPopup));
    
    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }
    
    public event EventHandler<bool> OnConfirm;
    
    public ConfirmPopup()
    {
        Content = new StackLayout
        {
            Padding = 20,
            Children =
            {
                new Label { Text = Message },
                new Button { Text = "确认", Command = new Command(() => { OnConfirm?.Invoke(this, true); Close(true); }) },
                new Button { Text = "取消", Command = new Command(() => { OnConfirm?.Invoke(this, false); Close(false); }) }
            }
        };
    }
}

// 使用
var popup = new ConfirmPopup { Message = "确定删除？" };
var result = await this.ShowPopupAsync(popup);
```

### 7.3 多窗口管理

**问题**: MAUI 多窗口支持在各平台不一致

**解决方案**:
```csharp
public class MultiWindowService
{
    private readonly Dictionary<string, Window> _windows = new();
    
    public async Task CreateWindowAsync(string id, Page page)
    {
        var window = new Window(page);
        _windows[id] = window;
        
        // 监听窗口关闭
        window.Destroying += (s, e) => _windows.Remove(id);
        
        await Application.Current.OpenWindowAsync(window);
    }
    
    public async Task ActivateWindowAsync(string id)
    {
        if (_windows.TryGetValue(id, out var window))
        {
            await window.ActivateAsync();
        }
    }
}
```

### 7.4 平台特定 UI 适配

```csharp
// 使用 OnPlatform 和 OnIdiom
<Label>
    <Label.FontSize>
        <OnPlatform x:TypeArguments="x:Double">
            <On Platform="iOS" Value="16" />
            <On Platform="Android" Value="14" />
            <On Platform="Windows" Value="15" />
        </OnPlatform>
    </Label.FontSize>
    <Label.Padding>
        <OnIdiom x:TypeArguments="Thickness">
            <OnIdiom.Phone Value="10" />
            <OnIdiom.Tablet Value="20" />
            <OnIdiom.Desktop Value="15" />
        </OnIdiom>
    </Label.Padding>
</Label>
```

---

## 八、预估工作量和时间线

| 阶段 | 任务 | 预估时间 | 依赖 |
|------|------|---------|------|
| 阶段 1 | 环境搭建 (.NET 10 SDK, 项目创建) | 0.5 天 | 无 |
| 阶段 2 | 核心层迁移 (OMDb.Core 等) | 1.5 天 | 阶段 1 |
| 阶段 3 | MAUI 框架搭建 (Shell, DI, 服务) | 2.5 天 | 阶段 2 |
| 阶段 4 | 页面迁移 (Views, ViewModels) | 6 天 | 阶段 3 |
| 阶段 5 | 自定义控件重写 (MyControls, Dialogs) | 4 天 | 阶段 3 |
| 阶段 6 | 平台特定功能 (文件，窗口，FFmpeg) | 2.5 天 | 阶段 4 |
| 阶段 7 | 测试和优化 (各平台编译，性能) | 2.5 天 | 阶段 6 |
| **总计** | | **~19.5 天** | |

### 里程碑

| 里程碑 | 目标 | 预计时间 |
|--------|------|---------|
| M1 | 框架搭建完成，Hello World 运行 | 第 3 天 |
| M2 | 核心页面迁移完成 | 第 10 天 |
| M3 | 所有功能迁移完成 | 第 17 天 |
| M4 | 各平台发布版本 | 第 20 天 |

---

## 九、验收标准

### 功能验收

- [ ] 首页显示和导航正常工作
- [ ] 词条浏览、搜索、详情查看正常工作
- [ ] 分类、片单、标签管理功能正常
- [ ] 存储管理功能正常
- [ ] 工具功能 (字幕、音视频转换) 正常
- [ ] 设置页面功能正常

### 平台验收

- [ ] **Windows 10/11**: 启动、导航、文件操作、多窗口正常
- [ ] **Android 10+**: 触摸交互、文件选择、后台处理正常
- [ ] **macOS 12+**: 菜单、快捷键、文件操作正常
- [ ] **iOS 15+**: 触摸交互、沙盒文件访问正常

### 质量验收

- [ ] 主题切换 (明/暗) 在所有页面正常
- [ ] 无内存泄漏 (使用 Profiler 验证)
- [ ] 启动时间 < 3 秒 (冷启动)
- [ ] 应用包体积合理 (Android APK < 100MB)
- [ ] 无未处理的崩溃

---

## 十、风险和建议

### 技术风险

| 风险 | 影响 | 缓解措施 |
|------|------|---------|
| .NET 10 MAUI 稳定性 | 新版本可能有 bug | 使用 LTS 版本，关注 Release Notes |
| iOS 沙盒限制 | 文件访问受限 | 使用 `FileSystem.AppDataDirectory` |
| Android 后台限制 | FFmpeg 处理可能被杀 | 使用 `ForegroundService` |
| macOS 签名 | 需要开发者证书 | 提前申请 Apple Developer |
| Windows 多窗口 | API 不成熟 | 使用单窗口 + Tab 模式 |

### 建议

1. **分阶段发布**
   - 先发布 Windows 版本 (最接近原 WinUI3)
   - 再发布 Android 版本
   - 最后发布 macOS/iOS 版本

2. **保留 WinUI3 代码**
   - 作为参考和 fallback
   - 用于对比功能完整性

3. **大量使用条件编译**
   ```csharp
   #if ANDROID
       // Android 优化
   #elif IOS
       // iOS 优化
   #elif WINDOWS
       // Windows 优化
   #endif
   ```

4. **优先考虑触摸体验**
   - 移动端以触摸交互为优先
   - 桌面端保留键鼠优化

5. **使用 Blazor Hybrid (可选)**
   - 复杂表单页面可用 BlazorWebView
   - 复用 Web 前端代码

---

## 附录 A: 关键文件对照表

| WinUI3 源文件 | MAUI 目标文件 | 迁移类型 | 状态 |
|--------------|--------------|---------|------|
| `App.xaml` | `App.xaml` | 语法转换 | ⏳ |
| `Wins/MainWindow.xaml` | `AppShell.xaml` | 重写 | ⏳ |
| `Views/ShellPage.xaml` | `AppShell.xaml` | 重写 | ⏳ |
| `Views/HomePage.xaml` | `Views/HomePage.xaml` | 语法转换 | ⏳ |
| `Views/EntryDetailPage.xaml` | `Views/EntryDetailPage.xaml` | 语法转换 | ⏳ |
| `MyControls/*.xaml` | `Controls/*.xaml` | 重写 | ⏳ |
| `Dialogs/*.xaml` | `Popups/*.xaml` | 重写 (Popup) | ⏳ |
| `Converters/*.cs` | `Converters/*.cs` | 接口调整 | ⏳ |
| `ViewModels/*.cs` | `ViewModels/*.cs` | 小调整 | ⏳ |
| `Services/*.cs` | `Services/*.cs` | 平台适配 | ⏳ |

---

## 附录 B: .NET 10 MAUI 新特性

### B.1 性能改进

- **AOT 编译默认启用** (Android/iOS)
- **更快的 XAML 加载**
- **改进的内存管理**
- **启动时间优化 30%+**

### B.2 新控件

- `CalendarPicker` - 日历选择器
- `DateRangePicker` - 日期范围选择
- `RgbColorPicker` - 颜色选择器
- `Stepper` - 步进器
- `GraphicsView` - 图形绘制

### B.3 改进的 API

```csharp
// .NET 10 简化的窗口 API
await Application.Current.OpenWindowAsync(window);

// 改进的文件访问
var file = await FilePicker.PickAsync(options);

// 简化的权限请求
var status = await Permissions.RequestAsync<Permissions.Camera>();
```

---

## 附录 C: 常用代码片段

### C.1 通用 ViewModel 基类

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OMDb.Maui.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isLoading;
    
    [ObservableProperty]
    private string title;
    
    [ObservableProperty]
    private bool isBusy;
    
    [RelayCommand]
    protected async Task ExecuteWithLoadingAsync(Func<Task> action)
    {
        if (IsBusy) return;
        
        try
        {
            IsBusy = true;
            IsLoading = true;
            await action();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("错误", ex.Message, "确定");
        }
        finally
        {
            IsLoading = false;
            IsBusy = false;
        }
    }
}
```

### C.2 通用转换器

```csharp
using System.Globalization;
using CommunityToolkit.Maui.Converters;

namespace OMDb.Maui.Converters;

// 空值转可见性
public class NullToVisibilityConverter : BaseConverterOneWay<object?, bool>
{
    public override bool DefaultConvertReturnValue { get; set; } = true;
    
    public override bool ConvertFrom(object? value, CultureInfo culture)
    {
        return value != null;
    }
}

// 布尔取反
public class InvertedBoolConverter : BaseConverterOneWay<bool, bool>
{
    public override bool DefaultConvertReturnValue { get; set; } = false;
    
    public override bool ConvertFrom(bool value, CultureInfo culture)
    {
        return !value;
    }
}

// 日期格式化
public class DateTimeFormatConverter : BaseConverterOneWay<DateTime?, string>
{
    public override string DefaultConvertReturnValue { get; set; } = string.Empty;
    
    public override string ConvertFrom(DateTime? value, CultureInfo culture)
    {
        return value?.ToString("yyyy-MM-dd") ?? string.Empty;
    }
}
```

### C.3 应用配置

```xml
<!-- App.xaml -->
<Application xmlns="http://schemas.dot.net/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="OMDb.Maui.App">
    <Application.Resources>
        <ResourceDictionary>
            
            <!-- 颜色 -->
            <Color x:Key="PrimaryColor">#512BD4</Color>
            <Color x:Key="LightBackground">#FFFFFF</Color>
            <Color x:Key="DarkBackground">#1C1C1E</Color>
            
            <!-- 主题绑定 -->
            <Style TargetType="Label">
                <Setter Property="TextColor" 
                        Value="{AppThemeBinding Light={StaticResource DarkText}, Dark={StaticResource LightText}}" />
            </Style>
            
            <!-- 间距 -->
            <Thickness x:Key="SmallPadding">10</Thickness>
            <Thickness x:Key="MediumPadding">15</Thickness>
            <Thickness x:Key="LargePadding">20</Thickness>
            
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

---

**文档版本**: 2.0 (.NET 10 版)  
**创建日期**: 2026-04-02  
**目标框架**: .NET 10.0  
**适用模型**: GPT-5 / Claude 4.5 / 其他 AI 编程助手  
**前置条件**: 已安装 .NET 10 SDK (https://dotnet.microsoft.com/download/dotnet/10)
