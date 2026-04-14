using Android.Content;
using Android.Hardware;
using Android.Runtime;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;

namespace AndroidSample.Avalonia.Views;

/// <summary>
/// 功能4：传感器 —— 加速度计、陀螺仪、摇一摇（低通滤波降噪）。
/// </summary>
public partial class SensorView : UserControl
{
    private readonly MainView _main;
    private SensorManager? _sensorMgr;
    private Sensor? _accel;
    private Sensor? _gyro;
    private SensorProxy? _proxy;

    public SensorView(MainView main)
    {
        _main = main;
        InitializeComponent();
    }

    private void OnStart(object? s, RoutedEventArgs e)
    {
        _sensorMgr = (SensorManager)Android.App.Application.Context
            .GetSystemService(Context.SensorService)!;
        _accel = _sensorMgr.GetDefaultSensor(SensorType.Accelerometer);
        _gyro  = _sensorMgr.GetDefaultSensor(SensorType.Gyroscope);

        _proxy = new SensorProxy(
            onAccel: (ax, ay, az, fx, fy, fz) =>
                Dispatcher.UIThread.Post(() => AccelText.Text = $"X={fx:F2}  Y={fy:F2}  Z={fz:F2}"),
            onShake: t =>
                Dispatcher.UIThread.Post(() => ShakeText.Text = $"🤝 摇一摇！ {t:HH:mm:ss}"),
            onGyro: (x, y, z) =>
                Dispatcher.UIThread.Post(() => GyroText.Text = $"X={x:F3}  Y={y:F3}  Z={z:F3}"));

        // 节能模式：使用 Game 频率
        _sensorMgr.RegisterListener(_proxy, _accel, SensorDelay.Game);
        _sensorMgr.RegisterListener(_proxy, _gyro,  SensorDelay.Game);

        BtnStart.IsEnabled = false;
        BtnStop.IsEnabled  = true;
    }

    private void OnStop(object? s, RoutedEventArgs e) => Unregister();

    private void Unregister()
    {
        if (_proxy != null) _sensorMgr?.UnregisterListener(_proxy);
        BtnStart.IsEnabled = true;
        BtnStop.IsEnabled  = false;
    }

    private void OnBack(object? s, RoutedEventArgs e)
    {
        Unregister();
        _main.BackToMenu();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Unregister();
    }

    // ── Java 代理：必须继承 Java.Lang.Object ──────────────────
    private sealed class SensorProxy : Java.Lang.Object, ISensorEventListener
    {
        private readonly Action<float, float, float, float, float, float> _onAccel;
        private readonly Action<DateTime> _onShake;
        private readonly Action<float, float, float> _onGyro;

        // 低通滤波状态
        private float[] _filtered = new float[3];
        private const float Alpha = 0.15f;
        private const float ShakeThreshold = 20f;
        private DateTime _lastShake = DateTime.MinValue;
        private const int ShakeCooldownMs = 1000;

        public SensorProxy(
            Action<float, float, float, float, float, float> onAccel,
            Action<DateTime> onShake,
            Action<float, float, float> onGyro)
        {
            _onAccel = onAccel;
            _onShake = onShake;
            _onGyro  = onGyro;
        }

        public void OnSensorChanged(SensorEvent? e)
        {
            if (e?.Values == null) return;

            if (e.Sensor?.Type == SensorType.Accelerometer)
            {
                float ax = e.Values[0], ay = e.Values[1], az = e.Values[2];

                // 低通滤波
                _filtered[0] = Alpha * ax + (1 - Alpha) * _filtered[0];
                _filtered[1] = Alpha * ay + (1 - Alpha) * _filtered[1];
                _filtered[2] = Alpha * az + (1 - Alpha) * _filtered[2];
                _onAccel(ax, ay, az, _filtered[0], _filtered[1], _filtered[2]);

                // 摇一摇
                double mag = Math.Sqrt(ax * ax + ay * ay + az * az);
                if (mag > ShakeThreshold)
                {
                    var now = DateTime.Now;
                    if ((now - _lastShake).TotalMilliseconds > ShakeCooldownMs)
                    {
                        _lastShake = now;
                        _onShake(now);
                    }
                }
            }
            else if (e.Sensor?.Type == SensorType.Gyroscope)
            {
                _onGyro(e.Values[0], e.Values[1], e.Values[2]);
            }
        }

        public void OnAccuracyChanged(Sensor? sensor, [GeneratedEnum] SensorStatus accuracy) { }
    }
}
