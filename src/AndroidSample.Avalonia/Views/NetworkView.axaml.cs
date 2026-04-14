using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidSample.Avalonia.Views;

/// <summary>
/// 功能5：网络与蓝牙 —— WiFi 状态、BLE 扫描、HttpClient 请求。
/// </summary>
public partial class NetworkView : UserControl
{
    private readonly MainView _main;
    private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(10) };
    private BluetoothLeScanner? _bleScanner;
    private SimpleScanCallback? _scanCallback;
    private CancellationTokenSource? _bleCts;

    public NetworkView(MainView main)
    {
        _main = main;
        InitializeComponent();
        RefreshWifi();
    }

    // ── WiFi ─────────────────────────────────────────────────
    private void OnRefreshWifi(object? s, RoutedEventArgs e) => RefreshWifi();

    private void RefreshWifi()
    {
        var cm = (ConnectivityManager)Android.App.Application.Context
            .GetSystemService(Context.ConnectivityService)!;
        var net = cm.ActiveNetwork;
        var caps = net != null ? cm.GetNetworkCapabilities(net) : null;

        string status;
        if (caps == null)
            status = "无网络连接";
        else if (caps.HasTransport(TransportType.Wifi))
            status = "✅ 已连接 WiFi";
        else if (caps.HasTransport(TransportType.Cellular))
            status = "📶 已连接移动数据";
        else
            status = "🌐 其他网络";

        WifiText.Text = status;
    }

    // ── BLE 扫描 ──────────────────────────────────────────────
    private async void OnBleScan(object? s, RoutedEventArgs e)
    {
        BtnBleScan.IsEnabled = false;
        BleText.Text = "扫描中…";
        var log = new StringBuilder();

        try
        {
            var bm = (BluetoothManager)Android.App.Application.Context
                .GetSystemService(Context.BluetoothService)!;
            var adapter = bm.Adapter;
            if (adapter == null || !adapter.IsEnabled)
            {
                BleText.Text = "蓝牙未开启";
                return;
            }

            _bleScanner = adapter.BluetoothLeScanner;
            _scanCallback = new SimpleScanCallback(result =>
            {
                var name = result.Device?.Name ?? "Unknown";
                var addr = result.Device?.Address ?? "";
                Dispatcher.UIThread.Post(() =>
                {
                    log.AppendLine($"{name}  {addr}");
                    BleText.Text = log.ToString();
                });
            });

            _bleCts = new CancellationTokenSource();
            _bleScanner?.StartScan(_scanCallback);

            await Task.Delay(5000, _bleCts.Token).ContinueWith(_ => { });

            _bleScanner?.StopScan(_scanCallback);
            Dispatcher.UIThread.Post(() =>
            {
                if (log.Length == 0) BleText.Text = "未发现设备";
            });
        }
        catch (Exception ex)
        {
            BleText.Text = $"错误：{ex.Message}";
        }
        finally
        {
            BtnBleScan.IsEnabled = true;
        }
    }

    // ── HTTP ─────────────────────────────────────────────────
    private async void OnHttpGet(object? s, RoutedEventArgs e)
    {
        HttpText.Text = "请求中…";
        try
        {
            var resp = await _http.GetStringAsync("https://httpbin.org/get");
            HttpText.Text = resp.Length > 500 ? resp[..500] + "…" : resp;
        }
        catch (Exception ex)
        {
            HttpText.Text = $"错误：{ex.Message}";
        }
    }

    private void OnBack(object? s, RoutedEventArgs e)
    {
        _bleCts?.Cancel();
        _bleScanner?.StopScan(_scanCallback);
        _main.BackToMenu();
    }

    // ── BLE 回调 ──────────────────────────────────────────────
    private sealed class SimpleScanCallback : ScanCallback
    {
        private readonly Action<ScanResult> _onResult;
        public SimpleScanCallback(Action<ScanResult> onResult) => _onResult = onResult;
        public override void OnScanResult([Android.Runtime.GeneratedEnum] ScanCallbackType callbackType, ScanResult? result)
        {
            if (result != null) _onResult(result);
        }
    }
}
