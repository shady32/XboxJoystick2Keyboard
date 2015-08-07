using J2i.Net.XInputWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoySticksHelper
{
    public static class XinputKey
    {
        public const int Down = 0x99;
        public const int Up = 0x98;
        public const int NaN = 0;
        static XboxControllerStateChangedEventArgs _arg;
        public static XinputKeyStatus XKS = new XinputKeyStatus();
        public static void GetXinputKey(XboxControllerStateChangedEventArgs arg)
        {
            _arg = arg;
            #region Tigger
            if (arg.CurrentInputState.Gamepad.bLeftTrigger != 0)
            {
                XKS.LeftTigger = Down;
            }
            else if (arg.CurrentInputState.Gamepad.bLeftTrigger == 0 && arg.PreviousInputState.Gamepad.bLeftTrigger != 0)
            {
                XKS.LeftTigger = Up;
            }
            else
            {
                XKS.LeftTigger = NaN;
            }

            if (arg.CurrentInputState.Gamepad.bRightTrigger != 0 && arg.PreviousInputState.Gamepad.bRightTrigger==0)
            {
                XKS.RightTigger = Down;//arg.CurrentInputState.Gamepad.bRightTrigger;
            }
            else if(arg.CurrentInputState.Gamepad.bRightTrigger == 0 && arg.PreviousInputState.Gamepad.bRightTrigger != 0)
            {
                XKS.RightTigger = Up;
            }
            else
            {
                XKS.RightTigger = NaN;
            }
            #endregion

            #region LeftStick
            if (arg.CurrentInputState.Gamepad.sThumbLY > 0)
            {
                XKS.LeftStickUp = arg.CurrentInputState.Gamepad.sThumbLY;
            }
            else if (arg.CurrentInputState.Gamepad.sThumbLY < 0)
            {
                XKS.LeftStickDown = -arg.CurrentInputState.Gamepad.sThumbLY;
            }
            else
            {
                XKS.LeftStickUp = NaN;
                XKS.LeftStickDown = NaN;
            }

            if (arg.CurrentInputState.Gamepad.sThumbLX > 0)
            {
                XKS.LeftStickRight = arg.CurrentInputState.Gamepad.sThumbLX;
            }
            else if (arg.CurrentInputState.Gamepad.sThumbLX < 0)
            {
                XKS.LeftStickLeft = -arg.CurrentInputState.Gamepad.sThumbLX;
            }
            else
            {
                XKS.LeftStickRight = NaN;
                XKS.LeftStickLeft = NaN;
            }
            #endregion

            #region RightStick
            if (arg.CurrentInputState.Gamepad.sThumbRY != 0)
            {
                XKS.RightStickUp = arg.CurrentInputState.Gamepad.sThumbRY;
            }
            else
            {
                XKS.RightStickUp = NaN;
            }

            if (arg.CurrentInputState.Gamepad.sThumbRX != 0)
            {
                XKS.RightStickLeft = arg.CurrentInputState.Gamepad.sThumbRX;
            }
            else
            {
                XKS.RightStickLeft = NaN;
            }
            #endregion

            #region buttons
            //十字键
            XKS.Up = setKeys((int)ButtonFlags.XINPUT_GAMEPAD_DPAD_UP);
            XKS.Down = setKeys((int)ButtonFlags.XINPUT_GAMEPAD_DPAD_DOWN);
            XKS.Left = setKeys((int)ButtonFlags.XINPUT_GAMEPAD_DPAD_LEFT);
            XKS.Right = setKeys((int)ButtonFlags.XINPUT_GAMEPAD_DPAD_RIGHT);
            //ABXY
            XKS.X = setKeys((int)ButtonFlags.XINPUT_GAMEPAD_X);
            XKS.Y = setKeys((int)ButtonFlags.XINPUT_GAMEPAD_Y);
            XKS.A = setKeys((int)ButtonFlags.XINPUT_GAMEPAD_A);
            XKS.B = setKeys((int)ButtonFlags.XINPUT_GAMEPAD_B);

            //Bumper&Stick
            XKS.LeftBumper = setKeys((int)ButtonFlags.XINPUT_GAMEPAD_LEFT_SHOULDER);
            XKS.RightBumper = setKeys((int)ButtonFlags.XINPUT_GAMEPAD_RIGHT_SHOULDER);
            XKS.LeftStick = setKeys((int)ButtonFlags.XINPUT_GAMEPAD_LEFT_THUMB);
            XKS.RightStick = setKeys((int)ButtonFlags.XINPUT_GAMEPAD_RIGHT_THUMB);

            //Start&Back
            XKS.Start = setKeys((int)ButtonFlags.XINPUT_GAMEPAD_START);
            XKS.Back = setKeys((int)ButtonFlags.XINPUT_GAMEPAD_BACK);

            #endregion
        }
        private static int setKeys(int JoyStickKeys)
        {
            if (_arg.CurrentInputState.Gamepad.IsButtonPressed(JoyStickKeys) && !_arg.PreviousInputState.Gamepad.IsButtonPressed(JoyStickKeys))
            {
                return Down;
            }
            else if (!_arg.CurrentInputState.Gamepad.IsButtonPressed(JoyStickKeys) && _arg.PreviousInputState.Gamepad.IsButtonPressed(JoyStickKeys))
            {
                return Up;
            }
            else
            {
                return NaN;
            }
        }
    }

    public class XinputKeyStatus
    {
        public int LeftStickLeft = 0;
        public int LeftStickRight = 0;
        public int LeftStickUp = 0;
        public int LeftStickDown = 0;
        public int RightStickLeft = 0;
        public int RightStickUp = 0;

        public int Up = 0;
        public int Down = 0;
        public int Left = 0;
        public int Right = 0;
        public int LeftTigger = 0;
        public int RightTigger = 0;
        public int LeftBumper = 0;
        public int RightBumper = 0;
        public int A = 0;
        public int B = 0;
        public int X = 0;
        public int Y = 0;
        public int Back = 0;
        public int Start = 0;
        public int LeftStick = 0;
        public int RightStick = 0;
    }
}
