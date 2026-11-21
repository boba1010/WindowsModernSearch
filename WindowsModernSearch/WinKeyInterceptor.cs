using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WindowsModernSearch;

public sealed partial class WinKeyInterceptor : IDisposable
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int VK_LWIN = 0x5B;
    private const int VK_RWIN = 0x5C;
    private const int VK_S = 0x53;
    private const int VK_Q = 0x51;

    private readonly LowLevelKeyboardProc _proc;
    private IntPtr _hookId = IntPtr.Zero;

    public event Action? SearchShortcutPressed;

    public WinKeyInterceptor()
    {
        _proc = HookCallback;
    }

    public void Start()
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule!;
        _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
        {
            int vkCode = Marshal.ReadInt32(lParam);

            // Fast exit: ignore 99.9% of keys immediately
            if (vkCode == VK_S || vkCode == VK_Q)
            {
                bool winDown = (GetAsyncKeyState(VK_LWIN) & 0x8000) != 0 ||
                               (GetAsyncKeyState(VK_RWIN) & 0x8000) != 0;

                if (winDown)
                {
                    // Send a dummy harmless key (VK_F24 = 0x87) so Windows doesn't trigger the Start Menu when Win is released!
                    keybd_event(0x87, 0, 0, UIntPtr.Zero);
                    keybd_event(0x87, 0, 2 /* KEYEVENTF_KEYUP */, UIntPtr.Zero);

                    SearchShortcutPressed?.Invoke();
                    return (IntPtr)1; // Swallow Win+S / Win+Q before Windows Search sees it
                }
            }
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    public void Dispose()
    {
        if (_hookId != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
}