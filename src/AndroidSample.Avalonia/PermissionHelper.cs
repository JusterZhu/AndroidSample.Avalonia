using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AndroidSample.Avalonia;

/// <summary>
/// 统一的 Android 动态权限申请工具类。
/// 使用方式：await PermissionHelper.RequestAsync(activity, Manifest.Permission.Camera);
/// </summary>
public static class PermissionHelper
{
    private static readonly Dictionary<int, TaskCompletionSource<bool>> _pendingRequests = new();
    private static int _nextRequestCode = 2000;

    /// <summary>检查是否已授予指定权限。</summary>
    public static bool IsGranted(string permission)
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.M) return true;
        return Android.App.Application.Context
            .CheckSelfPermission(permission) == Permission.Granted;
    }

    /// <summary>
    /// 异步申请单个权限，返回是否授予。
    /// 需要在 MainActivity.OnRequestPermissionsResult 中调用 OnResult。
    /// </summary>
    public static Task<bool> RequestAsync(Activity activity, string permission)
    {
        if (IsGranted(permission)) return Task.FromResult(true);

        var tcs = new TaskCompletionSource<bool>();
        var code = _nextRequestCode++;
        _pendingRequests[code] = tcs;
        activity.RequestPermissions(new[] { permission }, code);
        return tcs.Task;
    }

    /// <summary>
    /// 批量申请权限，返回全部是否授予。
    /// </summary>
    public static async Task<bool> RequestAllAsync(Activity activity, params string[] permissions)
    {
        foreach (var p in permissions)
        {
            if (!await RequestAsync(activity, p)) return false;
        }
        return true;
    }

    /// <summary>
    /// 由 MainActivity.OnRequestPermissionsResult 调用，分发结果。
    /// </summary>
    public static void OnResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        if (_pendingRequests.TryGetValue(requestCode, out var tcs))
        {
            _pendingRequests.Remove(requestCode);
            tcs.SetResult(grantResults.Length > 0 && grantResults[0] == Permission.Granted);
        }
    }
}
