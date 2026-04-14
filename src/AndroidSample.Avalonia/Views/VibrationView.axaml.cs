using Android.Content;
using Android.OS;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AndroidSample.Avalonia.Views;

/// <summary>
/// 功能8：震动与反馈 —— 系统震动、交互反馈、自定义波形。
/// </summary>
public partial class VibrationView : UserControl
{
    private readonly MainView _main;

    public VibrationView(MainView main)
    {
        _main = main;
        InitializeComponent();
    }

    private Vibrator? GetVibrator()
    {
        var ctx = Android.App.Application.Context;
        if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
        {
            var vm = (VibratorManager)ctx.GetSystemService(Context.VibratorManagerService)!;
            return vm.DefaultVibrator;
        }
#pragma warning disable CA1422
        return (Vibrator)ctx.GetSystemService(Context.VibratorService)!;
#pragma warning restore CA1422
    }

    private void OnDefaultVibrate(object? s, RoutedEventArgs e)
    {
        var vib = GetVibrator();
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            vib?.Vibrate(VibrationEffect.CreateOneShot(200, VibrationEffect.DefaultAmplitude));
        else
        {
#pragma warning disable CA1422
            vib?.Vibrate(200);
#pragma warning restore CA1422
        }
        StatusText.Text = "已震动 200 ms";
    }

    private void OnHapticFeedback(object? s, RoutedEventArgs e)
    {
        var vib = GetVibrator();
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            vib?.Vibrate(VibrationEffect.CreatePredefined(VibrationEffect.EffectClick));
        else
        {
#pragma warning disable CA1422
            vib?.Vibrate(50);
#pragma warning restore CA1422
        }
        StatusText.Text = "交互反馈震动";
    }

    private void OnPatternVibrate(object? s, RoutedEventArgs e)
    {
        // 波形：等待100ms，震100ms，停100ms，震300ms
        long[] pattern = { 100, 100, 100, 300 };
        var vib = GetVibrator();
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var waveform = VibrationEffect.CreateWaveform(pattern, -1);
            vib?.Vibrate(waveform);
        }
        else
        {
#pragma warning disable CA1422
            vib?.Vibrate(pattern, -1);
#pragma warning restore CA1422
        }
        StatusText.Text = "波形震动：100-100-300";
    }

    private void OnBack(object? s, RoutedEventArgs e) => _main.BackToMenu();
}
