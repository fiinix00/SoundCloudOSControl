using ChromeAutomation;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundCloudOSControl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var form = new SoundCloundListenerForm 
            { 
                Opacity = 0, 
                WindowState = FormWindowState.Minimized, 
                FormBorderStyle = FormBorderStyle.None, 
            };

            Application.Run(form);
        }

        public partial class SoundCloundListenerForm : Form
        {
            private const int CTRL_WIN_LEFT = 1;
            private const int CTRL_WIN_RIGHT = 2;

            protected override void OnVisibleChanged(EventArgs e)
            {
                base.OnVisibleChanged(e);
                this.Visible = false;
            }

            protected override void OnLoad(EventArgs e)
            {
                var ctrl_win = WindowsShell.MOD_CONTROL | WindowsShell.MOD_WIN | WindowsShell.MOD_NOREPEAT;

                WindowsShell.RegisterHotKey(this.Handle, CTRL_WIN_LEFT, ctrl_win, 0x25);
                WindowsShell.RegisterHotKey(this.Handle, CTRL_WIN_RIGHT, ctrl_win, 0x27);

                base.OnLoad(e);
            }

            protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
            {
                WindowsShell.UnregisterHotKey(this, CTRL_WIN_LEFT);
                WindowsShell.UnregisterHotKey(this, CTRL_WIN_RIGHT);

                base.OnClosing(e);
            }
                        
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == WindowsShell.WM_HOTKEY)
                {
                    switch (m.WParam.ToInt32())
                    {
                        case CTRL_WIN_LEFT: ChromeRemote.Click(".skipControl__previous"); break;
                        case CTRL_WIN_RIGHT: ChromeRemote.Click(".skipControl__next"); break;
                    }
                }
            }
        }
    }
}
