# Avalonia Android Demo

基于 **Avalonia UI 12.0.1** + **.NET 10 Android** 的极简功能演示，涵盖 13 个 Android 核心能力。

## 功能清单

| # | 功能 | 说明 |
|---|------|------|
| 1 | 应用生命周期 | ActivityLifecycleCallbacks 监听前/后台切换、低内存 |
| 2 | 触控交互 | 单击/长按/双击/多点触控/防抖动 |
| 3 | 启动页与动画 | Android 原生 SplashActivity + Avalonia 淡入动画 |
| 4 | 传感器 | 加速度计/陀螺仪/摇一摇（低通滤波） |
| 5 | 网络与蓝牙 | WiFi 状态/BLE 扫描/HttpClient |
| 6 | 多媒体 | 相机拍照/相册选图（Intent） |
| 7 | 定位与地图 | 单次 GPS 定位 + Geo URI 打开地图 |
| 8 | 震动与反馈 | 系统震动/交互反馈/自定义波形 |
| 9 | 状态栏与系统UI | 沉浸式/深色模式/导航栏控制 |
| 10 | 电话功能 | 拨号 Intent/来电状态监听 |
| 11 | 联系人功能 | ContentResolver 读取通讯录 |
| 12 | 文件存储 | 内部/外部文件读写/缓存清除 |
| 13 | 应用间数据传递 | Intent 传参/URL Scheme/系统分享 |

## 环境搭建

> **如果在 Visual Studio 中打开项目后出现红色波浪线或"无法加载项目"错误，请按以下步骤完整配置开发环境。**

### 方式一：Visual Studio 2022（推荐 Windows 用户）

1. **安装 Visual Studio 2022**（17.x 或更高版本）  
   下载地址：<https://visualstudio.microsoft.com/vs/>

2. **在 Visual Studio Installer 中勾选 ".NET MAUI 开发" 工作负载**  
   - 打开 **Visual Studio Installer** → 选择 **修改**  
   - 在"工作负载"选项卡中勾选 **".NET MAUI 开发"**（该工作负载会自动安装 Android SDK、Android NDK 及 .NET Android 工作负载）  
   - 点击 **修改** 等待安装完成

3. **安装 .NET 10 SDK**  
   下载地址：<https://dotnet.microsoft.com/download/dotnet/10.0>  
   安装完成后运行以下命令验证：
   ```bash
   dotnet --version
   # 应输出 10.x.x
   ```

4. **安装 .NET Android 工作负载**（命令行补充安装，确保完整）
   ```bash
   dotnet workload install android
   ```

5. **重新打开 Visual Studio**，在解决方案资源管理器中右键项目 → **重新加载项目**，即可消除红色错误。

---

### 方式二：命令行（跨平台 / macOS / Linux）

```bash
# 1. 安装 .NET 10 SDK（见上方下载链接）

# 2. 安装 Android workload
dotnet workload install android

# 3. 进入项目目录并构建
cd src/AndroidSample.Avalonia
dotnet build -f net10.0-android
```

---

### Android SDK 要求

| 项目 | 要求 |
|------|------|
| 最低 API 级别 | API 21（Android 5.0） |
| 推荐 API 级别 | API 35（Android 15） |
| 构建工具 | 由 .NET Android workload 自动管理 |

Visual Studio 安装 ".NET MAUI 开发" 工作负载后会自动下载并配置 Android SDK，无需手动安装。

## 构建与运行

```bash
# 连接 Android 真机（开启 USB 调试）或启动 Android 模拟器后：
cd src/AndroidSample.Avalonia

# 调试构建
dotnet build -f net10.0-android

# 直接部署运行
dotnet run -f net10.0-android
```

在 Visual Studio 中也可直接按 **F5** 或点击工具栏的运行按钮，选择目标设备后部署。

## 常见问题排查

| 现象 | 原因 | 解决方法 |
|------|------|----------|
| 打开项目后大量红色波浪线 | 未安装 .NET Android workload | 执行 `dotnet workload install android` 后重启 VS |
| "无法解析 Android.xxx 命名空间" | Android workload 缺失或 SDK 路径未配置 | 通过 VS Installer 重新安装 ".NET MAUI 开发" 工作负载 |
| `TargetFramework net10.0-android` 不被识别 | .NET SDK 版本过低（低于 10） | 升级至 .NET 10 SDK |
| 找不到 Android 设备 | 未开启 USB 调试 / 驱动未安装 | 在设备"开发者选项"中开启 USB 调试，并安装 OEM USB 驱动 |
| 模拟器启动失败 | Hyper-V / HAXM 未启用 | 在 BIOS 中开启 Intel VT-x/AMD-V，并启用 Windows Hyper-V 功能 |

## 项目结构

```
src/AndroidSample.Avalonia/
├── AndroidSample.Avalonia.csproj   # 项目文件
├── App.axaml / App.axaml.cs        # Avalonia 应用入口
├── MainActivity.cs                 # Android 主 Activity
├── SplashActivity.cs               # 启动页 Activity
├── AndroidManifest.xml             # 权限与组件声明
├── Views/                          # 13 个功能页 + 主菜单
└── Resources/                      # Android 资源文件
```

## 权限说明

应用在首次使用相关功能时会弹出权限申请对话框（联系人、位置、蓝牙、相机等均需用户授权）。
