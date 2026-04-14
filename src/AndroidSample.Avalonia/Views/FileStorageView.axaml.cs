using Android.OS;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.IO;

namespace AndroidSample.Avalonia.Views;

/// <summary>
/// 功能12：文件存储 —— 内部/外部文件读写、缓存管理。
/// </summary>
public partial class FileStorageView : UserControl
{
    private readonly MainView _main;
    private const string FileName = "demo_file.txt";

    public FileStorageView(MainView main)
    {
        _main = main;
        InitializeComponent();
    }

    // ── 内部存储 ──────────────────────────────────────────────
    private void OnWriteInternal(object? s, RoutedEventArgs e)
    {
        var ctx = Android.App.Application.Context;
        var path = Path.Combine(ctx.FilesDir!.AbsolutePath, FileName);
        var content = $"内部文件写入时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        File.WriteAllText(path, content);
        ResultText.Text = $"[内部] 写入成功：{path}\n{content}";
    }

    private void OnReadInternal(object? s, RoutedEventArgs e)
    {
        var ctx = Android.App.Application.Context;
        var path = Path.Combine(ctx.FilesDir!.AbsolutePath, FileName);
        ResultText.Text = File.Exists(path)
            ? $"[内部] 读取：\n{File.ReadAllText(path)}"
            : "[内部] 文件不存在，请先写入";
    }

    // ── 外部存储 ──────────────────────────────────────────────
    private void OnWriteExternal(object? s, RoutedEventArgs e)
    {
        // Android 10+ 使用应用专属外部目录，无需权限
        var ctx = Android.App.Application.Context;
        var dir = ctx.GetExternalFilesDir(null)?.AbsolutePath
                  ?? ctx.FilesDir!.AbsolutePath;
        var path = Path.Combine(dir, FileName);
        var content = $"外部文件写入时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        File.WriteAllText(path, content);
        ResultText.Text = $"[外部] 写入成功：{path}\n{content}";
    }

    private void OnReadExternal(object? s, RoutedEventArgs e)
    {
        var ctx = Android.App.Application.Context;
        var dir = ctx.GetExternalFilesDir(null)?.AbsolutePath
                  ?? ctx.FilesDir!.AbsolutePath;
        var path = Path.Combine(dir, FileName);
        ResultText.Text = File.Exists(path)
            ? $"[外部] 读取：\n{File.ReadAllText(path)}"
            : "[外部] 文件不存在，请先写入";
    }

    // ── 缓存清除 ──────────────────────────────────────────────
    private void OnClearCache(object? s, RoutedEventArgs e)
    {
        var ctx = Android.App.Application.Context;
        long freed = 0;
        foreach (var f in new DirectoryInfo(ctx.CacheDir!.AbsolutePath).GetFiles())
        {
            freed += f.Length;
            f.Delete();
        }
        ResultText.Text = $"缓存已清除，释放 {freed / 1024.0:F1} KB";
    }

    private void OnBack(object? s, RoutedEventArgs e) => _main.BackToMenu();
}
