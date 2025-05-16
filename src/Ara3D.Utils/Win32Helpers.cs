using System;
using System.Runtime.InteropServices;

namespace Ara3D.Utils.Win32
{
    public static class Win32Helpers
    {
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;

        public const int WS_MAXIMIZEBOX = 0x10000;
        public const int WS_MINIMIZEBOX = 0x20000;

        public const int WS_EX_DLGMODALFRAME = 0x00000001;

        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_FRAMECHANGED = 0x0020;

        public const uint WM_SETICON = 0x0080;

        // Define the window styles we use
        public const int WS_CHILD = 0x40000000;
        public const int WS_VISIBLE = 0x10000000;

        // Define the Windows messages we will handle
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_RBUTTONDBLCLK = 0x0206;
        public const int WM_MBUTTONDOWN = 0x0207;
        public const int WM_MBUTTONUP = 0x0208;
        public const int WM_MBUTTONDBLCLK = 0x0209;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_XBUTTONDOWN = 0x020B;
        public const int WM_XBUTTONUP = 0x020C;
        public const int WM_XBUTTONDBLCLK = 0x020D;
        public const int WM_MOUSELEAVE = 0x02A3;
        public const int WM_GESTURE = 0x0119;
        public const int WM_GESTURENOTIFY = 0x011A;
        public const int WM_WINDOWPOSCHANGING = 0x0046;
        public const int WM_KILLFOCUS = 0x0008;

        // One of the fields in GESTUREINFO structure is type of Int64 (8 bytes).
        // The relevant gesture information is stored in lower 4 bytes. This
        // bit mask is used to get 4 lower bytes from this argument.
        public const long ULL_ARGUMENTS_BIT_MASK = 0x00000000FFFFFFFF;

        // Gesture flags - GESTUREINFO.dwFlags
        public const int GF_BEGIN = 0x00000001;
        public const int GF_INERTIA = 0x00000002;
        public const int GF_END = 0x00000004;

        // The following lists the supported gesture commands.
        public const int GID_BEGIN          = 1; // Indicates a generic gesture is beginning.
        public const int GID_END            = 2; // Indicates a generic gesture end.
        public const int GID_ZOOM           = 3; // Indicates zoom start, zoom move, or zoom stop.The first GID_ZOOM command message begins a zoom but does not cause any zooming. The second GID_ZOOM command triggers a zoom relative to the state contained in the first GID_ZOOM.
        public const int GID_PAN            = 4; // Indicates pan move or pan start. The first GID_PAN command indicates a pan start but does not perform any panning. With the second GID_PAN command message, the application will begin panning.
        public const int GID_ROTATE         = 5; // Indicates rotate move or rotate start. The first GID_ROTATE command message indicates a rotate move or rotate start but will not rotate. The second GID_ROTATE command message will trigger a rotation operation relative to state contained in the first GID_ROTATE.
        public const int GID_TWOFINGERTAP   = 6; // Indicates two-finger tap gesture.
        public const int GID_PRESSANDTAP    = 7; // Indicates the press and tap gesture.

        // Gesture configuration flags - GESTURECONFIG.dwWant or GESTURECONFIG.dwBlock
        // Common gesture configuration flags - set GESTURECONFIG.dwID to zero
        public const int GC_ALLGESTURES = 0x00000001;
        // Zoom gesture configuration flags - set GESTURECONFIG.dwID to GID_ZOOM
        public const int GC_ZOOM = 0x00000001;

        // Pan gesture configuration flags - set GESTURECONFIG.dwID to GID_PAN
        public const int GC_PAN = 0x00000001;
        public const int GC_PAN_WITH_SINGLE_FINGER_VERTICALLY = 0x00000002;
        public const int GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY = 0x00000004;
        public const int GC_PAN_WITH_GUTTER = 0x00000008;
        public const int GC_PAN_WITH_INERTIA = 0x00000010;

        // Rotate gesture configuration flags - set GESTURECONFIG.dwID to GID_ROTATE
        public const int GC_ROTATE = 0x00000001;

        // Two finger tap gesture configuration flags - set GESTURECONFIG.dwID to GID_TWOFINGERTAP
        public const int GC_TWOFINGERTAP = 0x00000001;

        // PressAndTap gesture configuration flags - set GESTURECONFIG.dwID to GID_PRESSANDTAP
        public const int GC_PRESSANDTAP = 0x00000001;
        public const int GC_ROLLOVER = GC_PRESSANDTAP;

        public const int GESTURECONFIGMAXCOUNT = 256;             // Maximum number of gestures that can be included in a single call to SetGestureConfig / GetGestureConfig


        // Define the values that let us differentiate between the two extra mouse buttons
        public const int MK_XBUTTON1 = 0x020;
        public const int MK_XBUTTON2 = 0x040;

        // Define the cursor icons we use
        public const int IDC_ARROW = 32512;

        // Define the TME_LEAVE value so we can register for WM_MOUSELEAVE messages
        public const uint TME_LEAVE = 0x00000002;

        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;

        // If the calling thread and the thread that owns the window are attached to different input queues,
        // the system posts the request to the thread that owns the window.This prevents the calling thread
        // from blocking its execution while other threads process the request.
        public const uint SWP_ASYNCWINDOWPOS = 0x4000;

        // Does not change the owner window's position in the Z order.
        public const uint SWP_NOOWNERZORDER = 0x0200;

