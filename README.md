# OMDb - 个人媒体数据库管理系统

## 软件简介

OMDb (Open Media Database) 是一个基于 .NET MAUI 开发的跨平台个人媒体资源管理系统。它可以帮助用户整理、分类和管理个人收藏的电影、电视剧、纪录片等视频资源，提供完善的元数据管理和便捷的浏览体验。

### 主要特性

- **跨平台支持**: 基于 .NET MAUI，支持 Windows、macOS、iOS、Android 等平台
- **媒体资源管理**: 支持视频、音频、字幕、图片等多种媒体文件管理
- **分类系统**: 支持多级标签分类，可自定义分类结构
- **片单功能**: 可创建多个片单，将词条添加到不同片单中
- **观看记录**: 记录每次观看的时间和进度，支持标记观看完结
- **台词摘录**: 收藏和整理影片中的经典台词
- **元数据编辑**: 支持自定义封面、描述、评分等信息

---

## 名词解释

### 词条 (Entry)

类似于字典词条、维基词条，在软件中词条代表着一个电影、电视剧、纪录片或任何您想添加进软件进行管理的资源。可以简单理解为：**词条 = 电影/电视剧/纪录片等**。

每个词条都会有自己的独立文件夹，用于存放词条专属文件：

```
词条文件夹/
├── Img/           # 图片文件（封面、剧照等）
├── Video/         # 视频文件
├── Sub/           # 字幕文件
├── Resource/      # 资源文件（BT 种子、下载链接等）
├── metadata.json  # 词条元数据文件
```

**注意**：
- 词条可以存在多个名称（别名），在词条详细页可编辑
- 词条文件夹名称与词条默认名称一致
- 各文件夹下存放的文件类型没有严格限制，但软件只会显示支持的文件格式

### 标签 (Label)

标签用于标记词条的分类，是筛选和查找词条的重要工具。

- 标签支持多级树形结构（父子标签）
- 标签是非核心功能，可选择性使用
- 词条本身不区分类型（电影/电视等），通过标签来附加属性进行区分

示例标签结构：
```
媒体类型
├── 电影
│   ├── 动作片
│   ├── 喜剧片
│   └── 科幻片
└── 电视剧
    ├── 国产剧
    ├── 美剧
    └── 日剧
```

### 仓库 (Storage)

每个词条都有自己的独立文件夹，当存在大量词条时，需要一个集中管理的容器——这就是仓库。

**重要**：创建词条前必须先创建仓库。

仓库结构：
```
仓库文件夹/
├── Entries/       # 所有词条文件夹的容器
├── OMDb.db        # SQLite 数据库，记录词条信息
└── (其他配置)
```

**仓库特点**：
- 支持多个仓库（可分布在不同硬盘/分区）
- 每个仓库独立管理自己的词条
- 数据库记录词条名称、路径、标签、观看记录等
- 便于筛选和快速定位词条

---

## 文件说明

### OMDb.db（数据库文件）

SQLite 数据库文件，记录仓库内所有词条的核心数据：
- 词条名称和基本信息
- 词条相对路径
- 标签关联
- 观看记录
- 片单信息

**用途**：用于词条筛选、搜索和快速定位。

### metadata.json（词条元数据文件）

每个词条文件夹下的 JSON 文件，记录词条的基本信息：
- 词条名称
- 描述
- 评分
- 台词摘录
- 其他扩展信息

**为什么需要两个存储？**

| OMDb.db | metadata.json |
|---------|---------------|
| 参与筛选、搜索的数据 | 仅查看时加载的数据 |
| 依赖程序读取 | 可直接用文本编辑器打开 |
| 集中管理所有词条 | 每个词条独立文件 |
| 减少依赖，便于迁移 | 不依赖软件也能读取基本信息 |

---

## 项目结构

