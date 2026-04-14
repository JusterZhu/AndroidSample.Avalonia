using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.IO;

namespace AndroidSample.Avalonia.Views;

/// <summary>
/// 功能6：多媒体 —— 相机拍照、相册图片选取。
/// 通过 StartActivityForResult 与 MainActivity 协作。
/// </summary>
public partial class MediaView : UserControl
{
    private readonly MainView _main;

    // Activity Result 请求码
    public const int RequestCamera  = 1001;
    public const int RequestGallery = 1002;

    private Android.Net.Uri? _photoUri;
    private Bitmap? _currentBitmap;

    public MediaView(MainView main)
    {
        _main = main;
        InitializeComponent();

        // 注册 Activity Result 回调
        if (MainActivity.Current != null)
            MainActivity.Current.OnActivityResultCallback += OnActivityResult;
    }

    // ── 拍照 ─────────────────────────────────────────────────
    private void OnTakePhoto(object? s, RoutedEventArgs e)
    {
        var activity = MainActivity.Current;
        if (activity == null) return;

        // 创建临时文件 Uri
        var file = new Java.IO.File(activity.CacheDir, $"photo_{DateTime.Now.Ticks}.jpg");
        _photoUri = AndroidX.Core.Content.FileProvider.GetUriForFile(
            activity,
            $"{activity.PackageName}.fileprovider",
            file);

        var intent = new Intent(MediaStore.ActionImageCapture);
        intent.PutExtra(MediaStore.ExtraOutput, _photoUri);
        activity.StartActivityForResult(intent, RequestCamera);
    }

    // ── 相册 ─────────────────────────────────────────────────
    private void OnPickGallery(object? s, RoutedEventArgs e)
    {
        var intent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
        MainActivity.Current?.StartActivityForResult(intent, RequestGallery);
    }

    // ── 结果回调 ─────────────────────────────────────────────
    public void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        if (resultCode != Result.Ok) return;

        Android.Net.Uri? uri = requestCode switch
        {
            RequestCamera  => _photoUri,
            RequestGallery => data?.Data,
            _              => null
        };

        if (uri == null) return;

        try
        {
            using var stream = MainActivity.Current!.ContentResolver!.OpenInputStream(uri);
            if (stream == null) return;

            // 将 Android Stream 转为 Avalonia Bitmap
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var bitmap = new Bitmap(ms);
            _currentBitmap?.Dispose();
            _currentBitmap = bitmap;
            PreviewImage.Source = bitmap;
            StatusText.Text = $"图片已加载（{bitmap.PixelSize.Width}×{bitmap.PixelSize.Height}）";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"加载失败：{ex.Message}";
        }
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        if (MainActivity.Current != null)
            MainActivity.Current.OnActivityResultCallback -= OnActivityResult;
        _currentBitmap?.Dispose();
    }

    private void OnBack(object? s, RoutedEventArgs e)
    {
        if (MainActivity.Current != null)
            MainActivity.Current.OnActivityResultCallback -= OnActivityResult;
        _main.BackToMenu();
    }
}
