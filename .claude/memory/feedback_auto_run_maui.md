---
name: 自动运行和验证 MAUI 应用
description: 每次修改完代码后必须自动编译并运行 MAUI 应用进行验证
type: feedback
---

每次修改完 MAUI 代码后，必须执行以下步骤：

1. **编译验证**：`dotnet build OMDb.Maui/OMDb.Maui.csproj` - 确保 0 错误 0 警告
2. **运行应用**：`dotnet run --no-build` - 启动应用验证是否能正常运行
3. **自动化测试**：`dotnet test OMDb.Maui.Tests/OMDb.Maui.Tests.csproj` - 确保所有测试通过

**为什么**：用户明确要求"每次执行完，自己跑一下"，不只是编译通过，还要跑起来不报错。之前出现过编译成功但 XAML 运行时错误（如 CornerRadius 属性问题），只有通过实际运行才能发现这类错误。

**如何应用**：任何 MAUI 相关的修改（XAML、ViewModel、Controls 等）完成后，自动按顺序执行上述三个步骤。
