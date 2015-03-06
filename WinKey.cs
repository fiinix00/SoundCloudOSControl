using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundCloudOSControl
{
    public static class WinKey
    {
        public static void Register(Form form, int key)
        {
            var ctrl_win = WindowsShell.MOD_CONTROL | WindowsShell.MOD_WIN | WindowsShell.MOD_NOREPEAT;

            WindowsShell.RegisterHotKey(form.Handle, key, ctrl_win, key);

            form.FormClosing += (o, e) => 
            {
                WindowsShell.UnregisterHotKey(form, key);
            };
        }
    }
}
