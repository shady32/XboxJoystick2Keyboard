using J2i.Net.XInputWrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JoySticksHelper
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        #region Xbox
        XboxController xBoxControl;
        void xBoxControl_StateChanged(object sender, XboxControllerStateChangedEventArgs e)
        {
            XinputKey.GetXinputKey(e);
            SetKeys();
        }

        delegate void ChangetextDeg(string obj);
        public void ChangeText(string obj)
        {
            if (this.button1.InvokeRequired)
            {
                this.button1.Invoke(new ChangetextDeg(ChangeText), obj);
            }
            else
            {
                this.button1.Text = obj;
            }
        }

        Thread stickThread;
        Thread dpsThread;
        #endregion

        /// <summary>
        /// 开始装逼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (!WinIOLab.IsInitialize)
            {
                this.button1.Text = "让我歇会儿再装逼";
                WinIOLab.Initialize();
                xBoxControl = XboxController.RetrieveController(0);
                xBoxControl.StateChanged += xBoxControl_StateChanged;
                XboxController.StartPolling();
                stickThread = new Thread(StickStatic);
                stickThread.Start();
                dpsThread = new Thread(dps);
                dpsThread.Start();
            }
            else
            {
                this.button1.Text = "开始强行装逼";
                WinIOLab.Shutdown();
                XboxController.StopPolling();
                stickThread.Abort();
                dpsThread.Abort();
            }
        }

        private static class MouseMover
        {
            public static int _Xsticks;
            public static int _Ysticks;
        }
        private void StickStatic()
        {
            while (true)
            {
                WinIOLab.MouseMove(MouseMover._Xsticks, MouseMover._Ysticks);
                Thread.Sleep(1);
            }
        }

        private static bool DPS = false;
        private static bool taiyuan = false;

        private void dps()
        {
            while (true)
            {
                if (DPS)
                {
                    if (taiyuan)
                    {
                        WinIOLab.KeyClick(Keys.D6, false);
                    }
                    else
                    {
                        WinIOLab.KeyClick(Keys.D4, false);
                        Thread.Sleep(100);
                        WinIOLab.KeyClick(Keys.D5, false);
                        Thread.Sleep(100);
                    }
                }
            }
        }


        private void SetKeys()
        {

            #region leftsticks
            if (XinputKey.XKS.LeftStickLeft > 0)
            {
                WinIOLab.KeyDown(Keys.A);
            }
            else
            {
                WinIOLab.KeyUp(Keys.A);
            }

            if (XinputKey.XKS.LeftStickRight > 0)
            {
                WinIOLab.KeyDown(Keys.D);
            }
            else
            {
                WinIOLab.KeyUp(Keys.D);
            }

            if (XinputKey.XKS.LeftStickUp > 0)
            {
                WinIOLab.KeyDown(Keys.W);
            }
            else
            {
                WinIOLab.KeyUp(Keys.W);
            }

            if (XinputKey.XKS.LeftStickDown > 0)
            {
                WinIOLab.KeyDown(Keys.S);
            }
            else
            {
                WinIOLab.KeyUp(Keys.S);
            }
            #endregion

            #region tigger
            if (XinputKey.XKS.RightTigger == XinputKey.Down)
            {
                if (XinputKey.XKS.LeftTigger == XinputKey.Down)
                {
                    taiyuan = true;
                }
                else
                {
                    taiyuan = false;
                }
                DPS = true;
            }
            else if (XinputKey.XKS.RightTigger == XinputKey.Up)
            {
                DPS = false;
            }
            #endregion

            #region rightsticks
            if (XinputKey.XKS.RightStickUp != 0 | XinputKey.XKS.RightStickLeft != 0)
            {
                if (!WinIOLab.Mouse.RightKey && XinputKey.XKS.LeftTigger == 0)
                {
                    WinIOLab.MouseKeyEvent(WinIOLab.MouseKey.MOUSEEVENTF_RIGHTDOWN, false);
                }
                MouseMover._Xsticks = XinputKey.XKS.RightStickLeft;
                MouseMover._Ysticks = -XinputKey.XKS.RightStickUp;
            }
            else
            {
                if (WinIOLab.Mouse.RightKey)
                {
                    WinIOLab.MouseKeyEvent(WinIOLab.MouseKey.MOUSEEVENTF_RIGHTUP, false);
                }
                MouseMover._Xsticks = 0;
                MouseMover._Ysticks = 0;
            }
            #endregion

            #region key
            keyHelper(XinputKey.XKS.A, XinputKey.NaN, Keys.Space, null);
            keyHelper(XinputKey.XKS.B, XinputKey.XKS.LeftTigger, Keys.F, Keys.T);
            keyHelper(XinputKey.XKS.X, XinputKey.XKS.LeftTigger, Keys.D1, Keys.V);
            keyHelper(XinputKey.XKS.Y, XinputKey.XKS.LeftTigger, Keys.D2, Keys.D3);

            keyHelper(XinputKey.XKS.RightBumper, XinputKey.XKS.LeftTigger, Keys.XButton1, Keys.XButton2);
            keyHelper(XinputKey.XKS.LeftBumper, XinputKey.NaN, Keys.Tab, null);

            keyHelper(XinputKey.XKS.Up, XinputKey.XKS.LeftTigger, Keys.B, Keys.M);
            keyHelper(XinputKey.XKS.Down, XinputKey.NaN, Keys.P, null);
            keyHelper(XinputKey.XKS.Left, XinputKey.XKS.LeftTigger, Keys.C, Keys.K);
            keyHelper(XinputKey.XKS.Right, XinputKey.XKS.LeftTigger, Keys.L, Keys.I);

            keyHelper(XinputKey.XKS.Start, XinputKey.NaN, Keys.Escape, null);
            // keyHelper(XinputKey.XKS.LeftBumper, XinputKey.XKS.LeftTigger, Keys.D2, Keys.D0);
            #endregion
        }

        private void keyHelper(int JoykeyE, int? ShiftKeyE, Keys? Keykey1, Keys? Keykey2)
        {
            if (JoykeyE == XinputKey.Down && ShiftKeyE == XinputKey.Down)
            {
                if (Keykey2 == Keys.XButton2)
                {
                    WinIOLab.MouseKeyEvent(WinIOLab.MouseKey.MOUSEEVENTF_RIGHTDOWN, false);
                }
                else
                {
                    WinIOLab.KeyDown((Keys)Keykey2);
                }
            }
            else if (JoykeyE == XinputKey.Down && ShiftKeyE == XinputKey.NaN)
            {
                if (Keykey1 == Keys.XButton1)
                {
                    WinIOLab.MouseKeyEvent(WinIOLab.MouseKey.MOUSEEVENTF_LEFTDOWN, false);
                }
                else
                {
                    WinIOLab.KeyDown((Keys)Keykey1);
                }
            }
            else if (JoykeyE == XinputKey.Up && ShiftKeyE == XinputKey.Down)
            {
                if (Keykey2 == Keys.XButton2)
                {
                    WinIOLab.MouseKeyEvent(WinIOLab.MouseKey.MOUSEEVENTF_RIGHTUP, false);
                }
                else
                {
                    WinIOLab.KeyUp((Keys)Keykey2);
                }
            }
            else if (JoykeyE == XinputKey.Up && ShiftKeyE == XinputKey.NaN)
            {
                if (Keykey1 == Keys.XButton1)
                {
                    WinIOLab.MouseKeyEvent(WinIOLab.MouseKey.MOUSEEVENTF_LEFTUP, false);
                }
                else
                {
                    WinIOLab.KeyUp((Keys)Keykey1);
                }
            }
        }

        #region Menu Event
        private void 发行说明AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowInTaskbar = false;
            about.StartPosition = FormStartPosition.CenterParent;
            about.ShowDialog();
        }
        #endregion
    }
}