```
OMDb/
├── OMDb.Core/                 # 核心业务逻辑和数据模型
│   ├── Models/               # 数据模型定义（Entry、Label 等）
│   ├── DbModels/             # 数据库模型
│   ├── Services/             # 数据访问服务
│   ├── Enums/                # 枚举类型
│   └── Helpers/              # 工具类
│
├── OMDb.Maui/                # MAUI 客户端应用
│   ├── Views/                # 页面视图（XAML）
│   ├── ViewModels/           # 视图模型（MVVM 模式）
│   ├── Models/               # UI 专用模型
│   ├── Converters/           # 值转换器
│   ├── Services/             # 应用服务
│   ├── Helpers/              # 辅助类
│   └── Resources/            # 资源和样式
│
├── OMDb.WinUI3/              # WinUI3 版本（参考实现）
├── OMDb.Maui.AutoTest/       # 自动化测试项目
└── README.md                 # 项目文档
```

---

## 功能模块详解

### 1. 主页模块 (HomeViewModel)

**文件**: `OMDb.Maui/ViewModels/HomeViewModel.cs`

**功能**:
- 显示最近更新的词条列表
- 显示最近观看的文件记录
- 显示随机推荐的词条
- 显示统计数据（词条总数、观看次数等）

**主要属性**:
| 属性名 | 说明 |
|--------|------|
| `RecentlyUpdatedEntries` | 最近更新的词条列表 |
| `RecentlyWatchedFiles` | 最近观看的文件记录 |
| `RandomEntries` | 随机推荐的词条 |
| `Statistics` | 统计数据 |

**主要命令**:
| 命令名 | 说明 |
|--------|------|
| `RefreshCommand` | 刷新主页数据 |
| `NavigateCommand` | 导航到指定页面 |

---

### 2. 分类模块 (ClassificationViewModel)

**文件**: `OMDb.Maui/ViewModels/ClassificationViewModel.cs`

**功能**:
- 显示标签分类树形结构
- 显示推荐合集
- 支持按分类筛选词条
- 支持列表视图和网格视图切换

**主要属性**:
| 属性名 | 说明 |
|--------|------|
| `LabelTrees` | 标签分类树（树形结构） |
| `LabelClasses` | 扁平化的标签列表 |
| `LabelCollectionTrees` | 推荐合集列表 |
| `BannerItemsSource` | 轮播图数据源 |
| `IsList` | 是否为列表视图模式 |

**主要命令**:
| 命令名 | 说明 |
|--------|------|
| `LabelDetailCommand` | 查看标签详情 |
| `BannerDetailCommand` | 查看轮播图详情 |
| `ChangeShowTypeCommand` | 切换显示模式 |
| `RefreshCommand` | 刷新分类数据 |

**初始化流程**:
1. `InitLabels()`: 加载所有标签，构建树形结构
2. `InitBannerAsync()`: 初始化轮播图数据
3. `InitLabelCollectionAsync()`: 初始化推荐合集

---

### 3. 片单模块 (CollectionsViewModel)

**文件**: `OMDb.Maui/ViewModels/CollectionsViewModel.cs`

**功能**:
- 显示所有片单列表
- 创建新片单
- 编辑片单信息
- 删除片单及片单中的词条

**主要属性**:
| 属性名 | 说明 |
|--------|------|
| `EntryCollections` | 片单列表 |
| `EditTitle` | 编辑中的片单标题 |
| `EditDesc` | 编辑中的片单描述 |

**主要命令**:
| 命令名 | 说明 |
|--------|------|
| `AddNewCollectionCommand` | 创建新片单 |
| `ConfirmEditCommand` | 保存片单编辑 |
| `CancelEditCommand` | 取消片单编辑 |
| `RemoveCommand` | 删除片单 |
| `RemoveOneCommand` | 从片单中移除词条 |

---

### 4. 词条详情模块 (EntryDetailViewModel)

**文件**: `OMDb.Maui/ViewModels/EntryDetailViewModel.cs`

