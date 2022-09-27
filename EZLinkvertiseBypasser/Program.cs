using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Glumboi.Debug;

namespace EZLinkvertiseBypasser
{
    internal static class Program
    {
        public static readonly DebugConsole debugConsole = new DebugConsole(2, "EZLinkvertiseBypasser Debug Console", false, true);

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
