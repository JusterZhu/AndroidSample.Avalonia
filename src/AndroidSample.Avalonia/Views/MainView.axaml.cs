using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AndroidSample.Avalonia.Views;

/// <summary>
/// 主菜单：13 个功能按钮，点击后切换到对应功能页。
/// </summary>
public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    // ── 页面导航 ──────────────────────────────────────────────
    private void ShowFeature(UserControl view)
    {
        FeaturePage.Content = view;
        FeaturePage.IsVisible = true;
        MenuPanel.IsVisible = false;
    }

    public void BackToMenu()
    {
        FeaturePage.IsVisible = false;
        MenuPanel.IsVisible = true;
        FeaturePage.Content = null;
    }

    // ── 按钮事件 ─────────────────────────────────────────────
    private void OnLifecycle(object? s, RoutedEventArgs e)   => ShowFeature(new LifecycleView(this));
    private void OnTouch(object? s, RoutedEventArgs e)        => ShowFeature(new TouchView(this));
    private void OnSplash(object? s, RoutedEventArgs e)       => ShowFeature(new SplashDemoView(this));
    private void OnSensor(object? s, RoutedEventArgs e)       => ShowFeature(new SensorView(this));
    private void OnNetwork(object? s, RoutedEventArgs e)      => ShowFeature(new NetworkView(this));
    private void OnMedia(object? s, RoutedEventArgs e)        => ShowFeature(new MediaView(this));
    private void OnLocation(object? s, RoutedEventArgs e)     => ShowFeature(new LocationView(this));
    private void OnVibration(object? s, RoutedEventArgs e)    => ShowFeature(new VibrationView(this));
    private void OnStatusBar(object? s, RoutedEventArgs e)    => ShowFeature(new StatusBarView(this));
    private void OnPhone(object? s, RoutedEventArgs e)        => ShowFeature(new PhoneView(this));
    private void OnContacts(object? s, RoutedEventArgs e)     => ShowFeature(new ContactsView(this));
    private void OnFileStorage(object? s, RoutedEventArgs e)  => ShowFeature(new FileStorageView(this));
    private void OnIntent(object? s, RoutedEventArgs e)       => ShowFeature(new IntentView(this));
}
