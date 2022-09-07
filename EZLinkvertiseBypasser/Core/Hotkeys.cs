using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EZLinkvertiseBypasser.Core
{
    internal class Hotkeys
    {
        //Extern methods used to manage hotkeys
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public enum HotkeyIDs
        {
            HotkeyID = 1,
        }

        public static void LoadHotkeys(Control control)
        {
            RegisterHotKey(control.Handle, (int)HotkeyIDs.HotkeyID, 2, (int)Keys.R);
        }

        public static void Unloadhotkeys(Control control)
        {
            var enumLength = Enum.GetNames(typeof(HotkeyIDs)).Length;

            for (int i = 0; i < enumLength; i++)
            {
                UnregisterHotKey(control.Handle, i);
            }
        }
    }
}
