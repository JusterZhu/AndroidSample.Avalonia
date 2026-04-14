using Android.Content;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AndroidSample.Avalonia.Views;

/// <summary>
/// 功能13：应用间数据传递 —— Intent 传参、URL Scheme、系统分享。
/// </summary>
public partial class IntentView : UserControl
{
    private readonly MainView _main;

    public IntentView(MainView main)
    {
        _main = main;
        InitializeComponent();
    }

    // ── ACTION_SEND 传递文本 ───────────────────────────────────
    private void OnSendIntent(object? s, RoutedEventArgs e)
    {
        var text = ExtraText.Text ?? "";
        var intent = new Intent(Intent.ActionSend);
        intent.SetType("text/plain");
        intent.PutExtra(Intent.ExtraText, text);
        intent.SetFlags(ActivityFlags.NewTask);
        Android.App.Application.Context.StartActivity(intent);
        StatusText.Text = $"已发送 Intent，内容：{text}";
    }

    // ── URL Scheme ────────────────────────────────────────────
    private void OnOpenUrl(object? s, RoutedEventArgs e)
    {
        var url = SchemeUrl.Text?.Trim() ?? "https://www.bing.com";
        var uri = Android.Net.Uri.Parse(url);
        var intent = new Intent(Intent.ActionView, uri);
        intent.SetFlags(ActivityFlags.NewTask);
        Android.App.Application.Context.StartActivity(intent);
        StatusText.Text = $"已打开：{url}";
    }

    // ── 系统分享 ──────────────────────────────────────────────
    private void OnShare(object? s, RoutedEventArgs e)
    {
        var text = ShareText.Text ?? "";
        var intent = new Intent(Intent.ActionSend);
        intent.SetType("text/plain");
        intent.PutExtra(Intent.ExtraSubject, "Avalonia Demo 分享");
        intent.PutExtra(Intent.ExtraText, text);

        var chooser = Intent.CreateChooser(intent, "选择分享方式");
        chooser!.SetFlags(ActivityFlags.NewTask);
        Android.App.Application.Context.StartActivity(chooser);
        StatusText.Text = "已打开分享面板";
    }

    private void OnBack(object? s, RoutedEventArgs e) => _main.BackToMenu();
}
