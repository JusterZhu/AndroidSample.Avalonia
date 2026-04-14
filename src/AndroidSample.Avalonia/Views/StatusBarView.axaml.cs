using Android.OS;
using Android.Views;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AndroidSample.Avalonia.Views;

/// <summary>
/// 功能9：状态栏与系统UI —— 沉浸式、深色模式、导航栏控制。
/// </summary>
public partial class StatusBarView : UserControl
{
    private readonly MainView _main;

    public StatusBarView(MainView main)
    {
        _main = main;
        InitializeComponent();
    }

    private Android.App.Activity? Activity => MainActivity.Current;

    // ── 深色模式 ──────────────────────────────────────────────
    private void OnDarkMode(object? s, RoutedEventArgs e)
    {
        var mgr = Android.App.UiModeManager.FromContext(Android.App.Application.Context);
        if (mgr != null)
        {
            // 切换 Night Mode
            if (mgr.NightMode == Android.App.UiNightMode.Yes)
                mgr.NightMode = Android.App.UiNightMode.No;
            else
                mgr.NightMode = Android.App.UiNightMode.Yes;
            StatusText.Text = $"深色模式：{mgr.NightMode}";
        }
    }

    // ── 沉浸式状态栏 ──────────────────────────────────────────
    private void OnImmersive(object? s, RoutedEventArgs e)
    {
        var activity = Activity;
        if (activity?.Window == null) return;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
        {
            activity.Window.SetDecorFitsSystemWindows(false);
            var controller = activity.Window.InsetsController;
            controller?.Hide(WindowInsets.Type.StatusBars());
        }
        else
        {
#pragma warning disable CA1422
            activity.Window.DecorView.SystemUiVisibility =
                (StatusBarVisibility)(SystemUiFlags.ImmersiveSticky |
                                      SystemUiFlags.HideNavigation |
                                      SystemUiFlags.Fullscreen);
#pragma warning restore CA1422
        }
        StatusText.Text = "沉浸式模式已开启";
    }

    // ── 恢复状态栏 ────────────────────────────────────────────
    private void OnNormalBar(object? s, RoutedEventArgs e)
    {
        var activity = Activity;
        if (activity?.Window == null) return;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
        {
            activity.Window.SetDecorFitsSystemWindows(true);
            activity.Window.InsetsController?.Show(WindowInsets.Type.StatusBars());
        }
        else
        {
#pragma warning disable CA1422
            activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.Visible;
#pragma warning restore CA1422
        }
        StatusText.Text = "状态栏已恢复";
    }

    // ── 导航栏 ────────────────────────────────────────────────
    private void OnHideNavBar(object? s, RoutedEventArgs e)
    {
        var activity = Activity;
        if (activity?.Window == null) return;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            activity.Window.InsetsController?.Hide(WindowInsets.Type.NavigationBars());
        else
        {
#pragma warning disable CA1422
            activity.Window.DecorView.SystemUiVisibility =
                (StatusBarVisibility)(SystemUiFlags.HideNavigation | SystemUiFlags.ImmersiveSticky);
#pragma warning restore CA1422
        }
        StatusText.Text = "导航栏已隐藏";
    }

    private void OnShowNavBar(object? s, RoutedEventArgs e)
    {
        var activity = Activity;
        if (activity?.Window == null) return;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            activity.Window.InsetsController?.Show(WindowInsets.Type.NavigationBars());
        else
        {
#pragma warning disable CA1422
            activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.Visible;
#pragma warning restore CA1422
        }
        StatusText.Text = "导航栏已显示";
    }

    private void OnBack(object? s, RoutedEventArgs e) => _main.BackToMenu();
}
