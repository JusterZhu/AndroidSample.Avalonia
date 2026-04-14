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

## 构建要求

- .NET 10 SDK
- `dotnet workload install android`
- Android SDK (API 23+, 推荐 API 35)
- **JDK 17 或更高版本**（manifest merger 工具需要 Java 17；若使用 JDK 11 及以下则会报 `UnsupportedClassVersionError`）

### 设置 JAVA_HOME（重要）

> ⚠️ **仅将 JDK 17 加入 `PATH` 是不够的。**  
> MSBuild 通过 `JAVA_HOME` 环境变量定位 JDK，而不是通过 `PATH`。即使 `java -version` 显示 17，  
> 若 `JAVA_HOME` 未设置或仍指向旧版 JDK，构建依然会报 `UnsupportedClassVersionError`。

> ⚠️ **设置 `JAVA_HOME` 后，必须完全关闭并重新打开 Visual Studio 和所有终端窗口。**  
> 进程在启动时继承环境变量，已运行的进程无法感知之后发生的环境变量变化。  
> 即使在 PowerShell 中验证 `$env:JAVA_HOME` 正确，在原来的 Visual Studio 会话中构建仍会使用旧 JDK。

安装 JDK 17（推荐 [Eclipse Temurin](https://adoptium.net/)）后，按以下步骤设置 `JAVA_HOME`：

**Windows — 第一步：找到 JDK 17 安装路径**

打开 PowerShell，执行以下命令找到 JDK 17 的实际路径：
```powershell
# 方法一：查找 Eclipse Adoptium / Temurin 安装目录（最常见）
Get-ChildItem "C:\Program Files\Eclipse Adoptium" -ErrorAction SilentlyContinue

# 方法二：通过 java.exe 路径反推 JDK 目录
(Get-Command java).Source   # 例如：C:\Program Files\Eclipse Adoptium\jdk-17.0.18.8-hotspot\bin\java.exe
# JDK 目录即去掉末尾的 \bin\java.exe：C:\Program Files\Eclipse Adoptium\jdk-17.0.18.8-hotspot
```

**Windows — 第二步：永久设置 JAVA_HOME（PowerShell，用实际路径替换）**
```powershell
# 将下面路径替换为上一步查到的 JDK 17 目录
$jdk17 = "C:\Program Files\Eclipse Adoptium\jdk-17.0.18.8-hotspot"

# 永久写入当前用户的环境变量
[System.Environment]::SetEnvironmentVariable("JAVA_HOME", $jdk17, "User")

# 在当前 PowerShell 会话中立即生效
$env:JAVA_HOME = $jdk17
```

> **也可以用图形界面设置**：开始菜单 → 搜索"编辑系统环境变量" → 用户变量 → 新建，变量名 `JAVA_HOME`，值填 JDK 17 目录路径，确定后**重启 IDE 和命令行**。

**macOS / Linux（bash/zsh，追加到 `~/.bashrc` 或 `~/.zshrc`）**
```bash
export JAVA_HOME=$(/usr/libexec/java_home -v 17)   # macOS
# 或（Linux）
export JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64
```

**第三步：完全重启 Visual Studio 和终端，然后验证（重新打开终端后执行）**

> 跳过重启是最常见的错误——即使 PowerShell 中验证已通过，旧 Visual Studio 进程仍会使用旧 JDK。

```powershell
# Windows PowerShell（在新开的终端中执行）— JAVA_HOME 和 java.exe 都应显示 17
echo $env:JAVA_HOME
& "$env:JAVA_HOME\bin\java" -version
```
```bash
# macOS / Linux（在新开的终端中执行）
echo $JAVA_HOME
"$JAVA_HOME/bin/java" -version   # 应显示 openjdk 17...
```

项目文件已通过 `<JavaSdkDirectory>$(JAVA_HOME)</JavaSdkDirectory>` 将构建工具显式绑定到 `JAVA_HOME`。若构建时 `JAVA_HOME` 未设置，项目会立即报错并给出明确提示，而非产生难以理解的 `UnsupportedClassVersionError`。

## 构建与运行

```bash
# 安装 Android workload（首次）
dotnet workload install android

# 连接 Android 设备或启动模拟器后：
cd src/AndroidSample.Avalonia
dotnet build -f net10.0-android
dotnet run -f net10.0-android
```

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