        // Don’t send WM_WINDOWPOSCHANGING 
        public const uint SWP_NOSENDCHANGING = 0x0400; 

        public const int SW_SHOWNORMAL = 1;

        public const int WHEEL_DELTA = 120;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct RECT
        {
            public uint Left;
            public uint Top;
            public uint Right;
            public uint Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct POINT
        {
            public uint X;
            public uint Y;
        }

        public static uint GetXLParam(uint lParam) => LowWord(lParam);
        public static uint GetYLParam(uint lParam) => HighWord(lParam);
        public static int GetWheelDeltaWParam(uint wParam) => (short)HighWord(wParam);
        public static uint LowWord(uint input) => input & 0x0000ffff;
        public static uint HighWord(uint input) => (input >> 16) & 0x0000ffff;
        // https://github.com/pauldotknopf/WindowsSDK7-Samples/blob/master/Touch/MTGestures/CS/MTGestures.cs

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct POINTS
        {
            public ushort x;
            public ushort y;
        }

        //
        // Gesture information structure
        //   - Pass the HGESTUREINFO received in the WM_GESTURE message lParam 
        //     into the GetGestureInfo function to retrieve this information.
        //   - If cbExtraArgs is non-zero, pass the HGESTUREINFO received in 
        //     the WM_GESTURE message lParam into the GetGestureExtraArgs 
        //     function to retrieve extended argument information.
        //
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct GESTUREINFO
        {
            public uint cbSize;              // size, in bytes, of this structure (including variable length Args field)
            public uint dwFlags;             // see GF_* flags
            public uint dwID;                // gesture ID, see GID_* defines
            public IntPtr hwndTarget;          // handle to window targeted by this gesture

            [MarshalAs(UnmanagedType.Struct)]
            public POINTS ptsLocation;         // current location of this gesture
            public uint dwInstanceID;        // internally used
            public uint dwSequenceID;        // internally used
            public ulong ullArguments;        // arguments for gestures whose arguments fit in 8 BYTES
            public uint cbExtraArgs;         // size, in bytes, of extra arguments, if any, that accompany this gesture
        }

        //
        // Gesture configuration structure
        //   - Used in SetGestureConfig and GetGestureConfig
        //   - NoteState that any setting not included in either GESTURECONFIG.dwWant
        //     or GESTURECONFIG.dwBlock will use the parent window's preferences
        //     or system defaults.
        //
        // Touch API defined structures [winuser.h]
        [StructLayout(LayoutKind.Sequential)]
        public struct GESTURECONFIG
        {
            public int dwID;    // gesture ID
            public int dwWant;  // settings related to gesture ID that are to be
            // turned on
            public int dwBlock; // settings related to gesture ID that are to be
            // turned off
        }

        // size of GESTURECONFIG structure
        public static int GestureConfigSize { get; private set; }
        // size of GESTUREINFO structure
        public static int GestureInfoSize { get; private set; }

        public static void SetupStructSizes()
        {
            // Both GetGestureCommandInfo and GetTouchInputInfo need to be
            // passed the size of the structure they will be filling
            // we get the sizes upfront so they can be used later.
            GestureConfigSize = Marshal.SizeOf(new GESTURECONFIG());
            GestureInfoSize = Marshal.SizeOf(new GESTUREINFO());
        }

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetGestureInfo(IntPtr hGestureInfo, ref GESTUREINFO pGestureInfo);

        // Currently touch/multitouch access is done through unmanaged code
        // We must p/invoke into user32 [winuser.h]
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetGestureConfig(IntPtr hWnd, int dwReserved, int cIDs, ref GESTURECONFIG pGestureConfig, int cbSize);

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, [In, Out] ref POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, [In, Out] ref POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, [In, Out] ref RECT rect);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
        public static extern IntPtr CreateWindowEx(
            int dwExStyle,
            [MarshalAs(UnmanagedType.LPStr)] string lpClassName,
            [MarshalAs(UnmanagedType.LPStr)] string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("user32.dll")]
        public static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        public static extern uint GetKeyboardLayoutList(int nBuff, IntPtr[] lpList);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKeyExA(uint uCode, uint uMapType, IntPtr dwhkl);

        [DllImport("user32.dll")]
        public static extern uint ToAsciiEx(uint uVirtKey, uint uScanCode, byte[] lpKeyState, byte[] lpChar, uint uFlags, IntPtr dwhkl);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT point);

        [DllImport("user32.dll")]
        public static extern int ShowCursor(bool bShow);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos([In, Out] ref POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern int SetFocus(IntPtr hWnd);

        public const uint MOUSEEVENTF_MASK = 0xFFFFFF00;

        public const uint MOUSEEVENTF_FROMTOUCH = 0xFF515700;
        public const int WH_MOUSE_LL = 14;

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod,
            uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetMessageExtraInfo();


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X,
            int Y, int cx, int cy, uint uFlags);

        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        public static readonly IntPtr HWND_TOP = new IntPtr(0);
        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        public const uint SWP_NOREDRAW = 0x0008;
        public const uint SWP_NOACTIVATE = 0x0010;

        public const uint TOPMOST_FLAGS = SWP_NOACTIVATE | SWP_NOOWNERZORDER | SWP_NOSIZE | SWP_NOMOVE | SWP_NOREDRAW | SWP_NOSENDCHANGING;
    }
}
