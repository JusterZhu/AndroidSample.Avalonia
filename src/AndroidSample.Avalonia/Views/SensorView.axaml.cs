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
public partial class SensorView : UserControl, ISensorEventListener
{
    private readonly MainView _main;
    private SensorManager? _sensorMgr;
    private Sensor? _accel;
    private Sensor? _gyro;

    // ── 低通滤波 ──────────────────────────────────────────────
    private float[] _accelFiltered = new float[3];
    private const float Alpha = 0.15f;          // 滤波系数（越小越平滑）

    // ── 摇一摇 ────────────────────────────────────────────────
    private const float ShakeThreshold = 20f;   // m/s²
    private DateTime _lastShake = DateTime.MinValue;
    private const int ShakeCooldownMs = 1000;

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

        // 节能模式：使用 Game 频率而非最快频率
        _sensorMgr.RegisterListener(this, _accel, SensorDelay.Game);
        _sensorMgr.RegisterListener(this, _gyro,  SensorDelay.Game);

        BtnStart.IsEnabled = false;
        BtnStop.IsEnabled  = true;
    }

    private void OnStop(object? s, RoutedEventArgs e) => Unregister();

    private void Unregister()
    {
        _sensorMgr?.UnregisterListener(this);
        BtnStart.IsEnabled = true;
        BtnStop.IsEnabled  = false;
    }

    // ── ISensorEventListener ──────────────────────────────────
    public void OnSensorChanged(SensorEvent? e)
    {
        if (e?.Values == null) return;

        if (e.Sensor?.Type == SensorType.Accelerometer)
        {
            // 低通滤波
            _accelFiltered[0] = Alpha * e.Values[0] + (1 - Alpha) * _accelFiltered[0];
            _accelFiltered[1] = Alpha * e.Values[1] + (1 - Alpha) * _accelFiltered[1];
            _accelFiltered[2] = Alpha * e.Values[2] + (1 - Alpha) * _accelFiltered[2];

            var x = _accelFiltered[0]; var y = _accelFiltered[1]; var z = _accelFiltered[2];
            Dispatcher.UIThread.Post(() =>
                AccelText.Text = $"X={x:F2}  Y={y:F2}  Z={z:F2}");

            // 摇一摇检测（使用原始值计算幅度）
            float ax = e.Values[0], ay = e.Values[1], az = e.Values[2];
            double magnitude = Math.Sqrt(ax * ax + ay * ay + az * az);
            if (magnitude > ShakeThreshold)
            {
                var now = DateTime.Now;
                if ((now - _lastShake).TotalMilliseconds > ShakeCooldownMs)
                {
                    _lastShake = now;
                    Dispatcher.UIThread.Post(() =>
                        ShakeText.Text = $"🤝 摇一摇！ {now:HH:mm:ss}");
                }
            }
        }
        else if (e.Sensor?.Type == SensorType.Gyroscope)
        {
            float x = e.Values[0], y = e.Values[1], z = e.Values[2];
            Dispatcher.UIThread.Post(() =>
                GyroText.Text = $"X={x:F3}  Y={y:F3}  Z={z:F3}");
        }
    }

    public void OnAccuracyChanged(Sensor? sensor, [GeneratedEnum] SensorStatus accuracy) { }

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
}
