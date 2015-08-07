using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JoySticksHelper
{
    public class WinIOLab
    {
        private const int KBC_KEY_CMD = 0x64;
        private const int KBC_KEY_DATA = 0x60;
        [DllImport("winio32.dll")]
        private static extern bool InitializeWinIo();
        [DllImport("winio32.dll")]
        private static extern bool GetPortVal(IntPtr wPortAddr, out int pdwPortVal, byte bSize);
        [DllImport("winio32.dll")]
        private static extern bool SetPortVal(uint wPortAddr, IntPtr dwPortVal, byte bSize);
        [DllImport("winio32.dll")]
        private static extern byte MapPhysToLin(byte pbPhysAddr, uint dwPhysSize, IntPtr PhysicalMemoryHandle);
        [DllImport("winio32.dll")]
        private static extern bool UnmapPhysicalMemory(IntPtr PhysicalMemoryHandle, byte pbLinAddr);
        [DllImport("winio32.dll")]
        private static extern bool GetPhysLong(IntPtr pbPhysAddr, byte pdwPhysVal);
        [DllImport("winio32.dll")]
        private static extern bool SetPhysLong(IntPtr pbPhysAddr, byte dwPhysVal);
        [DllImport("winio32.dll")]
        private static extern void ShutdownWinIo();
        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(uint Ucode, uint uMapType);
        [DllImport("user32.dll")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point pt);

        public enum Direction { Left, Right, Up, Down, UL, UR, LL, LR };
        public enum MouseKey
        {
            MOUSEEVENTF_MOVE = 0x0001, MOUSEEVENTF_LEFTDOWN = 0x0002, MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008, MOUSEEVENTF_RIGHTUP = 0x0010, MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            MOUSEEVENTF_MIDDLEUP = 0x0040, MOUSEEVENTF_ABSOLUTE = 0x8000, MOUSEEVENTF_LEFTCLICK = 0x0002 | 0x0004,
            MOUSEEVENTF_RIGHTCLICK = 0x0008 | 0x0010, MOUSEEVENTF_MIDDLECLICK = 0x0020 | 0x0040
        };

        private WinIOLab()
        {
            IsInitialize = true;
        }
        public static void Initialize()
        {
            if (InitializeWinIo())
            {
                KBCWait4IBE();
                IsInitialize = true;
            }
        }
        public static void Shutdown()
        {
            if (IsInitialize)
                ShutdownWinIo();
            IsInitialize = false;
        }

        public static bool IsInitialize { get; set; }

        ///等待键盘缓冲区为空
        private static void KBCWait4IBE()
        {
            int dwVal = 0;
            do
            {
                bool flag = GetPortVal((IntPtr)0x64, out dwVal, 1);
            }
            while ((dwVal & 0x2) > 0);
        }
        /// 模拟键盘标按下
        public static void KeyDown(Keys vKeyCoad)
        {
            if (!IsInitialize) return;

            int btScancode = 0;
            btScancode = MapVirtualKey((uint)vKeyCoad, 0);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)0x60, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)btScancode, 1);
        }
        /// 模拟键盘弹出
        public static void KeyUp(Keys vKeyCoad)
        {
            if (!IsInitialize) return;

            int btScancode = 0;
            btScancode = MapVirtualKey((uint)vKeyCoad, 0);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)0x60, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)(btScancode | 0x80), 1);
        }

        public static void KeyClick(Keys vKeyCoad, bool doublelick)
        {
            KeyDown(vKeyCoad);
            KeyUp(vKeyCoad);
            if (doublelick)
            {
                KeyDown(vKeyCoad);
                KeyUp(vKeyCoad);
            }
        }

        public static class Mouse
        {
            public static bool LeftKey = false;
            public static bool RightKey = false;
            public static bool MiddleKey = false;
        }

        private static int MapVirtualMouse(int _value)
        {
            int value = Math.Abs(_value);
            int res = 0;
            if (value > 32000)
            {
                res = 3;
            }
            else if (value > 20000)
            {
                res = 2;
            }
            else if (value != 0)
            {
                res = 1;
            }
            else
            {
                res = 0;
            }
            if (_value < 0)
            {
                res = -res;
            }
            return res;
        }

        /// 模拟鼠标移动
        public static void MouseMove(int _xStick, int _yStick)
        {
            Point p = new Point(MapVirtualMouse(_xStick), MapVirtualMouse(_yStick));
            mouse_event((int)MouseKey.MOUSEEVENTF_MOVE, p.X, p.Y, 0, 0);
        }

        /// 模拟鼠标按键
        public static void MouseKeyEvent(MouseKey vKeyCoad, bool doubleClick)
        {
            mouse_event((int)vKeyCoad, 0, 0, 0, 0);
            if (vKeyCoad == MouseKey.MOUSEEVENTF_LEFTDOWN)
            {
                Mouse.LeftKey = true;
            }
            else if (vKeyCoad == MouseKey.MOUSEEVENTF_MIDDLEUP)
            {
                Mouse.LeftKey = false;
            }

            if (vKeyCoad == MouseKey.MOUSEEVENTF_MIDDLEDOWN)
            {
                Mouse.MiddleKey = true;
            }
            else if (vKeyCoad == MouseKey.MOUSEEVENTF_MIDDLEUP)
            {
                Mouse.MiddleKey = false;
            }

            if (vKeyCoad == MouseKey.MOUSEEVENTF_RIGHTDOWN)
            {
                Mouse.RightKey = true;
            }
            else if (vKeyCoad == MouseKey.MOUSEEVENTF_RIGHTUP)
            {
                Mouse.RightKey = false;
            }

            if (doubleClick)
            {
                mouse_event((int)vKeyCoad, 0, 0, 0, 0);
            }
        }
    }
}
