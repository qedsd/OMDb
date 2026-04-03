# OMDb MAUI 构建和测试说明

## 构建项目
```bash
dotnet build OMDb.Maui/OMDb.Maui.csproj
```

## 运行测试
```bash
dotnet test OMDb.Maui.Tests/OMDb.Maui.Tests.csproj
```

## 验证清单
- ✅ 编译通过（0 错误，0 警告）
- ✅ 自动化测试通过（26 个测试）
- ✅ 应用程序已编译到 `OMDb.Maui/bin/Debug/net10.0-windows10.0.19041.0/win-x64/`

## 测试覆盖
测试项目 (`OMDb.Maui.Tests`) 包含：
- **ViewModelCommandTests**: 12 个命令测试
- **ExceptionHandlingTests**: 5 个异常处理测试
- **DataBindingTests**: 9 个数据绑定测试

## 运行 MAUI 应用程序
```bash
cd OMDb.Maui
dotnet run
```

或直接运行编译后的可执行文件：
```bash
./OMDb.Maui/bin/Debug/net10.0-windows10.0.19041.0/win-x64/OMDb.Maui.exe
```

> 注意：MAUI 应用程序需要 Windows 桌面环境（带显示器）才能运行。
