# Avalonia Android Demo

基于 **Avalonia UI 11.2.3** + **.NET 9 Android** 的极简功能演示，涵盖 13 个 Android 核心能力。

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

- .NET 9 SDK
- `dotnet workload install android`
- Android SDK (API 21+, 推荐 API 34)

## 构建与运行

```bash
# 安装 Android workload（首次）
dotnet workload install android

# 连接 Android 设备或启动模拟器后：
cd src/AndroidSample.Avalonia
dotnet build -f net9.0-android
dotnet run -f net9.0-android
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
