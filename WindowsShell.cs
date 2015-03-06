using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SoundCloudOSControl
{
    public class WindowsShell
    {
        #region fields
        public static int MOD_ALT = 0x1;
        public static int MOD_CONTROL = 0x2;
        public static int MOD_SHIFT = 0x4;
        public static int MOD_WIN = 0x8;
        public static int MOD_NOREPEAT = 0x4000;

        public static int WM_HOTKEY = 0x312;

        #endregion

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private delegate void Func();

        public static void UnregisterHotKey(Form f, int id)
        {
            try
            {
                UnregisterHotKey(f.Handle, id); // modify this if you want more than one hotkey
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
