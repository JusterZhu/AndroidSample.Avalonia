using Android.App;
using Android.Content.ComponentCallbacks2;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Text;

namespace AndroidSample.Avalonia.Views;

/// <summary>
/// 功能1：生命周期监听。
/// 通过 Application.ActivityLifecycleCallbacks 和 ComponentCallbacks2
/// 监听前/后台切换及低内存事件。
/// </summary>
public partial class LifecycleView : UserControl, Application.IActivityLifecycleCallbacks
{
    private readonly MainView _main;
    private readonly StringBuilder _log = new();
    private Application? _app;

    public LifecycleView(MainView main)
    {
        _main = main;
        InitializeComponent();
        RegisterCallbacks();
    }

    // ── 注册 Android 生命周期回调 ─────────────────────────────
    private void RegisterCallbacks()
    {
        _app = Application.Context.ApplicationContext as Application;
        _app?.RegisterActivityLifecycleCallbacks(this);
        AppendLog("已注册生命周期监听");
    }

    private void UnregisterCallbacks()
    {
        _app?.UnregisterActivityLifecycleCallbacks(this);
    }

    // ── IActivityLifecycleCallbacks ───────────────────────────
    public void OnActivityCreated(Activity activity, Android.OS.Bundle? savedInstanceState)
        => AppendLog($"OnCreate  [{activity.LocalClassName}]");
    public void OnActivityStarted(Activity activity)
        => AppendLog($"OnStart   [{activity.LocalClassName}]");
    public void OnActivityResumed(Activity activity)
        => AppendLog($"OnResume  [{activity.LocalClassName}] ← 应用进入前台");
    public void OnActivityPaused(Activity activity)
        => AppendLog($"OnPause   [{activity.LocalClassName}]");
    public void OnActivityStopped(Activity activity)
        => AppendLog($"OnStop    [{activity.LocalClassName}] ← 应用进入后台");
    public void OnActivitySaveInstanceState(Activity activity, Android.OS.Bundle outState)
        => AppendLog($"OnSaveState [{activity.LocalClassName}]");
    public void OnActivityDestroyed(Activity activity)
        => AppendLog($"OnDestroy [{activity.LocalClassName}]");

    // ── 工具 ─────────────────────────────────────────────────
    private void AppendLog(string msg)
    {
        Dispatcher.UIThread.Post(() =>
        {
            _log.AppendLine($"{DateTime.Now:HH:mm:ss.fff}  {msg}");
            LogText.Text = _log.ToString();
        });
    }

    private void OnBack(object? s, RoutedEventArgs e)
    {
        UnregisterCallbacks();
        _main.BackToMenu();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        UnregisterCallbacks();
    }
}
