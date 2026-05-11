using System.Runtime.InteropServices;
using System.Text;
using MiCodeAutoToolBox.App.Models;

namespace MiCodeAutoToolBox.App.Services;

#pragma warning disable CA1416 // Windows only APIs
public sealed class WindowEnumerator : IWindowEnumerator
{
    public IReadOnlyList<WindowEntry> Enumerate()
    {
        var list = new List<WindowEntry>();
        EnumWindows((hWnd, lParam) =>
        {
            if (!IsWindowVisible(hWnd)) return true;
            int length = GetWindowTextLength(hWnd);
            if (length == 0) return true;
            var sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            GetWindowThreadProcessId(hWnd, out uint pid);
            list.Add(new WindowEntry((int)pid, GetProcessName((int)pid), sb.ToString(), hWnd));
            return true;
        }, IntPtr.Zero);
        return list;
    }

    private static string GetProcessName(int pid)
    {
        try
        {
            return System.Diagnostics.Process.GetProcessById(pid).ProcessName;
        }
        catch
        {
            return string.Empty;
        }
    }

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
}
#pragma warning restore CA1416
