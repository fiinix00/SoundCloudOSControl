using ChromeAutomation;
using System;
using System.Drawing;
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
                Size = new Size(0, 0),
                StartPosition = FormStartPosition.Manual,
                Location = new Point(0, 0),
                FormBorderStyle = FormBorderStyle.None,
            };

            Application.Run(form);
        }

        public partial class SoundCloundListenerForm : Form
        {
            private const int CTRL_WIN_UP = 0x26;
            private const int CTRL_WIN_DOWN = 0x28;

            private const int CTRL_WIN_LEFT = 0x25;
            private const int CTRL_WIN_RIGHT = 0x27;
            
            private const int CTRL_WIN_ENTER = 0x0D;

            protected override void OnVisibleChanged(EventArgs e)
            {
                base.OnVisibleChanged(e);
                this.Visible = false;
            }

            protected override void OnLoad(EventArgs e)
            {
                WinKey.Register(this, CTRL_WIN_UP);
                WinKey.Register(this, CTRL_WIN_DOWN);
                
                WinKey.Register(this, CTRL_WIN_LEFT);
                WinKey.Register(this, CTRL_WIN_RIGHT);
                
                WinKey.Register(this, CTRL_WIN_ENTER);

                base.OnLoad(e);
            }

            private static string volume(bool increase)
            {
                var sign = (increase ? " + " : " - ");

                return "webpackJsonp([],{0:function(exports,instance,globals){for(var i=0;i<100;i++)if('getVolume' in globals(i)){var audioManager=globals(i);audioManager.setVolume(audioManager.getVolume()" + sign + ".2);break}}});";
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == WindowsShell.WM_HOTKEY)
                {
                    switch (m.WParam.ToInt32())
                    {
                        case CTRL_WIN_UP: ChromeRemote.Send(volume(true)); break;
                        case CTRL_WIN_DOWN: ChromeRemote.Send(volume(false)); break;

                        case CTRL_WIN_LEFT: ChromeRemote.Click(".skipControl__previous"); break;
                        case CTRL_WIN_RIGHT: ChromeRemote.Click(".skipControl__next"); break;

                        case CTRL_WIN_ENTER: ChromeRemote.Click(".playControl"); break;
                    }
                }
            }
        }
    }
}

