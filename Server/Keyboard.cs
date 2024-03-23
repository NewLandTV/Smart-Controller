using System.Diagnostics;
using System.Runtime.InteropServices;

internal static class Keyboard
{
    // Enums
    public enum KeyCode
    {
        LeftMouseButton = 0x01, // Left mouse button
        RightMouseButton = 0x02,    // Right mouse button
        Cancel = 0x03,  // Control + Break processing
        MiddleMouseButton = 0x04,   // Middle mouse button
        Backspace = 0x08,   // Backspace
        Tab = 0x09, // Tab
        Return = 0x0d,  // Enter
        Shift = 0x10,   // Shift
        Control = 0x11, // Ctrl
        Alt = 0x12, // Alt
        Escape = 0x1b,  // Esc
        Space = 0x20,   // Spacebar
        Quote = 0x27,   // '
        Comma = 0x2c,   // ,
        Minus = 0x2d,   // -
        Dot = 0x2e,   // .
        Slash = 0x2f,   // /
        Number_0 = 0x30,    // 0
        Number_1 = 0x31,    // 1
        Number_2 = 0x32,    // 2
        Number_3 = 0x33,    // 3
        Number_4 = 0x34,    // 4
        Number_5 = 0x35,    // 5
        Number_6 = 0x36,    // 6
        Number_7 = 0x37,    // 7
        Number_8 = 0x38,    // 8
        Number_9 = 0x39,    // 9
        Semicolon = 0x3b,   // ;
        Equal = 0x3d,   // =
        A = 0x41,   // A
        B = 0x42,   // B
        C = 0x43,   // C
        D = 0x44,   // D
        E = 0x45,   // E
        F = 0x46,   // F
        G = 0x47,   // G
        H = 0x48,   // H
        I = 0x49,   // I
        J = 0x4a,   // J
        K = 0x4b,   // K
        L = 0x4c,   // L
        M = 0x4d,   // M
        N = 0x4e,   // N
        O = 0x4f,   // O
        P = 0x50,   // P
        Q = 0x51,   // Q
        R = 0x52,   // R
        S = 0x53,   // S
        T = 0x54,   // T
        U = 0x55,   // U
        V = 0x56,   // V
        W = 0x57,   // W
        X = 0x58,   // X
        Y = 0x59,   // Y
        Z = 0x5a,   // Z
        LeftSquareBracket = 0x5b,   // [
        Backslash = 0x5c,   // \
        RightSquareBracket = 0x5d,  // ]
        a = 0x61,   // a
        b = 0x62,   // b
        c = 0x63,   // c
        d = 0x64,   // d
        e = 0x65,   // e
        f = 0x66,   // f
        g = 0x67,   // g
        h = 0x68,   // h
        i = 0x69,   // i
        j = 0x6a,   // j
        k = 0x6b,   // k
        l = 0x6c,   // l
        m = 0x6d,   // m
        n = 0x6e,   // n
        o = 0x6f,   // o
        p = 0x70,   // p
        q = 0x71,   // q
        r = 0x72,   // r
        s = 0x73,   // s
        t = 0x74,   // t
        u = 0x75,   // u
        v = 0x76,   // v
        w = 0x77,   // w
        x = 0x78,   // x
        y = 0x79,   // y
        z = 0x7a,   // z
    }

    // Functions
    [DllImport("user32.dll")]
    public static extern void keybd_event(byte vk, byte scan, ulong flags, IntPtr extraInfo);

    [DllImport("user32.dll")]
    public static extern IntPtr PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    public static void KeyDown(KeyCode keyCode)
    {
        keybd_event((byte)keyCode, 0, 0x00, 0);
    }

    public static void KeyUp(KeyCode keyCode)
    {
        keybd_event((byte)keyCode, 0, 0x02, 0);
    }

    public static void SendKeyDownWinAPI(string processName, KeyCode keyCode)
    {
        Process[] processList = Process.GetProcessesByName(processName);

        for (int i = 0; i < processList.Length; i++)
        {
            PostMessage(processList[i].MainWindowHandle, 0x100, (IntPtr)keyCode, IntPtr.Zero);
        }
    }
}
