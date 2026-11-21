using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using Windows.Graphics;
using Windows.System;
using WinRT.Interop;

namespace WindowsModernSearch;

public sealed partial class SearchWindow : Window
{
    private long _lastShownTick;

    private const int Width = 700;
    private const int Height = 800;

    public SearchWindow()
    {
        InitializeComponent();
        PositionAboveTaskbar(Width, Height);

        Activated += (_, args) =>
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated &&
                Environment.TickCount64 - _lastShownTick > 150)
            {
                ((App)Application.Current).HideSearchWindow();
            }
        };

        if (Content is UIElement root)
        {
            root.KeyDown += (_, e) =>
            {
                if (e.Key == VirtualKey.Escape)
                    ((App)Application.Current).HideSearchWindow();
            };
        }

        OverlappedPresenter presenter = OverlappedPresenter.CreateForDialog();

        presenter.PreferredMaximumHeight = Height;
        presenter.PreferredMinimumHeight = Height;
        presenter.PreferredMaximumWidth = Width;
        presenter.PreferredMinimumWidth = Width;
        presenter.IsAlwaysOnTop = true;

        presenter.SetBorderAndTitleBar(true, false);

        AppWindow.SetPresenter(presenter);

        ExtendsContentIntoTitleBar = true;
    }

    public void OnShown()
    {
        _lastShownTick = Environment.TickCount64;
        SearchBox.Focus(FocusState.Programmatic);
    }

    private void PositionAboveTaskbar(int width = 680, int height = 520, int bottomMargin = 12)
    {
        var hwnd = WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
        var workArea = displayArea.WorkArea;

        int x = workArea.X + (workArea.Width - width) / 2;
        int y = (workArea.Y + workArea.Height) - height - bottomMargin;

        appWindow.MoveAndResize(new RectInt32(x, y, width, height));
    }

}