**功能**:
- 显示词条的详细信息
- 编辑词条描述
- 管理观看记录
- 管理台词摘录
- 评分管理
- 添加到片单

**主要属性**:
| 属性名 | 说明 |
|--------|------|
| `Entry` | 词条数据 |
| `Desc` | 编辑中的描述 |
| `Rating` | 用户评分 |
| `Names` | 词条别名列表 |
| `IsEditDesc` | 是否正在编辑描述 |
| `IsEditWatchHistory` | 是否正在编辑观看记录 |
| `ExtractsLines` | 台词摘录列表 |

**主要命令**:
| 命令名 | 说明 |
|--------|------|
| `EditDescCommand` | 开始编辑描述 |
| `SaveDescCommand` | 保存描述修改 |
| `CancelEditDescCommand` | 取消编辑描述 |
| `AddHistoryCommand` | 添加观看记录 |
| `EditHistoryCommand` | 编辑观看记录 |
| `DeleteHistoryCommand` | 删除观看记录 |
| `SaveHistoryCommand` | 保存观看记录 |
| `SaveRatingCommand` | 保存评分 |
| `AddToCollectionCommand` | 添加到片单 |
| `AddLineCommand` | 添加台词 |
| `EditLineCommand` | 编辑台词 |
| `DeleteLineCommand` | 删除台词 |

---

### 5. 设置模块 (SettingViewModel)

**文件**: `OMDb.Maui/ViewModels/SettingViewModel.cs`

**功能**:
- 数据库选择和管理
- 主题设置
- 首页项目配置
- 应用设置

**主要属性**:
| 属性名 | 说明 |
|--------|------|
| `DbSources` | 可用的数据库源列表 |
| `SelectedTheme` | 当前主题 |
| `ActiveHomeItems` | 启用的首页项目 |
| `InactiveHomeItems` | 未启用的首页项目 |

**主要命令**:
| 命令名 | 说明 |
|--------|------|
| `DbSelector_RefreshCommand` | 刷新数据库列表 |
| `SaveSettingsCommand` | 保存设置 |

---

### 6. 导航壳模块 (ShellViewModel)

**文件**: `OMDb.Maui/ViewModels/ShellViewModel.cs`

**功能**:
- 应用主导航
- 侧边栏菜单管理
- 全局状态管理

**主要命令**:
| 命令名 | 说明 |
|--------|------|
| `NavClickCommand` | 导航点击处理 |

---

## 页面说明

| 页面 | 文件路径 | 功能说明 |
|------|----------|----------|
| HomePage | `Views/HomePage.xaml` | 主页，展示最近更新、最近观看、随机推荐等 |
| ClassificationPage | `Views/ClassificationPage.xaml` | 分类页，标签浏览和推荐合集 |
| CollectionsPage | `Views/CollectionsPage.xaml` | 片单页，管理个人片单 |
| EntryHomePage | `Views/EntryHomePage.xaml` | 词条主页，词条列表和筛选 |
| EntryDetailPage | `Views/EntryDetailPage.xaml` | 词条详情页，查看和编辑词条信息 |
| ManagementPage | `Views/ManagementPage.xaml` | 管理页，标签和仓库管理 |
| ToolsPage | `Views/ToolsPage.xaml` | 工具页，各种实用工具 |
| SettingPage | `Views/SettingPage.xaml` | 设置页，应用配置 |

---

## 数据模型

### Entry (词条)
核心数据模型，包含：
- 基本信息：名称、别名、描述
- 媒体资源：视频、音频、字幕、图片
- 元数据：封面、评分、标签
- 关联数据：观看记录、台词摘录

### LabelClass (标签分类)
- 支持多级树形结构
- 每个标签可关联多个词条

### EntryCollection (片单)
- 片单基本信息
- 包含的词条列表

### WatchHistory (观看记录)
- 观看时间
- 观看进度
- 备注信息
- 是否观看完结

