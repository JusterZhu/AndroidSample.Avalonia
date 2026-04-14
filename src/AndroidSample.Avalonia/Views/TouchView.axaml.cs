using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidSample.Avalonia.Views;

/// <summary>
/// 功能2：触控交互 —— 单击、长按、双击、多点触控、防抖动。
/// </summary>
public partial class TouchView : UserControl
{
    private readonly MainView _main;
    private readonly StringBuilder _log = new();

    // ── 防抖动 ────────────────────────────────────────────────
    private DateTime _lastTap = DateTime.MinValue;
    private const int DebounceMs = 300;

    // ── 双击检测 ──────────────────────────────────────────────
    private DateTime _lastSingleTap = DateTime.MinValue;
    private const int DoubleTapMs = 350;

    // ── 长按检测 ──────────────────────────────────────────────
    private DateTime _pressStart;
    private const int LongPressMs = 600;

    // ── 多点追踪 ──────────────────────────────────────────────
    private readonly HashSet<int> _activePointers = new();

    public TouchView(MainView main)
    {
        _main = main;
        InitializeComponent();
        TouchArea.PointerPressed  += OnPointerPressed;
        TouchArea.PointerReleased += OnPointerReleased;
        TouchArea.PointerMoved    += OnPointerMoved;
    }

    private void OnPointerPressed(object? s, PointerPressedEventArgs e)
    {
        _pressStart = DateTime.Now;
        var pt = e.GetCurrentPoint(TouchArea);
        _activePointers.Add(e.Pointer.Id);

        Log($"PointerDown  id={e.Pointer.Id} ({pt.Position.X:F0},{pt.Position.Y:F0}) " +
            $"[{_activePointers.Count}点]");

        TouchInfo.Text = $"触摸点数：{_activePointers.Count}";
    }

    private void OnPointerReleased(object? s, PointerReleasedEventArgs e)
    {
        _activePointers.Remove(e.Pointer.Id);
        var elapsed = (DateTime.Now - _pressStart).TotalMilliseconds;

        if (elapsed >= LongPressMs)
        {
            Log($"长按  ({elapsed:F0} ms)");
            TouchInfo.Text = "长按";
            return;
        }

        // 防抖
        var now = DateTime.Now;
        if ((now - _lastTap).TotalMilliseconds < DebounceMs)
        {
            Log("防抖 —— 忽略");
            return;
        }
        _lastTap = now;

        // 双击
        if ((now - _lastSingleTap).TotalMilliseconds < DoubleTapMs)
        {
            Log("双击！");
            TouchInfo.Text = "双击！";
            _lastSingleTap = DateTime.MinValue;
        }
        else
        {
            Log("单击");
            TouchInfo.Text = "单击";
            _lastSingleTap = now;
        }
    }

    private void OnPointerMoved(object? s, PointerEventArgs e)
    {
        var pt = e.GetCurrentPoint(TouchArea);
        TouchInfo.Text = $"移动 ({pt.Position.X:F0},{pt.Position.Y:F0})  点数:{_activePointers.Count}";
    }

    private void Log(string msg)
    {
        Dispatcher.UIThread.Post(() =>
        {
            _log.AppendLine($"{DateTime.Now:HH:mm:ss.fff}  {msg}");
            // Keep log bounded to ~50 lines
            var text = _log.ToString();
            var lines = text.Split('\n');
            if (lines.Length > 50)
            {
                _log.Clear();
                _log.Append(string.Join('\n', lines[^50..]));
            }
            EventLog.Text = _log.ToString();
        });
    }

    private void OnBack(object? s, RoutedEventArgs e) => _main.BackToMenu();
}
