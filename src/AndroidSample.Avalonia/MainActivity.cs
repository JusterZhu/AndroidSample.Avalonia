using Android.App;
using Android.Content;
using Android.Content.PM;
using Avalonia.Android;

namespace AndroidSample.Avalonia;

/// <summary>
/// Android 主 Activity。
/// 提供静态 Current 属性供 View 层访问 Activity 功能（拍照回调等）。
/// </summary>
[Activity(
    Label = "Avalonia Android Demo",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@mipmap/ic_launcher",
    MainLauncher = false,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize |
                           ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
    /// <summary>当前活跃的 MainActivity 实例（用于 StartActivityForResult 等）。</summary>
    public static MainActivity? Current { get; private set; }

    /// <summary>Activity Result 回调（MediaView 等功能页订阅）。</summary>
    public event Action<int, Result, Intent?>? OnActivityResultCallback;

    protected override void OnResume()
    {
        base.OnResume();
        Current = this;
    }

    protected override void OnDestroy()
    {
        if (Current == this) Current = null;
        base.OnDestroy();
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        OnActivityResultCallback?.Invoke(requestCode, resultCode, data);
    }

    protected override void OnRequestPermissionsResult(int requestCode, string[] permissions,
        Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        PermissionHelper.OnResult(requestCode, permissions, grantResults);
    }
}
