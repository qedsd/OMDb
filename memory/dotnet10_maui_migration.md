---
name: .NET 10 MAUI 迁移完成
description: OMDb 项目已从 WinUI3/.NET 6 迁移到 .NET 10 MAUI，包含核心层和插件层升级
type: project
---

**迁移完成时间**: 2026-04-02

**迁移内容**:
- OMDb.Core 从 .NET 6 升级到 .NET 10
- OMDb.Douban/OMDb.IMDb/OMDb.JavDb 插件项目升级到 .NET 10
- 新建 OMDb.Maui 项目替代原 OMDb.WinUI3
- 移除 WinUI3 项目 from 解决方案

**技术栈**:
- 目标框架：.NET 10.0
- UI 框架：MAUI (Microsoft.Maui.Controls 10.0.20)
- MVVM: CommunityToolkit.Mvvm 8.4.2
- 数据库：SqlSugarCore 5.1.4.91
- 其他：Xabe.FFmpeg, DotNetCore.NPOI, HtmlAgilityPack, NLog

**编译状态**: 
- Debug 和 Release 配置均编译成功
- Windows 平台运行正常
- 当前仅针对 Windows 平台 (net10.0-windows10.0.19041.0)

**注意事项**:
- Android/iOS/MacCatalyst 平台因 DotNetCore.NPOI 包兼容性问题暂未启用
- CommunityToolkit.Maui 暂不支持 .NET 10，使用纯 MAUI 控件

**How to apply**: 
- 未来添加跨平台支持时需要解决 DotNetCore.NPOI 的兼容性问题
- 等待 CommunityToolkit.Maui 支持 .NET 10 后再添加
