using Android.Content;
using Android.Database;
using Android.Provider;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;

namespace AndroidSample.Avalonia.Views;

/// <summary>
/// 功能11：联系人 —— ContentResolver 读取系统通讯录，展示姓名+电话。
/// </summary>
public partial class ContactsView : UserControl
{
    private readonly MainView _main;

    public ContactsView(MainView main)
    {
        _main = main;
        InitializeComponent();
    }

    private void OnReadContacts(object? s, RoutedEventArgs e)
    {
        var ctx = Android.App.Application.Context;
        var results = new List<string>();

        try
        {
            // 查询联系人：姓名 + 电话
            var uri = ContactsContract.CommonDataKinds.Phone.ContentUri;
            string[] projection =
            {
                ContactsContract.CommonDataKinds.Phone.InterfaceConsts.DisplayName,
                ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Number
            };

            using var cursor = ctx.ContentResolver!.Query(uri, projection, null, null,
                ContactsContract.CommonDataKinds.Phone.InterfaceConsts.DisplayName);

            if (cursor == null)
            {
                results.Add("无法读取联系人（权限未授权？）");
                PermissionHint.IsVisible = true;
            }
            else
            {
                int count = 0;
                while (cursor.MoveToNext() && count < 20)
                {
                    var name   = cursor.GetString(0) ?? "(无姓名)";
                    var number = cursor.GetString(1) ?? "";
                    results.Add($"{name}  {number}");
                    count++;
                }
                if (count == 0) results.Add("联系人为空");
            }
        }
        catch (Java.Lang.SecurityException)
        {
            results.Add("权限被拒绝，请在系统设置中授权 READ_CONTACTS");
            PermissionHint.IsVisible = true;
        }

        ContactList.ItemsSource = results;
    }

    private void OnBack(object? s, RoutedEventArgs e) => _main.BackToMenu();
}