### ExtractsLineBase (台词摘录)
- 台词内容
- 来源
- 创建和更新时间

---

## 使用指南

### 初次使用

1. **创建仓库**
   - 进入管理页 → 仓库管理
   - 点击"添加仓库"
   - 选择仓库文件夹路径
   - 确认创建

2. **创建标签（可选）**
   - 进入管理页 → 标签管理
   - 点击"添加标签"
   - 输入标签名称
   - 选择父标签（创建子标签时）

3. **创建词条**
   - 进入主页
   - 点击"添加词条"
   - 输入词条名称
   - 选择所属仓库
   - 选择标签（可选）
   - 确认创建

4. **编辑词条详情**
   - 进入词条页
   - 选择要编辑的词条
   - 添加描述、评分等信息
   - 拖拽文件到相应区域（视频、字幕等）
   - 添加观看记录和台词摘录

### 日常使用

- **浏览**: 通过主页、分类页浏览词条
- **搜索**: 使用搜索功能快速定位词条
- **筛选**: 通过标签筛选特定类型的词条
- **更新**: 定期更新观看记录和词条信息

---

## 自动化测试

**项目**: `OMDb.Maui.AutoTest`

测试覆盖所有 ViewModel 的初始化和命令执行：

| 测试项 | 说明 |
|--------|------|
| HomeViewModel | 初始化和刷新命令 |
| ClassificationViewModel | 初始化、刷新、视图切换 |
| CollectionsViewModel | 初始化、添加片单、刷新 |
| ShellViewModel | 导航命令 |
| SettingViewModel | 数据库刷新 |
| EntryCollectionDetailViewModel | 编辑命令 |
| LabelCollectionViewModel | 项目点击 |

**运行测试**:
```bash
dotnet run --project OMDb.Maui.AutoTest/OMDb.Maui.AutoTest.csproj
```

---

## 技术栈

| 组件 | 版本 |
|------|------|
| 框架 | .NET MAUI 10.0.20 |
| MVVM | CommunityToolkit.Mvvm 8.4.2 |
| 数据库 | SQLite (SqlSugar ORM) |
| 目标平台 | Windows 10.0.19041.0+ |
| 运行时 | net10.0-windows10.0.19041.0 |

---

## 构建和运行

### 前提条件
- .NET 10 SDK
- Visual Studio 2022 或 JetBrains Rider
- Windows 10/11

### 构建命令

```bash
# 构建主项目
dotnet build OMDb.Maui/OMDb.Maui.csproj

# 运行应用
dotnet run --project OMDb.Maui/OMDb.Maui.csproj

# 运行测试
dotnet run --project OMDb.Maui.AutoTest/OMDb.Maui.AutoTest.csproj
```

---

## 注意事项

1. **数据库配置**: 首次使用前需要配置数据库路径
2. **媒体文件**: 视频、音频等文件需要放置在指定目录
3. **封面图片**: 支持常见图片格式（JPG、PNG、GIF 等）
4. **字幕文件**: 支持常见字幕格式（SRT、ASS、SSA 等）
5. **视频文件**: 支持常见视频格式（MP4、MKV、AVI 等）

---

## 开发计划

- [ ] 完善拖放功能（视频、字幕、图片）
- [ ] 添加 Dialog/Popup 支持
- [ ] 实现自定义控件
- [ ] 完善 Services 和 Helpers
- [ ] 添加更多转换器
- [ ] 优化 XAML 页面布局
- [ ] 支持更多平台（macOS、Linux）

---

## 版本历史

- **当前版本**: 开发中
- **框架版本**: .NET 10 + MAUI 10.0.20

---

## 许可证

本项目采用 MIT 许可证

---

## 贡献指南

欢迎提交 Issue 和 Pull Request 来帮助改进项目！

如有问题或建议，请通过以下方式联系：
- GitHub Issues
- 邮件联系
