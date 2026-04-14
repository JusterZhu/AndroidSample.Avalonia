using Android.Runtime;
using Avalonia;
using Avalonia.Android;

namespace AndroidSample.Avalonia;

/// <summary>
/// Android Application 入口：初始化 Avalonia 运行时并配置字体。
/// AvaloniaAndroidApplication&lt;TApp&gt; 负责构建 AppBuilder、初始化 Lifetime，
/// AvaloniaMainActivity 会通过 IAndroidApplication 接口获取 Lifetime 来启动 UI。
/// </summary>
[global::Android.App.Application]
public class MainApplication : AvaloniaAndroidApplication<App>
{
    public MainApplication(nint javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}
