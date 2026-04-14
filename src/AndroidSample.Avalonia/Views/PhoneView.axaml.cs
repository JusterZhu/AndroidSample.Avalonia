using Android.Content;
using Android.Telephony;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;

namespace AndroidSample.Avalonia.Views;

/// <summary>
/// 功能10：电话 —— 拨号 Intent、来电状态监听。
/// </summary>
public partial class PhoneView : UserControl
{
    private readonly MainView _main;
    private SimplePhoneStateListener? _listener;
    private TelephonyManager? _teleManager;
    private bool _monitoring;

    public PhoneView(MainView main)
    {
        _main = main;
        InitializeComponent();
    }

    private void OnDial(object? s, RoutedEventArgs e)
    {
        var number = PhoneNumber.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(number)) return;

        var uri = Android.Net.Uri.Parse($"tel:{number}");
        var intent = new Intent(Intent.ActionDial, uri);
        intent.SetFlags(ActivityFlags.NewTask);
        Android.App.Application.Context.StartActivity(intent);
    }

    private void OnToggleMonitor(object? s, RoutedEventArgs e)
    {
        if (!_monitoring)
        {
            _teleManager = (TelephonyManager)Android.App.Application.Context
                .GetSystemService(Context.TelephonyService)!;
            _listener = new SimplePhoneStateListener(state =>
                Dispatcher.UIThread.Post(() =>
                    CallStatus.Text += $"\n{DateTime.Now:HH:mm:ss}  状态：{state}"));
            _teleManager.Listen(_listener, PhoneStateListenerFlags.CallState);
            _monitoring = true;
            BtnMonitor.Content = "停止监听";
            CallStatus.Text = "监听中…";
        }
        else
        {
            _teleManager?.Listen(_listener, PhoneStateListenerFlags.None);
            _monitoring = false;
            BtnMonitor.Content = "开始监听来电";
        }
    }

    private void OnBack(object? s, RoutedEventArgs e)
    {
        if (_monitoring) _teleManager?.Listen(_listener, PhoneStateListenerFlags.None);
        _main.BackToMenu();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        _teleManager?.Listen(_listener, PhoneStateListenerFlags.None);
    }

    // ── PhoneStateListener ────────────────────────────────────
    private sealed class SimplePhoneStateListener : PhoneStateListener
    {
        private readonly Action<CallState> _callback;
        public SimplePhoneStateListener(Action<CallState> callback) => _callback = callback;
        public override void OnCallStateChanged([Android.Runtime.GeneratedEnum] CallState state, string? phoneNumber)
            => _callback(state);
    }
}
