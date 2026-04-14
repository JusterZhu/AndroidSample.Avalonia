using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views.Animations;
using Android.Widget;

namespace AndroidSample.Avalonia;

/// <summary>
/// Android 原生启动页（Splash Screen）。
/// 显示简单动画后跳转到 MainActivity。
/// </summary>
[Activity(
    Label = "Avalonia Android Demo",
    Theme = "@style/MyTheme.Splash",
    MainLauncher = true,
    NoHistory = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
public class SplashActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // 全屏布局
        SetContentView(Resource.Layout.splash_layout);

        // 淡入动画
        var logo = FindViewById<ImageView>(Resource.Id.splash_logo);
        if (logo != null)
        {
            var fadeIn = AnimationUtils.LoadAnimation(this, Resource.Animation.splash_fade_in);
            logo.StartAnimation(fadeIn);
        }

        // 延迟跳转 MainActivity
        new Handler(Looper.MainLooper!).PostDelayed(() =>
        {
            StartActivity(new Intent(this, typeof(MainActivity)));
            Finish();
        }, 1500);
    }
}
