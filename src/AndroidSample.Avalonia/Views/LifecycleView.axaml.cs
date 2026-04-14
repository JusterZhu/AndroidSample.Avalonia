using Android.App;
using Android.Content.ComponentCallbacks2;
using Android.OS;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Text;

namespace AndroidSample.Avalonia.Views;

/// <summary>
/// 功能1：生命周期监听。
/// 通过 Application.ActivityLifecycleCallbacks 监听前/后台切换及低内存事件。
/// </summary>
public partial class LifecycleView : UserControl
{
    private readonly MainView _main;
    private readonly StringBuilder _log = new();
    private Application? _app;
    private ActivityCallbackProxy? _proxy;

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
        _proxy = new ActivityCallbackProxy(AppendLog);
        _app?.RegisterActivityLifecycleCallbacks(_proxy);
        AppendLog("已注册生命周期监听");
    }

    private void UnregisterCallbacks()
    {
        if (_proxy != null)
            _app?.UnregisterActivityLifecycleCallbacks(_proxy);
    }

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

    // ── Java 代理：继承 Java.Lang.Object 以便 Android 运行时回调 ──
    private sealed class ActivityCallbackProxy
        : Java.Lang.Object, Application.IActivityLifecycleCallbacks
    {
        private readonly Action<string> _log;
        public ActivityCallbackProxy(Action<string> log) => _log = log;

        public void OnActivityCreated(Activity activity, Bundle? savedInstanceState)
            => _log($"OnCreate  [{activity.LocalClassName}]");
        public void OnActivityStarted(Activity activity)
            => _log($"OnStart   [{activity.LocalClassName}]");
        public void OnActivityResumed(Activity activity)
            => _log($"OnResume  [{activity.LocalClassName}] ← 前台");
        public void OnActivityPaused(Activity activity)
            => _log($"OnPause   [{activity.LocalClassName}]");
        public void OnActivityStopped(Activity activity)
            => _log($"OnStop    [{activity.LocalClassName}] ← 后台");
        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
            => _log($"OnSaveState [{activity.LocalClassName}]");
        public void OnActivityDestroyed(Activity activity)
            => _log($"OnDestroy [{activity.LocalClassName}]");
    }
}
