using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using System;
using System.Threading.Tasks;

namespace AndroidSample.Avalonia.Views;

/// <summary>
/// 功能3：展示 Splash 配置说明，并演示 Avalonia 内部过渡动画。
/// </summary>
public partial class SplashDemoView : UserControl
{
    private readonly MainView _main;

    public SplashDemoView(MainView main)
    {
        _main = main;
        InitializeComponent();
    }

    private async void OnPlay(object? s, RoutedEventArgs e)
    {
        AnimBox.Opacity = 0;
        // 简单淡入动画（Opacity 0 → 1，持续 800 ms）
        var transition = new DoubleTransition
        {
            Property = OpacityProperty,
            Duration = TimeSpan.FromMilliseconds(800),
            Easing = new CubicEaseOut()
        };
        AnimBox.Transitions = [transition];
        AnimBox.Opacity = 1;
        await Task.Delay(900);
        AnimBox.Transitions = null;
    }

    private void OnBack(object? s, RoutedEventArgs e) => _main.BackToMenu();
}
