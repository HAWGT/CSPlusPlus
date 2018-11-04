using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace CSGOPlusPlus.Helpers
{
    class KBMHelper
    {
        private static InputSimulator s = new InputSimulator();

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const int KEYEVENTF_KEYUP = 0x0002;

        public static void LClick()
        {
            s.Mouse.LeftButtonClick();
        }

        public static void RClick()
        {
            s.Mouse.RightButtonClick();
        }

        public static void PressKey(VirtualKeyCode key)
        {
            s.Keyboard.KeyPress(key);
        }
    }
}
