using H.NotifyIcon;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Runtime.InteropServices;
using WinRT.Interop;

namespace WindowsModernSearch;


public partial class App : Application
{
    private WinKeyInterceptor? _interceptor;
    private SearchWindow? _searchWindow;
    private TaskbarIcon? _trayIcon;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var uiQueue = DispatcherQueue.GetForCurrentThread();

        _searchWindow = new SearchWindow();
        SetupTrayIcon();
        HideSearchWindow();

        _interceptor = new WinKeyInterceptor();
        _interceptor.SearchShortcutPressed += () => uiQueue.TryEnqueue(ShowSearchWindow);
        _interceptor.Start();
    }

    private void SetupTrayIcon()
    {
        var openItem = new MenuFlyoutItem { Text = "Open" };
        openItem.Click += (_, _) => ShowSearchWindow();

        var exitItem = new MenuFlyoutItem { Text = "Exit" };
        exitItem.Click += (_, _) =>
        {
            _interceptor?.Dispose();
            _trayIcon?.Dispose();
            Environment.Exit(0);
        };

        var menu = new MenuFlyout();
        menu.Items.Add(openItem);
        menu.Items.Add(new MenuFlyoutSeparator());
        menu.Items.Add(exitItem);

        _trayIcon = new TaskbarIcon
        {
            ToolTipText = "WindowsModernSearch (Win + S)",
            //IconSource = new BitmapImage(new Uri("ms-appx:///Assets/SearchTray.ico")),
            ContextFlyout = menu,
            ContextMenuMode = ContextMenuMode.SecondWindow,
        };
        _trayIcon.ForceCreate();
    }

    public void ShowSearchWindow()
    {
        if (_searchWindow == null) return;

        var hwnd = WindowNative.GetWindowHandle(_searchWindow);
        var appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(hwnd));

        appWindow.Show();
        _searchWindow.Activate();
        SetForegroundWindow(hwnd);
        _searchWindow.OnShown();
    }

    public void HideSearchWindow()
    {
        if (_searchWindow == null) return;

        var hwnd = WindowNative.GetWindowHandle(_searchWindow);
        var appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(hwnd));
        appWindow.Hide();
    }

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);
}