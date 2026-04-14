using Android.Content;
using Android.Locations;
using Android.OS;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;

namespace AndroidSample.Avalonia.Views;

/// <summary>
/// 功能7：定位 —— 获取单次 GPS/Network 定位；
///         地图：通过 Intent 打开地图应用展示坐标。
/// </summary>
public partial class LocationView : UserControl
{
    private readonly MainView _main;
    private LocationManager? _locMgr;
    private LocationProxy? _proxy;
    private double _lat, _lon;

    public LocationView(MainView main)
    {
        _main = main;
        InitializeComponent();
    }

    private void OnGetLocation(object? s, RoutedEventArgs e)
    {
        LocationText.Text = "定位中…";
        _locMgr = (LocationManager)Android.App.Application.Context
            .GetSystemService(Context.LocationService)!;

        // 先尝试最后已知位置
        var last = _locMgr.GetLastKnownLocation(LocationManager.GpsProvider)
                ?? _locMgr.GetLastKnownLocation(LocationManager.NetworkProvider);
        if (last != null)
        {
            UpdateLocation(last);
            return;
        }

        // 请求单次更新（Network Provider）
        _proxy = new LocationProxy(loc =>
        {
            UpdateLocation(loc);
            _locMgr?.RemoveUpdates(_proxy);
        });
        _locMgr.RequestLocationUpdates(LocationManager.NetworkProvider, 0, 0, _proxy);
    }

    private void OnShowMap(object? s, RoutedEventArgs e)
    {
        if (_lat == 0 && _lon == 0)
        {
            MapStatus.Text = "请先获取位置";
            return;
        }
        // 通过 Geo URI 打开系统地图应用
        var uri = Android.Net.Uri.Parse($"geo:{_lat},{_lon}?q={_lat},{_lon}(当前位置)");
        var intent = new Intent(Intent.ActionView, uri);
        intent.SetFlags(ActivityFlags.NewTask);
        Android.App.Application.Context.StartActivity(intent);
        MapStatus.Text = $"已打开地图：{_lat:F6}, {_lon:F6}";
    }

    private void UpdateLocation(Location loc)
    {
        _lat = loc.Latitude;
        _lon = loc.Longitude;
        Dispatcher.UIThread.Post(() =>
            LocationText.Text =
                $"纬度：{_lat:F6}\n经度：{_lon:F6}\n精度：{loc.Accuracy:F1} m\n" +
                $"时间：{DateTimeOffset.FromUnixTimeMilliseconds(loc.Time):HH:mm:ss}");
    }

    private void OnBack(object? s, RoutedEventArgs e)
    {
        if (_proxy != null) _locMgr?.RemoveUpdates(_proxy);
        _main.BackToMenu();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        if (_proxy != null) _locMgr?.RemoveUpdates(_proxy);
    }

    // ── Java 代理 ─────────────────────────────────────────────
    private sealed class LocationProxy : Java.Lang.Object, ILocationListener
    {
        private readonly Action<Location> _onLocation;
        public LocationProxy(Action<Location> onLocation) => _onLocation = onLocation;

        public void OnLocationChanged(Location location) => _onLocation(location);
        public void OnProviderEnabled(string provider) { }
        public void OnProviderDisabled(string provider) { }
        public void OnStatusChanged(string? provider, Availability status, Bundle? extras) { }
    }
}
